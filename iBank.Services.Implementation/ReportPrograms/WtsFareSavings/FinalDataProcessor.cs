using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Helper;
using Domain.Models.ReportPrograms.WtsFareSavings;
using Domain.Orm.iBankClientQueries;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.Client;

namespace iBank.Services.Implementation.ReportPrograms.WtsFareSavings
{
    public class FinalDataProcessor
    {
        private readonly IClientDataStore _clientStore;

        private readonly IMasterDataStore _masterStore;
        private readonly ClientFunctions _clientFunctions;
        private readonly ReportGlobals _globals;

        private readonly List<RawData> _rawDataList;
        private readonly List<UdidData> _udidData;
        private List<FinalData> _finalDataList;
        private readonly string _spaces80;

        public List<ReasonLookup> ReasonLookups { get; set; }

        public FinalDataProcessor(IMasterDataStore masterStore, IClientDataStore clientStore, ClientFunctions clientFunctions, ReportGlobals globals, List<RawData> rawDataList, List<UdidData> udidData)
        {
            _masterStore = masterStore;
            _clientStore = clientStore;
            _clientFunctions = clientFunctions;
            _globals = globals;
            _spaces80 = new string(' ', 80);
            _rawDataList = rawDataList;
            _udidData = udidData;

            //Get all the reason descriptions so we don't have to look them up one by one. The list will be used in both GetFinalData and GetSummary
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(_clientStore.ClientQueryDb, _globals.Agency);
            ReasonLookups = rawDataList.Select(s => new { Acct = s.Acct.Trim(), ReasCode = s.ReasCode.Trim() }).Distinct().Select(s => new ReasonLookup
            {
                Acct = s.Acct,
                ReasonCode = s.ReasCode,
                ReasonDescription = clientFunctions.LookupReason(getAllMasterAccountsQuery, s.ReasCode, s.Acct, _clientStore, _globals, _masterStore.MastersQueryDb)
            }).ToList();
        }

        public List<FinalData> GetFinalData()
        {
            var routeItineraries = SharedProcedures.GetRouteItinerary(_rawDataList, true);

            var finaDatalQuery = _rawDataList.Select(s => new FinalData
            {
                RecKey = s.RecKey,
                Invoice = s.Invoice.Trim(),
                Ticket = s.Ticket.Trim(),
                Acct = _globals.User.AccountBreak ? s.Acct.Trim() : Constants.NotApplicable,
                Acctdesc =
                    _globals.User.AccountBreak
                        ? _clientFunctions.LookupCname(new GetAllMasterAccountsQuery(_clientStore.ClientQueryDb, _globals.Agency),
                            s.Acct, _globals)
                        : Constants.NotApplicable,
                Break1 =
                    _globals.User.AccountBreak
                        ? string.IsNullOrEmpty(s.Break1.Trim()) ? Constants.None : s.Break1.Trim()
                        : Constants.NotApplicable,
                Break2 =
                    _globals.User.AccountBreak
                        ? string.IsNullOrEmpty(s.Break2.Trim()) ? Constants.None : s.Break2.Trim()
                        : Constants.NotApplicable,
                Break3 =
                    _globals.User.AccountBreak
                        ? string.IsNullOrEmpty(s.Break3.Trim()) ? Constants.None : s.Break3.Trim()
                        : Constants.NotApplicable,
                Invdate = s.Invdate ?? DateTime.MinValue,
                Passfrst = s.Passfrst.Trim(),
                Passlast = s.Passlast.Trim(),
                Valcarr =
                    LookupFunctions.LookupAlineCode(_masterStore, s.Airline,
                        s.Mode),
                Itinerary = GetItinerary(s.RecKey, routeItineraries),
                Depdate = s.Depdate ?? DateTime.MinValue,
                Bookdate = s.Bookdate ?? DateTime.MinValue,
                Plusmin = s.Plusmin,
                ReasCode = s.ReasCode.Trim(),
                ReasDesc = LookupReason(s.ReasCode, s.Acct, ReasonLookups),
                Airchg = s.Airchg,
                Exchange = s.Exchange,
                Origticket = s.Origticket,
                Offrdchg = s.Offrdchg > 0 && s.Airchg < 0 ? 0 - s.Offrdchg : s.Offrdchg == 0 ? s.Airchg : s.Offrdchg,
                Stndchg = s.Stndchg == 0 || (s.Stndchg > 0 && s.Airchg < 0) ? s.Airchg : s.Stndchg
            })
            .Distinct(new RawDataDistinctComparer())
            .ToList();

            _finalDataList = _globals.IsParmValueOn(WhereCriteria.CBSORTBYSVGSCODE)
                ? finaDatalQuery
                    .OrderBy(s => s.Acctdesc)
                    .ThenBy(s => s.ReasCode)
                    .ThenBy(s => s.Passlast)
                    .ThenBy(s => s.Passfrst)
                    .ThenBy(s => s.RecKey)
                    .ToList()
                : finaDatalQuery
                    .OrderBy(s => s.Acctdesc)
                    .ThenBy(s => s.ReasCode)
                    .ThenBy(s => s.Break1)
                    .ThenBy(s => s.Break2)
                    .ThenBy(s => s.Break3)
                    .ThenBy(s => s.Passlast)
                    .ThenBy(s => s.Passfrst)
                    .ThenBy(s => s.RecKey)
                    .ToList();

            foreach (var row in _finalDataList)
            {
                row.Savings = row.Stndchg - row.Airchg;
                row.LostAmt = row.Airchg - row.Offrdchg;
            }

            SetReasonDescriptionFromUdid(_finalDataList, _udidData);

            return _finalDataList;
        }

        private string LookupReason(string reasCode, string acct, List<ReasonLookup> reasons)
        {

            var reason = reasons.FirstOrDefault(s => s.Acct == acct && s.ReasonCode == reasCode);
            return reason == null ? _spaces80 : reason.ReasonDescription;
        }

        private static string GetItinerary(int recKey, IReadOnlyDictionary<int, string> routeItineraries)
        {
            string itinerary;
            var found = routeItineraries.TryGetValue(recKey, out itinerary);
            return found ? itinerary : new string(' ', 240);
        }

        private static void SetReasonDescriptionFromUdid(IEnumerable<FinalData> finalDataList, IReadOnlyCollection<UdidData> udidData)
        {
            foreach (var row in finalDataList)
            {
                var udid = udidData.FirstOrDefault(s => s.RecKey == row.RecKey);
                if (!string.IsNullOrEmpty(udid?.UdidText.Trim())) row.ReasDesc = udid.UdidText;
            }
        }

        public List<SummaryData> GetSummary()
        {
            //We have to look up the reasons again here, since some of them will have been overwritten by Udid Text. 
            var temp = _finalDataList
                .Where(s => !string.IsNullOrEmpty(s.ReasCode.Trim()) || (s.Airchg - s.Offrdchg != 0) || (s.Stndchg - s.Airchg != 0))
                .GroupBy(s => new { s.ReasCode, s.Acct }, (key, recs) =>
                {
                    var reclist = recs.ToList();
                    return new SummaryData
                    {
                        ReasCode = key.ReasCode,
                        ReasDesc = LookupReason(key.ReasCode, key.Acct, ReasonLookups),
                        Savings = reclist.Sum(s => s.Stndchg - s.Airchg),
                        LostAmt = reclist.Sum(s => s.Airchg - s.Offrdchg),
                        ReasCount = reclist.Count
                    };
                })
            .GroupBy(s => new { s.ReasCode, s.ReasDesc }, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new SummaryData
                {
                    ReasCode = key.ReasCode,
                    ReasDesc = key.ReasDesc,
                    Savings = reclist.Sum(s => s.Savings),
                    LostAmt = reclist.Sum(s => s.LostAmt),
                    ReasCount = reclist.Sum(s => s.ReasCount)
                };
            })
            .OrderBy(s => s.ReasCode).ToList();

            foreach (var row in temp)
            {
                if (string.IsNullOrEmpty(row.ReasCode) && string.IsNullOrEmpty(row.ReasDesc)) row.ReasDesc = "Loss/Savings - No Code";
                if (string.IsNullOrEmpty(row.ReasDesc)) row.ReasDesc = "Loss/Savings - No Description";
            }
            return temp.ToList();
        }

    }
}
