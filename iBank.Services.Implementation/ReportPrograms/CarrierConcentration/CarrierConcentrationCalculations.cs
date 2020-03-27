using System.Collections.Generic;
using Domain.Constants;
using Domain.Helper;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.CarrierConcentration
{
    public class CarrierConcentrationCalculations
    {
        public string GetCrystalReportName(bool suppressAverageDifference, bool excludeSavings, bool useAirportCodes)
        {
            if (suppressAverageDifference && excludeSavings && !useAirportCodes) return ReportNames.CARRIER_CONCENTRATION_2;

            if (suppressAverageDifference && excludeSavings && useAirportCodes) return ReportNames.CARRIER_CONCENTRATION_2A;

            if ((!suppressAverageDifference || !excludeSavings) && useAirportCodes) return ReportNames.CARRIER_CONCENTRATION_A;

            return ReportNames.CARRIER_CONCENTRATION;
        }

        public void SwapOriginsAndDestinations(ReportGlobals globals)
        {
            //origin/dest
            var temp = globals.GetParmValue(WhereCriteria.ORIGIN);
            globals.SetParmValue(WhereCriteria.ORIGIN, globals.GetParmValue(WhereCriteria.DESTINAT));
            globals.SetParmValue(WhereCriteria.DESTINAT, temp);
            temp = globals.GetParmValue(WhereCriteria.INORGS);
            globals.SetParmValue(WhereCriteria.INORGS, globals.GetParmValue(WhereCriteria.INDESTS));
            globals.SetParmValue(WhereCriteria.INDESTS, temp);

            //metro
            temp = globals.GetParmValue(WhereCriteria.METROORG);
            globals.SetParmValue(WhereCriteria.METROORG, globals.GetParmValue(WhereCriteria.METRODEST));
            globals.SetParmValue(WhereCriteria.METRODEST, temp);
            temp = globals.GetParmValue(WhereCriteria.INMETROORGS);
            globals.SetParmValue(WhereCriteria.INMETROORGS, globals.GetParmValue(WhereCriteria.INMETRODESTS));
            globals.SetParmValue(WhereCriteria.INMETRODESTS, temp);

            //country
            temp = globals.GetParmValue(WhereCriteria.ORIGCOUNTRY);
            globals.SetParmValue(WhereCriteria.ORIGCOUNTRY, globals.GetParmValue(WhereCriteria.DESTCOUNTRY));
            globals.SetParmValue(WhereCriteria.DESTCOUNTRY, temp);
            temp = globals.GetParmValue(WhereCriteria.INORIGCOUNTRY);
            globals.SetParmValue(WhereCriteria.INORIGCOUNTRY, globals.GetParmValue(WhereCriteria.INDESTCOUNTRY));
            globals.SetParmValue(WhereCriteria.INDESTCOUNTRY, temp);


            //region
            temp = globals.GetParmValue(WhereCriteria.ORIGREGION);
            globals.SetParmValue(WhereCriteria.ORIGREGION, globals.GetParmValue(WhereCriteria.DESTREGION));
            globals.SetParmValue(WhereCriteria.DESTREGION, temp);
            temp = globals.GetParmValue(WhereCriteria.INORIGREGION);
            globals.SetParmValue(WhereCriteria.INORIGREGION, globals.GetParmValue(WhereCriteria.INDESTREGION));
            globals.SetParmValue(WhereCriteria.INDESTREGION, temp);
        }

        public IList<string> GetExportFields()
        {
            return new List<string>
            {
     //comment out 3 columns which didn't appear on the report I test. 7/11/2018, TODO:we can delete them later.
     //           "orgdesc",
     //           "destdesc",
                "origin",
                "destinat",
                "segments",
                "totvolume",
                "avgsegcost",
                "carrier",
                "carrsegs",
                "pcntoftot",
                "carrvolume",
                "carravgseg",
                "avgsegdiff",
                "carrsvngs",
                "othsvngs"//,
       //         "fare"
            };
        }

        public bool IsExcelOrCsvOutput(ReportGlobals globals)
        {
            return globals.OutputFormat == DestinationSwitch.Csv || globals.OutputFormat == DestinationSwitch.Xls;
        }

        public decimal GetAvgFare(decimal fare, int segments)
        {
            return MathHelper.Round(fare / segments, 2);
        }

        public decimal GetCarrierPercentage(int carrierOneSegs, int segments)
        {
            return MathHelper.Percent(carrierOneSegs,segments, 2);
        }

        public decimal GetOtherPercentage(int segments, int carrierOneSegs)
        {
            // ReSharper disable once PossibleLossOfFraction
            return MathHelper.Percent((segments - carrierOneSegs), segments, 2);
        }

        public decimal GetCarrierAverage(decimal carrierOneFare, int carrierOneSegs)
        {
            return MathHelper.Round(carrierOneFare / carrierOneSegs, 2);
        }

        public decimal GetOtherAverage(decimal fare, decimal carrierOneFare, int segments, int carrierOneSegs)
        {
            return MathHelper.Round((fare - carrierOneFare) / (segments - carrierOneSegs), 2);
        }

        public decimal GetCarrierSavings(int segments, int carrierOneSegs, decimal otherAvg, decimal carrierAvg)
        {
            var savings = (segments - carrierOneSegs) * (otherAvg - carrierAvg);

            return savings < 0 ? 0 : savings;
        }

        public decimal GetOtherSavings(int carrierOneSegs, decimal carrierAvg, decimal otherAvg, int segments)
        {
            var otherSavings = carrierOneSegs * (carrierAvg - otherAvg);
            if (otherSavings < 0 || (segments - carrierOneSegs == 0)) return 0;

            return otherSavings;
        }

        public decimal GetAvgSegmentDifference(decimal otherAvg, decimal carrierAvg)
        {
            return (otherAvg > 0 && carrierAvg > 0)
                    ? otherAvg - carrierAvg
                    : 0;
        }

        public bool UseRecordLimit(string sortBy)
        {
            return sortBy == "2" || sortBy == "3";
        }

        public decimal GetAccountFare(decimal airCharge, int numberSegments)
        {
            return MathHelper.Round(airCharge / numberSegments, 2);
        }

        public bool IsSortByAscending(ReportGlobals globals)
        {
            return globals.ParmValueEquals(WhereCriteria.RBSORTDESCASC, "2");
        }
    }
}