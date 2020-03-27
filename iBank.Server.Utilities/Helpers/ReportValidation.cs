using System;
using System.Collections.Generic;
using System.Linq;
using com.ciswired.libraries.CISLogger;
using Domain;
using Domain.Exceptions;
using Domain.Helper;
using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Agency;
using Domain.Orm.iBankMastersQueries.Configuration;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

namespace iBank.Server.Utilities.Helpers
{
    public class ReportValidation
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        private IMasterDataStore MasterStore { get; }

        public ReportValidation(IMasterDataStore ds)
        {
            MasterStore = ds;
        }

        public bool IsReportConvertedAndAgencyEnabled(string reportId, int userNumber)
        {
            var crit = GetReportCriteria(reportId);
            var processId = GetProcessId(crit);
            var agency = GetAgency(crit);

            return IsReportConvertedAndAgencyEnabled(userNumber, agency, processId);
        }

        public bool IsReportConvertedAndAgencyEnabled(int userNumber, string agency, int processId)
        {
            var agencySettings = GetAgencySettings(agency);

            if (agencySettings.IsSharingAgency)
            {
                LOG.Info($"Shared Corp Account: {agency}");
                var childAgencies = GetChildAgencies(agencySettings.AgencyName);
                foreach (var child in childAgencies)
                {
                    var childSettings = GetAgencySettings(child);
                    if (!IsReportConvertedAndAgencyEnabled(processId, childSettings, userNumber))
                    {
                        LOG.Info($"Agency {child} is not enabled for .NET or Report {processId} is not converted .NET Report and/or user " +
                                 $" number: {userNumber} is not allowed as a .net user tester");
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return IsReportConvertedAndAgencyEnabled(processId, agencySettings, userNumber);
            }
        }

        private bool IsReportConvertedAndAgencyEnabled(int processKey, AgencyDotNetSettings agencySettings, int userNumber)
        {
            if (!agencySettings.IsDotNetEnabled) return false;

            var report = GetReportDetails(processKey, MasterStore.MastersQueryDb);

            if (!report.dotnet) return false;

            if (!report.staged) return true;

            if (IsUserAllowedToRunReport(processKey, agencySettings.AgencyName, userNumber)) return true;

            return IsAgencyStagedForReport(processKey, agencySettings.AgencyName);
        }

        private IList<reporthandoff> GetReportCriteria(string reportId)
        {
            var reportCritQuery = new GetReportInputCriteriaByReportIdQuery(MasterStore.MastersQueryDb, reportId);
            return reportCritQuery.ExecuteQuery();
        }

        private int GetProcessId(IEnumerable<reporthandoff> crit)
        {
            var processIdRow = crit.FirstOrDefault(x => x.parmname.Trim().ToUpper() == "PROCESSID");

            if (processIdRow == null) throw new Exception("Process id row not found in report criteria.");

            var rowValue = processIdRow.parmvalue.Trim();
            var processId = 0;
            if (!int.TryParse(rowValue, out processId))
            {
                throw new Exception($"Process id of [{rowValue}] not valid integer.");
            }

            return processId;
        }

        private string GetAgency(IEnumerable<reporthandoff> crit)
        {
            var agencyRow = crit.FirstOrDefault(x => x.parmname.Trim().ToUpper() == "AGENCY");

            if (agencyRow == null) throw new Exception("Agency row not found in report criteria.");

            var agency = agencyRow.parmvalue.ToUpper().Trim();

            if (string.IsNullOrEmpty(agency)) throw new Exception("Agency field empty in report criteria.");

            return agency;
        }

        private AgencyDotNetSettings GetAgencySettings(string agency)
        {
            var query = new GetAgencyDotNetSettingsQuery(MasterStore.MastersQueryDb, agency);
            return query.ExecuteQuery();
        }

        private IEnumerable<string> GetChildAgencies(string agency)
        {
            var agenciesQuery = new GetAgenciesByJunctionAgcyCorpQuery(MasterStore.MastersQueryDb, agency);
            return agenciesQuery.ExecuteQuery();
        }
        
        private bool IsUserAllowedToRunReport(int processKey, string agencyName, int userNumber)
        {            
            var query = new IsUserAllowToTestDotNetReportQuery(MasterStore.MastersQueryDb, processKey, agencyName, userNumber);
            return query.ExecuteQuery();            
        }
        
        private ibproces GetReportDetails(int processId, IMastersQueryable masterQueryDb)
        {
            var query = new GetReportByProcessIdQuery(masterQueryDb, processId);
            var rec = query.ExecuteQuery();

            if (rec == null) throw new ReportNotFoundException($"Process key [{processId}] not found!");

            return rec;
        }

        private bool IsAgencyStagedForReport(int processKey, string agency)
        {
            var query = new IsAgencyStagedForReportQuery(MasterStore.MastersQueryDb, processKey, agency);
            return query.ExecuteQuery();
        }
    }
}
