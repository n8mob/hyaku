using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hyaku
{
    public class SquareMovedEventArgs
    {
        public int OldColumn
        {
            get;
            set;
        }
        
        public int OldRow
        {
            get;
            set;
        }

        public int NewColumn
        {
            get;
            set;
        }

        public int NewRow
        {
            get;
            set;
        }
    }
}
