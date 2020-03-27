using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Helper;

using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;

using iBankDomain.RepositoryInterfaces;

namespace iBank.Server.Utilities.ReportCriteriaHandlers
{
    public class AdvancedParameterRetriever : AbstractAdvancedCriteriaRetriever
    {
        private readonly string _advCritAndOr = "AOCANDOR";
        public IList<AdvancedParameter> GetAdvancedParametersFromSavedReport3(IList<savedrpt3> savedRpt3Data, IQuery<IList<collist2>> getActiveColumnsQuery)
        {
            var advancedCols = getActiveColumnsQuery.ExecuteQuery();

            var data = savedRpt3Data.Join(advancedCols,
                                            sr3 => sr3.colname.Trim(),
                                            ac => ac.colname.Trim(),
                                            (sr3, ac) => new { sr3, ac })
                                    .Where(x => !x.sr3.colname.Substring(0, 4).EqualsIgnoreCase(_mudPrefix)
                                                && !x.sr3.colname.EqualsIgnoreCase(_advCritAndOr)
                                                && !x.sr3.colname.EqualsIgnoreCase(_multiUdidAndOr))
                                    .Select(x => new AdvancedParameter
                                    {
                                        FieldName = x.sr3.colname.Trim(),
                                        Operator = x.sr3.oper.Trim().ToOperator(),
                                        Value1 = string.IsNullOrEmpty(x.sr3.value1) ? "" : x.sr3.value1.Trim(),
                                        Value2 = string.IsNullOrEmpty(x.sr3.value1a) ? "" : x.sr3.value1a.Trim(),
                                        Type = string.IsNullOrEmpty(x.ac.coltype) ? "TEXT" : x.ac.coltype.Trim(),
                                        AdvancedFieldName = x.ac.advcolname.Trim(),
                                        IsLookup = x.ac.islookup,
                                        IsMultiUdid = false
                                    }).ToList();

            TransformDataForSpecialCases(data);

            return data;

        }

        public AndOr GetAdvancedParametersAndOr(IList<savedrpt3> savedRpt3Data)
        {
            var andOr = savedRpt3Data.FirstOrDefault(x => x.colname.Trim().EqualsIgnoreCase(_advCritAndOr));

            return GetAndOrFromSavedReport3Data(andOr);
        }

        public AndOr GetAdvancedParametersAndOr(IList<ReportCriteria> reportCriteria)
        {
            var advParams = GetAdvancedCriteriaFromReportCriteria(reportCriteria);
            var andOr = advParams.FirstOrDefault(s => s.VarName.Trim().EqualsIgnoreCase(_advCritAndOr));
            
            return GetAndOrFromReportCriteria(andOr);
        }

        public IList<reporthandoff> GetReportHandoffRecordsFromAdvancedCriteria(IList<AdvancedParameter> advancedParameters, AndOr advancedParameterAndOr, string reportId, 
            string userLanguage, string cfBox, int userNumber, string agency, DateTime dateCreated)
        {
            var reportHandoffRecords = new List<reporthandoff>();
            advancedParameters = advancedParameters.Where(x => !x.IsMultiUdid).ToList();

            if (!advancedParameters.Any()) return reportHandoffRecords;

            var handoffCreator = new ReportHandoffRecordHandler(reportId, userLanguage, cfBox, userNumber, agency, dateCreated);
           
            reportHandoffRecords.Add(handoffCreator.CreateReportHandoffRecord(true, _advCritAndOr, advancedParameterAndOr == AndOr.And ? "1" : "2"));

            var i = 1;
            foreach (var param in advancedParameters)
            {
                //AOCFLD
                reportHandoffRecords.Add(handoffCreator.CreateReportHandoffRecord(true, CreateIteratedVariableName("AOCFLD", i), param.FieldName));

                //AOCOPER
                var oper = param.Operator.ToFriendlyString();
                reportHandoffRecords.Add(handoffCreator.CreateReportHandoffRecord(true, CreateIteratedVariableName("AOCOPER", i), oper));

                //AOCVALUE
                reportHandoffRecords.Add(handoffCreator.CreateReportHandoffRecord(true, CreateIteratedVariableName("AOCVALUE", i), param.Value1));

                //AOCVALUEA
                reportHandoffRecords.Add(handoffCreator.CreateReportHandoffRecord(true, CreateIteratedVariableName("AOCVALUEA", i), param.Value2));

                //AOCSELECT
                reportHandoffRecords.Add(handoffCreator.CreateReportHandoffRecord(true, CreateIteratedVariableName("AOCSELECT", i), "1"));

                i++;
            }

            return reportHandoffRecords;
        }

        public IList<AdvancedParameter> GetAdvancedParametersFromReportCriteria(IList<ReportCriteria> reportCriteria, IQuery<IList<collist2>> getActiveColumnsQuery)
        {
            var collist = getActiveColumnsQuery.ExecuteQuery();

            //any parameters starting with AOC are Advanced criteria.
            var advParams = GetAdvancedCriteriaFromReportCriteria(reportCriteria).ToList();

            //Get the individual groups (can be up to 10)
            var groups = advParams.Where(s => s.VarName.Trim().ToUpper() != _advCritAndOr).Select(s => s.VarName.Right(2)).Distinct().ToList();
            var advancedParameters = new List<AdvancedParameter>();
            foreach (var group in groups)
            {
                var newParam = new AdvancedParameter();
                var group1 = group;
                var groupRows = advParams.Where(s => s.VarName.Right(2) == group1);
                foreach (var groupRow in groupRows)
                {
                    //field, value, or operator
                    var rowType = groupRow.VarName.Substring(3, groupRow.VarName.Length - 5);

                    switch (rowType)
                    {
                        case "FLD":
                            newParam.FieldName = groupRow.VarValue;
                            break;
                        case "VALUE":
                            newParam.Value1 = groupRow.VarValue.Replace("\n", string.Empty);
                            break;
                        case "VALUEA":
                            newParam.Value2 = groupRow.VarValue.Replace("\n", string.Empty);
                            break;
                        case "OPER":
                            newParam.Operator = groupRow.VarValue.ToOperator();
                            break;
                    }

                }
                var col = collist.FirstOrDefault(s => s.colname.Trim().EqualsIgnoreCase(newParam.FieldName));
                if (col != null)
                {
                    newParam.Type = col.coltype.Trim();
                    newParam.AdvancedFieldName = col.advcolname.Trim();
                    newParam.IsLookup = col.islookup;
                }
                advancedParameters.Add(newParam);
            }

            TransformDataForSpecialCases(advancedParameters);

            return advancedParameters;
        }
        
        private IList<ReportCriteria> GetAdvancedCriteriaFromReportCriteria(IList<ReportCriteria> reportCriteria)
        {
            return reportCriteria.Where(s => s.VarName.Left(3) == "AOC").ToList();
        }

        //FoxPro has made all kinds of exceptions to the rule over the years...here is where we try and handle those
        private void TransformDataForSpecialCases(IList<AdvancedParameter> parameters)
        {
            var specialCases = new List<string>{
                                                    AdvancedParameterSpecialCases.EXCHANGE,
                                                    AdvancedParameterSpecialCases.TREFUNDABL,
                                                    AdvancedParameterSpecialCases.HINVBYAGCY,
                                                    AdvancedParameterSpecialCases.AINVBYAGCY,
                                                    AdvancedParameterSpecialCases.CONNECT
                                                };


            var alternateTrue = new List<string> { "Y", "YES", "TRUE", "T", "1" };
            foreach (var param in parameters.Where(x => specialCases.Contains(x.FieldName.ToUpper())))
            {
                if (param.FieldName.EqualsIgnoreCase(AdvancedParameterSpecialCases.CONNECT))
                {
                    //ibbldadvwh.prg - line 556 - 562
                    param.Value1 = (param.Value1.EqualsIgnoreCase("Y") || param.Value1.EqualsIgnoreCase("X")) ? "X" : "O";
                }
                else
                {
                    //ibbldadvwh.prg - line 544 - 554
                    param.Value1 = alternateTrue.Contains(param.Value1.ToUpper()) ? "1" : "0";
                }
            }
        }
    }
}
