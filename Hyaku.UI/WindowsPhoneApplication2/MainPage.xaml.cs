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
using Microsoft.Phone.Controls;

namespace WindowsPhoneApplication2
{
    public partial class MainPage : PhoneApplicationPage
    {
        Rectangle currentRectangle = null;
        Point dragStart;
        Point dragEnd;

        public MainPage()
        {
            InitializeComponent();
            Columns.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(Columns_ManipulationStarted);
            Columns.ManipulationDelta += new EventHandler<ManipulationDeltaEventArgs>(Columns_ManipulationDelta);
            Columns.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(Columns_ManipulationCompleted);
            Columns.MouseLeftButtonDown += new MouseButtonEventHandler(Columns_MouseLeftButtonDown);


        }

        void Columns_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ChooseCurrentRectangle(e.GetPosition(Columns));
        }

        void Columns_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            GeneralTransform canvasFactor = e.ManipulationContainer.TransformToVisual(Columns);
            dragStart = canvasFactor.Transform(e.ManipulationOrigin);
            coords.Text = "Start: " + dragStart;
        }

        void Columns_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            dragEnd = new Point(dragStart.X + e.CumulativeManipulation.Translation.X, dragStart.Y + e.CumulativeManipulation.Translation.Y);
            ChooseCurrentRectangle(dragEnd);
            coords.Text = "Delta: " + dragEnd;
        }

        void Columns_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            dragEnd = new Point(dragStart.X + e.TotalManipulation.Translation.X, dragStart.Y + e.TotalManipulation.Translation.Y);
            ChooseCurrentRectangle(dragEnd);
            coords.Text = "Complete: " + dragEnd + " " + currentRectangle.Name;
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
                dropAnimation.Stop();
                // set NextSquare to new value
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
            NextBlock.SetValue(Canvas.LeftProperty, (double)r.GetValue(Canvas.LeftProperty) + (double)gridBorders.GetValue(Canvas.LeftProperty));

            if (currentRectangle != null && currentRectangle != r)
            {
                currentRectangle.Fill = new SolidColorBrush(Colors.Black);
            }

            currentRectangle = r;

        }

        private void MoveColumnSelection(object sender, MouseEventArgs e)
        {
            //UpdateCoords(e.GetPosition(Columns));
            Canvas c = sender as Canvas;
            if (c != null)
            {
                Point p = e.GetPosition(c);
                Rectangle r = GetRectangle(p);

                if (r == null)
                {
                    return;
                }

                r.Fill = new SolidColorBrush(Colors.Gray);
                NextBlock.SetValue(Canvas.LeftProperty, (double)r.GetValue(Canvas.LeftProperty) + (double)gridBorders.GetValue(Canvas.LeftProperty));

                if (currentRectangle != null && currentRectangle != r)
                {
                    currentRectangle.Fill = new SolidColorBrush(Colors.Black);
                }

                currentRectangle = r;
            }
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
    }
}
