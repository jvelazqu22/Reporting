using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Helper;
using Domain.Models.ReportPrograms.UpcomingPlans;
using Domain.Orm.Classes;

using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using iBank.Services.Implementation.Shared.Client;
using iBankDomain.RepositoryInterfaces;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;

namespace iBank.Services.Implementation.ReportPrograms.UpcomingPlans
{
    public class UpcomingPlansData
    {
        public List<string> GetExportFields(bool acctBrk, UserBreaks userBreaks, UserInformation user)
        {
            var fieldList = new List<string>();

            if (acctBrk)
            {
                fieldList.Add("acct");
                fieldList.Add("acctdesc");
            }

            if (userBreaks.UserBreak1)
            {
                var break1 = string.IsNullOrEmpty(user.Break1Name) ? "break_1" : user.Break1Name;
                fieldList.Add($"break1 as {break1}");
            }
            if (userBreaks.UserBreak2)
            {
                var break2 = string.IsNullOrEmpty(user.Break2Name) ? "break_2" : user.Break2Name;
                fieldList.Add($"break2 as {break2}");
            }
            if (userBreaks.UserBreak3)
            {
                var break3 = string.IsNullOrEmpty(user.Break3Name) ? "break_3" : user.Break3Name;
                fieldList.Add($"break3 as {break3}");
            }

            fieldList.Add("destinat");
            fieldList.Add("destdesc");
            fieldList.Add("weeknum");
            fieldList.Add("rdepdate");
            fieldList.Add("passlast");
            fieldList.Add("passfrst");
            fieldList.Add("origin");
            fieldList.Add("orgdesc");
            fieldList.Add("airline");
            fieldList.Add("alinedesc");
            fieldList.Add("fltno");
            fieldList.Add("class");
            fieldList.Add("deptime");
            fieldList.Add("seqno");

            return fieldList;
        }

        public IList<FinalData> MapRawToFinal(IList<RawData> rawData, bool accountBreak, ClientFunctions clientFunctions, IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery,
            IMasterDataStore store, ReportGlobals globals, UserBreaks userBreaks, DateTime sundayDate)
        {
            var calc = new UpcomingCalculations();
            return rawData.Select(s => new FinalData
            {
                Recloc = s.Recloc,
                Seqno = s.SeqNo,
                Acct = !accountBreak ? Constants.NotApplicable : s.Acct,
                Acctdesc = !accountBreak ? Constants.NotApplicable : clientFunctions.LookupCname(getAllMasterAccountsQuery, s.Acct, globals),
                Break1 = !userBreaks.UserBreak1 ? Constants.NotApplicable : string.IsNullOrEmpty(s.Break1.Trim()) ? "NONE".PadRight(30) : s.Break1,
                Break2 = !userBreaks.UserBreak2 ? Constants.NotApplicable : string.IsNullOrEmpty(s.Break2.Trim()) ? "NONE".PadRight(30) : s.Break2,
                Break3 = !userBreaks.UserBreak3 ? Constants.NotApplicable : string.IsNullOrEmpty(s.Break3.Trim()) ? "NONE".PadRight(16) : s.Break3,
                Weeknum = calc.GetWeekNum(s.RDepDate, sundayDate),
                Origin = s.Origin,
                Orgdesc = AportLookup.LookupAport(store, s.Origin, s.Mode, globals.Agency),
                Destinat = s.Destinat,
                Destdesc = AportLookup.LookupAport(store, s.Destinat, s.Mode, globals.Agency),
                Airline = s.Airline,
                Alinedesc = LookupFunctions.LookupAirline(store, s.Airline.Trim()).Item2,
                Actfare = s.ActFare,
                Rdepdate = s.RDepDate ?? DateTime.MinValue,
                Passfrst = s.Passfrst,
                Passlast = s.Passlast,
                ClassCode = s.ClassCode,
                Deptime = s.DepTime,
                Fltno = s.fltno
            }).OrderBy(x => x.Acctdesc)
              .ThenBy(x => x.Break1)
              .ThenBy(x => x.Break2)
              .ThenBy(x => x.Break3)
              .ThenBy(x => x.Destdesc)
              .ThenBy(x => x.Destinat)
              .ThenBy(x => x.Rdepdate)
              .ThenBy(x => x.Passlast)
              .ThenBy(x => x.Passfrst)
              .ToList();
        }
    }
}
