using Domain.Constants;
using System;

namespace iBank.Services.Implementation.ReportPrograms.AirFareSavings
{
    public class AirFareSavingsCalculations
    {
        public string GetCrystalReportName(bool isPageSummaryOnly, bool lExSavings, bool lExNegoSvgs)
        {
            if (isPageSummaryOnly) return ReportNames.AIR_FARE_SAVINGS_SUMMARY_RPT;
            if (lExSavings && lExNegoSvgs) return ReportNames.AIR_FARE_SAVINGS_RPT_4;
            if (lExSavings) return ReportNames.AIR_FARE_SAVINGS_RPT_3;
            if (lExNegoSvgs) return ReportNames.AIR_FARE_SAVINGS_RPT_2;
            return ReportNames.AIR_FARE_SAVINGS_RPT_1;
        }

        public decimal GetOffRdChange(bool lDeriveSvgCode, decimal offrdchg, decimal airChg)
        {
            if (lDeriveSvgCode)
            {
                if (offrdchg > 0 && airChg < 0)
                {
                    return 0 - offrdchg;
                }
                else
                {
                    return offrdchg == 0 ? airChg : offrdchg;
                }
            }
            else
            {
                return (offrdchg > 0 && airChg < 0) ? (0 - offrdchg) : offrdchg;
            }
        }

        public decimal GetStndCharge(bool lDeriveSvgCode, decimal stndChg, decimal airChg)
        {
            if (lDeriveSvgCode)
            {
                return (Math.Abs(stndChg) < Math.Abs(airChg) || stndChg == 0 || (stndChg > 0 && airChg < 0)) ? airChg : stndChg;
            }
            else
            {
                return (stndChg > 0 && airChg < 0) ? (0 - stndChg) : stndChg;
            }
        }

        public decimal GetSavings(decimal stndchg, decimal airchg, decimal baseFare, bool useBaseFare)
        {
            return useBaseFare ? stndchg - baseFare : stndchg - airchg;
        }

        public decimal GetNegotiatedSavings(decimal airchg, decimal offrdchg, int plusmin, decimal baseFare, bool useBaseFare)
        {
            decimal lostamt = useBaseFare
                ? baseFare - offrdchg
                : airchg - offrdchg;

            if ((lostamt < 0 && plusmin > 0) || (lostamt > 0 && plusmin < 0))
            {
                return 0 - lostamt;
            }
            else
            {
                return 000000000.00m;
            }
        }

        public decimal GetLostAmount(decimal airchg, decimal offrdchg, int plusmin, decimal baseFare, bool useBaseFare)
        {
            decimal lostamt;
            if (useBaseFare)
            {
                lostamt = (baseFare > offrdchg)
                    ? baseFare - offrdchg
                    : Convert.ToDecimal(0000.00);
            }
            else
            {
                lostamt = airchg - offrdchg;
            }

            if ((lostamt < 0 && plusmin > 0) || (lostamt > 0 && plusmin < 0))
            {
                return lostamt = 0;
            }

            return lostamt;
        }

        public decimal GetLossPercentage(decimal airChange, decimal offRecordChange, decimal baseFare, bool useBaseFare)
        {
            if (useBaseFare)
            {
                airChange = baseFare;
            }

            return (airChange != 0 && offRecordChange != 0)
                ? Decimal.Round(100 * ((airChange - offRecordChange) / airChange), 2)
                : Convert.ToDecimal(0000.00);
        }

        public string GetSavingsCode(string savingsCode, bool lDeriveSvgCode, string reasonCode, decimal lostamt, decimal savings)
        {
            // 4936 Enhancement 00165066 -  Fare Savings - Air - Populate Code for $0 Savings and no Losses/Negotiated Savings Records
            return savingsCode;
        }
    }
}
