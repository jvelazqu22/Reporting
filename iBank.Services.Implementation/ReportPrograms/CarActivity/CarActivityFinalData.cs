using Domain.Helper;
using Domain.Models.ReportPrograms.CarActivityReport;
using Domain.Orm.iBankClientQueries;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Shared.Client;

namespace iBank.Services.Implementation.ReportPrograms.CarActivity
{
    public class CarActivityFinalData
    {
        public List<FinalData> GetFinalData(DateTime firstSunday, ReportGlobals Globals, IClientQueryable clientQueryDb, 
            List<RawData> RawDataList, ClientFunctions clientFunctions, UserBreaks userBreaks)
        {
            List<FinalData> finalDataList = new List<FinalData>();

            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(clientQueryDb, Globals.Agency);
            finalDataList = RawDataList.Select(s => new FinalData
            {
                Recloc = s.Recloc,
                Cplusmin = s.Cplusmin,
                Acct = Globals.User.AccountBreak ? s.Acct : "^na^",
                Acctdesc = Globals.User.AccountBreak ? clientFunctions.LookupCname(getAllMasterAccountsQuery, s.Acct, Globals) : "^na^",
                Break1 = userBreaks.UserBreak1 ? !string.IsNullOrEmpty(s.Break1.Trim()) ? s.Break1 : "NONE".PadRight(30) : "^na^",
                Break2 = userBreaks.UserBreak2 ? !string.IsNullOrEmpty(s.Break2.Trim()) ? s.Break2 : "NONE".PadRight(30) : "^na^",
                Break3 = userBreaks.UserBreak3 ? !string.IsNullOrEmpty(s.Break3.Trim()) ? s.Break3 : "NONE".PadRight(16) : "^na^",
                Passlast = s.Passlast,
                Passfrst = s.Passfrst,
                Autocity = s.Autocity,
                Autostat = s.Autostat,
                Rentdate = s.Rentdate ?? DateTime.MinValue,
                Company = s.Company,
                Cartype = s.Cartype,
                Days = s.Days,
                Abookrat = s.Abookrat,
                Milecost = s.Milecost,
                Ratetype = s.Ratetype,
                Weeknum = (s.Rentdate.GetValueOrDefault() - firstSunday).Days / 7 + 1
            }).ToList();

            switch (Globals.GetParmValue(WhereCriteria.PROCESSID))
            {
                case "70":
                    return ApplyOrderForAnalysisByCityReport(finalDataList);
                case "72":
                    return ApplyOrderForAdvanceReservationsReport(finalDataList);
                case "74":
                    return ApplyOrderForAnalysisByVendorReport(finalDataList);
                case "76":
                    return ApplyOrderForCarRentalActivityReport(finalDataList);
            }

            return finalDataList;
        }

        public List<FinalData> ApplyOrderForAnalysisByCityReport(List<FinalData> finalDataList)
        {
            return finalDataList.OrderBy(s => s.Autocity)
                .ThenBy(s => s.Autostat)
                .ThenBy(s => s.Passlast)
                .ThenBy(s => s.Passfrst)
                .ThenBy(s => s.Rentdate)
                .ToList();
        }

        public List<FinalData> ApplyOrderForAdvanceReservationsReport(List<FinalData> finalDataList)
        {
            return finalDataList.OrderBy(s => s.Acctdesc)
                .ThenBy(s => s.Break1)
                .ThenBy(s => s.Break2)
                .ThenBy(s => s.Break3)
                .ThenBy(s => s.Autocity)
                .ThenBy(s => s.Autostat)
                .ThenBy(s => s.Passlast)
                .ThenBy(s => s.Passfrst)
                .ThenBy(s => s.Rentdate)
                .ToList();
        }

        public List<FinalData> ApplyOrderForAnalysisByVendorReport(List<FinalData> finalDataList)
        {
            return finalDataList.OrderBy(s => s.Company)
                .ThenBy(s => s.Autocity)
                .ThenBy(s => s.Autostat)
                .ThenBy(s => s.Passlast)
                .ThenBy(s => s.Passfrst)
                .ThenBy(s => s.Rentdate)
                .ToList();
        }

        public List<FinalData> ApplyOrderForCarRentalActivityReport(List<FinalData> finalDataList)
        {
            return finalDataList.OrderBy(s => s.Acctdesc)
                .ThenBy(s => s.Break1)
                .ThenBy(s => s.Break2)
                .ThenBy(s => s.Break3)
                .ThenBy(s => s.Passlast)
                .ThenBy(s => s.Passfrst)
                .ThenBy(s => s.Rentdate)
                .ThenBy(s => s.Autocity)
                .ThenBy(s => s.Autostat)
                .ToList();
        }
    }
}
