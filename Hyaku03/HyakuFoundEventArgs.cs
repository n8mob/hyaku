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
using Hyaku.Data;

namespace Hyaku
{
    public class HyakuFoundEventArgs : EventArgs
    {
        public int HyakuScoreCount
        {
            get;
            set;
        }

        public virtual Sum Sum
        {
            get;
            set;
        }

        public List<Square> SquaresToMark
        {
            get;
            set;
        }

        public HyakuFoundEventArgs(Sum distanceSum, List<Square> squaresTomark)
        {
            Sum = distanceSum;
            SquaresToMark = squaresTomark;
        }
    }
}
