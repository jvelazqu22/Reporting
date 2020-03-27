using Domain.Helper;
using Domain.Models.ReportPrograms.TopBottomExceptionReasonReport;
using Domain.Orm.iBankClientQueries;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Shared.Client;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomExceptionReason
{
    public class TopBottomExceptionReasonFinalData
    {
        List<FinalData> FinalDataList = new List<FinalData>();
        public List<FinalData> GetFinalData(List<RawData> rawDataList, ReportGlobals globals, ClientFunctions clientFunctions, IClientDataStore clientStore, IMasterDataStore masterStore)
        {

            if (globals.IsParmValueOn(WhereCriteria.CBUSEBASEFARE))
                foreach (var row in rawDataList.Where(s => s.Basefare != 0))
                    row.Airchg = row.Basefare;

            if (globals.ProcessKey == 60)
                return GetFinalDataForExceptionReasonsReport(rawDataList, globals, clientFunctions, clientStore, masterStore);
            else
                return GetFinalDataForExceptionTravelersReport(rawDataList);
        }

        private List<FinalData> GetFinalDataForExceptionReasonsReport(List<RawData> rawDataList, ReportGlobals globals, ClientFunctions clientFunctions, IClientDataStore clientStore,
            IMasterDataStore masterStore)
        {
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(clientStore.ClientQueryDb, globals.Agency);

            var reasonCodeAndAcctFromRawList = rawDataList.Select(s => new Tuple<string, string>(s.Reascode, s.Acct)).Distinct();
            var reasLookups = reasonCodeAndAcctFromRawList
                .Select(s => new Tuple<string, string, string>(s.Item1, s.Item2, clientFunctions.LookupReason(getAllMasterAccountsQuery, s.Item1, s.Item2,
                                clientStore, globals, masterStore.MastersQueryDb))).ToList();

            FinalDataList = rawDataList
                .Where(s => s.Airchg != s.LowChg)
                .GroupBy(s => TopExceptionHelpers.SpeedLookup(s.Reascode, s.Acct, reasLookups),
                (key, recs) =>
                {
                    var reclist = recs.ToList();
                    return new FinalData
                    {
                        Category = key,
                        NumOccurs = reclist.Sum(s => s.Plusmin),
                        LostAmt = reclist.Sum(s => Math.Abs(s.Airchg) > Math.Abs(s.LowChg) ? s.Airchg - s.LowChg : 0)
                    };

                }).ToList();

            return FinalDataList;
        }

        public List<FinalData> GetFinalDataForExceptionTravelersReport(List<RawData> RawDataList)
        {
            FinalDataList = RawDataList
                .Where(s => s.Airchg != s.LowChg)
                .GroupBy(s => (s.Passlast.Trim() + ", " + s.Passfrst).PadRight(36),
               (key, recs) =>
               {
                   var reclist = recs.ToList();
                   return new FinalData
                   {
                       Category = key,
                       NumOccurs = reclist.Sum(s => s.Plusmin),
                       LostAmt = reclist.Sum(s => Math.Abs(s.Airchg) > Math.Abs(s.LowChg) ? s.Airchg - s.LowChg : 0)
                   };

               }).ToList();

            return FinalDataList;
        }
    }
}
