using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Helper;
using Domain.Models.ReportPrograms.PTATravelersDetailReport;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.PTARequestActivity;
using iBank.Services.Implementation.Shared.Client;
using iBank.Services.Implementation.Utilities;
using iBankDomain.RepositoryInterfaces;

namespace iBank.Services.Implementation.ReportPrograms.PTATravelersDetail
{
    public class RowBuilderAuthorizerData
    {
        public void SetAuthorizerDta(List<TripAuthorizerRawData> tripAuthorizerRawDataList, int recKey, int travAuthNo, bool includeNotifyOnly,
            FinalData firstRow, GroupedTripAuthData row, ref int rowCounter, ReportGlobals globals, List<Tuple<string, string>> acctLookups,
            UserBreaks userBreaks, IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, double offsetHours, IClientDataStore clientStore,
            IMasterDataStore masterStore, List<KeyValue> authStatuses, List<string> notRequired, bool includeAuthComm, ClientFunctions clientFunctions)
        {
            var authCounter = 0;
            foreach (var tripAuthRow in tripAuthorizerRawDataList.Where(s => s.RecKey == recKey && s.TravAuthNo == travAuthNo))
            {
                var notifyOnly = tripAuthRow.AuthStatus.Trim().Equals("N");
                if (notifyOnly && !includeNotifyOnly) continue;

                authCounter++;
                if (authCounter > 5) break;

                if (authCounter == 1)
                {
                    UpdateRowDataPart1(tripAuthRow, firstRow, row, ref rowCounter, globals, acctLookups, notifyOnly, userBreaks, getAllMasterAccountsQuery, 
                        offsetHours, clientStore, masterStore, authStatuses, notRequired, includeAuthComm, clientFunctions);
                }
                else
                {
                    UpdateRowDataPart2(tripAuthRow, firstRow, clientStore, authStatuses, notRequired, authCounter);
                }
            }
        }

        private void UpdateRowDataPart1(TripAuthorizerRawData tripAuthRow, FinalData firstRow, GroupedTripAuthData row, ref int rowCounter, ReportGlobals globals, 
            List<Tuple<string, string>> acctLookups, bool notifyOnly, UserBreaks userBreaks, IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, 
            double offsetHours, IClientDataStore clientStore, IMasterDataStore masterStore, List<KeyValue> authStatuses, List<string> notRequired, 
            bool includeAuthComm, ClientFunctions clientFunctions)
        {
            firstRow.Reckey = row.RecKey;
            firstRow.Rownum = rowCounter++;
            firstRow.Acct = globals.User.AccountBreak ? tripAuthRow.Acct : Constants.NotApplicable;
            firstRow.Acctname = globals.User.AccountBreak
                ? SpeedLookup.Lookup(tripAuthRow.Acct, acctLookups)
                : Constants.NotApplicable;
            firstRow.Break1 = userBreaks.UserBreak1 ? tripAuthRow.Break1 : Constants.NotApplicable;
            firstRow.Break2 = userBreaks.UserBreak2 ? tripAuthRow.Break2 : Constants.NotApplicable;
            firstRow.Break3 = userBreaks.UserBreak3 ? tripAuthRow.Break3 : Constants.NotApplicable;
            firstRow.Passlast = tripAuthRow.Passlast;
            firstRow.Passfrst = tripAuthRow.Passfrst;
            firstRow.Recloc = tripAuthRow.Recloc;
            firstRow.Travreason = clientFunctions.LookupReason(getAllMasterAccountsQuery, tripAuthRow.Rtvlcode, tripAuthRow.Acct, clientStore, globals, masterStore.MastersQueryDb, true);
            firstRow.Cliauthnbr = tripAuthRow.CliAuthNbr.Trim();
            firstRow.Statusdate = tripAuthRow.StatusTime.GetValueOrDefault().AddHours(offsetHours).Date;
            firstRow.Statustime = tripAuthRow.StatusTime.GetValueOrDefault().AddHours(offsetHours).ToLongTimeString();
            firstRow.Statusdesc = notifyOnly
                ? "Notified"
                : PtaLookups.LookupAuthStatus(authStatuses, tripAuthRow.AuthStatus);
            firstRow.Travauthno = tripAuthRow.TravAuthNo;

            UpdateOopreasons(tripAuthRow, firstRow, globals, getAllMasterAccountsQuery, clientStore, masterStore, clientFunctions);

            firstRow.Authemail1 = tripAuthRow.Auth1Email;
            firstRow.Authorizr1 = PtaLookups.LookupAuthName(clientStore.ClientQueryDb, tripAuthRow.AuthrzrNbr, tripAuthRow.ApSequence, tripAuthRow.AuthStatus, tripAuthRow.Auth1Email);

            UpdateAuthStat1(tripAuthRow, firstRow, notifyOnly, authStatuses, notRequired);

            firstRow.Apvreason1 = tripAuthRow.ApvReason;
            firstRow.Authcomm = includeAuthComm
                ? tripAuthRow.Authcomm.Trim()
                : string.Empty;
            firstRow.Bookedgmt = tripAuthRow.Bookedgmt ?? DateTime.MinValue;
        }

        private void UpdateAuthStat1(TripAuthorizerRawData tripAuthRow, FinalData firstRow, bool notifyOnly, List<KeyValue> authStatuses, List<string> notRequired)
        {
            if (notifyOnly)
            {
                firstRow.Authstat1 = "Notified";
            }
            else
            {
                firstRow.Authstat1 = notRequired.Contains(tripAuthRow.AuthStatus) && !notRequired.Contains(tripAuthRow.DetlStatus)
                    ? "Not Req'd"
                    : PtaLookups.LookupAuthStatus(authStatuses, tripAuthRow.DetlStatus);
            }
        }

        private void UpdateOopreasons(TripAuthorizerRawData tripAuthRow, FinalData firstRow, ReportGlobals globals, IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, 
            IClientDataStore clientStore, IMasterDataStore masterStore, ClientFunctions clientFunctions)
        {
            if (!string.IsNullOrEmpty(tripAuthRow.OutPolCods))
            {
                var oopCodes = tripAuthRow.OutPolCods.Split(',');
                var limit = oopCodes.Length > 5 ? 5 : oopCodes.Length;//limit to five max

                for (int i = 0; i < limit; i++)
                {
                    var reason = clientFunctions.LookupReason(getAllMasterAccountsQuery, oopCodes[i].Trim(), tripAuthRow.Acct, clientStore, globals, masterStore.MastersQueryDb, true);
                    switch (i)
                    {
                        case 0:
                            firstRow.Oopreason1 = reason;
                            break;
                        case 1:
                            firstRow.Oopreason2 = reason;
                            break;
                        case 2:
                            firstRow.Oopreason3 = reason;
                            break;
                        case 3:
                            firstRow.Oopreason4 = reason;
                            break;
                        case 4:
                            firstRow.Oopreason5 = reason;
                            break;
                    }
                }
            }
        }

        private void UpdateRowDataPart2(TripAuthorizerRawData tripAuthRow, FinalData firstRow, IClientDataStore clientStore, List<KeyValue> authStatuses, 
            List<string> notRequired, int authCounter)
        {
            switch (authCounter)
            {
                case 2:
                    firstRow.Authemail2 = tripAuthRow.Auth1Email;
                    firstRow.Authorizr2 = PtaLookups.LookupAuthName(clientStore.ClientQueryDb, tripAuthRow.AuthrzrNbr, tripAuthRow.ApSequence, tripAuthRow.AuthStatus, tripAuthRow.Auth1Email);
                    firstRow.Authstat2 = notRequired.Contains(tripAuthRow.AuthStatus) && !notRequired.Contains(tripAuthRow.DetlStatus)
                        ? "Not Req'd"
                        : PtaLookups.LookupAuthStatus(authStatuses, tripAuthRow.DetlStatus);
                    firstRow.Apvreason2 = tripAuthRow.ApvReason;
                    break;
                case 3:
                    firstRow.Authemail3 = tripAuthRow.Auth1Email;
                    firstRow.Authorizr3 = PtaLookups.LookupAuthName(clientStore.ClientQueryDb, tripAuthRow.AuthrzrNbr, tripAuthRow.ApSequence, tripAuthRow.AuthStatus, tripAuthRow.Auth1Email);
                    firstRow.Authstat3 = notRequired.Contains(tripAuthRow.AuthStatus) && !notRequired.Contains(tripAuthRow.DetlStatus)
                        ? "Not Req'd"
                        : PtaLookups.LookupAuthStatus(authStatuses, tripAuthRow.DetlStatus);
                    firstRow.Apvreason3 = tripAuthRow.ApvReason;
                    break;
                case 4:
                    firstRow.Authemail4 = tripAuthRow.Auth1Email;
                    firstRow.Authorizr4 = PtaLookups.LookupAuthName(clientStore.ClientQueryDb, tripAuthRow.AuthrzrNbr, tripAuthRow.ApSequence, tripAuthRow.AuthStatus, tripAuthRow.Auth1Email);
                    firstRow.Authstat4 = notRequired.Contains(tripAuthRow.AuthStatus) && !notRequired.Contains(tripAuthRow.DetlStatus)
                        ? "Not Req'd"
                        : PtaLookups.LookupAuthStatus(authStatuses, tripAuthRow.DetlStatus);
                    firstRow.Apvreason4 = tripAuthRow.ApvReason;
                    break;
                case 5:
                    firstRow.Authemail5 = tripAuthRow.Auth1Email;
                    firstRow.Authorizr5 = PtaLookups.LookupAuthName(clientStore.ClientQueryDb, tripAuthRow.AuthrzrNbr, tripAuthRow.ApSequence, tripAuthRow.AuthStatus, tripAuthRow.Auth1Email);
                    firstRow.Authstat5 = notRequired.Contains(tripAuthRow.AuthStatus) && !notRequired.Contains(tripAuthRow.DetlStatus)
                        ? "Not Req'd"
                        : PtaLookups.LookupAuthStatus(authStatuses, tripAuthRow.DetlStatus);
                    firstRow.Apvreason5 = tripAuthRow.ApvReason;
                    break;
            }
        }
    }
}
