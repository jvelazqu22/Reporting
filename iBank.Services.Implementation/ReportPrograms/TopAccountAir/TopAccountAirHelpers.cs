using CODE.Framework.Core.Utilities;
using Domain.Models.ReportPrograms.TopAccountAir;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.TopAccountAir
{
    public static class TopAccountAirHelpers
    {
        public static List<FinalData> SortData(List<FinalData> finalDataList, bool descending, string sortBy)
        {
            switch (sortBy)
            {
                case "1":
                    finalDataList = @descending
                        ? finalDataList.OrderByDescending(s => s.Amt).ToList()
                        : finalDataList.OrderBy(s => s.Amt).ToList();
                    break;
                case "2":
                    finalDataList = @descending
                        ? finalDataList.OrderByDescending(s => s.Trips == 0 ? 0 : s.Amt / s.Trips).ToList()
                        : finalDataList.OrderBy(s => s.Trips == 0 ? 0 : s.Amt / s.Trips).ToList();
                    break;
                case "3":
                    finalDataList = @descending
                        ? finalDataList.OrderByDescending(s => s.Trips).ToList()
                        : finalDataList.OrderBy(s => s.Trips).ToList();
                    break;
                case "4":
                    finalDataList = @descending
                        ? finalDataList.OrderByDescending(s => s.Acommisn).ToList()
                        : finalDataList.OrderBy(s => s.Acommisn).ToList();
                    break;
                case "6":
                    finalDataList = @descending
                        ? finalDataList.OrderByDescending(s => s.Svcfee).ToList()
                        : finalDataList.OrderBy(s => s.Svcfee).ToList();
                    break;
                default:
                    finalDataList = @descending
                        ? finalDataList.OrderByDescending(s => s.AcctName).ToList()
                        : finalDataList.OrderBy(s => s.AcctName).ToList();
                    break;
            }

            return finalDataList;
        }

        public static List<RawData> Collapse(List<RawData> rawDataList)
        {
            var bobs = rawDataList.OrderBy(s => s.RecKey).ThenBy(s => s.SeqNo).GroupBy(s => s.RecKey);

            var newData = new List<RawData>();
            var newRec = new RawData();
            foreach (var bob in bobs)
            {
                var firstRow = true;
                foreach (var row in bob)
                {
                    if (firstRow)
                    {

                        newRec = new RawData();
                        Mapper.Map(row, newRec);
                        newData.Add(newRec);
                    }
                    firstRow = false;
                    if (row.Connect.Equals("O"))
                    {
                        newRec.RArrDate = row.RArrDate;
                        newRec.ArrTime = row.ArrTime;
                        newRec.Destinat = row.Destinat;
                        firstRow = true;
                    }

                }
            }
            return newData;

        }

        public static List<string> GetExportFields(string groupBy, bool lowFare)
        {
            var fields = new List<string>();
            if (groupBy.Equals("3"))
            {
                fields.Add("account as SourceAbbr");
                fields.Add("acctname as SourceDesc");
            }
            else
            {
                fields.Add("account");
                fields.Add("acctname");
            }

            fields.Add("Amt");
            fields.Add("Trips");
            fields.Add("SvcFee");
            fields.Add("AvgCost");
            fields.Add("aCommisn");

            if (lowFare)
            {
                fields.Add("LowFare");
                fields.Add("LostAmt");
            }


            return fields;
        }

        public static string GetColHead1(string groupBy)
        {
            switch (groupBy)
            {
                case "2":
                    return "Parent Account";
                case "3":
                    return "Data Source";
                default:
                    return "Account";

            }
        }

        public static decimal GetGraphData1(FinalData rec, string sortBy)
        {
            switch (sortBy)
            {
                case "2":
                    return rec.AvgCost;
                case "3":
                    return rec.Trips;
                case "4":
                    return rec.Acommisn;
                default:
                    return rec.Amt;
            }
        }

        // Removes Service fee records not included in filtered data
        public static List<SvcFeeData> FilterSvcFees(List<SvcFeeData> svcFeeData, List<RawData> filteredData)
        {
            svcFeeData.RemoveAll(s =>
            {
                var filteredDataRecord = filteredData.LastOrDefault(t => t.RecKey.Equals(s.RecKey));
                return filteredDataRecord == null;
            });
            return svcFeeData;
        }
    }
}
