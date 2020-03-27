using System.Linq;
using Domain.Exceptions;
using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.Interfaces;

namespace iBank.Server.Utilities.ReportGlobalsSetters
{

    public class ReportGlobalsCreator
    {
        public static ReportGlobals CreateFromOnlineReport(IReportGlobalsCreatorParams parms, int serverNumber)
        {
            var globals = new ReportGlobals
            {
                ReportInformation = parms.ReportInformation,
                CrystalDirectory = parms.CrystalDirectory,
                iBankVersion = parms.iBankVersion,
                ServerNumber = serverNumber,
                IsOfflineServer = parms.IsOfflineServer
            };
            
            var reportCrit = parms.CriteriaRetriever.GetReportCriteriaFromReportId(parms.GetReportCriteriaQuery);
            if (!reportCrit.Any())
            {
                throw new ReportNotFoundException($"Report id [{parms.ReportInformation.ReportId}] not found.");
            }

            globals.UserNumber = parms.CriteriaRetriever.GetUserNumberFromReportCriteria(reportCrit);
            globals.ReportLogKey = parms.CriteriaRetriever.GetReportLogKeyFromReportCriteria(reportCrit);
            globals.ReportParameters = parms.CriteriaRetriever.GenerateStandardCriteria(parms.WhereCriteriaLookup, reportCrit);
            
            globals.AdvancedParameters.Parameters = parms.AdvParamRetriever.GetAdvancedParametersFromReportCriteria(reportCrit, 
                parms.GetActiveColumnsQuery).ToList();
            globals.AdvancedParameters.AndOr = parms.AdvParamRetriever.GetAdvancedParametersAndOr(reportCrit);

            globals.MultiUdidParameters.Parameters = parms.UdidRetriever.GetMultiUdidParametersFromReportCriteria(reportCrit).ToList();
            globals.MultiUdidParameters.AndOr = parms.UdidRetriever.GetMultiUdidAndOr(reportCrit);
            globals.TestReportName = parms.CriteriaRetriever.GetTestReportNameFromReportCriteria(reportCrit);
            globals.TestReportPath = parms.CriteriaRetriever.GetTestReportPathFromReportCriteria(reportCrit);

            var setter = new GlobalValuesSetter();
            setter.SetGlobals(globals);

            return globals;
        }
    }
}
