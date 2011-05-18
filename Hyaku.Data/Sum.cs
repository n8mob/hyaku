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
    public class Sum : IdBase
    {
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

        //public Sum()
        //{
        //}

        public Sum(int maxDistance, int total = 0)
            : base()
        {
            MaxDistance = maxDistance;
            Total = total;
        }

        public static int AddSquareValues(Square sq1, Square sq2)
        {
            if (sq1 == null) {
                throw new ArgumentNullException("sq1");
            }

            return sq2 != null ? sq1.Value + sq2.Value : sq1.Value;
        }
    }
}
