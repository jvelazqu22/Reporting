using Domain.Models.ReportPrograms.TopBottomCars;
using iBank.Services.Implementation.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomCars
{
    public class TopBottomCarsRawData
    {
        public List<GroupedRawData> GroupRawData(GlobalsCalculator GlobalCalc, List<RawData> RawDataList, string _groupBy, ref string _subTitle, ref string _catDesc1, ref string _catDesc2)
        {
            List<GroupedRawData> groupedRawData;
            switch (_groupBy)
            {
                case "2":
                    groupedRawData =
                        RawDataList.GroupBy(s => new { s.Autocity, s.Autostat, s.Abookrat },
                            (key, recs) => GetGroupedRecord(recs, GlobalCalc.IsReservationReport(), key.Abookrat, key.Autocity, key.Autostat))
                            .ToList();
                    _subTitle = "Rental Cities";
                    _catDesc1 = "Car Rental City";
                    _catDesc2 = "Car Rental Cities";
                    break;
                case "3":
                    groupedRawData =
                        RawDataList.GroupBy(s => new { s.Autostat, s.Abookrat },
                            (key, recs) => GetGroupedRecord(recs, GlobalCalc.IsReservationReport(), key.Abookrat, key.Autostat))
                            .ToList();
                    _subTitle = "Rental States/Countries";
                    _catDesc1 = "Car Rental State/Country";
                    _catDesc2 = "Car Rental States/Countries";
                    break;
                case "4":
                    groupedRawData =
                        RawDataList.GroupBy(s => new { s.Autocity, s.Autostat, s.Company, s.Abookrat },
                            (key, recs) => GetGroupedRecord(recs, GlobalCalc.IsReservationReport(), key.Abookrat, key.Autocity, key.Autostat, key.Company))
                            .ToList();
                    _subTitle = "Rental Cities, with Breakdown by Rental Company";
                    _catDesc1 = "Car Rental City";
                    _catDesc2 = "Car Rental Cities";
                    break;

                default:
                    groupedRawData =
                        RawDataList.GroupBy(s => new { s.Company, s.Abookrat },
                            (key, recs) => GetGroupedRecord(recs, GlobalCalc.IsReservationReport(), key.Abookrat, key.Company))
                            .ToList();
                    _subTitle = "Rental Companies";
                    _catDesc1 = "Car Rental Company";
                    _catDesc2 = "Car Rental Companies";
                    break;
            }

            return groupedRawData;
        }

        private static GroupedRawData GetGroupedRecord(IEnumerable<RawData> recs, bool isPreview, decimal bookRate, string key, string col2 = "", string col3 = "")
        {
            var reclist = recs.ToList();
            return isPreview
                ? new GroupedRawData
                    {
                        Category = key.Trim(),
                        GroupColumn2 = col2.Trim(),
                        GroupColumn3 = col3.Trim(),
                        Rentals = reclist.Count,
                        Days = reclist.Sum(s => s.Days),
                        CarCost = reclist.Sum(s => s.Abookrat * s.Days),
                        SumBkRate = reclist.Sum(s => s.Abookrat),
                        ABookRate = bookRate
                    }
                : new GroupedRawData
                    {
                        Category = key.Trim(),
                        GroupColumn2 = col2.Trim(),
                        GroupColumn3 = col3.Trim(),
                        Rentals = reclist.Sum(s => s.Cplusmin),
                        Days = reclist.Sum(s => s.Days * s.Cplusmin),
                        CarCost = reclist.Sum(s => s.Abookrat * s.Days),
                        SumBkRate = reclist.Sum(s => s.Abookrat),
                        ABookRate = bookRate
                    };
        }
    }
}
