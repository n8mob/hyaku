﻿using System;
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
    public class Square
    {
        [DataMember]
        public UInt16 Column
        {
            get;
            set;
        }

        [DataMember]
        public UInt16 Row
        {
            get;
            set;
        }

        [DataMember]
        public int Value
        {
            get;
            set;
        }

        [DataMember]
        public UInt32 Id
        {
            get
            {
                UInt16 column = Column;
                UInt16 row = Row;
                UInt32 id = ConcatUInt16ToUInt32(column, row);
                return id;
            }
        }

        public override string ToString()
        {
            return string.Format("[{0}][{1}]: {2}", Column, Row, Value);
        }

        public static UInt32 ConcatUInt16ToUInt32(UInt16 column, UInt16 row)
        {
            byte[] columnBits = BitConverter.GetBytes(column);
            byte[] rowBits = BitConverter.GetBytes(row);
            byte[] idBits = new byte[4];
            columnBits.CopyTo(idBits, 0);
            rowBits.CopyTo(idBits, 2);
            UInt32 id = BitConverter.ToUInt32(idBits, 0);
            return id;
        }

        public static UInt16[] SplitUInt32ToUInt16(UInt32 squareId)
        {
            byte[] idBits = BitConverter.GetBytes(squareId);
            byte[] columnBits = new byte[2];
            byte[] rowBits = new byte[2];
            Array.Copy(idBits, 0, columnBits, 0, 2);
            Array.Copy(idBits, 2, rowBits, 0, 2);
            UInt16 column = BitConverter.ToUInt16(columnBits, 0);
            UInt16 row = BitConverter.ToUInt16(rowBits, 2);
            return new UInt16[] { column, row };
        }

        public static int GetDistance(Square sq1, Square sq2)
        {
            if (sq2 == null) {
                return 0;
            }
            return Math.Max(Math.Abs(sq1.Column - sq2.Column), Math.Abs(sq1.Row - sq2.Row));
        }

        //public Square()
        //{
        //}

        public Square(UInt16 column, UInt16 row, int value = 0)
        {
            this.Column = column;
            this.Row = row;
            this.Value = value;
        }

        public static int GetDistance(Square sq, System.Collections.Generic.List<Square> squaresInSum)
        {
            int maxDistance = 0;
            foreach (Square sq2 in squaresInSum) {
                maxDistance = Math.Max(maxDistance, GetDistance(sq, sq2));
            }
            return maxDistance;
        }
    }
}
