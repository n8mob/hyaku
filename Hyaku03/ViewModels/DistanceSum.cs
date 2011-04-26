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

namespace Hyaku.ViewModels
{
    public class DistanceSum : ViewModelBase
    {
        #region Declarations

        private int _distance;
        private int _sum;
        private List<SquareViewModel> _squaresInSum;

        #endregion Declarations

        #region Properties

        public int Distance
        {
            get
            {
                return _distance;
            }
            set
            {
                _distance = value;
                NotifyPropertyChanged("Distance");
            }
        }

        public int Sum
        {
            get
            {
                return _sum;
            }
            set
            {
                _sum = value;
                NotifyPropertyChanged("Sum");
            }
        }

        public List<SquareViewModel> SquaresInSum
        {
            get
            {
                return _squaresInSum;
            }
            set
            {
                _squaresInSum = value;
                NotifyPropertyChanged("SquaresInSum");
            }
        }

        #endregion Properties

        #region Constructors



        #endregion Constructors

        internal DistanceSum AddSum(int additionalDistance, int additionalAmmount, SquareViewModel sq)
        {
            DistanceSum distanceSum = new DistanceSum()
            {
                Distance = Distance + additionalDistance,
                Sum = Sum + additionalAmmount
            };
            distanceSum.SquaresInSum = new List<SquareViewModel>(SquaresInSum);
            distanceSum.SquaresInSum.Add(sq);
            return distanceSum;
        }
    }
}
