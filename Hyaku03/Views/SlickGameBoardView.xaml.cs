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

namespace Hyaku.Views
{
    public partial class SlickGameBoardView : UserControl
    {
        Rectangle currentRectangle = null;
        Point dragStart;
        Point dragEnd;
        int nextNumber;
        List<Rectangle> rectangles = new List<Rectangle>(9);
        int[] rowTops = new int[] { 0,41,82,123,164,205,246,287,328 };

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
        }

        private void SetNextNumber()
        {
            nextNumber = GameBoard.NextNumber;
            NextNumberImage.Source = new BitmapImage(GetImageUriFromNumber(nextNumber));
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
            GeneralTransform canvasFactor = e.ManipulationContainer.TransformToVisual(Columns);
            dragStart = canvasFactor.Transform(e.ManipulationOrigin);
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

            SquareViewModel currentSquare = GameBoard.SelectSquare(columnIndex);
            if (currentSquare != null) {
                Storyboard dropAnimation = (Storyboard)this.Resources["DropAnimationStoryboard"];
                
                // TODO set end position of animation

                // hook up the animation complete handler
                dropAnimation.Completed += new EventHandler(dropAnimation_Completed);
                dropAnimation.Begin();

                // turn off "Manipulation" handlers until animation is complete
                DisconnectManipulationHandlers();
            }
        }

        void dropAnimation_Completed(object sender, EventArgs e)
        {
            Storyboard dropAnimation = sender as Storyboard;
            // create new square at the correct grid location
            if (dropAnimation != null)
            {
                // draw the number to its place on the grid

                dropAnimation.Stop();
                // annimation handler is added when the animation begins
                dropAnimation.Completed -= new EventHandler(dropAnimation_Completed);
                
                int column = GameBoard.CurrentSquare.Column;
                int row = GameBoard.CurrentSquare.Row;
                Rectangle r = rectangles[column];

                // send number to game engine
                GameBoard.SendNumber(nextNumber);
                
                Image numberImage = new Image();           
                numberImage.Source = new BitmapImage(GetImageUriFromNumber(nextNumber));
                numberImage.SetValue(Canvas.TopProperty, (double)rowTops[row]);
                numberImage.SetValue(Canvas.LeftProperty, (double)r.GetValue(Canvas.LeftProperty));
                Columns.Children.Add(numberImage);
                                                           
                // set NextNumberImage to new value        
                SetNextNumber();
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

        private Uri GetImageUriFromNumber(int nextNumber)
        {
            Uri imageUri = null;
            string imagePathFormatString = @"../Images/NumberBlocks/{0}.png";
            string imagePath = string.Format(imagePathFormatString, nextNumber.ToString("D2"));
            //imagePath = string.Format(imagePathFormatString, "hyaku");
            imageUri = new Uri(imagePath, UriKind.Relative);
            return imageUri;
        }
    }
}
