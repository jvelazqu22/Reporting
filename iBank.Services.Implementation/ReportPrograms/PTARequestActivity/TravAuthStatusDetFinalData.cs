using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Helper;
using Domain.Models.ReportPrograms.PTARequestActivityReport;
using Domain.Orm.iBankClientQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities;
using iBank.Services.Implementation.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.Client;
using iBank.Repository.SQL.Repository;

namespace iBank.Services.Implementation.ReportPrograms.PTARequestActivity
{
    public class TravAuthStatusDetFinalData
    {
        public List<FinalData> GetFinalData(IMasterDataStore masterStore, ReportGlobals globals, IClientDataStore clientStore, 
            List<RawData> rawDataList, ClientFunctions clientFunctions, UserBreaks userBreaks)
        {
            var authStatuses = new GetMiscParamListQuery(masterStore.MastersQueryDb, "TRAVAUTHSTAT", globals.UserLanguage).ExecuteQuery().ToList();
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(clientStore.ClientQueryDb, globals.Agency);
            
            var parm = new GetMasterUrlPathQuery(new iBankMastersQueryable());
            var urlPath = parm.ExecuteQuery();

            var accts = rawDataList.Select(s => s.Acct).Distinct().ToList();
            var acctLookups = accts.Select(s => new Tuple<string, string>(s, clientFunctions.LookupCname(getAllMasterAccountsQuery, s, globals))).ToList();
            var finalList = new List<FinalData>();
            foreach (var s in rawDataList)
            {
                var finalData = new FinalData();
                finalData.Reckey = s.RecKey;
                finalData.Acct = s.Acct;
                finalData.Acctdesc = SpeedLookup.Lookup(s.Acct, acctLookups);
                finalData.Break1 = !userBreaks.UserBreak1 ? Constants.NotApplicable : (string.IsNullOrEmpty(s.Break1.Trim()) ? Constants.None.PadRight(30) : s.Break1.Trim());
                finalData.Break2 = !userBreaks.UserBreak2 ? Constants.NotApplicable : (string.IsNullOrEmpty(s.Break2.Trim()) ? Constants.None.PadRight(30) : s.Break2.Trim());
                finalData.Break3 = !userBreaks.UserBreak3 ? Constants.NotApplicable : (string.IsNullOrEmpty(s.Break3.Trim()) ? Constants.None.PadRight(16) : s.Break3.Trim());
                finalData.DepartureDate = s.Depdate.GetValueOrDefault();
                finalData.Deptime = s.SeqNo == 1 ? s.DepTime : "";
                finalData.Depdate = s.Depdate.GetValueOrDefault().ToShortDateString(); //for some reason, depdate is a string in this report
                finalData.Bookdate = s.BookDate ?? DateTime.MinValue;
                finalData.Statustime = s.Statustime ?? DateTime.MinValue;
                finalData.Recloc = s.Recloc;
                finalData.Daysadvanc = (s.Depdate.GetValueOrDefault().Date - s.BookDate.GetValueOrDefault().Date).Days;
                finalData.Passlast = s.Passlast;
                finalData.Passfrst = s.Passfrst;
                finalData.Rtvlcode = s.Rtvlcode;
                finalData.Tvlreasdes = clientFunctions.LookupReason(getAllMasterAccountsQuery, s.Rtvlcode, s.Acct, clientStore, globals, masterStore.MastersQueryDb);
                finalData.Outpolcods = s.OutPolCods;
                finalData.Oopreasdes = clientFunctions.LookupReason(getAllMasterAccountsQuery, s.OutPolCods, s.Acct, clientStore, globals, masterStore.MastersQueryDb).PadRight(200);
                finalData.Authrzrnbr = s.AuthrzrNbr;
                finalData.Authorizer = PtaLookups.LookupAuthName(clientStore.ClientQueryDb, s.AuthrzrNbr, s.ApSequence, s.AuthStatus, s.Auth1Email);
                finalData.Authstatus = s.AuthStatus;
                finalData.Statusdesc = PtaLookups.LookupAuthStatus(authStatuses, s.AuthStatus);
                finalData.Detlstatus = s.DetlStatus;
                finalData.Auth1email = s.Auth1Email;
                finalData.Detstatdes = PtaLookups.LookupAuthStatus(authStatuses, s.DetlStatus);
                finalData.Detstattim = s.DetStatTim ?? DateTime.MinValue;
                finalData.Apvreason = s.ApvReason;
                finalData.Apsequence = s.ApSequence;
                finalData.Travauthno = s.TravAuthNo;
                finalData.Sgroupnbr = s.Sgroupnbr;
                finalData.Airchg = s.AirChg;
                finalData.Offrdchg = s.OffrdChg;
                finalData.Lostsvngs = s.AirChg > 0 && s.OffrdChg > 0 ? s.AirChg - s.OffrdChg : 0;
                finalData.Authpglink = PtaLookups.LookupAuthPageLink(urlPath.UrlPath, globals.AgencyInformation.ClientURL, s.Sgroupnbr, s.TravAuthNo, s.AuthStatus);
                finalData.SortCol = clientFunctions.LookupReason(getAllMasterAccountsQuery, s.OutPolCods, s.Acct, clientStore, globals, masterStore.MastersQueryDb).PadRight(30);
                finalList.Add(finalData);
            }

            return finalList.RemoveDuplicates().ToList();
        }
    }
}
