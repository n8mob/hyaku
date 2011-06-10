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
using System.Windows.Media.Imaging;
using System.IO;
using Hyaku.Data;
using System.Diagnostics;
using Hyaku;

namespace Hyaku.Views
{
    public partial class SlickGameBoardView : UserControl
    {
        string defaultTheme = "blossomTheme";
        int[] rowTops = new int[] { 0,41,82,123,164,205,246,287,328 };
        Rectangle currentRectangle = null;
        Point dragStart;
        Point dragEnd;
        int nextNumber;
        List<Rectangle> rectangles = new List<Rectangle>(9);
        Image currentImage;
        Image[,] squares;

        private GameBoardViewModel _gameBoard;

        public GameBoardViewModel GameBoard
        {
            get { return _gameBoard; }
            set
            {
                _gameBoard = value;
                if (_gameBoard != null)
                {
                    BindBoard();
                }
            }
        }

        private void BindBoard()
        {
            SetNextNumber();
            this.DataContext = GameBoard;
            GameBoard.GameOver += new GameOverEventHandler(GameBoard_GameOver);
            GameBoard.HyakusFound += new HyakusFoundEventHandler(GameBoard_HyakusFound);
            GameBoard.SquareDeleted += new SquareDeletedEventHandler(GameBoard_SquareDeleted);
            GameBoard.SquareMoved += new SquareMovedEventHandler(GameBoard_SquareMoved);
            GameBoard.NumberDrop += new NumberDropEventHandler(GameBoard_NumberDrop);
            GameBoard.NumberAdded += new NumberAddedEventHandler(GameBoard_NumberAdded);
            if (!GameBoard.Timer.IsEnabled) {
                GameBoard.Timer.Start();
            }
        }

        void GameBoard_NumberAdded(object sender, NumberAddedEventArgs e)
        {
            double rowTop = (double)rowTops[e.Square.Row];
            double columnLeft = (double)rectangles[e.Square.Column].GetValue(Canvas.LeftProperty);

            Image numberImage = new Image();
            numberImage.Source = GetNormalImageUriFromNumber(e.Square.Value);
            numberImage.SetValue(Canvas.TopProperty, rowTop);
            numberImage.SetValue(Canvas.LeftProperty, columnLeft);
            Columns.Children.Add(numberImage);
            squares[e.Square.Column, e.Square.Row] = numberImage;
            numberImage.SetValue(Canvas.ZIndexProperty, 10);
        }

        void GameBoard_SquareDeleted(object sender, SquareDeletedEventArgs e)
        {
            DeleteSquare(e.Column, e.Row);
        }

        void GameBoard_SquareMoved(object sender, SquareMovedEventArgs e)
        {
            MoveSquare(e.OldColumn, e.OldRow, e.NewColumn, e.NewRow);
        }

        void GameBoard_HyakusFound(object sender, HyakuFoundEventArgs e)
        {
            foreach (Square sq in e.SquaresToMark) {
                if (squares[sq.Column, sq.Row] == null) {
                    squares[sq.Column, sq.Row] = new Image();
                }

                squares[sq.Column, sq.Row].Source = GetHyakuImageUriFromNumber(sq.Value);
            }
        }

        void GameBoard_GameOver(object sender, GameOverEventArgs e)
        {
            MessageBox.Show(e.Reason.ToString(), "Game Over", MessageBoxButton.OK);

            GameBoard.GameOver -= new GameOverEventHandler(GameBoard_GameOver);

            GameBoard = GameBoardViewModel.CreateNewGame();
        }

        private void SetNextNumber()
        {
            nextNumber = GameBoard.GetNextNumber();
            NextNumberImage.Source = GetNormalImageUriFromNumber(nextNumber);
        }

        public SlickGameBoardView()
        {
            InitializeComponent();

            rectangles.Insert(0, Column0);
            rectangles.Insert(1, Column1);
            rectangles.Insert(2, Column2);
            rectangles.Insert(3, Column3);
            rectangles.Insert(4, Column4);
            rectangles.Insert(5, Column5);
            rectangles.Insert(6, Column6);
            rectangles.Insert(7, Column7);
            rectangles.Insert(8, Column8);

            squares = new Image[9, 9];

            ConnectManipulationHandlers();
            GameBoard = GameBoardViewModel.CreateNewGame();
        }

        private void ConnectManipulationHandlers()
        {
            Columns.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(Columns_ManipulationStarted);
            Columns.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(Columns_ManipulationDelta);
            Columns.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(Columns_ManipulationCompleted);
            Columns.MouseLeftButtonDown += new MouseButtonEventHandler(Columns_MouseLeftButtonDown);
        }

        private void DisconnectManipulationHandlers()
        {
            Columns.ManipulationStarted -= new EventHandler<ManipulationStartedEventArgs>(Columns_ManipulationStarted);
            Columns.ManipulationDelta -= new EventHandler<ManipulationDeltaEventArgs>(Columns_ManipulationDelta);
            Columns.ManipulationCompleted -= new EventHandler<ManipulationCompletedEventArgs>(Columns_ManipulationCompleted);
            Columns.MouseLeftButtonDown -= new MouseButtonEventHandler(Columns_MouseLeftButtonDown);
        }

        void Columns_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ChooseCurrentRectangle(e.GetPosition(Columns));
        }

        void Columns_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            // I noticed that someimes the number doesn't drop with just a tap
            // maybe this line will help that.
            ChooseCurrentRectangle(e.ManipulationOrigin);
            GeneralTransform canvasFactor = e.ManipulationContainer.TransformToVisual(Columns);
            dragStart = canvasFactor.Transform(e.ManipulationOrigin);
            // if the ChooseCurrentRectangle line above doesn't fix the problem, try the line below
            dragEnd = canvasFactor.Transform(e.ManipulationOrigin);
        }

        void Columns_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            dragEnd = new Point(dragStart.X + e.CumulativeManipulation.Translation.X, dragStart.Y + e.CumulativeManipulation.Translation.Y);
            ChooseCurrentRectangle(dragEnd);
        }

        void Columns_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            dragEnd = new Point(dragStart.X + e.TotalManipulation.Translation.X, dragStart.Y + e.TotalManipulation.Translation.Y);
            ChooseCurrentRectangle(dragEnd);
            Rectangle r = GetRectangle(dragEnd);
            int columnIndex = rectangles.IndexOf(r);

            GameBoard.NumberDrop += new NumberDropEventHandler(GameBoard_NumberDrop);
            SquareViewModel currentSquare;
            GameBoard.SelectSquare(columnIndex, out currentSquare);
        }

        void GameBoard_NumberDrop(object sender, NumberAddedEventArgs e)
        {
            DropNumberToCurrentSquare(e.SelectedSquare);
        }

        private void DropNumberToCurrentSquare(SquareViewModel currentSquare)
        {
            if (currentSquare != null) {
                Storyboard dropAnimation = (Storyboard)this.Resources["DropKeyFrameAnimationStoryboard"];
                if (dropAnimation == null) {
                    return;
                }

                // turn off "Manipulation" handlers until animation is complete
                DisconnectManipulationHandlers();

                // hook up the animation complete handler
                dropAnimation.Completed += new EventHandler(dropAnimation_Completed);
                dropAnimation.Begin();
            }
        }

        public void DeleteSquare(int column, int row)
        {
            // insert hyaku remove animation here
            Image squareToDelete = squares[column, row];
            if (squareToDelete != null) {
                Columns.Children.Remove(squareToDelete);
            }
        }

        public void MoveSquare(int oldColumn, int oldRow, int newColumn, int newRow)
        {
            currentImage = squares[oldColumn, oldRow];
            squares[oldColumn, oldRow] = null;
            squares[newColumn, newRow] = currentImage;

            DoubleAnimation moveSquareAnimation = new DoubleAnimation();
            moveSquareAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            moveSquareAnimation.From = rowTops[oldRow];
            moveSquareAnimation.To = rowTops[newRow];

            Storyboard moveSquareStoryBoard = new Storyboard();
            moveSquareStoryBoard.Children.Add(moveSquareAnimation);
            Storyboard.SetTarget(moveSquareStoryBoard, currentImage);
            Storyboard.SetTargetProperty(moveSquareAnimation, new PropertyPath("(Canvas.Top)"));
            //moveSquareStoryBoard.Completed += new EventHandler(moveSquareAnimation_Completed);
            moveSquareStoryBoard.Begin();
        }

        //void moveSquareAnimation_Completed(object sender, EventArgs e)
        //{
        //    Storyboard moveSquareStoryBoard = sender as Storyboard;
        //    if (moveSquareStoryBoard == null) {
        //        return;
        //    }


        //}

        void dropAnimation_Completed(object sender, EventArgs e)
        {
            Storyboard dropAnimationStoryboard = sender as Storyboard;
            // create new square at the correct grid location
            if (dropAnimationStoryboard != null)
            {
                // draw the number to its place on the grid

                dropAnimationStoryboard.Stop();
                // annimation handler is added when the animation begins
                dropAnimationStoryboard.Completed -= new EventHandler(dropAnimation_Completed);

                if (GameBoard.CurrentSquare == null) {
                    Debug.WriteLine("GameBoard.CurrentSquare is empty");
                    return;
                }

                int column = GameBoard.CurrentSquare.Column;
                int row = GameBoard.CurrentSquare.Row;
                Rectangle r = rectangles[column];

                // send number to game engine
                GameBoard.SendNumber(nextNumber);
                                                           
                // set NextNumberImage to new value        
                SetNextNumber();
                // set rectangle back to black
                if (currentRectangle != null) {
                    currentRectangle.Fill = new SolidColorBrush(Colors.Black);
                }
                // reconnect "Manipulation" handlers for next time.
                ConnectManipulationHandlers();              
            }                                              
            // find new hyakus and run their animations
        }

        private void ChooseCurrentRectangle(Point dragEnd)
        {
            Rectangle r = GetRectangle(dragEnd);

            if (r == null)
            {
                return;
            }

            r.Fill = new SolidColorBrush(Colors.Gray);
            NextNumberImage.SetValue(Canvas.LeftProperty, (double)r.GetValue(Canvas.LeftProperty) + (double)gridBorders.GetValue(Canvas.LeftProperty));

            if (currentRectangle != null && currentRectangle != r)
            {
                currentRectangle.Fill = new SolidColorBrush(Colors.Black);
            }

            currentRectangle = r;

        }

        private Rectangle GetRectangle(Point p)
        {
            if (p.X < (double)Column1.GetValue(Canvas.LeftProperty))
            {
                return Column0;
            }
            else if (p.X < (double)Column2.GetValue(Canvas.LeftProperty))
            {
                return Column1;
            }
            else if (p.X < (double)Column3.GetValue(Canvas.LeftProperty))
            {
                return Column2;
            }
            else if (p.X < (double)Column4.GetValue(Canvas.LeftProperty))
            {
                return Column3;
            }
            else if (p.X < (double)Column5.GetValue(Canvas.LeftProperty))
            {
                return Column4;
            }
            else if (p.X < (double)Column6.GetValue(Canvas.LeftProperty))
            {
                return Column5;
            }
            else if (p.X < (double)Column7.GetValue(Canvas.LeftProperty))
            {
                return Column6;
            }
            else if (p.X < (double)Column8.GetValue(Canvas.LeftProperty))
            {
                return Column7;
            }
            else
            {
                return Column8;
            }
        }

        public void LeavingPage()
        {
            if (GameBoard != null) {
                GameBoard.Stop();
            }
        }

        private BitmapImage GetHyakuImageUriFromNumber(int number)
        {
            ImageType imageType = ImageType.Hyaku;
            return GetThemedImageUriFromNumber(number, imageType);
        }

        private BitmapImage GetNormalImageUriFromNumber(int number)
        {
            ImageType imageType = ImageType.Normal;
            return GetThemedImageUriFromNumber(number, imageType);
        }

        private BitmapImage GetThemedImageUriFromNumber(int number, ImageType imageType)
        {
            return GetThemedImageUriFromNumber(number, defaultTheme, imageType);
        }

        private BitmapImage GetThemedImageUriFromNumber(int nextNumber, string theme, ImageType imageType)
        {
            Uri imageUri = null;

            string imagePathFormatString = @"../Images/Themes/{0}/{1}/{2}.png";
            string imagePath = string.Format(imagePathFormatString, theme, imageType.ToString(), nextNumber.ToString("D2"));
            
            imageUri = new Uri(imagePath, UriKind.Relative);
            return new BitmapImage(imageUri);
        }
    }

    enum ImageType
    {
        Hyaku,
        Normal
    }
}
