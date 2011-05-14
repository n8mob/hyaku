using System;
using System.Net;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using Hyaku.Views;
using NateGrigg.Mobile.Random;
using System.Text;
using NateGrigg.Mobile.Utility;
using System.Windows.Threading;
using System.Diagnostics;

namespace Hyaku.ViewModels
{
    public delegate void GameOverEventHandler(object sender, GameOverEventArgs e);

    public class GameBoardViewModel : ViewModelBase
    {
        #region Events

        public event GameOverEventHandler GameOver;

        #endregion Events

        #region Declarations

        private DispatcherTimer _timer;
        List<int> numberSelections;
        private HyakuSettings hyakuSettings;
        private int _randomListSize = 100;
        private RandomListGenerator _randomListGenerator = null;
        private DistanceSumsTable _sumsTable = null;
        private SquareViewModel _currentSquare = null;
        private int _score;
        private int _counts;
        private int _emptySquares;
        private int _currentHyakuCount;
        private int _minAvailibleSquares;
        private int _maxAvailibleSquares;

        #endregion Declarations

        #region Properties

        protected virtual int RandomListSize
        {
            get
            {
                return _randomListSize;
            }
            set
            {
                _randomListSize = value;
            }
        }

        public virtual RandomListGenerator RandomListGenerator
        {
            get
            {
                if (_randomListGenerator == null)
                {
                    _randomListGenerator = new RandomListGenerator();
                }
                return _randomListGenerator;
            }
            set { _randomListGenerator = value; }
        }

        public virtual DispatcherTimer Timer
        {
            get
            {
                return _timer;
            }
            set
            {
                _timer = value;
            }
        }

        public DistanceSumsTable SumsTable
        {
            get
            {
                if (_sumsTable == null) {
                    _sumsTable = new DistanceSumsTable();
                    _sumsTable.HyakuFound += new HyakuFoundEventHandler(HyakuFoundHandler);
                }
                return _sumsTable;
            }
            set
            {
                _sumsTable = value;
            }
        }

        public int Score
        {
            get
            {
                return _score;
            }
            set
            {
                _score = value;
                NotifyPropertyChanged("Score");
            }
        }

        public int Counts
        {
            get
            {
                return _counts;
            }
            set
            {
                _counts = value;
                NotifyPropertyChanged("Counts");
            }
        }

        public int SweepSteps
        {
            get
            {
                return hyakuSettings.SweepTimerPeriodSetting;
            }
        }

        public SquareViewModel CurrentSquare
        {
            get
            {
                return _currentSquare;
            }
            set
            {
                _currentSquare = value;
                if (_currentSquare != null) {
                    _currentSquare.IsCurrent = true;
                }
            }
        }
        /// <summary>
        /// A column-major two-dimensional array of SquareViewModels
        /// </summary>
        /// <remarks>"Column-major" meaning that for coordinates [i,j], i is the colum and j is the row.</remarks>
        public List<List<SquareViewModel>> GameGrid { get; set; }

        public int NextNumber
        {
            get
            {
                if (numberSelections == null || numberSelections.Count < 1)
                {
                    if (hyakuSettings.UseDebugNumbersSetting && hyakuSettings.DebugNumbersSetting != null && hyakuSettings.DebugNumbersSetting.Count > 0) {
                        numberSelections = hyakuSettings.DebugNumbersSetting;
                        hyakuSettings.UseDebugNumbersSetting = false;
                    } else {
                        numberSelections = RandomListGenerator.SelectRandomItems(RandomListSize, hyakuSettings.NumberListSetting);
                        //numberSelections = RandomListGenerator.SelectRandomItems(RandomListSize, new List<int>(new int[] { 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32, 34, 36, 38, 40, 42, 44, 46, 48, 50 }));
                    }
                }

                int nextNumber = numberSelections[0];
                numberSelections.RemoveAt(0);
                return nextNumber;
            }
        }

        public int EmptySquares
        {
            get
            {
                return _emptySquares;
            }
            set
            {
                if (value < _minAvailibleSquares) {
                    // WTF?
                    OnGameOver(GameOverReason.LessThanZero);
                } else if (value > _maxAvailibleSquares) {
                    OnGameOver(GameOverReason.MoreThanMax);
                } else {
                    _emptySquares = value;
#if DEBUG
                    Debug.WriteLine("EmptySquares set to {0}", _emptySquares);
#endif
                }
            }
        }

//        public virtual int CurrentHyakuCount
//        {
//            get
//            {
//                return _currentHyakuCount;
//            }
//            set
//            {
//                if (value < _minAvailibleSquares) {
//                    OnGameOver(GameOverReason.LessThanZero);
//                } else if (value > _maxAvailibleSquares) {
//                    OnGameOver(GameOverReason.MoreThanMax);
//                } else {
//                    _currentHyakuCount = value;
//#if DEBUG
//                    Debug.WriteLine("CurrentHyakuCount set to {0}", _currentHyakuCount);
//#endif
//                }
//            }
//        }

        #endregion Properties

        #region Constructors 

        public GameBoardViewModel()
        {
            hyakuSettings = new HyakuSettings();
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromMilliseconds(hyakuSettings.TimerTickIntervalSetting);
            Timer.Tick += new EventHandler(Tick);
            GameGrid = new List<List<SquareViewModel>>(hyakuSettings.GameSizeSetting);
            _minAvailibleSquares = 0;
            _maxAvailibleSquares = hyakuSettings.GameSizeSetting * hyakuSettings.GameSizeSetting;
            EmptySquares = _maxAvailibleSquares;
            //CurrentHyakuCount = _minAvailibleSquares;
            Counts = 0;
        }

        #endregion Constructors

        #region Methods

        public virtual void SendNumber(int theNumber)
        {
            if (CurrentSquare != null && CurrentSquare.Value == 0)
            {
                if (EmptySquares < 0) {
                    OnGameOver(GameOverReason.RanOutOfSpace);
                } else {
                    CurrentSquare.Value = theNumber;
                    if (theNumber > 0) {
                        EmptySquares -= 1;
                        FindNewHyakus(CurrentSquare);
                    }

                    CurrentSquare.IsCurrent = false;
                    CurrentSquare.IsLocked = true;
                    CurrentSquare = null;
                }
            }
        }

        public virtual void MarkHyakuBlocks(List<SquareViewModel> hyaku)
        {
            foreach (SquareViewModel sq in hyaku)
            {
                sq.Score += 100;
            }
        }

        public virtual void FindNewHyakus(SquareViewModel sq1)
        {
            for (int columnOffset = -1; columnOffset <= 1; columnOffset++)
            {
                int columnToCheck = sq1.Column + columnOffset;
                if (0 <= columnToCheck && columnToCheck < GameGrid.Count) // GameGrid is column-major
                {
                    for (int rowOffset = -1; rowOffset <= 1; rowOffset++)
                    {
                        int rowTocheck = sq1.Row + rowOffset;
                        if (0 <= rowTocheck && rowTocheck < GameGrid[columnToCheck].Count)
                        {
                            SquareViewModel sq2 = GameGrid[columnToCheck][rowTocheck];
                            if (sq1 != sq2)
                            {
                                SumsTable.AddSumsForNewSquare(sq1, sq2);
                            }
                        }
                    }
                }
            }
        }

        public virtual void HyakuFoundHandler(object sender, HyakuFoundEventArgs e)
        {
            MarkHyakuBlocks(e.Sum.SquaresInSum);
        }

        public virtual List<SquareViewModel> CheckTwoSquares(SquareViewModel sq1, SquareViewModel sq2)
        {
            List<SquareViewModel> retval = null;
            if ((sq1 != null && sq2 != null) &&
                (sq1.Value + sq2.Value == 100))
            {
                retval = new List<SquareViewModel>() { sq1, sq2 };
            }
            return retval;
        }

        public virtual void Tick(object sender, EventArgs e)
        {
//#if DEBUG
//            // turn off the timer so tick events don't pile up
//            Timer.Stop();
//#endif
            Counts += 1;
            if (Counts == hyakuSettings.SweepTimerPeriodSetting) {
                Counts = 0;
                DoSweep();
                AddTrashBlocksToAllColumns();
            }
//#if DEBUG
//            // turn on the timer again
//            Timer.Start();
//#endif
        }

        protected virtual void DoSweep()
        {
            SquareViewModel target = null;
            SquareViewModel source = null;
            List<SquareViewModel> movedSquares = new List<SquareViewModel>();
            List<SquareViewModel> newHyakus = new List<SquareViewModel>();
            foreach (List<SquareViewModel> column in GameGrid)
            {
                target = FirstHyaku(column);
                while (target != null) {
                    source = NextSource(column, target);

                    if (target.Score > 0) {
                        CountScore(target);
                    }

                    if (source != null) {
                        target.Value = source.Value;
                        target.CurrentState = source.CurrentState;
                        movedSquares.Add(target);
                        source.Reset();
                        EmptySquares += 1;
                    } else {
                        target.Reset();
                        EmptySquares += 1;
                    }

                    target = NextTarget(column, target);
                }
                target = null;
                source = null;
            }
            foreach (SquareViewModel sq in movedSquares) {
                FindNewHyakus(sq);
            }
        }

        protected virtual SquareViewModel FirstHyaku(List<SquareViewModel> column)
        {
            for (int i = column.Count - 1; i >= 0; i -= 1)
            {
                if (column[i] != null && column[i].CurrentState == SquareState.Hyaku)
                {
                    return column[i];
                }
            }
            return null;
        }

        protected virtual SquareViewModel NextTarget(List<SquareViewModel> column, SquareViewModel previousTarget)
        {
            int startIndex = previousTarget != null ? previousTarget.Row - 1 : column.Count - 1;
            for (int i = startIndex; i >= 0; i -= 1)
            {
                if (column[i] != null && (column[i].Score > 0 || column[i].Value == 0))
                {
                    return column[i];
                }
            }
            return null;
        }

        protected virtual SquareViewModel NextSource(List<SquareViewModel> column, SquareViewModel target)
        {
            int startIndex = target != null ? target.Row - 1 : column.Count - 1;
            for (int i = startIndex; i >= 0; i -= 1)
            {
                if (column[i] != null && (!(column[i].Score > 0) && column[i].Value > 0))
                {
                    return column[i];
                }
            }
            return null;
        }

        protected virtual SquareViewModel FirstEmpty(List<SquareViewModel> column)
        {
            for (int i = column.Count - 1; i >= 0; i -= 1) {
                if (column[i] != null && column[i].Value <= 0) {
                    return column[i];
                }
            }
            return null;
        }

        protected virtual void AddTrashBlocksToAllColumns()
        {
            // remember all the new squares, so we can check them for Hyaku later
            List<SquareViewModel> trashSquares = new List<SquareViewModel>();
            foreach (List<SquareViewModel> column in GameGrid) {
                SquareViewModel newSquare = AddTrashBlockToOneColumn(column);
                if (newSquare != null) {
                    trashSquares.Add(newSquare);
                } else {
                    OnGameOver(GameOverReason.PushedPastTop);
                    return;
                }
            }
            foreach (SquareViewModel sq in trashSquares) {
                FindNewHyakus(sq);
            }
        }

        public virtual void OnGameOver(GameOverReason reason)
        {
            if (GameOver != null) {
                GameOver(this, new GameOverEventArgs(reason));
            }
        }

        protected virtual SquareViewModel AddTrashBlockToOneColumn(List<SquareViewModel> column)
        {
            SquareViewModel firstEmptyBlock = FirstEmpty(column);
            if (firstEmptyBlock == null) {
                return null;
            } else {
                return InsertNumber(column, column.Count - 1, firstEmptyBlock, NextNumber);
            }
        }

        protected virtual SquareViewModel InsertNumber(List<SquareViewModel> column, int insertIndex, SquareViewModel firstEmptyBlock, int valueToInsert)
        {
            for (int i = firstEmptyBlock.Row; i < insertIndex; i += 1) {
                SquareViewModel target = column[i];
                SquareViewModel source = column[i + 1];
                target.Value = source.Value;
                target.CurrentState = source.CurrentState;
                source.Reset();
            }
            this.CurrentSquare = column[insertIndex];
            this.SendNumber(valueToInsert);
            return column[insertIndex];
        }

        protected virtual void CountScore(SquareViewModel target)
        {
            Score += target.Score;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (List<SquareViewModel> column in GameGrid) {
                foreach (SquareViewModel sqView in column) {
                    sb.Append(sqView.Value);
                    // add row marker ('.')
                    if (column.IndexOf(sqView) != column.Count - 1) {
                        sb.Append('.');
                    }
                }
                // add column marker (';')
                sb.Append(';');
            }
            // save the score at the end
            sb.Append(Score);
            return sb.ToString();
        }

        #endregion Methods

        public static GameBoardViewModel LoadGameFromString(string savedGame)
        {
            GameBoardViewModel gameBoard = new GameBoardViewModel();
            string[] columnArray = savedGame.Split(';');
            List<string> columns = new List<string> (columnArray);
            string scoreString = columns[columns.Count - 1];
            int score = 0;
            if (int.TryParse(scoreString, out score)) {
                gameBoard.Score = score;
            }
            columns.Remove(scoreString);
            for (int columnIndex = 0; columnIndex < columns.Count; columnIndex++) {
                string[] row = columns[columnIndex].Split('.');
                gameBoard.GameGrid.Insert(columnIndex, new List<SquareViewModel>());
                for (int rowIndex = 0; rowIndex < row.Length; rowIndex++) {
                    int parsedValue;
                    int.TryParse(row[rowIndex], out parsedValue);
                    SquareViewModel sq = new SquareViewModel(columnIndex, rowIndex);
                    gameBoard.CurrentSquare = sq;
                    gameBoard.SendNumber(parsedValue);
                    gameBoard.GameGrid[columnIndex].Insert(rowIndex, sq);
                }
            }

            return gameBoard;
        }

        public static GameBoardViewModel CreateNewGame()
        {
            HyakuSettings settings = new HyakuSettings();
            GameBoardViewModel gameBoard = new GameBoardViewModel();
            for (int columnIndex = 0; columnIndex < settings.GameSizeSetting ; columnIndex++) {
                gameBoard.GameGrid.Insert(columnIndex, new List<SquareViewModel>());
                for (int rowIndex = 0; rowIndex < settings.GameSizeSetting; rowIndex++) {
                    gameBoard.GameGrid[columnIndex].Insert(rowIndex, new SquareViewModel(columnIndex, rowIndex));
                }
            }

            return gameBoard;
        }
    }
}
