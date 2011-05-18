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

namespace Hyaku.Data
{
    [DataContract]
    public class SquareSum : IdBase
    {
        [DataMember]
        public int SquareColumn
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [DataMember]
        public int SquareRow
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [DataMember]
        public ulong SumId
        {
            get;
            set;
        }

        [DataMember]
        public UInt32 SquareId
        {
            get;
            set;
        }

        //public SquareSum()
        //{
        //}

        public SquareSum(UInt32 squareId, ulong sumId)
            : base()
        {
            SquareId = squareId;
            SumId = sumId;
        }
    }
}
