using Domain;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Server.Utilities;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ReportLokkup;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.LookupFieldHandlers
{
    public class CppTicketIndicatorHelper
    {
        public string FindIndicator(int reckey, ReportLookups reportLookups, List<MarketSegmentRawData> marketSegmentDataList)
        {
            /* The new field will have a value of Y if any of the Fare Type values from the segment level are Dash CA, YCA, or CPP Business. 
              * The new field will have a value of N if the Fare Type values are anything other than the three listed above.
              * */
            if (marketSegmentDataList.Count == 0) return "N";

            var recs = marketSegmentDataList.Where(x => x.RecKey == reckey).ToList();

            if (recs.Count > 0)
            {
                foreach (var item in recs)
                {
                    var fareType = string.Empty;
                    if (Features.FareType.IsEnabled())
                    {
                        fareType = FareTypeHandler.LookupFareType(item.Prdfbase);
                    }
                    else
                    {
                        fareType = reportLookups.LookupFareType(item.Prdfbase);
                    }
                    if (fareType.EqualsIgnoreCase("Dash CA")) return "Y";
                    if (fareType.EqualsIgnoreCase("YCA")) return "Y";
                    if (fareType.EqualsIgnoreCase("CPP Business")) return "Y";
                }
            }
            return "N";
        }
    }
}
