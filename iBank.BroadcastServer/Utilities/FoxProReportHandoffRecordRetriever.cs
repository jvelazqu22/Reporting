using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Helper;

using iBank.Entities.MasterEntities;
using iBank.Server.Utilities;
using iBank.Server.Utilities.ReportCriteriaHandlers;

namespace iBank.BroadcastServer.Utilities
{
    public class FoxProReportHandoffRecordRetriever
    {
        private ReportHandoffRecordHandler HandoffRecordCreator { get; set; }

        public FoxProReportHandoffRecordRetriever(ReportHandoffRecordHandler handoffHandler)
        {
            HandoffRecordCreator = handoffHandler;
        }

        public IEnumerable<reporthandoff> GetReportParameterRecordsExceptReportPeriodDates(Dictionary<int, ReportCriteria> reportParameters)
        {
            return reportParameters.Values.Where(x => !x.VarName.EqualsIgnoreCase("BEGDATE") 
                                                        && !x.VarName.EqualsIgnoreCase("ENDDATE")
                                                        && !x.VarName.EqualsIgnoreCase("PROCESSID"))
                                          .Select(x => HandoffRecordCreator.CreateReportHandoffRecord(true, x.VarName, x.VarValue));
        }

        public IEnumerable<reporthandoff> GetGeneralParameters(int processKey, int eProfileNumber)
        {
            var handoffRecs = new List<reporthandoff>
            {
                HandoffRecordCreator.CreateReportHandoffRecord(true, "PROCESSID", processKey.ToString()),
                HandoffRecordCreator.CreateReportHandoffRecord(true, "DOTNET_BCST", "YES"),
                HandoffRecordCreator.CreateReportHandoffRecord(true, "REPORTSTATUS", "PENDING"),
                HandoffRecordCreator.CreateReportHandoffRecord(true, "AGENCY", HandoffRecordCreator.Agency),
                HandoffRecordCreator.CreateReportHandoffRecord(true, "USERNBR", HandoffRecordCreator.UserNumber.ToString()),
                HandoffRecordCreator.CreateReportHandoffRecord(true, "REPORTINGTRAVET", "YES")
            };

            if (eProfileNumber > 0)
            {
                handoffRecs.Add(HandoffRecordCreator.CreateReportHandoffRecord(true, "EPROFILENO", eProfileNumber.ToString()));
            }

            return handoffRecs;
        }

        public IEnumerable<reporthandoff> GetReportPeriodDateRecords(DateTime reportPeriodStart, DateTime reportPeriodEnd)
        {
            return new List<reporthandoff>
            {
                HandoffRecordCreator.CreateReportHandoffRecord(true, "BEGDATE", reportPeriodStart.ToIBankDateFormat()),
                HandoffRecordCreator.CreateReportHandoffRecord(true, "ENDDATE", reportPeriodEnd.ToIBankDateFormat())
            };
        }

        public IEnumerable<reporthandoff> GetAdvancedParameterRecords(IList<AdvancedParameter> advancedParameters, AndOr advancedParameterAndOr)
        {
            var advancedParamRetriever = new AdvancedParameterRetriever();
            return advancedParamRetriever.GetReportHandoffRecordsFromAdvancedCriteria(advancedParameters, advancedParameterAndOr, HandoffRecordCreator.ReportId,
                HandoffRecordCreator.UserLanguage, HandoffRecordCreator.CfBox, HandoffRecordCreator.UserNumber, HandoffRecordCreator.Agency, HandoffRecordCreator.DateCreated);
        }

        public IEnumerable<reporthandoff> GetMultiUdidRecords(IList<AdvancedParameter> advancedParameters, AndOr advancedParameterAndOr)
        {
            var multiUdidRetriever = new MultiUdidParameterRetriever();
            return multiUdidRetriever.CreateReportHandoffRecordsFromMultiUdidCriteria(advancedParameters, advancedParameterAndOr, HandoffRecordCreator.ReportId, 
                HandoffRecordCreator.UserLanguage, HandoffRecordCreator.CfBox, HandoffRecordCreator.UserNumber, HandoffRecordCreator.Agency, HandoffRecordCreator.DateCreated);
        }
    }
}
