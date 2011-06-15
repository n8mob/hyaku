using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hyaku.Data;

namespace Hyaku
{
    public class SquareCountedEventArgs
    {
        public Square Square
        {
            get;
            set;
        }

        public Sum Sum
        {
            get;
            set;
        }

        public int ScoreMultiplier
        {
            get;
            set;
        }

        public int ScoreValue
        {
            get
            {
                return Sum.Total * Sum.SquareCount * ScoreMultiplier;
            }
        }
    }
}
