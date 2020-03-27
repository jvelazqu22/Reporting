using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.TopBottomCars;
using iBank.Server.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomCars
{
    public class TopCarHelpers
    {
        public List<FinalData> SortFinalData(string sortBy, List<FinalData> list, bool desc)
        {
            if (desc)
            {
                switch (sortBy)
                {
                    case "1":
                        return list.OrderByDescending(s => s.Carcost).ToList();
                    case "2":
                        return list.OrderByDescending(s => s.Rentals).ToList();
                    case "3":
                        return list.OrderByDescending(s => s.Days).ToList();
                    case "4":
                        return list.OrderByDescending(s => s.Avgbook).ToList();
                    default:
                        return list.OrderBy(s => s.Category).ToList();
                }

            }
            else
            {
                switch (sortBy)
                {
                    case "1":
                        return list.OrderBy(s => s.Carcost).ToList();
                    case "2":
                        return list.OrderBy(s => s.Rentals).ToList();
                    case "3":
                        return list.OrderBy(s => s.Days).ToList();
                    case "4":
                        return list.OrderBy(s => s.Avgbook).ToList();
                    default:
                        return list.OrderBy(s => s.Category).ToList();
                }
            }

        }

        public List<GroupedByCar> GroupData( string groupBy, List<GroupedRawData> groupedRawData)
        {
            List<GroupedByCar> groupedByCar;
            switch (groupBy)
            {
                case "4":
                    groupedByCar = groupedRawData
                        .Select(s => new
                        {
                            Category =
                                (string.IsNullOrEmpty(s.Category) ? "[UNKNOWN]" : s.Category) +
                                (string.IsNullOrEmpty(s.GroupColumn2) ? "" : ", " + s.GroupColumn2),
                            Company = string.IsNullOrEmpty(s.GroupColumn3) ? "[UNKNOWN]" : s.GroupColumn3,
                            s.Rentals,
                            s.Days,
                            s.ABookRate,
                            s.CarCost,
                            s.SumBkRate
                        })
                        .GroupBy(s => new { s.Category, s.Company }, (key, recs) =>
                        {
                            var reclist = recs.ToList();
                            return new GroupedByCar
                            {
                                Category = key.Category,
                                Company = key.Company,
                                Rentals = reclist.Sum(s => s.Rentals),
                                Days = reclist.Sum(s => s.Days),
                                NzDays = reclist.Sum(s => s.ABookRate == 0 ? 0 : s.Days),
                                CarCost = reclist.Sum(s => s.CarCost),
                                BookRate = reclist.Sum(s => s.SumBkRate),
                                BookCnt = reclist.Sum(s => s.ABookRate == 0 ? 0 : s.Rentals)
                            };
                        }).ToList();
                    break;
                case "2":
                    groupedByCar = groupedRawData
                        .Select(s => new
                        {
                            Category =
                                (string.IsNullOrEmpty(s.Category) ? "[UNKNOWN]" : s.Category) +
                                (string.IsNullOrEmpty(s.GroupColumn2) ? "" : ", " + s.GroupColumn2),
                            Company = string.IsNullOrEmpty(s.GroupColumn3) ? "[UNKNOWN]" : s.GroupColumn3,
                            s.Rentals,
                            s.Days,
                            s.ABookRate,
                            s.CarCost,
                            s.SumBkRate
                        })
                        .GroupBy(s => s.Category,
                            (key, recs) =>
                            {
                                var reclist = recs.ToList();
                                return new GroupedByCar
                                {
                                    Category = key,
                                    Rentals = reclist.Sum(s => s.Rentals),
                                    Days = reclist.Sum(s => s.Days),
                                    NzDays = reclist.Sum(s => s.ABookRate == 0 ? 0 : s.Days),
                                    CarCost = reclist.Sum(s => s.CarCost),
                                    BookRate = reclist.Sum(s => s.SumBkRate),
                                    BookCnt = reclist.Sum(s => s.ABookRate == 0 ? 0 : s.Rentals)
                                };
                            }).ToList();
                    break;

                default:
                    groupedByCar = groupedRawData
                        .GroupBy(s => string.IsNullOrEmpty(s.Category) ? "[UNKNOWN]" : s.Category,
                            (key, recs) =>
                            {
                                var reclist = recs.ToList();
                                return new GroupedByCar
                                {
                                    Category = key,
                                    Rentals = reclist.Sum(s => s.Rentals),
                                    Days = reclist.Sum(s => s.Days),
                                    NzDays = reclist.Sum(s => s.ABookRate == 0 ? 0 : s.Days),
                                    CarCost = reclist.Sum(s => s.CarCost),
                                    BookRate = reclist.Sum(s => s.SumBkRate),
                                    BookCnt = reclist.Sum(s => s.ABookRate == 0 ? 0 : s.Rentals)
                                };
                            }).ToList();
                    break;
            }
            return groupedByCar;
        }

        public List<FinalData> SortJoinedData(bool descending, string sortBy, List<FinalData> finalDataList)
        {
            if (!descending && sortBy != "5")
            {
                switch (sortBy)
                {
                    case "1":
                        finalDataList = finalDataList.OrderByDescending(s => s.Carcost)
                            .ThenBy(s => s.Category)
                            .ThenByDescending(s => s.Carcost2)
                            .ToList();
                        break;
                    case "2":
                        finalDataList = finalDataList.OrderByDescending(s => s.Rentals)
                            .ThenBy(s => s.Category)
                            .ThenByDescending(s => s.Rentals2)
                            .ToList();
                        break;
                    case "3":
                        finalDataList = finalDataList.OrderByDescending(s => s.Days)
                            .ThenBy(s => s.Category)
                            .ThenByDescending(s => s.Days2)
                            .ToList();
                        break;
                    case "4":
                        finalDataList = finalDataList.OrderByDescending(s => s.Avgbook)
                            .ThenBy(s => s.Category)
                            .ThenByDescending(s => s.Avgbook2)
                            .ToList();
                        break;
                }
            }
            else
            {
                switch (sortBy)
                {
                    case "1":
                        finalDataList = finalDataList.OrderBy(s => s.Carcost)
                            .ThenBy(s => s.Category)
                            .ThenBy(s => s.Carcost2)
                            .ToList();
                        break;
                    case "2":
                        finalDataList = finalDataList.OrderBy(s => s.Rentals)
                            .ThenBy(s => s.Category)
                            .ThenBy(s => s.Rentals2)
                            .ToList();
                        break;
                    case "3":
                        finalDataList = finalDataList.OrderBy(s => s.Days)
                            .ThenBy(s => s.Category)
                            .ThenBy(s => s.Days2)
                            .ToList();
                        break;
                    case "4":
                        finalDataList = finalDataList.OrderBy(s => s.Avgbook)
                            .ThenBy(s => s.Category)
                            .ThenBy(s => s.Avgbook2)
                            .ToList();
                        break;
                    case "5":
                        finalDataList = finalDataList.OrderBy(s => s.Category)
                            .ThenBy(s => s.Company)
                            .ToList();
                        break;
                }
            }
            return finalDataList;
        }

        public void JoinRanges(List<FinalData> finalDataList, List<FinalData> groupedRawData2)
        {
            var rowsToRemove = new List<FinalData>();
            foreach (var row in finalDataList)
            {
                var rows = groupedRawData2.FirstOrDefault(s => row.Category.ContainsIgnoreCase(s.Category));

                if (rows != null)
                {
                    row.Rentals2 = rows.Rentals;
                    row.Days2 = rows.Days;
                    row.Carcost2 = rows.Carcost;
                    row.Avgbook2 = rows.Avgbook;
                    row.Nzdays2 = rows.Nzdays;
                    rowsToRemove.Add(rows);
                }
            }
            groupedRawData2.RemoveAll(s => rowsToRemove.Contains(s));

            foreach (var rows in groupedRawData2)
            {
                var row = new FinalData();
                row.Category = rows.Category;
                row.Rentals2 = rows.Rentals;
                row.Days2 = rows.Days;
                row.Carcost2 = rows.Carcost;
                row.Avgbook2 = rows.Avgbook;
                row.Nzdays2 = rows.Nzdays;
                finalDataList.Add(row);
            }
        }

        public string GetReportName(bool secondRange, string groupBy)
        {
            //if (secondRange) return "ibTopCar2";
            if (secondRange) return "ibTopCar5";
            //if (groupBy.Equals("4")) return "ibTopCar3";
            if (groupBy.Equals("4")) return "ibTopCar4";
            return "ibTopCars";
        }

        public List<string> GetExportFields(bool secondRange, string groupBy)
        {
            var fieldList = new List<string>();
            

            fieldList.Add("category");
            if (groupBy.Equals("4")) fieldList.Add("company");
            fieldList.Add("rentals");
            fieldList.Add("days");
            fieldList.Add("carcost");
            fieldList.Add("bookrate");
            fieldList.Add("bookcnt");
            fieldList.Add("nzdays");
            fieldList.Add("avgbook");
            if (secondRange | groupBy.Equals("4"))
            {
                fieldList.Add("rentals2");
                fieldList.Add("days2");
                fieldList.Add("carcost2");
                fieldList.Add("bookrate2");
                fieldList.Add("bookcnt2");
                fieldList.Add("nzdays2");
                fieldList.Add("avgbook2");
            }

            return fieldList;
        }
    }
}
