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
        private bool _isHyaku;
        private SquareView _uiSquare;
        private SquareState _currentState;

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

        public virtual bool IsHyakuBlock
        {
            get { return _isHyaku; }
            set {
                _isHyaku = value;
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

        public SquareViewModel(int columnIndex, int rowIndex)
        {
            this.Column = columnIndex;
            this.Row = rowIndex;
        }

        #endregion Constructors

        #region Methods

        private void UpdateState()
        {
            if (IsHyakuBlock)
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

        public void Reset()
        {
            this.IsHyakuBlock = false;
            this.IsLocked = false;
            this.Value = 0;
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
