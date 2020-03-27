using System.Collections.Generic;
using Domain.Models.ReportPrograms.WtsFareSavings;

namespace iBank.Services.Implementation.ReportPrograms.WtsFareSavings
{
    /// <summary>
    /// Compares two IRecKey objects just on the basis of their reckey.
    /// </summary>
    public class RawDataDistinctComparer : IEqualityComparer<FinalData>
    {
        public bool Equals(FinalData x, FinalData y)
        {
            return x.RecKey == y.RecKey;
        }

        public int GetHashCode(FinalData obj)
        {
            return obj.RecKey;
        }
    }
}
