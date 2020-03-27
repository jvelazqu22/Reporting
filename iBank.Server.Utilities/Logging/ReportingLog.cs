using System;
using System.Collections.Generic;
using System.Globalization;

using com.ciswired.libraries.CISLogger;

using iBank.Server.Utilities.Classes;

using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Repository;

namespace iBank.Server.Utilities.Logging
{
    public class ReportingLog
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ReportLogLogger.ReportMode ReportMode { get; set; }
        
        public ReportingLog()
        {
            ReportMode = ReportLogLogger.ReportMode.REGULAR;

        }

        public ReportingLog(ReportLogLogger.ReportMode mode)
        {
            ReportMode = mode;
        }

        /// <summary>
        /// Adds/updates ibRptLog record and ibRptLogCrit records for the processing report
        /// </summary>
        /// <param name="globals"></param>
        public void StartLogging(ReportGlobals globals, ApplicationInformation appInfo)
        {
            var reportLog = new ReportLogLogger();
            var rptLog = new GetReportLogByLogKeyQuery(new iBankMastersQueryable(), globals.ReportLogKey).ExecuteQuery();
            if (rptLog == null)
            {
                //create the log
                var username = globals.User.FirstName + " " + globals.User.LastName;
                rptLog = reportLog.CreateLog(globals.Agency, globals.User.UserId, username, ReportMode, globals.UserNumber,
                    globals.iBankVersion, globals.ProcessKey, (byte)globals.ServerNumber, ReportLogLogger.ReportStatus.PENDING);

                globals.ReportLogKey = rptLog.rptlogno;
            }

            LOG.Info(string.Format("Setting up criteria for report log key [{0}]", globals.ReportLogKey));
            reportLog.UpdateLog(rptLog, DateTime.Now, ReportMode, (byte)globals.ServerNumber);

            var rptLogCritRecsToAdd = new List<ibRptLogCrit>();
            var critLogger = new ReportLogCritLogger(globals.ReportLogKey);
            
            rptLogCritRecsToAdd.AddRange(critLogger.GetReportCriteriaRecords(globals.ReportParameters, globals.SuppressCriteria, globals.BeginDate, globals.EndDate));

            rptLogCritRecsToAdd.AddRange(critLogger.GetAdvancedCriteriaRecords(globals.AdvancedParameters));
            
            rptLogCritRecsToAdd.AddRange(critLogger.GetSystemVariableRecords(globals.BatchNumber, appInfo.ExecutingAssemblyVersion.ToString()));

            rptLogCritRecsToAdd.Add(critLogger.CreateRecord("PICKRECNUM", globals.PickRecNumber.ToString(CultureInfo.InvariantCulture), "", false));
            
            //add the records to the ibrptlogcrit table
            critLogger.AddRecords(rptLogCritRecsToAdd, new iBankMastersCommandDb());
        }
    }
}
