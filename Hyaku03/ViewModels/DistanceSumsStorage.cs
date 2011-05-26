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

namespace Hyaku.ViewModels
{
    public delegate void HyakuFoundEventHandler(object sender, HyakuFoundEventArgs e);

    public class DistanceSumsStorage : ViewModelBase
    {
        #region Events
        public event HyakuFoundEventHandler HyakuFound;
        #endregion Events

        #region Declarations

        private HyakuSettings _settings;
        private Dictionary<uint, Square> _squares;
        private Dictionary<ulong, Sum> _sums;
        private Dictionary<ulong, SquareSum> _squareSums;

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

        public virtual Dictionary<uint, Square> Squares
        {
            get
            {
                if (_squares == null) {
                    _squares = new Dictionary<uint, Square>();
                }
                return _squares;
            }
        }

        public virtual Dictionary<ulong, Sum> Sums
        {
            get
            {
                if (_sums == null) {
                    _sums = new Dictionary<ulong, Sum>();
                }
                return _sums;
            }
        }

        public virtual Dictionary<ulong, SquareSum> SquareSums
        {
            get
            {
                if (_squareSums == null) {
                    _squareSums = new Dictionary<ulong, SquareSum>();
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
            Square sq = new Square(column, row, value);
            if (!Squares.ContainsKey(sq.Id)) {
                Squares.Add(sq.Id, sq);
            } else {
                Squares[sq.Id] = sq;
            }
            return sq;
        }

        public virtual ulong SaveNewSum(int distance, int total)
        {
            // TODO Sum constructor will always generate a new key, so even if the Sum is already in the dictionary, it will be added again.
            Sum sum = new Sum(distance, total);
            Sums.Add(sum.Id, sum);
            return sum.Id;
        }

        public virtual ulong SaveNewSquareSum(uint squareId, ulong sumId)
        {
            SquareSum sqsum = new SquareSum(squareId, sumId);
            SquareSums.Add(sqsum.Id, sqsum);
            return sqsum.Id;
        }


        private void CreateNewSumForSquare(Square newSquare)
        {
            ulong newSumIdForNewSq = SaveNewSum(0, newSquare.Value);
            ulong newSquareSumIdForNewSq = SaveNewSquareSum(newSquare.Id, newSumIdForNewSq);
        }

        /// <summary>
        /// Creates 1 new Sum and a new SquareSum for each square.
        /// </summary>
        /// <param name="sq1"></param>
        /// <param name="sq2"></param>
        /// <returns>The list of new SquareSum IDs</returns>
        public List<ulong> CreateAndSaveNewSum(Square sq1, Square sq2)
        {
            if (sq1 == null) { throw new ArgumentNullException("sq1"); }

            int distance = Square.GetDistance(sq1, sq2);
            int total = Sum.AddSquareValues(sq1, sq2);
            ulong newSumId = SaveNewSum(distance, total);
            List<ulong> newSquareSumIds = new List<ulong>();
            try {
                newSquareSumIds.Add(SaveNewSquareSum(sq1.Id, newSumId));
                if (sq2 != null) {
                    newSquareSumIds.Add(SaveNewSquareSum(sq2.Id, newSumId));
                }
            } catch {
                // do nothing
            }
            return newSquareSumIds;
        }

        public List<ulong> CreateAndSaveNewSum(Square newSquare, List<Square> squaresInSum)
        {
            if (newSquare == null) {
                throw new ArgumentNullException("newSquare");
            }

            if (squaresInSum == null) {
                throw new ArgumentNullException("squaresInSum");
            }

            int distance = Square.GetDistance(newSquare, squaresInSum);
            int total = newSquare.Value + squaresInSum.Sum(sq2 => sq2.Value);
            ulong newSumId = SaveNewSum(distance, total);
            List<ulong> newSquareSumIds = new List<ulong>();
            try {
                newSquareSumIds.Add(SaveNewSquareSum(newSquare.Id, newSumId));
                foreach (Square square in squaresInSum) {
                    newSquareSumIds.Add(SaveNewSquareSum(square.Id, newSumId));
                }
            } catch {
                // do nothing
            }
            if (total == 100) { // TODO make "100" a setting
                OnHyakuFound(newSumId);
            }
            return newSquareSumIds;
        }

        public virtual List<Sum> InitializeSumsForSquare(SquareViewModel newSquare)
        {
            if (newSquare == null) {
                throw new ArgumentNullException("newSquare");
            }
            if (newSquare.Value <= 0) {
                throw new ArgumentException(string.Format("Cannot create sum for value {0}", newSquare.Value.ToString()), "newSquare");
            }

            // save the square (assuming it's new)
            Square newSq = SaveSquare(newSquare.Column, newSquare.Row, newSquare.Value);
            return InitializeSumsForSquare(newSq);
        }

        public virtual List<Sum> InitializeSumsForSquare(Square sq)
        {
            // get sums for newSquare
            List<Sum> sumsForNewSquare = null;
            sumsForNewSquare = GetSumsBySquare(sq.Id);

            if (sumsForNewSquare == null) {
                // I don't know if GetSumsBySquare returns null or an empty list
                sumsForNewSquare = new List<Sum>();
            }

            if (sumsForNewSquare.Count <= 0) {
                // create the new sum
                CreateNewSumForSquare(sq);
                sumsForNewSquare = GetSumsBySquare(sq.Id);
            }
            return sumsForNewSquare;
        }

        public List<SquareSum> GetSquareSumsBySquare(uint squareId)
        {
            var squareSums = from kvp in SquareSums
                             where kvp.Value.SquareId == squareId
                             select kvp.Value;
            return squareSums.ToList();
        }

        public List<Sum> GetSumsBySquare(uint squareId)
        {
            var sums = from sqSum in SquareSums.Values
                       join sum in Sums.Values
                       on sqSum.SumId equals sum.Id
                       where sum.MaxDistance < Settings.MaxDistanceSetting
                       select sum;
            return sums.ToList();
        }

        public List<Square> GetSquaresBySum(ulong sumId)
        {
            var squares = from sqSum in SquareSums.Values
                          join sq in Squares.Values
                          on sqSum.SquareId equals sq.Id
                          where sqSum.SumId == sumId
                          select sq;
            return squares.ToList();
        }

        private void OnHyakuFound(ulong hyakuSumId)
        {
            List<Square> squaresToMark = GetSquaresBySum(hyakuSumId);
            List<SquareViewModel> squareViewModelsToark = (from sq in squaresToMark
                                                           select new SquareViewModel(sq)).ToList();

            Sum sum = Sums[hyakuSumId];
            DistanceSum distanceSum = new DistanceSum(sum);
            if (HyakuFound != null) {
                HyakuFound(this, new HyakuFoundEventArgs(distanceSum, squareViewModelsToark));
            }
        }

        public virtual void AddSumsForNewSquare(SquareViewModel newSquareviewModel, SquareViewModel existingSqare)
        {
            Square sq = Squares[newSquareviewModel.Id];

            List<Sum> sumsForNewSquare;
            List<Sum> sumsForExistingSquare;

            sumsForNewSquare = InitializeSumsForSquare(newSquareviewModel);
            sumsForExistingSquare = InitializeSumsForSquare(existingSqare);

            // from each sum for existingSquare that's < 100, create a new sum including newSquare
            foreach (Sum sum in sumsForExistingSquare) {
                List<Square> squaresInSum = GetSquaresBySum(sum.Id);
                if (sum.Total < 100) { // TODO make "100" a setting
                    CreateAndSaveNewSum(sq, squaresInSum);
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
            return GetSquare(sqId);

        #endregion Methods
        }

        public virtual Square GetSquare(uint sqId)
        {
            try {
                return Squares[sqId];
            } catch (KeyNotFoundException) {
                return null;
            }
        }
    }
}
