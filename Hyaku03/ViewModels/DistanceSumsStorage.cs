using System;
using System.Linq;
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
using System.Diagnostics;

namespace Hyaku.ViewModels
{
    public delegate void HyakuFoundEventHandler(object sender, HyakuFoundEventArgs e);

    public class DistanceSumsStorage : ViewModelBase
    {
        #region Events
        public event HyakuFoundEventHandler HyakuFound;
        #endregion Events

        #region Declarations

#if DEBUG
        [Flags]
        private enum WhichToDebug {
            None = 0x0,
            Squares = 0x1,
            Sums = 0x2,
            SquareSums = 0x4 };
        private WhichToDebug whichToDebug = WhichToDebug.None;// (WhichToDebug.Squares | WhichToDebug.SquareSums | WhichToDebug.Sums);
#endif

        private HyakuSettings _settings;
        private Dictionary<int, Square> _squares;
        private Dictionary<int, Sum> _sums;
        private Dictionary<int, SquareSum> _squareSums;

        #endregion Declarations

        #region Properties

        protected virtual HyakuSettings Settings
        {
            get
            {
                if (_settings == null) {
                    _settings = new HyakuSettings();
                }
                return _settings;
            }
            set
            {
                _settings = value;
            }
        }


        public virtual Dictionary<int, Square> Squares
        {
            get
            {
                if (_squares == null) {
                    _squares = new Dictionary<int, Square>();
                }
                return _squares;
            }
        }

        public virtual Dictionary<int, Sum> Sums
        {
            get
            {
                if (_sums == null) {
                    _sums = new Dictionary<int, Sum>();
                }
                return _sums;
            }
        }

        public virtual Dictionary<int, SquareSum> SquareSums
        {
            get
            {
                if (_squareSums == null) {
                    _squareSums = new Dictionary<int, SquareSum>();
                }
                return _squareSums;
            }
        }

        //public virtual Wintellect.Sterling.ISterlingDatabaseInstance Database
        //{
        //    get
        //    {
        //        return _database;
        //    }
        //}

        #endregion Properties

        #region Constructors

        #endregion Constructors

        #region Methods

        public virtual Square SaveSquare(ushort column, ushort row, int value)
        {
            Square sq1 = new Square(column, row, value);
            Square sq2;
            if (!Squares.TryGetValue(sq1.GetHashCode(), out sq2)) {
                Squares.Add(sq1.GetHashCode(), sq1);
#if DEBUG
                if ((whichToDebug & WhichToDebug.Squares) == WhichToDebug.Squares) {
                    Debug.WriteLine("Added square: 0x{0}: {1} - {2} squares", sq1.GetHashCode().ToString("x"), sq1.ToString(), Squares.Count);
                }
#endif
            } else {
                // Square exists - replace it
                if (!sq1.Equals(sq2)) {
                    Squares[sq1.GetHashCode()] = sq1;
#if DEBUG
                    if ((whichToDebug & WhichToDebug.Squares) == WhichToDebug.Squares) {
                        Debug.WriteLine("Replaced square: {0} with square {1}; {2} squares", sq1.ToString(), sq2.ToString(), Squares.Count);
                    }
#endif
                }
            }
            return sq1;
        }

        public virtual Sum SaveSum(params Square[] squares)
        {
            Sum sum1 = new Sum(squares);
            if (sum1.Total <= 100) { // TODO make 100 a setting
                Sum sum2;
                if (!Sums.TryGetValue(sum1.GetHashCode(), out sum2)) {
                    Sums.Add(sum1.GetHashCode(), sum1);
                } else {
                    // sum exists - replace it
                    Sums[sum1.GetHashCode()] = sum1;
                }
#if DEBUG
                if ((whichToDebug & WhichToDebug.Sums) == WhichToDebug.Sums) {
                    Debug.WriteLine("Saved sum: {0} - {1} sums", sum1.ToString(), Sums.Count);
                }
#endif
                return sum1;
            } else {
                return null;
            }
        }

        public virtual SquareSum SaveSquareSum(int squareHashCode, int sumHashCode)
        {
            SquareSum sqsum1 = new SquareSum(squareHashCode, sumHashCode);
            SquareSum sqsum2;
            if (!SquareSums.TryGetValue(sqsum1.GetHashCode(), out sqsum2)) {
                SquareSums.Add(sqsum1.GetHashCode(), sqsum1);
            } else {
                // SquareSum exists - replace it
                SquareSums[sqsum1.GetHashCode()] = sqsum1;
            }
#if DEBUG
            if ((whichToDebug & WhichToDebug.SquareSums) == WhichToDebug.SquareSums) {
                Debug.WriteLine("Saved SquareSum: {0} - {1} square-sums", sqsum1.ToString(), SquareSums.Count);
            }
#endif
            return sqsum1;
        }

        private void CreateNewSumForSquare(Square newSquare)
        {
            Sum sum = SaveSum(newSquare);
            SaveSquareSum(newSquare.GetHashCode(), sum.GetHashCode());
        }

        ///// <summary>
        ///// Creates 1 new Sum and a new SquareSum for each square.
        ///// </summary>
        ///// <param name="sq1"></param>
        ///// <param name="sq2"></param>
        ///// <returns>The list of new SquareSum IDs</returns>
        //public List<int> CreateAndSaveNewSum(Square sq1, Square sq2)
        //{
        //    if (sq1 == null) { throw new ArgumentNullException("sq1"); }

        //    //int distance = Sum.GetDistance(sq1, sq2);
        //    //int total = Sum.AddSquareValues(sq1, sq2);
        //    Sum newSum = SaveSum(sq1, sq2);
        //    List<int> newSquareSumHashCodes = new List<int>();
        //    try {
        //        newSquareSumHashCodes.Add(SaveSquareSum(sq1.GetHashCode(), newSum.GetHashCode()).GetHashCode());
        //        if (sq2 != null) {
        //            newSquareSumHashCodes.Add(SaveSquareSum(sq2.GetHashCode(), newSum.GetHashCode()).GetHashCode());
        //        }
        //    } catch {
        //        // do nothing
        //    }
        //    return newSquareSumHashCodes;
        //}

        public List<int> CreateAndSaveNewSum(Square newSquare, List<Square> squaresInSum)
        {
            if (newSquare == null) {
                throw new ArgumentNullException("newSquare");
            }

            if (squaresInSum == null) {
                throw new ArgumentNullException("squaresInSum");
            }

            if (squaresInSum.Contains(newSquare)) {
                return null;
            }

            squaresInSum.Add(newSquare);

            Sum newSum = SaveSum(squaresInSum.ToArray());

            List<int> newSquareSumHashCodes = new List<int>();
            try {
                newSquareSumHashCodes.Add(SaveSquareSum(newSquare.GetHashCode(), newSum.GetHashCode()).GetHashCode());
                foreach (Square square in squaresInSum) {
                    newSquareSumHashCodes.Add(SaveSquareSum(square.GetHashCode(), newSum.GetHashCode()).GetHashCode());
                }
            } catch {
                // do nothing
            }

            if (newSum != null && newSum.Total == 100) { // TODO make "100" a setting
                OnHyakuFound(newSum);
            }
            return newSquareSumHashCodes;
        }

        public virtual List<Sum> InitializeSumsForSquare(SquareViewModel newSquare)
        {
            if (newSquare == null) {
                throw new ArgumentNullException("newSquare");
            }
            if (newSquare.Value <= 0) {
                throw new ArgumentException(string.Format("Cannot create sum for value {0}", newSquare.Value.ToString()), "newSquare");
            }
            Square newSq = SaveSquare(newSquare.Column, newSquare.Row, newSquare.Value);
            return InitializeSumsForSquare(newSq);
        }

        public virtual List<Sum> InitializeSumsForSquare(Square sq)
        {
            // get sums for newSquare
            List<Sum> sumsForNewSquare = null;
            sumsForNewSquare = GetSumsBySquare(sq.GetHashCode());

            if (sumsForNewSquare == null) {
                // I don't know if GetSumsBySquare returns null or an empty list
                sumsForNewSquare = new List<Sum>();
            }

            if (sumsForNewSquare.Count <= 0) {
                // create the new sum
                CreateNewSumForSquare(sq);
                sumsForNewSquare = GetSumsBySquare(sq.GetHashCode());
            }
            return sumsForNewSquare;
        }

        public List<SquareSum> GetSquareSumsBySquare(int sqHashCode)
        {
            var squareSums = from kvp in SquareSums
                             where kvp.Value.SquareHashCode == sqHashCode
                             select kvp.Value;
            return squareSums.ToList();
        }

        public List<Sum> GetSumsBySquare(int sqHashCode)
        {
            var sums = from sqSum in SquareSums.Values
                       where sqSum.SquareHashCode == sqHashCode
                       join sum in Sums.Values
                       on sqSum.SumHashCode equals sum.GetHashCode()
                       where sum.MaxDistance < Settings.MaxDistanceSetting
                       select sum;
            return sums.ToList();
        }

        public List<Square> GetSquaresBySum(int sumHashCode)
        {
            var squares = from sqSum in SquareSums.Values
                          join sq in Squares.Values
                          on sqSum.SquareHashCode equals sq.GetHashCode()
                          where sqSum.SumHashCode == sumHashCode
                          select sq;
            return squares.ToList();
        }

        private void OnHyakuFound(Sum newHyakuSum)
        {
            List<Square> squaresToMark = GetSquaresBySum(newHyakuSum.GetHashCode());

            Sum sum = Sums[newHyakuSum.GetHashCode()];
            if (HyakuFound != null) {
                HyakuFound(this, new HyakuFoundEventArgs(sum, squaresToMark));
            }
        }

        public virtual void AddSumsForNewSquare(SquareViewModel newSquareviewModel, SquareViewModel existingSqare)
        {
            Square newSquare;
            if (newSquareviewModel.IsHyakuBlock || existingSqare.IsHyakuBlock)
            {
                return;
            }

            if (!Squares.TryGetValue(newSquareviewModel.GetHashCode(), out newSquare)) {
                newSquare = SaveSquare(newSquareviewModel.Column, newSquareviewModel.Row, newSquareviewModel.Value);
            }

            List<Sum> sumsForNewSquare;
            List<Sum> sumsForExistingSquare;

            sumsForNewSquare = InitializeSumsForSquare(newSquareviewModel);
            sumsForExistingSquare = InitializeSumsForSquare(existingSqare);

            // from each sum for existingSquare that's < 100, create a new sum including newSquare
            foreach (Sum sum in sumsForExistingSquare) {
                if (sum.MaxDistance <= Settings.MaxDistanceSetting) {
                    List<Square> squaresInExistingSum = GetSquaresBySum(sum.GetHashCode());
                    if (!squaresInExistingSum.Contains(newSquare) && sum.Total <= 100) { // TODO make "100" a setting
                        CreateAndSaveNewSum(newSquare, squaresInExistingSum);
                        if (sum.Total == 100) {
                            OnHyakuFound(sum);
                        }
                    }
                }
            }
        }

        public virtual Square GetSquare(int column, int row)
        {
            return GetSquare((ushort)column, (ushort)row);
        }

        public virtual Square GetSquare(ushort column, ushort row)
        {
            uint sqId = Square.ConcatUInt16ToUInt32(column, row);
            return GetSquare((int)sqId);

        #endregion Methods
        }

        public virtual Square GetSquare(int sqHashCode)
        {
            try {
                return Squares[sqHashCode];
            } catch (KeyNotFoundException) {
                return null;
            }
        }

        /// <summary>
        /// Deletes Sums, SquareSums, and finaly the Square itself
        /// </summary>
        internal void DeleteSquareAndCascade(ushort column, ushort row)
        {
            // delete sums
            Square sq = new Square(column, row);
            foreach (Sum sum in GetSumsBySquare(sq.GetHashCode()))
            {
                Sums.Remove(sum.GetHashCode());
            }

            // delete sum-squares
            foreach (SquareSum sqSum in GetSquareSumsBySquare(sq.GetHashCode()))
            {
                SquareSums.Remove(sqSum.GetHashCode());
            }

            // delete square
            Squares.Remove(sq.GetHashCode());
        }
    }
}
