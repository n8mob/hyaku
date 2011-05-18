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
using Hyaku.Data;

namespace Hyaku.ViewModels
{
    public class SquareViewModel : ViewModelBase
    {
        #region Declarations

        private Square _dataSquare;
        private int _value;
        private bool _isCurrent;
        private bool _isLocked;
        //private List<int> _sums;
        //private bool _isHyaku;
        private int _score;
        private SquareView _uiSquare;
        private SquareState _currentState;


        #endregion Declarations

        #region Properties

        public ushort Row
        {
            get { return _dataSquare.Row; }
            set {
                _dataSquare.Row = value;
                if (this.UiSquare != null)
                {
                    Grid.SetRow(this.UiSquare, _dataSquare.Row);
                }
            }
        }

        public ushort Column
        {
            get { return _dataSquare.Column; }
            set {
                _dataSquare.Column = value;
                if (this.UiSquare != null)
                {
                    Grid.SetColumn(this.UiSquare, _dataSquare.Column);
                }
            }
        }

        public uint Id
        {
            get
            {
                return _dataSquare.Id;
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

        //public virtual List<int> Sums
        //{
        //    get
        //    {
        //        return _sums;
        //    }
        //    set
        //    {
        //        _sums = value;
        //        NotifyPropertyChanged("Sums");
        //    }
        //}

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

        #endregion Properties

        #region Constructors

        public SquareViewModel(Hyaku.Data.Square dataSquare) : base()
        {
            this._dataSquare = dataSquare;
        }

        public SquareViewModel(ushort columnIndex, ushort rowIndex) : this (new Square(columnIndex, rowIndex))
        {
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
