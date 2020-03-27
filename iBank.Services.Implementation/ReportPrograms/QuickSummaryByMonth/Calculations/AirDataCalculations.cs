using System;
using System.Collections.Generic;

namespace iBank.Services.Implementation.ReportPrograms.QuickSummaryByMonth.Calculations
{
    public class AirDataCalculations
    {
        public decimal CalculateStandardCharge(decimal standardCharge, decimal airCharge)
        {
            if (Math.Abs(standardCharge) < Math.Abs(airCharge)) return airCharge;

            if (standardCharge == 0) return airCharge;

            if (standardCharge > 0 && airCharge < 0) return airCharge;

            return standardCharge;
        }

        public decimal CalculateOfferedCharge(decimal offeredCharge, decimal airCharge)
        {
            if (offeredCharge > 0 && airCharge < 0) return 0 - offeredCharge;

            if (offeredCharge == 0) return airCharge;

            return offeredCharge;
        }

        public decimal CalculateLostAmount(decimal airCharge, decimal offeredCharge)
        {
            return airCharge - offeredCharge;
        }

        public decimal CalculateLostAmount2(decimal lostAmount, int plusMin)
        {
            return (lostAmount < 0 && plusMin > 0) || (lostAmount > 0 && plusMin < 0) ? 0 : lostAmount;
        }

        public decimal CalculateSavings(decimal airCharge, decimal standardCharge)
        {
            return standardCharge - airCharge;
        }

        public string UpdateReasCode(string reasCode, IList<string> reasExclude)
        {
            if (string.IsNullOrEmpty(reasCode?.Trim())) return "";

            if (reasExclude.Contains(reasCode)) return "";
            
            return reasCode;
        }

        public bool IsOkToUpdateSavingCode(string savingCode, string reasCode, decimal lostAmount, decimal savings)
        {
            if (!string.IsNullOrEmpty(savingCode?.Trim())) return false;

            if (string.IsNullOrEmpty(reasCode?.Trim())) return false;

            if (lostAmount != 0) return false;

            if (savings <= 0) return false;

            return true;
        } 
    }
}
