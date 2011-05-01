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

        private int _maxDistance;
        private int _sum;
        private List<SquareViewModel> _squaresInSum;

        #endregion Declarations

        #region Properties

        public virtual int MaxDistance
        {
            get
            {
                return _maxDistance;
            }
            private set
            {
                _maxDistance = value;
                NotifyPropertyChanged("MaxDistance");
            }
        }

        public virtual int Sum
        {
            get
            {
                return _sum;
            }
            protected set
            {
                _sum = value;
                NotifyPropertyChanged("Sum");
            }
        }

        public virtual List<SquareViewModel> SquaresInSum
        {
            get
            {
                if (_squaresInSum == null) {
                    _squaresInSum = new List<SquareViewModel>();
                }
                return _squaresInSum;
            }
        }

        public virtual void AddSquareToSum(SquareViewModel newSquare)
        {
            SquaresInSum.Add(newSquare);
            int sum = 0;
            foreach (SquareViewModel sq in _squaresInSum) {
                sum += sq.Value;
                int maxDistance = newSquare.DistanceTo(sq);
                if (maxDistance > MaxDistance)
                {
                    MaxDistance = maxDistance;
                }
            }
            Sum = sum;
            NotifyPropertyChanged("SquaresInSum");
        }

        #endregion Properties

        #region Constructors

        public DistanceSum(SquareViewModel sq)
        {
            AddSquareToSum(sq);

        }

        public DistanceSum(SquareViewModel newSquare, DistanceSum existingSum)
        {
            foreach (SquareViewModel sq in existingSum.SquaresInSum) {
                AddSquareToSum(sq);
            }
            AddSquareToSum(newSquare);
        }

        #endregion Constructors
    }
}
