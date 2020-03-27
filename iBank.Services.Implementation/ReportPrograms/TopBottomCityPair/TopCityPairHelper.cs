using System;
using iBank.Server.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomCityPair
{
    public class TopCityPairHelper 
    {
        public TopCityPairHelper()
        {
        }

        public string GetCrystalReportName(bool onlineAdopt, bool excludeMileage, bool useTicketCount, bool carbonReporting)
        {
            string standardRpt = onlineAdopt
                ? "ibTopCityPairOnlineAdopt"
                : excludeMileage
                    ? "ibTopCityPair2"
                    : "ibTopCityPair";
            if (useTicketCount) standardRpt += "A";

            if (carbonReporting && !onlineAdopt) standardRpt += "Carb";
            return standardRpt;
        }

        public int HowManyRecords(string howMany, DataTypes.SortBy sortBy)
        {
            int records = Convert.ToInt32(howMany);
            switch (sortBy)
            {
                case DataTypes.SortBy.VOLUME_BOOKED:
                case DataTypes.SortBy.AVG_COST_PER_SEGMENT_OR_TRIP:
                case DataTypes.SortBy.NO_OF_SEGMENT_OR_TRIP:
                    break;
                default:
                    records = 0;
                    break;
            }
            return records;
        }
       
        public bool GetCarbonReporting(bool carbonEmissions, bool isExcludeMileage, string carbCalc)
        {
            if (isExcludeMileage) return false;
            if (!carbCalc.IsNullOrWhiteSpace() && carbonEmissions) return true;
            else return false;
        }

    }
}
