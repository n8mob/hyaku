using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hyaku
{
    public class SquareDeletedEventArgs
    {
        public int Column
        {
            get;
            set;
        }

        public int Row
        {
            get;
            set;
        }
    }
}
