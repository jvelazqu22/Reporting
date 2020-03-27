using Domain.Models.ReportPrograms.TripChangesSendOffReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Utilities.ClientData;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.TripChanges.TripChangesSendOff
{
    public class UdidInformationRetriever
    {
        private readonly int _udidNumber;
        public string UdidLabel { get; set; }
        public List<UdidData> UdidData { get; set; }

        public UdidInformationRetriever(int udidNumber, string udidLabel)
        {
            _udidNumber = udidNumber;
            UdidLabel = udidLabel;
        }

        public void SetUdidInformation(bool includeCancelledTrips, IClientQueryable clientQueryDb,
            BuildWhere buildWhere, ReportGlobals globals, string originalWhereClause)
        {
            var calc = new SendOffCalculations();
            var creator = new SendOffSqlCreator();

            UdidLabel = calc.GetUdidWhereText(_udidNumber, UdidLabel);

            var udidOneSql = creator.GetUdidSql(originalWhereClause, _udidNumber);
            UdidData = ClientDataRetrieval.GetRawData<UdidData>(udidOneSql, true, buildWhere, globals, false, false).ToList();

            if (includeCancelledTrips)
            {
                udidOneSql.FromClause = creator.CancelledTripFrom;
                var cancelledUdidsNumber1 = ClientDataRetrieval.GetRawData<UdidData>(udidOneSql, true, buildWhere, globals, false, false).ToList();
                UdidData.AddRange(cancelledUdidsNumber1);
            }
        }
    }
}
