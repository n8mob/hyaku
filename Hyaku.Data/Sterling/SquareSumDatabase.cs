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
using Wintellect.Sterling.Database;

namespace Hyaku.Data.Sterling
{
    public class SquareSumDatabase : BaseDatabaseInstance
    {
        //public static string MaxDistanceIndexName = "IndexMaxDistance";
        //public static string SquareSumsBySquareIndexName = "SquareSumsBySquare";
        //public static string SquareSumsBySumIndexName = "SquareSumsBySum";
        //public static string SquareSumsBySumAndSquareName = "SquareSumsBySumAndSquare";

        public override string Name
        {
            get
            {
                return "SquareSumDatabase";
            }
        }

        protected override System.Collections.Generic.List<ITableDefinition> _RegisterTables()
        {
            return new System.Collections.Generic.List<ITableDefinition>();
        //    return new System.Collections.Generic.List<ITableDefinition>
        //    {
        //        CreateTableDefinition<Square, UInt32>(sq => sq.Id),

        //        CreateTableDefinition<Sum, ulong>(sum => sum.Id)
        //        .WithIndex<Sum, int, ulong>(MaxDistanceIndexName, sum=>sum.MaxDistance),

        //        CreateTableDefinition<SquareSum, ulong>(sqSum => sqSum.Id)
        //        .WithIndex<SquareSum, ulong, ulong>(SquareSumsBySquareIndexName, sqSum=>sqSum.SquareId)
        //        .WithIndex<SquareSum, uint, ulong, ulong>(SquareSumsBySumAndSquareName, sqSum => new Tuple<uint, ulong>(sqSum.SquareId, sqSum.SumId))
        //    };
        }
    }
}
