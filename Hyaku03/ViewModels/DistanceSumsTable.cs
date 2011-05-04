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
    public delegate void HyakuFoundEventHandler(object sender, HyakuFoundEventArgs e);

    public class DistanceSumsTable : ViewModelBase
    {
        private HyakuSettings _settings;

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

        #region Events
        public event HyakuFoundEventHandler HyakuFound;
        #endregion Events

        private Dictionary<SquareViewModel, List<DistanceSum>> _table;

        public virtual Dictionary<SquareViewModel, List<DistanceSum>> Table
        {
            get
            {
                if (_table == null) {
                    _table = new Dictionary<SquareViewModel, List<DistanceSum>>();
                }
                return _table;
            }
        }

        public virtual void AddSumsForNewSquare(SquareViewModel newSquare, SquareViewModel existingSquare)
        {
            if (newSquare.DistanceTo(existingSquare) > Settings.MaxDistanceSetting) {
                // not interested in squares further away than the max
                return;
            }

            if (newSquare == null || newSquare.Value <= 0) {
                return;
            }

            List<DistanceSum> newSquareSums = null;
            List<DistanceSum> existingSquareSums = null;
            // add the "all alone" sum
            if (!Table.ContainsKey(newSquare)) {
                // if there is not a list for the new square - add it.
                Table.Add(newSquare, new List<DistanceSum>());
            }

            // retrieve the (possibly brand new) list of sums
            // make a copy so we can modify the copy while we iterate over the original
            newSquareSums = new List<DistanceSum>(Table[newSquare]);

            if (newSquareSums.Count == 0) {
                // add the "all alone" sum
                newSquareSums.Add(new DistanceSum(newSquare));
            }

            if (existingSquare != null && existingSquare.Value != 0) {
                // if the new square isn't by itself, create new sums for existing square that includes the new square.
                if (!Table.ContainsKey(existingSquare)) {
                    Table.Add(existingSquare, new List<DistanceSum>());
                }
                // make a copy so we can modify the copy while we iterate over the original
                existingSquareSums = new List<DistanceSum>(Table[existingSquare]);
                foreach (DistanceSum ds in Table[existingSquare]) {
                    if (ds.MaxDistance <= Settings.MaxDistanceSetting) { // not interested in any distances beyond the max setting
                        DistanceSum newDs = new DistanceSum(newSquare, ds);
                        if (newDs.Sum == 100) {
                            OnHyakuFound(newDs);
                        }
                        // update the existing sums to include the new square
                        if (newDs.Sum < 100) {
                            existingSquareSums.Add(newDs);
                            if (ds.MaxDistance < Settings.MaxDistanceSetting) {
                                // make sums for the new square
                                newSquareSums.Add(newDs);
                            }
                        }
                    }
                }
            }

            if (Table.ContainsKey(newSquare)) {
                Table[newSquare] = newSquareSums;
            } else {
                Table.Add(newSquare, newSquareSums);
            }

            if (existingSquareSums != null) {
                Table[existingSquare] = existingSquareSums;
            }
            NotifyPropertyChanged("Table");
        }

        private void OnHyakuFound(DistanceSum distanceSum)
        {
            if (HyakuFound != null) {
                HyakuFound(this, new HyakuFoundEventArgs(distanceSum));
            }
        }
    }
}
