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
using Hyaku.ViewModels;
using System.Collections.Generic;

namespace Hyaku
{
    public class HyakuFoundEventArgs
    {
        public virtual DistanceSum Sum
        {
            get;
            set;
        }

        public List<SquareViewModel> SquaresToMark
        {
            get;
            set;
        }

        public HyakuFoundEventArgs(DistanceSum distanceSum, List<SquareViewModel> squaresTomark)
        {
            Sum = distanceSum;
            SquaresToMark = squaresTomark;
        }
    }
}
