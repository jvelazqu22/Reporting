using System;
using System.Collections.Generic;
using System.Linq;
using CODE.Framework.Core.Utilities.Extensions;
using Domain.Helper;
using Domain.Models.ReportPrograms.TripDurationReport;
using Domain.Orm.iBankClientQueries;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared.Client;

namespace iBank.Services.Implementation.ReportPrograms.TripDuration
{
    public class FinalDataHandler
    {
        public List<FinalData> GetFinalList(ReportGlobals globals, List<RawData> rawDataList, IClientDataStore clientStore, ClientFunctions clientFunctions, UserBreaks userBreaks)
        {
            var finalDataList = new List<FinalData>();

            var dataList = GetSimplerRawDataList(globals, rawDataList, clientStore, clientFunctions, userBreaks);

            var routeItineraries = SharedProcedures.GetRouteItinerary(rawDataList, true);

            var tripDuration = globals.GetParmValue(WhereCriteria.RBTRIPDURATION);

            var daysTripDuration = globals.GetParmValue(WhereCriteria.NBRDAYSTRIPDUR).TryIntParse(0);

            if (daysTripDuration > 0)
            {
                switch (tripDuration)
                {
                    case "2":
                        globals.WhereText = $"Trip duration < {daysTripDuration} days; " + globals.WhereText;
                        break;
                    case "3":
                        globals.WhereText = $"Trip duration = {daysTripDuration} days; " + globals.WhereText;
                        break;
                    default:
                        globals.WhereText = $"Trip duration > {daysTripDuration} days; " + globals.WhereText;
                        break;
                }
            }

            finalDataList = dataList.GroupBy(s => new { s.RecKey, s.Acct, s.Acctdesc, s.Break1, s.Break2, s.Break3, s.Recloc, s.Passfrst, s.Passlast, s.Invoice, s.Arrdate, s.Depdate, s.Airchg }, (key, g) => new FinalData
            {
                Reckey = key.RecKey,
                Acct = key.Acct,
                Acctdesc = key.Acctdesc,
                Break1 = key.Break1,
                Break2 = key.Break2,
                Break3 = key.Break3,
                Recloc = key.Recloc,
                Passlast = key.Passlast,
                Passfrst = key.Passfrst,
                Invoice = key.Invoice,
                Itinerary = routeItineraries[key.RecKey],
                Arrdate = key.Arrdate ?? Constants.NullDate,
                Depdate = key.Depdate ?? Constants.NullDate,
                Days = key.Airchg < 0 ? (key.Depdate.HasValue && key.Arrdate.HasValue ? (key.Depdate.Value - key.Arrdate.Value).Days - 1 : 0)
                              : (key.Depdate.HasValue && key.Arrdate.HasValue ? (key.Arrdate.Value - key.Depdate.Value).Days + 1 : 0),
                Airchg = key.Airchg ?? 0,
            }).Where(r => (daysTripDuration < 1)
                || (tripDuration == "2" && Math.Abs(r.Days) < daysTripDuration)
                || (tripDuration == "3" && Math.Abs(r.Days) == daysTripDuration)
                || (daysTripDuration >= 1 && tripDuration != "2" && tripDuration != "3" && (Math.Abs(r.Days) > daysTripDuration))).ToList();

            finalDataList = finalDataList.OrderBy(s => s.Acct)
            .ThenBy(s => s.Break1)
            .ThenBy(s => s.Break2)
            .ThenBy(s => s.Break3)
            .ThenBy(s => s.Passlast)
            .ThenBy(s => s.Passfrst)
            .ToList();

            return finalDataList;
        }

        private List<RawData> GetSimplerRawDataList(ReportGlobals globals, List<RawData> rawDataList, IClientDataStore clientStore, ClientFunctions clientFunctions, UserBreaks userBreaks)
        {
            var accountBreak = globals.User.AccountBreak;
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(clientStore.ClientQueryDb, globals.Agency);

            var dataList = rawDataList.Select(s => new RawData
                {
                    RecKey = s.RecKey,
                    Recloc = s.Recloc,
                    Invoice = s.Invoice.Trim(),
                    Ticket = s.Ticket.Trim(),
                    SeqNo = s.SeqNo,
                    Acct = accountBreak ? s.Acct.Trim() : Constants.NotApplicable,
                    Acctdesc = accountBreak ? clientFunctions.LookupCname(getAllMasterAccountsQuery, s.Acct, globals) : Constants.NotApplicable,
                    Break1 = !userBreaks.UserBreak1 ? Constants.NotApplicable : string.IsNullOrEmpty(s.Break1.Trim()) ? "NONE".PadRight(30) : s.Break1.Trim(),
                    Break2 = !userBreaks.UserBreak2 ? Constants.NotApplicable : string.IsNullOrEmpty(s.Break2.Trim()) ? "NONE".PadRight(30) : s.Break2.Trim(),
                    Break3 = !userBreaks.UserBreak3 ? Constants.NotApplicable : string.IsNullOrEmpty(s.Break3.Trim()) ? "NONE".PadRight(30) : s.Break3.Trim(),
                    Passlast = s.Passlast.Trim(),
                    Passfrst = s.Passfrst.Trim(),
                    Arrdate = s.Arrdate ?? Constants.NullDate,
                    Depdate = s.Depdate ?? Constants.NullDate,
                    Origin = s.Origin.Trim(),
                    Destinat = s.Destinat.Trim(),
                    Mode = s.Mode.Trim(),
                    OrigOrigin = s.OrigOrigin.Trim(),
                    OrigDest = s.OrigDest.Trim(),
                    Connect = s.Connect.Trim(),
                    Airchg = s.Airchg
                }).OrderBy(s => s.Acct)
                .ThenBy(s => s.Break1)
                .ThenBy(s => s.Break2)
                .ThenBy(s => s.Break3)
                .ThenBy(s => s.Passlast)
                .ThenBy(s => s.Passfrst)
                .ThenBy(s => s.RecKey)
                .ThenBy(s => s.SeqNo).ToList();

            return dataList;
        }
    }
}
