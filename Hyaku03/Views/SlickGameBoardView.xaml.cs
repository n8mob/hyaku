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
            Columns.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(Columns_ManipulationStarted);
            Columns.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(Columns_ManipulationDelta);
            Columns.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(Columns_ManipulationCompleted);
            Columns.MouseLeftButtonDown += new MouseButtonEventHandler(Columns_MouseLeftButtonDown);
            GameBoard = new GameBoardViewModel();
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

            Storyboard dropAnimation = (Storyboard)this.Resources["DropAnimationStoryboard"];

            dropAnimation.Completed += new EventHandler(dropAnimation_Completed);
            dropAnimation.Begin();
        }

        void dropAnimation_Completed(object sender, EventArgs e)
        {
            Storyboard dropAnimation = sender as Storyboard;
            // create new square at the correct grid location
            if (dropAnimation != null)
            {
                // draw the number to its place on the grid

                dropAnimation.Stop();

                // send number to game engine
                GameBoard.SendNumber(nextNumber);

                // set NextNumberImage to new value
                SetNextNumber();
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
