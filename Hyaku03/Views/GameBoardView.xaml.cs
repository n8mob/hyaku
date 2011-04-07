using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Hyaku.ViewModels;
using System.Windows.Threading;

namespace Hyaku.Views
{
    public partial class GameBoardView : UserControl
    {
        internal DispatcherTimer timer;

        private GameBoardViewModel _gameBoard;
        public GameBoardViewModel GameBoard
        {
            get { return _gameBoard; }
            set {
                _gameBoard = value;
                BindBoard();
            }
        }

        public GameBoardView()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 15);
        }

        private void ChildSquareClicked(object sender, EventArgs e)
        {
            SquareView selectedUiSquare = sender as SquareView;
            if (selectedUiSquare == null)
            {
                return;
            }

            SquareViewModel selectedSquareModel = selectedUiSquare.Square;
            if (selectedSquareModel == null)
            {
                return;
            }

            // find selected column
            int selectedColumn = selectedSquareModel.Column;
            // find lowest empty square
            SquareViewModel lowestEmptySquare = null;
            List<SquareViewModel> column = GameBoard.GameGrid[selectedColumn];
            for (int i = column.Count - 1; i >= 0; i -= 1)
            {
                if (column[i] != null && column[i].Value == 0)
                {
                    lowestEmptySquare = column[i];
                    break;
                }
            }
            // select lowest empty square instead of sender
            if (lowestEmptySquare != null)
            {
                selectedSquareModel = lowestEmptySquare;
            }
            else
            {
                // no squares
                // TODO handle full column
                return;
            }

            if (GameBoard.CurrentSquare != null)
            {
                GameBoard.CurrentSquare.IsCurrent = false;
            }

            if (GameBoard.CurrentSquare == selectedSquareModel)
            {
                GameBoard.CurrentSquare = null;
            }
            else
            {
                GameBoard.CurrentSquare = selectedSquareModel;
                GameBoard.CurrentSquare.IsCurrent = true;
                GameBoard.SendNumber((int)NextNumberTextBlock.Tag);
                
                NextNumberTextBlock.Tag = GameBoard.NextNumber;
                NextNumberTextBlock.Text = NextNumberTextBlock.Tag.ToString();
            }

            if (!timer.IsEnabled)
            {
                timer.Start();
            }
        }

        private void BindBoard()
        {
            NextNumberTextBlock.Tag = GameBoard.NextNumber;
            NextNumberTextBlock.Text = NextNumberTextBlock.Tag.ToString();
            for (int columnIndex = 0; columnIndex < 9; columnIndex++)
            {
                for (int rowIndex = 0; rowIndex < 9; rowIndex++)
                {
                    SquareViewModel square = GameBoard.GameGrid[columnIndex][rowIndex];
                    List<SquareViewModel> hyakuBlocks = GameBoard.CheckSurroundingSquares(square);
                    if (hyakuBlocks == null) {
                        square.IsHyakuBlock = false;
                    } else {
                        GameBoardViewModel.MarkHyakuBlocks(hyakuBlocks);
                    }
                    SquareView uiSquare = new SquareView(square);

                    GameGrid.Children.Add(uiSquare);
                    uiSquare.SquareClicked += new EventHandler(ChildSquareClicked);
                }
            }

            this.DataContext = GameBoard;
            timer.Tick += new EventHandler(GameBoard.Sweep);
        }
    }
}
