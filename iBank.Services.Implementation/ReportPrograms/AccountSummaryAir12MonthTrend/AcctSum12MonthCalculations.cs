using System;
using System.Collections.Generic;
using System.Linq;
using CODE.Framework.Core.Utilities.Extensions;
using Domain.Helper;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using Domain.Models.ReportPrograms.AccountSummary12MonthTrend;

namespace iBank.Services.Implementation.ReportPrograms.AccountSummaryAir12MonthTrend
{
    public class AcctSum12MonthCalculations
    {
        public string GetCrystalReportName()
        {
            return "ibAcctSum2";
        }

        public int GetStartMonth(ReportGlobals globals)
        {
            var startMonthString = globals.GetParmValue(WhereCriteria.STARTMONTH);
            return startMonthString.MonthNumberFromName();
        }

        public int GetStartYear(ReportGlobals globals)
        {
            var startYearString = globals.GetParmValue(WhereCriteria.STARTYEAR);
            return startYearString.TryIntParse(0);
        }

        public DateTime GetStartDate(int startMonth, int startYear)
        {
            return new DateTime(startYear, startMonth, 1);
        }

        public DateTime GetEndDate(DateTime startDate)
        {
            return startDate.AddYears(1).AddSeconds(-1);
        }

        public IList<string> GetAccountsSearchedFor(ReportGlobals globals)
        {
            var accts = new List<string>();
            var acct = globals.GetParmValue(WhereCriteria.ACCT);
            if (string.IsNullOrEmpty(acct))
            {
                var inAccts = globals.GetParmValue(WhereCriteria.INACCT);
                if (!string.IsNullOrEmpty(inAccts)) accts = inAccts.Split(',').ToList();
            }
            else
            {
                accts.Add(acct);
            }

            return accts;
        }

        public int[,]FillMonthArray(DateTime startDate)
        {
            var months = new int[12, 2];
            var yearMod = 0;
            for (int i = 0; i < 12; i++)
            {
                var mnth = startDate.Month + i;
                if (mnth > 12)
                {
                    mnth -= 12;
                    yearMod = 1;
                }

                months[i, 0] = mnth;
                months[i, 1] = startDate.Year + yearMod;
            }

            return months;
        }

        public decimal GetYearAmount(FinalData rec)
        {
            return rec.Amt1 + rec.Amt2 + rec.Amt3 + rec.Amt4 + rec.Amt5 + rec.Amt6 + rec.Amt7 +
                              rec.Amt8 + rec.Amt9 + rec.Amt10 + rec.Amt11 + rec.Amt12;
        }

        public int GetYearTrips(FinalData rec)
        {
            return rec.Trips1 + rec.Trips2 + rec.Trips3 + rec.Trips4 + rec.Trips5 + rec.Trips6 + rec.Trips7 +
                              rec.Trips8 + rec.Trips9 + rec.Trips10 + rec.Trips11 + rec.Trips12;
        }

        public bool IsGroupByParentAccount(ReportGlobals globals)
        {
            return globals.ParmValueEquals(WhereCriteria.GROUPBY, "2");
        }

        public IList<string> GetExportFields()
        {
            return new List<string>
                                {
                                    "acct",
                                    "acctdesc",
                                    "trips1",
                                    "trips2",
                                    "trips3",
                                    "trips4",
                                    "trips5",
                                    "trips6",
                                    "trips7",
                                    "trips8",
                                    "trips9",
                                    "trips10",
                                    "trips11",
                                    "trips12",
                                    "amt1",
                                    "amt2",
                                    "amt3",
                                    "amt4",
                                    "amt5",
                                    "amt6",
                                    "amt7",
                                    "amt8",
                                    "amt9",
                                    "amt10",
                                    "amt11",
                                    "amt12",
                                    "yrtrips",
                                    "yramt"
                                };
        }
    }
}
