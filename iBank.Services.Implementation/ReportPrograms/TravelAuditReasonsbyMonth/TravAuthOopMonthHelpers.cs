using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using Domain.Helper;
using Domain.Models.ReportPrograms.TravelAuditReasonsbyMonthReport;
using Domain.Models.TravelAuditReasonsbyMonthReport;
using Domain.Orm.iBankClientQueries;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using iBank.Services.Implementation.Shared.Client;

namespace iBank.Services.Implementation.ReportPrograms.TravelAuditReasonsbyMonth
{
    public static class TravAuthOopMonthHelpers
    {
        public static List<GroupedData> SplitCodes(List<GroupedData> groupedData, ReportGlobals globals)
        {
            var groupedDataWithMultipleCodes = groupedData.Where(s => s.OutPolCods.Contains(",")).ToList();
            groupedData.RemoveAll(s => groupedDataWithMultipleCodes.Contains(s));

            var oopCrit = globals.GetParmValue(WhereCriteria.INOOPCODES);
            var notInOopList = globals.IsParmValueOn(WhereCriteria.NOTINOOPCODES);

            var oopCritList = string.IsNullOrEmpty(oopCrit)
                ? new List<string>()
                : oopCrit.Split(',').ToList();

            foreach (var row in groupedDataWithMultipleCodes)
            {
                var recCntr = 1;
                foreach (var oopCode in row.OutPolCods.Split(','))
                {
                    var keep = true;
                    if (oopCritList.Any())
                    {
                        if (notInOopList)
                        {
                            if (oopCritList.Contains(oopCode))
                            {
                                keep = false;
                            }
                        }
                        else
                        {
                            if (!oopCritList.Contains(oopCode))
                            {
                                keep = false;
                            }
                        }
                    }
                    if (keep)
                    {
                        groupedData.Add(new GroupedData
                        {
                            RecKey = row.RecKey,
                            Acct = row.Acct,
                            OutPolCods = oopCode,
                            UseDate = row.UseDate,
                            RecCntr = recCntr,
                            AuthStatus = row.AuthStatus
                        });
                        recCntr = 0;
                    }
                }
            }

            return groupedData;
        }

        public static List<FinalData> BuildMainReport(List<GroupedData> groupedData, int year, ReportGlobals globals, ClientFunctions clientFunctions,
            IClientDataStore clientStore, IMasterDataStore masterStore)
        {
            var months = GetMonths(year);

            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(clientStore.ClientQueryDb, globals.Agency);

            var temp = groupedData.GroupBy(
                    s =>
                        new
                        {
                            s.OutPolCods,
                            OopDesc = clientFunctions.LookupReason(getAllMasterAccountsQuery, s.OutPolCods, s.Acct, clientStore, globals, masterStore.MastersQueryDb)
                        },
                    (key, recs) =>
                    {
                        var reclist = recs.ToList();
                        return new FinalData
                        {
                            Outpolcods = key.OutPolCods.Trim(),
                            Oopdesc = key.OopDesc.Trim(),
                            Mth1Cnt = reclist.Sum(s => s.UseDate.IsBetween(months[0].Item1, months[0].Item2) ? 1 : 0),
                            Mth2Cnt = reclist.Sum(s => s.UseDate.IsBetween(months[1].Item1, months[1].Item2) ? 1 : 0),
                            Mth3Cnt = reclist.Sum(s => s.UseDate.IsBetween(months[2].Item1, months[2].Item2) ? 1 : 0),
                            Mth4Cnt = reclist.Sum(s => s.UseDate.IsBetween(months[3].Item1, months[3].Item2) ? 1 : 0),
                            Mth5Cnt = reclist.Sum(s => s.UseDate.IsBetween(months[4].Item1, months[4].Item2) ? 1 : 0),
                            Mth6Cnt = reclist.Sum(s => s.UseDate.IsBetween(months[5].Item1, months[5].Item2) ? 1 : 0),
                            Mth7Cnt = reclist.Sum(s => s.UseDate.IsBetween(months[6].Item1, months[6].Item2) ? 1 : 0),
                            Mth8Cnt = reclist.Sum(s => s.UseDate.IsBetween(months[7].Item1, months[7].Item2) ? 1 : 0),
                            Mth9Cnt = reclist.Sum(s => s.UseDate.IsBetween(months[8].Item1, months[8].Item2) ? 1 : 0),
                            Mth10Cnt = reclist.Sum(s => s.UseDate.IsBetween(months[9].Item1, months[9].Item2) ? 1 : 0),
                            Mth11Cnt = reclist.Sum(s => s.UseDate.IsBetween(months[10].Item1, months[10].Item2) ? 1 : 0),
                            Mth12Cnt = reclist.Sum(s => s.UseDate.IsBetween(months[11].Item1, months[11].Item2) ? 1 : 0),
                            YtdCnt = reclist.Count
                        };
                    })
                    .OrderBy(s => s.Oopdesc)
                    .ToList();

            return CalculatePercentages(temp);
        }

        public static List<SummaryFinalData> BuildSummary(List<GroupedData> groupedData, int year, ReportGlobals globals)
        {
            var months = GetMonths(year);
            var sixtySpaces = new string(' ', 60);
            var temp = groupedData.GroupBy(s => s.AuthStatus.Trim(),
                (key, recs) =>
                {
                    var reclist = recs.ToList();
                    return new SummaryFinalData
                    {
                        AuthStatus = key,
                        StatDesc = sixtySpaces,
                        Mth1Cnt = reclist.Sum(s => s.UseDate.IsBetween(months[0].Item1, months[0].Item2) ? s.RecCntr : 0),
                        Mth2Cnt = reclist.Sum(s => s.UseDate.IsBetween(months[1].Item1, months[1].Item2) ? s.RecCntr : 0),
                        Mth3Cnt = reclist.Sum(s => s.UseDate.IsBetween(months[2].Item1, months[2].Item2) ? s.RecCntr : 0),
                        Mth4Cnt = reclist.Sum(s => s.UseDate.IsBetween(months[3].Item1, months[3].Item2) ? s.RecCntr : 0),
                        Mth5Cnt = reclist.Sum(s => s.UseDate.IsBetween(months[4].Item1, months[4].Item2) ? s.RecCntr : 0),
                        Mth6Cnt = reclist.Sum(s => s.UseDate.IsBetween(months[5].Item1, months[5].Item2) ? s.RecCntr : 0),
                        Mth7Cnt = reclist.Sum(s => s.UseDate.IsBetween(months[6].Item1, months[6].Item2) ? s.RecCntr : 0),
                        Mth8Cnt = reclist.Sum(s => s.UseDate.IsBetween(months[7].Item1, months[7].Item2) ? s.RecCntr : 0),
                        Mth9Cnt = reclist.Sum(s => s.UseDate.IsBetween(months[8].Item1, months[8].Item2) ? s.RecCntr : 0),
                        Mth10Cnt = reclist.Sum(s => s.UseDate.IsBetween(months[9].Item1, months[9].Item2) ? s.RecCntr : 0),
                        Mth11Cnt = reclist.Sum(s => s.UseDate.IsBetween(months[10].Item1, months[10].Item2) ? s.RecCntr : 0),
                        Mth12Cnt = reclist.Sum(s => s.UseDate.IsBetween(months[11].Item1, months[11].Item2) ? s.RecCntr : 0),
                        YtdCnt = reclist.Sum(s => s.RecCntr),
                    };
                })
                .ToList();

            temp = SetStatusDescription(temp, globals);
            return CalculatePercentages(temp);

        }

        private static List<Tuple<DateTime, DateTime>> GetMonths(int year)
        {
            var months = new List<Tuple<DateTime, DateTime>>();
            for (int i = 1; i <= 12; i++)
            {
                var startMonth = new DateTime(year, i, 1);

                months.Add(new Tuple<DateTime, DateTime>(startMonth, startMonth.AddMonths(1).AddSeconds(-1)));
            }

            return months;
        }

        public static List<RawData> ProcessStatuses(List<RawData> rawDataList, ReportGlobals globals)
        {
            var includes = new List<string>();
            if (!globals.IsParmValueOn(WhereCriteria.CBEXCLAPPRTVL))
            {
                includes.Add("A");
            }
            if (!globals.IsParmValueOn(WhereCriteria.CBEXCLDECLINEDTVL))
            {
                includes.Add("D");
            }
            if (!globals.IsParmValueOn(WhereCriteria.CBEXPIREDREQS))
            {
                includes.Add("E");
            }
            if (!globals.IsParmValueOn(WhereCriteria.CBEXCLNOTIFONLY))
            {
                includes.Add("N");
            }
            return rawDataList.Where(s => includes.Contains(s.AuthStatus.Trim())).ToList();

        }

        public static List<RawData> ProcessOutOfPolicy(List<RawData> rawDataList, ReportGlobals globals)
        {

            var rowsToKeep = new List<RawData>();
            var oopCrit = globals.GetParmValue(WhereCriteria.INOOPCODES);
            if (string.IsNullOrEmpty(oopCrit)) return rawDataList;

            var oopCritList = oopCrit.Split(',');

            var notIn = globals.IsParmValueOn(WhereCriteria.NOTINOOPCODES);
            foreach (var row in rawDataList)
            {
                if (!string.IsNullOrEmpty(row.OutPolCods))
                {
                    var codes = row.OutPolCods.Split(',');
                    foreach (var code in codes)
                    {
                        if (notIn)
                        {
                            if (!oopCritList.Contains(code))
                            {
                                rowsToKeep.Add(row);
                            }
                        }
                        else
                        {
                            if (oopCritList.Contains(code))
                            {
                                rowsToKeep.Add(row);
                            }
                        }
                    }
                }
            }

            return rowsToKeep;
        }

        public static List<SummaryFinalData> SetStatusDescription(List<SummaryFinalData> summaryList, ReportGlobals globals)
        {
            var xTotalApproved = LookupFunctions.LookupLanguageTranslation("xTotalApproved", "Total Approved", globals.LanguageVariables);
            var xTotalDeclined = LookupFunctions.LookupLanguageTranslation("xTotalDeclined", "Total Declined", globals.LanguageVariables);
            var xTotalExpired = LookupFunctions.LookupLanguageTranslation("xTotalExpired", "Total Expired", globals.LanguageVariables);
            var xTotalNotified = LookupFunctions.LookupLanguageTranslation("xTotalNotified", "Total Notified", globals.LanguageVariables);

            if (!globals.IsParmValueOn(WhereCriteria.CBEXCLAPPRTVL))
            {
                var rec = summaryList.FirstOrDefault(s => s.AuthStatus.Equals("A"));
                if (rec == null)
                {
                    rec = new SummaryFinalData
                    {
                        AuthStatus = "A"
                    };
                    summaryList.Add(rec);
                }
                rec.StatDesc = xTotalApproved;
                rec.SortOrder = 1;
            }

            if (!globals.IsParmValueOn(WhereCriteria.CBEXCLDECLINEDTVL))
            {
                var rec = summaryList.FirstOrDefault(s => s.AuthStatus.Equals("D"));
                if (rec == null)
                {
                    rec = new SummaryFinalData
                    {
                        AuthStatus = "D"
                    };
                    summaryList.Add(rec);
                }
                rec.StatDesc = xTotalDeclined;
                rec.SortOrder = 2;
            }

            if (!globals.IsParmValueOn(WhereCriteria.CBEXPIREDREQS))
            {
                var rec = summaryList.FirstOrDefault(s => s.AuthStatus.Equals("N"));
                if (rec == null)
                {
                    rec = new SummaryFinalData
                    {
                        AuthStatus = "N"
                    };
                    summaryList.Add(rec);
                }
                rec.StatDesc = xTotalNotified;
                rec.SortOrder = 3;
            }

            if (!globals.IsParmValueOn(WhereCriteria.CBEXCLNOTIFONLY))
            {
                var rec = summaryList.FirstOrDefault(s => s.AuthStatus.Equals("E"));
                if (rec == null)
                {
                    rec = new SummaryFinalData
                    {
                        AuthStatus = "E"
                    };
                    summaryList.Add(rec);
                }
                rec.StatDesc = xTotalExpired;
                rec.SortOrder = 4;
            }

            return summaryList.OrderBy(s => s.SortOrder).ToList();

        }

        public static List<T> CalculatePercentages<T>(List<T> summaryFinalDataList) where T : class, IFinalData
        {
            var mth1Tot = summaryFinalDataList.Sum(s => s.Mth1Cnt);
            var mth2Tot = summaryFinalDataList.Sum(s => s.Mth2Cnt);
            var mth3Tot = summaryFinalDataList.Sum(s => s.Mth3Cnt);
            var mth4Tot = summaryFinalDataList.Sum(s => s.Mth4Cnt);
            var mth5Tot = summaryFinalDataList.Sum(s => s.Mth5Cnt);
            var mth6Tot = summaryFinalDataList.Sum(s => s.Mth6Cnt);
            var mth7Tot = summaryFinalDataList.Sum(s => s.Mth7Cnt);
            var mth8Tot = summaryFinalDataList.Sum(s => s.Mth8Cnt);
            var mth9Tot = summaryFinalDataList.Sum(s => s.Mth9Cnt);
            var mth10Tot = summaryFinalDataList.Sum(s => s.Mth10Cnt);
            var mth11Tot = summaryFinalDataList.Sum(s => s.Mth11Cnt);
            var mth12Tot = summaryFinalDataList.Sum(s => s.Mth12Cnt);
            var ytdTot = summaryFinalDataList.Sum(s => s.YtdCnt);


            foreach (var row in summaryFinalDataList)
            {
                row.Mth1Pcnt = mth1Tot == 0 ? 0 : Math.Round(100 * (row.Mth1Cnt / mth1Tot), 2);
                row.Mth2Pcnt = mth2Tot == 0 ? 0 : Math.Round(100 * (row.Mth2Cnt / mth2Tot), 2);
                row.Mth3Pcnt = mth3Tot == 0 ? 0 : Math.Round(100 * (row.Mth3Cnt / mth3Tot), 2);
                row.Mth4Pcnt = mth4Tot == 0 ? 0 : Math.Round(100 * (row.Mth4Cnt / mth4Tot), 2);
                row.Mth5Pcnt = mth5Tot == 0 ? 0 : Math.Round(100 * (row.Mth5Cnt / mth5Tot), 2);
                row.Mth6Pcnt = mth6Tot == 0 ? 0 : Math.Round(100 * (row.Mth6Cnt / mth6Tot), 2);
                row.Mth7Pcnt = mth7Tot == 0 ? 0 : Math.Round(100 * (row.Mth7Cnt / mth7Tot), 2);
                row.Mth8Pcnt = mth8Tot == 0 ? 0 : Math.Round(100 * (row.Mth8Cnt / mth8Tot), 2);
                row.Mth9Pcnt = mth9Tot == 0 ? 0 : Math.Round(100 * (row.Mth9Cnt / mth9Tot), 2);
                row.Mth10Pcnt = mth10Tot == 0 ? 0 : Math.Round(100 * (row.Mth10Cnt / mth10Tot), 2);
                row.Mth11Pcnt = mth11Tot == 0 ? 0 : Math.Round(100 * (row.Mth11Cnt / mth11Tot), 2);
                row.Mth12Pcnt = mth12Tot == 0 ? 0 : Math.Round(100 * (row.Mth12Cnt / mth12Tot), 2);
                row.YtdPcnt = ytdTot == 0 ? 0 : Math.Round(100 * (row.YtdCnt / ytdTot), 2);
            }


            return summaryFinalDataList;
        }

        public static List<FinalData> CombineFinalAndSummary(List<FinalData> finalDataList, List<SummaryFinalData> summaryFinalDataList)
        {
            finalDataList.AddRange(summaryFinalDataList.Select(s =>
            {
                var newFinalData = new FinalData();
                Mapper.Map(s, newFinalData);
                newFinalData.Outpolcods = s.AuthStatus;
                newFinalData.Oopdesc = s.StatDesc;
                return newFinalData;
            }));

            return finalDataList;
        }

        public static List<string> GetExportFields()
        {
            return new List<string>
            {
                "outPolCods as oopAutCode",
                "oopDesc as oopAutDesc",
                "mth1Cnt",
                "mth1Pcnt",
                "mth2Cnt",
                "mth2Pcnt",
                "mth3Cnt",
                "mth3Pcnt",
                "mth4Cnt",
                "mth4Pcnt",
                "mth5Cnt",
                "mth5Pcnt",
                "mth6Cnt",
                "mth6Pcnt",
                "mth7Cnt",
                "mth7Pcnt",
                "mth8Cnt",
                "mth8Pcnt",
                "mth9Cnt",
                "mth9Pcnt",
                "mth10Cnt",
                "mth10Pcnt",
                "mth11Cnt",
                "mth11Pcnt",
                "mth12Cnt",
                "mth12Pcnt",
                "ytdCnt",
                "ytdPcnt"
            };

        }

        public static DateTime GetUseDate(RawData row, string dateRange)
        {
            switch (dateRange)
            {
                case "3":
                    return row.Bookdate.GetValueOrDefault();
                case "12":
                    return row.StatusTime.GetValueOrDefault();
                default:
                    return row.Depdate.GetValueOrDefault();
            }
        }

        public static int ProcessYear(ReportGlobals globals)
        {
            var year = globals.GetParmValue(WhereCriteria.STARTYEAR).TryIntParse(-1);
            if (!year.IsBetween(1998, 2030))
            {
                globals.ReportInformation.ReturnCode = 2;
                globals.ReportInformation.ErrorMessage = globals.ReportMessages.RptMsg_DateRange;
                return -1;
            }
            globals.BeginDate = new DateTime(year, 1, 1);
            globals.EndDate = new DateTime(year, 12, 31, 23, 59, 59);
            return year;
        }
    }
}
