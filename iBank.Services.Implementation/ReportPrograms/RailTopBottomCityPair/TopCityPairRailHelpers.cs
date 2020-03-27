using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Helper;
using Domain.Models.ReportPrograms.TopBottomCityPairRail;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.RailTopBottomCityPair
{
    public static class TopCityPairRailHelpers
    {
        /// <summary>
        /// handle calculated fields that are needed for Excel, CSV, but are automatically handled by Crystal
        /// </summary>
        /// <param name="finalDataList"></param>
        /// <param name="globals"></param>
        /// <param name="totCnt"></param>
        /// <param name="excludeMileage"></param>
        /// <returns></returns>
        public static void XlsCalculations(List<FinalData> finalDataList, ReportGlobals globals, int totCnt, bool excludeMileage)
        {
            if (!globals.ParmValueEquals(WhereCriteria.OUTPUTTYPE, "2") && !globals.ParmValueEquals(WhereCriteria.OUTPUTTYPE, "5")) return;

            foreach (var row in finalDataList)
            {
                if (totCnt != 0)
                {
                    row.CpPctTtl = Math.Round((decimal)(100 * (row.Cpsegs / totCnt)), 2);
                }
                if (row.Cpsegs != 0)
                {
                    row.CpPctTtl = Math.Round((decimal)(100 * (row.Segments / row.Cpsegs)), 2);
                }
                if (!excludeMileage)
                {
                    if (row.Cpmiles != 0)
                    {
                        row.CpCst_Mile = Math.Round(row.Cpcost / row.Cpmiles, 2);
                    }
                    if (row.Miles != 0)
                    {
                        row.Cst_Mile = Math.Round(row.Cost / row.Miles, 2);
                    }
                }
            }
            
        }
        public static List<FinalData> SortFinalData(List<FinalData> finalDataList, string sortBy, bool sortDescending, bool useTicketCount)
        {
            switch (sortBy)
            {
                case "1":
                    finalDataList = sortDescending
                        ? finalDataList.OrderByDescending(s => s.Cpcost)
                            .ThenBy(s => s.Orgdesc)
                            .ThenBy(s => s.Destdesc)
                            .ThenBy(s => s.Airline)
                            .ToList()
                        : finalDataList.OrderBy(s => s.Cpcost)
                            .ThenBy(s => s.Orgdesc)
                            .ThenBy(s => s.Destdesc)
                            .ThenBy(s => s.Airline)
                            .ToList();

                    break;
                case "2":
                    finalDataList = sortDescending
                        ? finalDataList.OrderByDescending(s => s.Cpavgcost)
                            .ThenBy(s => s.Orgdesc)
                            .ThenBy(s => s.Destdesc)
                            .ThenBy(s => s.Airline)
                            .ToList()
                        : finalDataList.OrderBy(s => s.Cpavgcost)
                            .ThenBy(s => s.Orgdesc)
                            .ThenBy(s => s.Destdesc)
                            .ThenBy(s => s.Airline)
                            .ToList();

                    break;
                case "3":
                    if (useTicketCount)
                    {
                        finalDataList = sortDescending
                            ? finalDataList.OrderByDescending(s => s.Cpnumticks)
                                .ThenByDescending(s => s.CpPctTtl)
                                .ThenBy(s => s.Orgdesc)
                                .ThenBy(s => s.Destdesc)
                                .ThenBy(s => s.Airline)
                                .ToList()
                            : finalDataList.OrderBy(s => s.Cpnumticks)
                                .ThenBy(s => s.CpPctTtl)
                                .ThenBy(s => s.Orgdesc)
                                .ThenBy(s => s.Destdesc)
                                .ThenBy(s => s.Airline)
                                .ToList();
                    }
                    else
                    {
                        finalDataList = sortDescending
                            ? finalDataList.OrderByDescending(s => s.Cpsegs)
                                .ThenByDescending(s => s.CpPctTtl)
                                .ThenBy(s => s.Orgdesc)
                                .ThenBy(s => s.Destdesc)
                                .ThenBy(s => s.Airline)
                                .ToList()
                            : finalDataList.OrderBy(s => s.Cpsegs)
                                .ThenBy(s => s.CpPctTtl)
                                .ThenBy(s => s.Orgdesc)
                                .ThenBy(s => s.Destdesc)
                                .ThenBy(s => s.Airline)
                                .ToList();
                    }


                    break;
                case "4":
                    finalDataList = finalDataList.OrderBy(s => s.Orgdesc).ThenBy(s => s.Destdesc).ToList();
                    break;
            }

            return finalDataList;
        }

        
        public static void AllocateAirCharge(List<RawData> list)
        {
            var temp = list.Where(s => s.ActFare == 0)
                .GroupBy(s => new {s.RecKey, AirChg = s.Airchg - s.Faretax}, (key, recs) => new
                {
                    key.RecKey,
                    key.AirChg,
                    NumSegs = recs.Count()
                }).ToList();

            foreach (var item in temp.Where(s => s.AirChg != 0 && s.NumSegs != 0))
            {
                var item1 = item;
                foreach (var row in list.Where(s => s.RecKey == item1.RecKey && s.ActFare == 0))
                {
                    row.ActFare = Math.Round(item.AirChg/item.NumSegs, 2);
                }
            }

        }

        public static void SwapOriginsAndDestinations(ReportGlobals globals)
        {
            var temp = globals.GetParmValue(WhereCriteria.ORIGIN);
            globals.SetParmValue(WhereCriteria.ORIGIN, globals.GetParmValue(WhereCriteria.DESTINAT));
            globals.SetParmValue(WhereCriteria.DESTINAT, temp);
            temp = globals.GetParmValue(WhereCriteria.INORGS);
            globals.SetParmValue(WhereCriteria.INORGS, globals.GetParmValue(WhereCriteria.INDESTS));
            globals.SetParmValue(WhereCriteria.INDESTS, temp);

            temp = globals.GetParmValue(WhereCriteria.METROORG);
            globals.SetParmValue(WhereCriteria.METROORG, globals.GetParmValue(WhereCriteria.METRODEST));
            globals.SetParmValue(WhereCriteria.METRODEST, temp);
            temp = globals.GetParmValue(WhereCriteria.INMETROORGS);
            globals.SetParmValue(WhereCriteria.INMETROORGS, globals.GetParmValue(WhereCriteria.INMETRODESTS));
            globals.SetParmValue(WhereCriteria.INMETRODESTS, temp);

            temp = globals.GetParmValue(WhereCriteria.ORIGCOUNTRY);
            globals.SetParmValue(WhereCriteria.ORIGCOUNTRY, globals.GetParmValue(WhereCriteria.DESTCOUNTRY));
            globals.SetParmValue(WhereCriteria.DESTCOUNTRY, temp);
            temp = globals.GetParmValue(WhereCriteria.INORIGCOUNTRY);
            globals.SetParmValue(WhereCriteria.INORIGCOUNTRY, globals.GetParmValue(WhereCriteria.INDESTCOUNTRY));
            globals.SetParmValue(WhereCriteria.INDESTCOUNTRY, temp);

            temp = globals.GetParmValue(WhereCriteria.ORIGREGION);
            globals.SetParmValue(WhereCriteria.ORIGREGION, globals.GetParmValue(WhereCriteria.DESTREGION));
            globals.SetParmValue(WhereCriteria.DESTREGION, temp);
            temp = globals.GetParmValue(WhereCriteria.INORIGREGION);
            globals.SetParmValue(WhereCriteria.INORIGREGION, globals.GetParmValue(WhereCriteria.INDESTREGION));
            globals.SetParmValue(WhereCriteria.INDESTREGION, temp);
        }

        public static List<string> GetExportFields(bool _excludeMileage)
        {
            var fieldList = new List<string>();

            fieldList.Add("origin");
            fieldList.Add("orgdesc");
            fieldList.Add("destinat");
            fieldList.Add("destdesc");
            fieldList.Add("cpsegs");
            fieldList.Add("cpNumTicks");
            fieldList.Add("cpPctTtl");
            fieldList.Add("cpcost");
            fieldList.Add("cpavgcost");
            if (!_excludeMileage)
            {
                fieldList.Add("cpmiles");
                fieldList.Add("cpCst_Mile");
            }

            fieldList.Add("airline");
            fieldList.Add("alinedesc");
            fieldList.Add("segments");
            fieldList.Add("PctTtl");
            fieldList.Add("NumTicks");
            fieldList.Add("cost");
            if (!_excludeMileage)
            {
                fieldList.Add("miles");
                fieldList.Add("Cst_Mile");
            }

            return fieldList;
        }

    }
}
