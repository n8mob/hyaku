using System;
using System.Linq;
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
using Hyaku.Data;

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
        private DistanceSumsStorage _distanceSumsStorage = null;
        private SquareViewModel _currentSquare = null;
        private int _score;
        private int _tickCount;
        private int _emptyBlockCount;
        private int _hyakuBlockCount;
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

        public DistanceSumsStorage SumsStorage
        {
            get
            {
                if (_distanceSumsStorage == null) {
#if DEBUG
                    Debug.WriteLine("Initializing SumStorage: {0}", this.GetHashCode());
#endif
                    _distanceSumsStorage = new DistanceSumsStorage();
                    _distanceSumsStorage.HyakuFound += new HyakuFoundEventHandler(HyakuFoundHandler);
                }
                return _distanceSumsStorage;
            }
            set
            {
                _distanceSumsStorage = value;
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

        public int TickCount
        {
            get
            {
                return _tickCount;
            }
            set
            {
                _tickCount = value;
                NotifyPropertyChanged("TickCount");
            }
        }

        public int EmptyBlockCount
        {
            get
            {
                return _emptyBlockCount;
            }
            set
            {
                //Debug.WriteLine("EmptyBlockCount changing from {0} to {1} - {2} HyakuBlocks", _emptyBlockCount, value, HyakuBlockCount);
                int adjustedValue = value + HyakuBlockCount;
                if (adjustedValue <= _minAvailibleSquares)
                {
                    OnGameOver(GameOverReason.LessThanZero);
                }
                else if (adjustedValue > _maxAvailibleSquares)
                {
                    // WTF?
                    OnGameOver(GameOverReason.MoreThanMax);
                }
                else
                {
                    _emptyBlockCount = value;
                    NotifyPropertyChanged("EmptyBlockCount");
                }
            }
        }

        public int HyakuBlockCount
        {
            get
            {
                return _hyakuBlockCount;
            }
            set
            {
                //Debug.WriteLine("HyakuBlockCount changing from {0} to {1}", _hyakuBlockCount, value);
                _hyakuBlockCount = value;
                NotifyPropertyChanged("HyakuBlockCount");
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
            EmptyBlockCount = _maxAvailibleSquares;
            HyakuBlockCount = _minAvailibleSquares;
        }

        #endregion Constructors

        #region Methods
        
        public virtual SquareViewModel SelectSquare(int columnIndex)
        {
            List<SquareViewModel> column = GameGrid[columnIndex];
            SquareViewModel lowestEmptySquare = null;
            for (int i = column.Count - 1; i >= 0; i -= 1) {
                if (column[i] != null && column[i].Value == 0) {
                    lowestEmptySquare = column[i];
                    break;
                }
            }

            if (lowestEmptySquare == null) {
                return null;
            }

            if (CurrentSquare != null) {
                CurrentSquare.IsCurrent = false;
            }

            if (CurrentSquare == lowestEmptySquare) {
                CurrentSquare = null;
            } else {
                CurrentSquare = lowestEmptySquare;
            }

            return lowestEmptySquare;
        }

        public virtual void SendNumber(int theNumber)
        {
            if (CurrentSquare != null && CurrentSquare.Value == 0) {
                // square is not occupied, let's set it.
                if (EmptyBlockCount < _minAvailibleSquares) {
                    OnGameOver(GameOverReason.RanOutOfSpace);
                } else {
                    CurrentSquare.Value = theNumber;
                    if (theNumber > 0) {
                        // TODO this doesn't seem like the right call
                        Square sq = SumsStorage.SaveSquare(CurrentSquare.Column, CurrentSquare.Row, CurrentSquare.Value);
                        SumsStorage.InitializeSumsForSquare(sq);
                        CurrentSquare.IsLocked = true;
                        EmptyBlockCount -= 1;
                        FindNewHyakus(CurrentSquare);
                    } else { // the number is <= 0
                        // ??
                    }

                    CurrentSquare.IsCurrent = false;
                    CurrentSquare = null;
                }
            }
        }

        public virtual void MarkHyakuBlocks(List<Square> hyaku)
        {
#if DEBUG
            List<string> squareStrings = new List<string>();
#endif
            foreach (Square sq in hyaku)
            {
                SquareViewModel sqV = GameGrid[sq.Column][sq.Row];
#if DEBUG
                squareStrings.Add(sqV.ToString());
#endif
                if (!sqV.IsHyakuBlock)
                {
                    sqV.IsHyakuBlock = true;
                    HyakuBlockCount += 1;
                }
            }
#if DEBUG
            //Debug.WriteLine("marking hyaku: " + string.Join(",", squareStrings.ToArray()));
#endif
        }

        public virtual void FindNewHyakus(SquareViewModel movedSquare)
        {
            if (movedSquare.Value <= 0) {
                return;
            }

            for (int columnOffset = -1; columnOffset <= 1; columnOffset++)
            {
                int columnToCheck = movedSquare.Column + columnOffset;
                if (0 <= columnToCheck && columnToCheck < GameGrid.Count) // GameGrid is column-major
                {
                    for (int rowOffset = -1; rowOffset <= 1; rowOffset++)
                    {
                        int rowTocheck = movedSquare.Row + rowOffset;
                        if (0 <= rowTocheck && rowTocheck < GameGrid[columnToCheck].Count)
                        {
                            Square squareToCheck = SumsStorage.GetSquare(columnToCheck, rowTocheck);
                            if (squareToCheck != null) {
                                SquareViewModel neighborSquare = GameGrid[columnToCheck][rowTocheck];
                                if (movedSquare != neighborSquare && movedSquare.Value > 0) {
                                    SumsStorage.AddSumsForNewSquare(movedSquare, neighborSquare);
                                }
                            }
                        }
                    }
                }
            }
        }

        public virtual void HyakuFoundHandler(object sender, HyakuFoundEventArgs e)
        {
            MarkHyakuBlocks(e.SquaresToMark);
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
            TickCount += 1;
            if (TickCount == hyakuSettings.SweepTimerPeriodSetting) {
                TickCount = 0;
                DoSweep();
                if (hyakuSettings.EnableTrashRowsSetting == true) {
                    AddTrashBlocksToAllColumns();
                }
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
                //Debug.WriteLine("Sweeping column {0}", GameGrid.IndexOf(column));

                target = FirstHyaku(column);
                while (target != null) {
                    source = NextSource(column, target);

                    if (source != null) {
                        //Debug.WriteLine("Moving source {0} to target {1}", source.ToString(), target.ToString());
                        ClearSquare(target);
                        CurrentSquare = target;
                        target.Value = source.Value;
                        target.IsLocked = source.IsLocked;
                        target.IsCurrent = false;
                        EmptyBlockCount -= 1;
                        SumsStorage.SaveSquare(target.Column, target.Row, source.Value);
                        movedSquares.Add(target);
                        ClearSquare(source);
                    } else {
                        //Debug.WriteLine("No source, clearing target {0}", target.ToString());
                        ClearSquare(target);
                    }

                    target = NextTarget(column, target);
                }
                target = null;
                source = null;
            }
            foreach (SquareViewModel sq in movedSquares)
            {
                FindNewHyakus(sq);
            }
        }

        private void ClearSquare(SquareViewModel sq)
        {
            if (sq.IsHyakuBlock)
            {
                CountScore(sq);
                HyakuBlockCount -= 1;
            }
            if (sq.Value > 0)
            {
                EmptyBlockCount += 1;
            }
            sq.Reset();
            SumsStorage.DeleteSquareAndCascade(sq.Column, sq.Row);
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
            //Debug.WriteLine("Finding next target in column {0} starting from {1}", GameGrid.IndexOf(column), previousTarget.ToString());
            int startIndex = previousTarget != null ? previousTarget.Row - 1 : column.Count - 1;
            for (int i = startIndex; i >= 0; i -= 1)
            {
                //Debug.WriteLine("Checking row {0}", i);
                if (column[i] != null)
                {
                    //Debug.WriteLine("Row {0}: {1}", i, column[i].ToString());
                    if ((column[i].IsHyakuBlock || column[i].Value == 0))
                    {
                        return column[i];
                    }
                }
                else
                {
                    //Debug.WriteLine("Row {0}: null");
                }
            }
            return null;
        }

        protected virtual SquareViewModel NextSource(List<SquareViewModel> column, SquareViewModel target)
        {
            int startIndex = target != null ? target.Row - 1 : column.Count - 1;
            for (int i = startIndex; i >= 0; i -= 1)
            {
                if (column[i] != null && (!(column[i].IsHyakuBlock) && column[i].Value > 0))
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
                ClearSquare(target);
                target.Value = source.Value;
                target.IsLocked = source.IsLocked;
                target.IsHyakuBlock = source.IsHyakuBlock;
                target.IsCurrent = source.IsCurrent;
                ClearSquare(source);
            }
            this.CurrentSquare = column[insertIndex];
            this.SendNumber(valueToInsert);
            return column[insertIndex];
        }

        protected virtual void CountScore(SquareViewModel target)
        {
            Score += 100;
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
            for (ushort columnIndex = 0; columnIndex < columns.Count; columnIndex++) {
                string[] row = columns[columnIndex].Split('.');
                gameBoard.GameGrid.Insert(columnIndex, new List<SquareViewModel>());
                for (ushort rowIndex = 0; rowIndex < row.Length; rowIndex++) {
                    int parsedValue;
                    int.TryParse(row[rowIndex], out parsedValue);
                    SquareViewModel sq = new SquareViewModel(columnIndex, rowIndex);
                    gameBoard.CurrentSquare = sq;
                    gameBoard.GameGrid[columnIndex].Insert(rowIndex, sq);
                    gameBoard.SendNumber(parsedValue);
                }
            }

            return gameBoard;
        }

        public static GameBoardViewModel CreateNewGame()
        {
            HyakuSettings settings = new HyakuSettings();
            GameBoardViewModel gameBoard = new GameBoardViewModel();
            for (ushort columnIndex = 0; columnIndex < settings.GameSizeSetting; columnIndex++) {
                gameBoard.GameGrid.Insert(columnIndex, new List<SquareViewModel>());
                for (ushort rowIndex = 0; rowIndex < settings.GameSizeSetting; rowIndex++) {
                    SquareViewModel square = new SquareViewModel(columnIndex, rowIndex);
                    gameBoard.CurrentSquare = square;
                    gameBoard.SendNumber(0);
                    gameBoard.GameGrid[columnIndex].Insert(rowIndex, square);
                }
            }

            return gameBoard;
        }

        internal void Stop()
        {
            this.Timer.Tick -= new EventHandler(Tick);
        }
    }
}
