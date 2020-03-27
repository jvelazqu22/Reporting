using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using com.ciswired.libraries.CISLogger;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.UserDefined.DataSqlScripts;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Utilities.ClientData;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.DataSetHandler
{
    public class UdidDataSetHandler
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public List<UdidRawData> GetUdidData(string whereClauseFull, bool isReservationReport, ReportGlobals globals, BuildWhere buildWhere, SwitchManager switchManager)
        {
            LOG.Debug("Retrieving Udid data.");

            //need to remove none udid related date. Don't need datein
            var pos = whereClauseFull.IndexOf("datein", StringComparison.Ordinal);
            var udidWhere = pos > -1
                                ? whereClauseFull.Substring(0, pos - 4)
                                : whereClauseFull;

            var sqlToExecute = "";
            sqlToExecute = new UdidSqlBuilder().GetSql(isReservationReport, switchManager, udidWhere, buildWhere);
            
            return ClientDataRetrieval.GetUdidFilteredOpenQueryData<UdidRawData>(sqlToExecute, globals, buildWhere.Parameters, isReservationReport).ToList();
        }
    }
}
