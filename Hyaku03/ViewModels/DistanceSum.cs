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
using System.Threading;

namespace Hyaku.ViewModels
{
    public class DistanceSum : ViewModelBase
    {
        #region Declarations
        
        private int _maxDistance;
        private int _sum;

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

        #endregion Properties

        #region Constructors

        public DistanceSum(SquareViewModel sq) : base()
        {
            this.MaxDistance = 0;
            this.Sum = sq.Value;
        }

        public DistanceSum(SquareViewModel newSquare, DistanceSum existingSum, int newMaxDistance) : base()
        {
            this.MaxDistance = newMaxDistance;
            this.Sum = existingSum.Sum + newSquare.Value;
        }

        public DistanceSum(Data.Sum existingSum)
        {
            this.MaxDistance = existingSum.MaxDistance;
            this.Sum = existingSum.Total;
        }

        #endregion Constructors
    }
}
