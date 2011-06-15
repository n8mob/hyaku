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
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Hyaku.Data
{
    [DataContract]
    public class Sum : IComparable<Sum>, IEquatable<Sum>
    {
        private Square[] _squares;

        [DataMember]
        private string _squareString;

        [DataMember]
        public int MaxDistance
        {
            get;
            set;
        }

        [DataMember]
        public int Total
        {
            get;
            set;
        }

        [DataMember]
        public int SquareCount
        {
            get
            {
                if (_squares != null) {
                    return _squares.Length;
                }
                return 0;
            }
        }

        [DataMember]
        public bool HasScoreBeenCounted
        {
            get;
            set;
        }

        public override string ToString()
        {
            return string.Format("0x{0}: {1} [{2}]", GetHashCode().ToString("x"), Total, _squareString);
        }

        public virtual string ToShortString()
        {
            return string.Format("{0} squares: {1} ({2})", _squares.Length, Total, GetHashCode());
        }

        [DebuggerStepThrough]
        public override int GetHashCode()
        {
            return _squareString.GetHashCode();
        }

        public bool Equals(Sum that)
        {
            return this._squareString.Equals(that._squareString) && this.Total.Equals(that.Total);
        }

        //public Sum(int maxDistance, int total = 0)
        //    : base()
        //{
        //    MaxDistance = maxDistance;
        //    Total = total;
        //}
        // TODO Make new constructor that takes a list of squares

        public Sum(params Square[] squares)
        {
            HasScoreBeenCounted = false;

            if (squares == null || squares.Length == 0) {
                throw new ArgumentException("Sum must contain squares");
            }

            _squares = squares;
            Array.Sort(_squares);
            string[] squareStrings = new string[_squares.Length];
            int sumTotal = 0;
            int maxDistance = 0;
            for (int i = 0; i < _squares.Length; i++) {
                if (_squares[i].Value <= 0) {
                    throw new ArgumentException("Squares must have a value greater than zero");
                }

                foreach (Square sq in _squares) {
                    maxDistance = Math.Max(maxDistance, GetDistance(_squares[i], sq));
                }

                sumTotal += _squares[i].Value;
                MaxDistance = maxDistance;
                squareStrings[i] = string.Format("({0},{1})", _squares[i].Column, _squares[i].Row);
            }

            Total = sumTotal;
            _squareString = string.Join(",", squareStrings);
        }

        public static int AddSquareValues(Square sq1, Square sq2)
        {
            if (sq1 == null) {
                throw new ArgumentNullException("sq1");
            }

            return sq2 != null ? sq1.Value + sq2.Value : sq1.Value;
        }

        public static int GetDistance(Square sq1, Square sq2)
        {
            if (sq2 == null) {
                return 0;
            }
            return Math.Max(Math.Abs(sq1.Column - sq2.Column), Math.Abs(sq1.Row - sq2.Row));
        }

        

        public int CompareTo(Sum that)
        {
            return _squareString.CompareTo(that._squareString);
        }
    }
}
