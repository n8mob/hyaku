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
using System.Windows.Navigation;

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
            timer.Interval = new TimeSpan(0, 0, 1);
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
            this.GameGrid.ColumnDefinitions.Clear();
            this.GameGrid.RowDefinitions.Clear();
            for (int columnIndex = 0; columnIndex < GameBoard.GameGrid.Count; columnIndex++)
            {
                this.GameGrid.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    MinWidth = GameGrid.Width / (double)GameBoard.GameGrid.Count
                });
                for (int rowIndex = 0; rowIndex < GameBoard.GameGrid[0].Count; rowIndex++)
                {
                    if (this.GameGrid.RowDefinitions.Count <= rowIndex) {
                        this.GameGrid.RowDefinitions.Add(new RowDefinition()
                        {
                            MinHeight = this.GameGrid.Height / GameBoard.GameGrid.Count
                        });
                    }
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
            timer.Tick += new EventHandler(GameBoard.Tick);
        }
    }
}
