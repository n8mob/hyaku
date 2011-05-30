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
using System.Diagnostics;

namespace Hyaku.Data
{
    [DataContract]
    public class SquareSum
    {
        [DataMember]
        public int SumHashCode
        {
            get;
            set;
        }

        [DataMember]
        public int SquareHashCode
        {
            get;
            set;
        }

        [DebuggerStepThrough]
        public override int GetHashCode()
        {
            return SumHashCode ^ SquareHashCode;
        }

        public override string ToString()
        {
            return string.Format("0x{0}: (sq 0x{1}, sum 0x{2})", GetHashCode().ToString("x"), SquareHashCode.ToString("x"), SumHashCode.ToString("x"));
        }

        public SquareSum(int squareHashCode, int sumHashCode)
        {
            SquareHashCode = squareHashCode;
            SumHashCode = sumHashCode;
        }
    }
}
