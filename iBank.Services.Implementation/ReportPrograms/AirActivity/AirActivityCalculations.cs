using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Helper;
using Domain.Models.ReportPrograms.AirActivityReport;
using iBank.Services.Implementation.Shared;
using Domain.Orm.Classes;

namespace iBank.Services.Implementation.ReportPrograms.AirActivity
{
    public class AirActivityCalculations
    {

        public int GetTotalVoidedTickets(List<FinalData> finalList)
        {
            return finalList.Where(w => w.Trantype.Equals("V")).Select(s => s.RecKey).Distinct().Count();
        }

        public decimal GetTotalValuedOfVoidedTickets(List<FinalData> finalList)
        {
            var groupRecords = finalList.GroupBy(s => new { s.RecKey }, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new FinalData
                {
                    RecKey = key.RecKey,
                    Trantype = recs.First().Trantype,
                    Airchg = recs.First().Airchg
                };
            }).Where(x=>x.Trantype.Equals("V"))
            .ToList();

            return groupRecords.Sum(s => s.Airchg);
        }

        public string GetCrystalReportName(bool isReservationReport, bool isCarbonReport, bool includeAltEmissions)
        {
            if (isReservationReport)
            {
                if (isCarbonReport)
                {
                    if (includeAltEmissions)
                    {
                        return ReportNames.AIR_ACTIVITY_RPT_2_ALT_EMISSIONS;
                    }

                    return ReportNames.AIR_ACTIVITY_RPT_2_NO_ALT_EMISSIONS;
                }

                return ReportNames.AIR_ACTIVITY_RPT_2;
            }
            else
            {
                if (isCarbonReport)
                {
                    if (includeAltEmissions)
                    {
                        return ReportNames.AIR_ACTIVITY_RPT_ALT_EMISSIONS;
                    }

                    return ReportNames.AIR_ACTIVITY_RPT_NO_ALT_EMISSIONS;
                }

                return ReportNames.AIR_ACTIVITY_RPT;
            }
        }

        public string GetColumnHeader(bool dateSort, int sortBy, List<LanguageVariableInfo> languageVariables)
        {
            var colHead = "Departure Date";
            
            if (dateSort)
            {
                switch ((DateType)sortBy)
                {
                    case DateType.InvoiceDate:
                        colHead = LookupFunctions.LookupLanguageTranslation("xInvoiceDate", "Invoice Date", languageVariables);
                        break;
                    case DateType.BookedDate:
                        colHead = LookupFunctions.LookupLanguageTranslation("ll_BookedDate", "Booked Date", languageVariables);
                        break;
                    default:
                        colHead = LookupFunctions.LookupLanguageTranslation("ll_DepartureDate", "Departure Date", languageVariables);
                        break;
                }
            }

            return colHead;
        }

        public decimal GetTotalAirCharge(IList<FinalData> rawData)
        {
            var grouped = rawData.GroupBy(s => new { reckey = s.RecKey, airchg = s.Airchg }).Select(s => new { s.Key.reckey, s.Key.airchg });

            return grouped.Sum(x => x.airchg);
        }

        public string GetUnitOfWeightType(bool useMetric, bool isCarbonReport, string carbonCalculator)
        {
            var unit = "Lbs.";

            if (useMetric && isCarbonReport && !string.IsNullOrEmpty(carbonCalculator))
            {
                unit = "Kgs";
            }

            return unit;
        }

        public int GetUdid(string possibleUdid)
        {
            var udid = 0;
            int.TryParse(possibleUdid, out udid);
            return udid;
        }

        public string GetFlightSegmentsWhereText(string flightSegments)
        {
            if (!string.IsNullOrEmpty(flightSegments)) return "Flight Markets = " + flightSegments;

            return "";
        }

    }
}
