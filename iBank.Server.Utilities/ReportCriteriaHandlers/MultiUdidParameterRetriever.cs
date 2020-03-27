using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Helper;

using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;

namespace iBank.Server.Utilities.ReportCriteriaHandlers
{
    public class MultiUdidParameterRetriever : AbstractAdvancedCriteriaRetriever
    {
        public IList<AdvancedParameter> GetMultiUdidParametersFromSavedReport3(IList<savedrpt3> savedRpt3Data)
        {
            return savedRpt3Data.Where(s => s.colname.Substring(0, 4).Equals(_mudPrefix, StringComparison.OrdinalIgnoreCase)).ToList()
                                        .Select(s => new AdvancedParameter
                                        {
                                            FieldName = s.colname.Replace(_mudPrefix, "").Trim(),
                                            Operator = s.oper.Trim().ToOperator(),
                                            Value1 = string.IsNullOrEmpty(s.value1) ? "" : s.value1.Trim(),
                                            Value2 = string.IsNullOrEmpty(s.value1a) ? "" : s.value1a.Trim(),
                                            IsMultiUdid = true
                                        }).ToList();
        }

        public AndOr GetMultiUdidAndOr(IList<savedrpt3> savedRpt3Data)
        {
            var andOr = savedRpt3Data.FirstOrDefault(x => x.colname.Trim().Equals(_multiUdidAndOr, StringComparison.OrdinalIgnoreCase));

            return GetAndOrFromSavedReport3Data(andOr);
        }

        public AndOr GetMultiUdidAndOr(IList<ReportCriteria> reportCriteria)
        {
            var advParams = GetMultiUdidCriteriaFromReportCriteria(reportCriteria);

            var andOr = advParams.FirstOrDefault(s => s.VarName.Trim().Equals(_multiUdidAndOr.Trim(), StringComparison.OrdinalIgnoreCase));

            return GetAndOrFromReportCriteria(andOr);
        }

        public IList<reporthandoff> CreateReportHandoffRecordsFromMultiUdidCriteria(IList<AdvancedParameter> advancedParameters, AndOr advancedParametersAndOr,
                string reportId, string userLanguage, string cfBox, int userNumber, string agency, DateTime dateCreated)
        {
            var handoffRecords = new List<reporthandoff>();
            var multiUdids = advancedParameters.Where(x => x.IsMultiUdid).ToList();

            if (!multiUdids.Any()) return handoffRecords;

            var handoffCreator = new ReportHandoffRecordHandler(reportId, userLanguage, cfBox, userNumber, agency, DateTime.Now);
            
            //for reporthandoff values a 1 = And | 2 = Or
            handoffRecords.Add(handoffCreator.CreateReportHandoffRecord(true, _multiUdidAndOr, advancedParametersAndOr == AndOr.And ? "1" : "2"));
            
            var i = 1;
            foreach (var udid in multiUdids)
            {
                //MUD
                handoffRecords.Add(handoffCreator.CreateReportHandoffRecord(true, CreateIteratedVariableName("MUD", i), udid.FieldName));

                //MUDFLD
                var formattedUdid = DatabaseFormattedMudField(udid.FieldName);
                handoffRecords.Add(handoffCreator.CreateReportHandoffRecord(true, CreateIteratedVariableName("MUDFLD", i), formattedUdid));

                //MUDOPER
                var oper = udid.Operator.ToFriendlyString();
                handoffRecords.Add(handoffCreator.CreateReportHandoffRecord(true, CreateIteratedVariableName("MUDOPER", i), oper));

                //MUDTEXT
                handoffRecords.Add(handoffCreator.CreateReportHandoffRecord(true, CreateIteratedVariableName("MUDTEXT", i), udid.Value1));

                i++;
            }

            return handoffRecords;
        }

        public IList<AdvancedParameter> GetMultiUdidParametersFromReportCriteria(IList<ReportCriteria> reportCriteria)
        {
            var advParams = GetMultiUdidCriteriaFromReportCriteria(reportCriteria);

            //Get the individual groups (can be up to 10)
            var groups = advParams.Where(s => s.VarName.Substring(0, 3).Equals("MUD", StringComparison.OrdinalIgnoreCase)).Select(s => s.VarName.Right(2)).Distinct().ToList();

            var advancedParameters = new List<AdvancedParameter>();
            foreach (var group in groups)
            {
                if (group == "OR") continue;
                var newParam = new AdvancedParameter {IsMultiUdid = true};
                
                var groupRows = advParams.Where(s => s.VarName.Right(2).Equals(group));
                foreach (var groupRow in groupRows)
                {
                    //field, value, or operator
                    var rowType = groupRow.VarName.Substring(3, groupRow.VarName.Length - 5);

                    switch (rowType)
                    {
                        case "": //this will be the udid number
                            newParam.FieldName = groupRow.VarValue;
                            break;
                        case "TEXT":
                            newParam.Value1 = groupRow.VarValue;
                            break;
                        case "OPER":
                            newParam.Operator = groupRow.VarValue.ToOperator();
                            break;
                    }
                }

                if (newParam.Value1.HasWildCards()) newParam.Value1 = newParam.Value1.ReplaceWildcards();

                advancedParameters.Add(newParam);
            }

            return advancedParameters;
        }

        private string DatabaseFormattedMudField(string udidNumber)
        {
            return _mudPrefix + udidNumber;
        }

        private IList<ReportCriteria> GetMultiUdidCriteriaFromReportCriteria(IList<ReportCriteria> reportCriteria)
        {
            return reportCriteria.Where(s => s.VarName.Left(3) == "MUD").ToList();
        }
    }
}
