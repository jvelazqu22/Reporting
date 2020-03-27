using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.EventLog;
using Domain.Orm.iBankClientQueries;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.Client;

namespace iBank.Services.Implementation.ReportPrograms.EventLog
{
    public class EventLogProcessor
    {
        private readonly EventLogHelper _eventLogHelper = new EventLogHelper();
        public List<FinalData> GetFinalDataUsingSingleAgency(List<RawData> rawDataList, IClientDataStore clientStore, ClientFunctions clientFunctions, ReportGlobals globals)
        {
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(clientStore.ClientQueryDb, globals.Agency);
            return rawDataList.Select(s => new FinalData
            {
                Category = s.EventType,
                Eventdesc = s.EventDesc,
                Userid = s.UserID,
                Username = s.UserName,
                Ipaddress = s.IPAddress,
                Eventdate = s.DateStamp,
                Targettype = _eventLogHelper.GetTargetType(s.EventTarget),
                Evnttarget = s.TargetUserID,
                Targetname = _eventLogHelper.LookupEventTarget(s.EventTarget, s.TargetUserID, getAllMasterAccountsQuery, clientStore, clientFunctions, globals)
            }).ToList();
        }

        public List<FinalData> GetFinalDataUsingUsingMultipleAgencies(List<RawData> rawDataList, IClientDataStore clientStore, ClientFunctions clientFunctions, ReportGlobals globals)
        {
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(clientStore.ClientQueryDb, globals.Agency);
            return rawDataList.Select(s => new FinalData
            {
                Category = s.EventType,
                Eventdesc = s.EventDesc,
                Userid = s.UserID,
                Username = s.UserName,
                Ipaddress = s.IPAddress,
                Eventdate = s.DateStamp,
                Targettype = _eventLogHelper.GetTargetType(s.EventTarget),
                Evnttarget = s.TargetUserID,
                Targetname = _eventLogHelper.LookupEventTargetInMultipleAgencies(s.EventTarget, s.TargetUserID, getAllMasterAccountsQuery, clientFunctions, globals)
            }).ToList();
        }
    }
}
