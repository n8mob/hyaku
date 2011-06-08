using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hyaku.Data;

namespace Hyaku
{
    public class NumberAddedEventArgs
    {
        private ViewModels.SquareViewModel _selectedSquare;

        public ViewModels.SquareViewModel SelectedSquare
        {
            get
            {
                return _selectedSquare;
            }
            set
            {
                _selectedSquare = value;
            }
        }

        public NumberAddedEventArgs(ViewModels.SquareViewModel selectedSquare)
        {
            _selectedSquare = selectedSquare;
        }

        public Square Square
        {
            get
            {
                return new Square(SelectedSquare.Column, SelectedSquare.Row, SelectedSquare.Value);
            }
        }
    }
}
