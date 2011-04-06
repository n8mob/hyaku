using System;
using System.Net;
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

namespace Hyaku.ViewModels
{
    public class GameBoardViewModel : ViewModelBase
    {
        const string savedGameFileName = "hyaku.dat";
        List<int> numberSelections;
        private int _gameSize;
        private int _randomListSize = 100;
        private RandomListGenerator _randomListGenerator = null;
        private int _score;

        public int GameSize
        {
            get {
                if (_gameSize < 2) {
                    _gameSize = (int)(IsolatedStorageSettings.ApplicationSettings["gameSize"] ?? 9);
                }
                return _gameSize;
            }
        }

        internal int RandomListSize
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

        public RandomListGenerator RandomListGenerator
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

        public SquareViewModel CurrentSquare { get; set; }
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
                    numberSelections = RandomListGenerator.SelectRandomItems(RandomListSize, new List<int>(new int[] { 5, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95 }));
                }

                int nextNumber = numberSelections[0];
                numberSelections.RemoveAt(0);
                return nextNumber;
            }
        }

        public GameBoardViewModel()
        {
            GameGrid = new List<List<SquareViewModel>>(GameSize);
        }

        public void SendNumber(int theNumber)
        {
            if (CurrentSquare != null)
            {
                CurrentSquare.Value = theNumber;
                List<SquareViewModel> hyaku = null;
                hyaku = CheckSurroundingSquares(CurrentSquare);
                if (hyaku != null)
                {
                    MarkHyakuBlocks(hyaku);
                }
                CurrentSquare.IsCurrent = false;
                CurrentSquare.IsLocked = true;
                CurrentSquare = null;
            }
        }

        private static void MarkHyakuBlocks(List<SquareViewModel> hyaku)
        {
            foreach (SquareViewModel sq in hyaku)
            {
                sq.IsHyakuBlock = true;
            }
        }

        public List<SquareViewModel> CheckSurroundingSquares(SquareViewModel sq1)
        {
            List<SquareViewModel> hyaku = null;
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
                                hyaku = CheckTwoSquares(sq1, sq2);
                                if (hyaku != null)
                                {
                                    return hyaku;
                                }
                            }
                        }
                    }
                }
            }
            return hyaku;
        }

        public List<SquareViewModel> CheckTwoSquares(SquareViewModel sq1, SquareViewModel sq2)
        {
            List<SquareViewModel> retval = null;
            if ((sq1 != null && sq2 != null) &&
                (sq1.Value + sq2.Value == 100))
            {
                retval = new List<SquareViewModel>() { sq1, sq2 };
            }
            return retval;
        }

        public void Sweep(object sender, EventArgs e)
        {
#if DEBUG
            // turn off the timer so tick events don't pile up
            GameBoardView gameBoardView = sender as GameBoardView;
            if (gameBoardView != null)
            {
                gameBoardView.timer.Stop();
            }
#endif
            DoSweep();
#if DEBUG
            // turn on the timer again
            if (gameBoardView != null)
            {
                gameBoardView.timer.Start();
            }
#endif
        }

        private void DisplayScore()
        {
            throw new NotImplementedException();
        }

        private void DoSweep()
        {
            SquareViewModel target = null;
            SquareViewModel source = null;
            List<SquareViewModel> movedSquares = new List<SquareViewModel>();
            List<SquareViewModel> newHyakus = new List<SquareViewModel>();
            foreach (List<SquareViewModel> column in GameGrid)
            {
                target = FirstHyaku(column);
                while (target != null)
                {
                    {
                        source = NextSource(column, target);

                        if (target.IsHyakuBlock)
                        {
                            CountScore(target);
                        }

                        if (source != null)
                        {
                            target.Value = source.Value;
                            target.CurrentState = source.CurrentState;
                            //newHyakus = CheckSurroundingSquares(target);
                            //if (newHyakus != null)
                            //{
                            //    MarkHyakuBlocks(newHyakus);
                            //}
                            movedSquares.Add(target);
                            source.Reset();
                        }
                        else
                        {
                            target.Reset();
                        }
                    }

                    target = NextTarget(column, target);
                }
                target = null;
                source = null;
            }
            foreach (SquareViewModel sq in movedSquares)
            {
                List<SquareViewModel> comboHyakus = CheckSurroundingSquares(sq);
                if (comboHyakus != null)
                {
                    newHyakus.AddRange(comboHyakus);
                }
            }
            MarkHyakuBlocks(newHyakus);
        }

        private SquareViewModel FirstHyaku(List<SquareViewModel> column)
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

        private SquareViewModel NextTarget(List<SquareViewModel> column, SquareViewModel previousTarget)
        {
            int startIndex = previousTarget != null ? previousTarget.Row - 1 : column.Count - 1;
            for (int i = startIndex; i >= 0; i -= 1)
            {
                if (column[i] != null && (column[i].IsHyakuBlock || column[i].Value == 0))
                {
                    return column[i];
                }
            }
            return null;
        }

        private SquareViewModel NextSource(List<SquareViewModel> column, SquareViewModel target)
        {
            int startIndex = target != null ? target.Row - 1 : column.Count - 1;
            for (int i = startIndex; i >= 0; i -= 1)
            {
                if (column[i] != null && (!column[i].IsHyakuBlock && column[i].Value > 0))
                {
                    return column[i];
                }
            }
            return null;
        }

        private void CountScore(SquareViewModel target)
        {
            Score += 100;
        }

        public void ClearGrid()
        {
            foreach (List<SquareViewModel> column in GameGrid)
            {
                foreach (SquareViewModel square in column)
                {
                    square.Value = 0;
                    square.IsHyakuBlock = false;
                }
            }
        }

        public static GameBoardViewModel CreateNewGame()
        {
            GameBoardViewModel gameBoard = new GameBoardViewModel();

            for (int columnIndex = 0; columnIndex < 9; columnIndex++)
            {
                gameBoard.GameGrid.Insert(columnIndex, new List<SquareViewModel>());
                for (int rowIndex = 0; rowIndex < 9; rowIndex++)
                {
                    gameBoard.GameGrid[columnIndex].Insert(rowIndex, new SquareViewModel(columnIndex, rowIndex));
                }
            }

            return gameBoard;
        }
    }
}
