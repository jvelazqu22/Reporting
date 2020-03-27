using Domain.Helper;
using Domain.Interfaces.BroadcastServer;

using System;
using System.Collections.Generic;

using iBank.Services.Implementation.Utilities;
using System.Linq;
using Domain.Orm.iBankClientQueries.BroadcastQueries;
using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Helpers;
using Domain;

namespace iBank.BroadcastServer.BroadcastReport
{
    public class BatchReportRetriever : IBatchReportRetriever
    {
        public int? BatchNumber { get; set; }

        private IClientDataStore ClientDataStore { get; set; }
        private IMasterDataStore MasterDataStore { get; set; }

        public BatchReportRetriever(int? batchNumber, IClientDataStore clientDataStore, IMasterDataStore masterDataStore)
        {
            BatchNumber = batchNumber;
            ClientDataStore = clientDataStore;
            MasterDataStore = masterDataStore;
        }

        private void UpdateUserDefinedReportName(IList<BroadcastReportInformation> savedReports, IList<ProcessCaptionInformation> processCaptions)
        {
            foreach (var item in savedReports.Where(x => x.UdrKey > 0 && x.UserReportName.Length >= 5 && x.UserReportName.Substring(0,5).Equals("sysDR")))
            {
                var caption = processCaptions.FirstOrDefault(x => x.ProcessKey == item.ProcessKey);
                    
                item.UserReportName = caption == null ? item.UserReportName : caption.Caption;
            }
        }
        

        public IList<BroadcastReportInformation> GetAllReportsInBatch(IList<ProcessCaptionInformation> processCaptions, string agency, bool isDemo, int userNumber)
        {
            var standardReports = GetStandardReports(processCaptions);
            var userDefinedReports = GetUserDefinedReports();
            var xmlStandardReports = GetXmlStandardReports();
            var xmlUserDefinedReports = GetXmlUserDefinedReports();
            var savedReports = GetSavedReports();

            UpdateUserDefinedReportName(savedReports, processCaptions);

            var allReports = new List<BroadcastReportInformation>();
            allReports.AddRange(standardReports);
            allReports.AddRange(userDefinedReports);
            allReports.AddRange(xmlStandardReports);
            allReports.AddRange(xmlUserDefinedReports);
            allReports.AddRange(savedReports);

            if (Features.TravelOptixImplimentation.IsEnabled())
            {
                var travelOptixReports = GetTravelOptixReports();
                allReports.AddRange(travelOptixReports);
            }

            var checker = new ReportValidation(MasterDataStore);
            foreach (var report in allReports)
            {
                if (report.CrystalReportType.EqualsIgnoreCase("TTR")) //all ticket tracker custom reports are handled by FoxPro
                {
                    report.IsDotNetEnabled = false;
                    report.ProcessKey = 505;
                }
                else if (!report.CrystalReportType.EqualsIgnoreCase("TravelOptix"))                
                {
                    report.IsDotNetEnabled = IsDotNetEnabled(agency, isDemo, checker, report.ProcessKey, userNumber);
                }
            }

            return allReports;
        }

        private bool IsDotNetEnabled(string agency, bool isDemo, ReportValidation checker, int processKey, int userNumber)
        {
            if (isDemo) return true;
            
            return checker.IsReportConvertedAndAgencyEnabled(userNumber, agency, processKey);
        }

        private IList<BroadcastReportInformation> GetStandardReports(IEnumerable<ProcessCaptionInformation> processCaptions)
        {
            var standardReportsQuery = new GetStandardReportsByBatchNumberQuery(ClientDataStore.ClientQueryDb, BatchNumber, processCaptions);
            return standardReportsQuery.ExecuteQuery();
        }

        private IList<BroadcastReportInformation> GetUserDefinedReports()
        {
            var userReportsQuery = new GetUserDefinedReportsByBatchNumberQuery(ClientDataStore.ClientQueryDb, BatchNumber);
            var userReports = userReportsQuery.ExecuteQuery();

            foreach (var rpt in userReports)
            {
                rpt.ProcessKey = GetUserDefinedReportProcessKeyFromReportType(rpt);
            }

            return userReports;
        }

        private IList<BroadcastReportInformation> GetXmlUserDefinedReports()
        {
            var xmlUserDefinedReportsQuery = new GetXmlUserDefinedReportsByBatchNumberQuery(ClientDataStore.ClientQueryDb, BatchNumber);
            return xmlUserDefinedReportsQuery.ExecuteQuery();
        }

        private IList<BroadcastReportInformation> GetXmlStandardReports()
        {
            var xmlStdReportsQuery = new GetXmlStandardReportsByBatchNumberQuery(ClientDataStore.ClientQueryDb, BatchNumber);
            var xmlStdReports = xmlStdReportsQuery.ExecuteQuery();

            var xmlReportNames = GetXmlReportNames();

            foreach (var rpt in xmlStdReports)
            {
                var key = Math.Abs(rpt.UdrKey);
                if (xmlReportNames.ContainsKey(key)) rpt.UserReportName = xmlReportNames[key];
            }

            return xmlStdReports;
        }

        private IList<BroadcastReportInformation> GetTravelOptixReports()
        {
            var travelOptixReportsQuery = new GetTravleOptixReportsByBatchNumberQuery(ClientDataStore.ClientQueryDb, BatchNumber);
            return travelOptixReportsQuery.ExecuteQuery();
        }

        private IList<BroadcastReportInformation> GetSavedReports()
        {
            var savedReportsQuery = new GetSavedReportsByBatchNumberQuery(ClientDataStore.ClientQueryDb, BatchNumber);
            return savedReportsQuery.ExecuteQuery();
        }

        private static int GetUserDefinedReportProcessKeyFromReportType(BroadcastReportInformation rpt)
        {
            switch (rpt.CrystalReportType)
            {
                case "AIR":
                    return 501;
                case "HOTEL":
                    return 502;
                case "CAR":
                    return 503;
                case "SVCFEE":
                    return 504;
                case "TRACS":
                    return 7051;
                case "CALLS":
                    return 7052;
                case "PRODT":
                    return 7053;
                case "COMBDET":
                    return 520;
                case "SUMCOMB":
                    return 519;
                case "SUMFSEGD":
                case "SUMFSEGO":
                case "SUMSEGD":
                    return 518;
                case "SUMAIR":
                    return 511;
                case "SUMHOTEL":
                    return 512;
                case "SUMCAR":
                    return 513;
                default:
                    return rpt.ProcessKey;
            }
        }

        private Dictionary<int, string> GetXmlReportNames()
        {
            var xmlReportNamesQuery = new GetXmlReportNamesQuery(MasterDataStore.MastersQueryDb);
            return xmlReportNamesQuery.ExecuteQuery();
        }
    }
}
