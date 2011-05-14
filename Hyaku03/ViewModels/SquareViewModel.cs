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
using Hyaku.Views;
using System.Collections.Generic;

namespace Hyaku.ViewModels
{
    public class SquareViewModel : ViewModelBase
    {
        #region Declarations

        private int _row;
        private int _column;
        private int _value;
        private bool _isCurrent;
        private bool _isLocked;
        //private bool _isHyaku;
        private int _score;
        private SquareView _uiSquare;
        private SquareState _currentState;
        private Dictionary<int, List<DistanceSum>> _distanceSums;


        #endregion Declarations

        #region Properties

        public int Row
        {
            get { return _row; }
            set {
                _row = value;
                if (this.UiSquare != null)
                {
                    Grid.SetRow(this.UiSquare, _row);
                }
            }
        }

        public int Column
        {
            get { return _column; }
            set {
                _column = value;
                if (this.UiSquare != null)
                {
                    Grid.SetColumn(this.UiSquare, _column);
                }
            }
        }

        public int Value
        {
            get { return _value; }
            set
            {
                _value = value;
                NotifyPropertyChanged("Value");
                NotifyPropertyChanged("StringValue");
            }
        }

        public string StringValue
        {
            get { return Value > 0 ? Value.ToString() : string.Empty; }
        }

        /// <summary>
        /// Same as "IsSelected" in Soduku sample.
        /// </summary>
        public virtual bool IsCurrent
        {
            get
            {
                return _isCurrent;
            }
            set
            {
                _isCurrent = value;
                UpdateState();
            }
        }

        public virtual bool IsLocked
        {
            get { return _isLocked; }
            set
            {
                _isLocked = value;
                UpdateState();
            }
        }

        //public virtual bool IsHyakuBlock
        //{
        //    get { return _isHyaku; }
        //    set {
        //        _isHyaku = value;
        //        UpdateState();
        //    }
        //}

        public virtual int Score
        {
            get
            {
                return _score;
            }
            set
            {
                _score = value;
                UpdateState();
            }
        }

        public virtual SquareState CurrentState
        {
            get
            {
                return _currentState;
            }
            set
            {
                _currentState = value;
                NotifyPropertyChanged("CurrentState");
            }
        }

        public SquareView UiSquare
        {
            get
            {
                return _uiSquare;
            }
            set
            {
                _uiSquare = value;
                if (_uiSquare != null)
                {
                    Grid.SetColumn(_uiSquare, Column);
                    Grid.SetRow(_uiSquare, Row);
                }
            }
        }

        public Dictionary<int, List<DistanceSum>> DistanceSums
        {
            get
            {
                if (_distanceSums == null) {
                    _distanceSums = new Dictionary<int, List<DistanceSum>>();
                }
                return _distanceSums;
            }
            set
            {
                _distanceSums = value;
                NotifyPropertyChanged("DistanceSums");
            }
        }

        #endregion Properties

        #region Constructors

        public SquareViewModel(int columnIndex, int rowIndex)
        {
            this.Column = columnIndex;
            this.Row = rowIndex;
        }

        #endregion Constructors

        #region Methods

        private void UpdateState()
        {
            if (Score > 0)
            {
                CurrentState = SquareState.Hyaku;
            }
            else if (IsCurrent)
            {
                CurrentState = SquareState.Current;
            }
            else if (IsLocked)
            {
                CurrentState = SquareState.Locked;
            }
            else
            {
                CurrentState = SquareState.Default;
            }
        }

        public override string ToString()
        {
            // [Column][Row]: State
            return string.Format("[{0}][{1}]: {2} {3}", Column, Row, StringValue, CurrentState);
        }

        public virtual void Reset()
        {
            //this.IsHyakuBlock = false;
            this.Score = 0;
            this.Value = 0;
            this.IsLocked = false;
        }

        public virtual int DistanceTo(SquareViewModel sq)
        {
            int rowDistance = Math.Abs(Row - sq.Row);
            int columnDistance = Math.Abs(Column - sq.Column);
            return Math.Max(rowDistance, columnDistance);
        }

        public virtual void UpdateAllSums(SquareViewModel sq)
        {
            int distanceToSquare = DistanceTo(sq);
            foreach (int givenDistance in sq.DistanceSums.Keys) {
                int distanceForNewSum = givenDistance + distanceToSquare;
                if (distanceForNewSum > 2) { // TODO make a setting
                    // skip distances athat are too far
                    continue;
                }
                List<DistanceSum> sqSumsForGivenDistance = sq.DistanceSums[givenDistance];
                List<DistanceSum> mySumsForGivenDistance = new List<DistanceSum>();
                foreach (DistanceSum sum in sqSumsForGivenDistance) {
                    // create my sums
                    // mySumsForGivenDistance.Add(sum.AddSum(distanceForNewSum, Value, this));
                    // TODO update his sums
                }
                if (mySumsForGivenDistance.Count > 0) {
                    if (!DistanceSums.ContainsKey(distanceForNewSum)) {
                        DistanceSums.Add(distanceForNewSum, mySumsForGivenDistance);
                    } else {
                        DistanceSums[distanceForNewSum].AddRange(mySumsForGivenDistance);
                    }
                }
            }
        }

        #endregion Methods
    }

    public enum SquareState
    {
        Default,
        Current,
        Locked,
        Hyaku
    }

    public class SquareValueChangedEventArgs : EventArgs
    {
        public int Value { get; set; }

        public SquareValueChangedEventArgs(int newValue)
        {
            this.Value = newValue;
        }
    }
}
