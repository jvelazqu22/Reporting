using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using Domain.Helper;
using Domain.Orm.Classes;

using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;

namespace iBank.Services.Implementation.Shared.AdvancedClause
{
    public class AdvancedClauseBuilder
    {
        private readonly List<string> _tlsExcludedFields = new List<string>
        {
            "VENDTYPES","VENDNAMES","TRPVENDCOD","CCCOMPANY","INVAMTNFEE","FLTCOUNT","HTLCOUNT","CARCOUNT","HTLAVGRATE","CARAVGRATE","INVQTR","INVMONTH","INVMTHNAM","INVMTHABBR","BKQTR","BKMONTH","BKMTHNAM","BKMTHABBR","BKYEAR",
            "MSARRDATE","MSARRTIME","MSDEPDATE","MSDEPTIME","CABINSEAT","MXCHAINCOD","MEALDESC","NITECOUNT","OPT","ARRIVERMKS","DEPARTRMKS","MXTOURCODE","MXTOURNAME","TRNSFRRMKS","MXVENDNAME","MXSGSTATUS","TOURCOUNT",
            "MSCONFIRM","SEGAMT","MSCLASS","MSMONEYTYP","MSDESTINAT","MSEXCPRATE","MSLOSSCODE","MSORIGIN","MSSVGCODE","MSSEQNO","SVCIDNBR","MSSTNDRATE","MSTRANTYPE","SEGTYPE","MSVENDCODE"
        };

        private IParamsBuilder _paramsBuilder;
        private PickListParms _pickListParam;

        //TODO: svcfees readonly etc tables....

        public string GetAdvancedWhereClause(ReportGlobals globals, IList<AdvancedColumnInformation> advancedColumns, bool excludeTls)
        {
            var dummy = string.Empty;
            return GetAdvancedWhereClause(globals, advancedColumns, excludeTls, ref dummy);
        }

        public string GetAdvancedWhereClause(ReportGlobals globals, IList<AdvancedColumnInformation> advancedColumns, bool excludeTls, ref string serviceFeeAdvancedWhereClause)
        {
            var fixer = new AdvancedParameterFixer();
            if (globals.AdvancedParameters.Parameters.Count == 0) return "";

            globals.AdvancedParameters.Parameters = fixer.RemoveAdvancedFieldNameAndFieldNamePrefix(globals.AdvancedParameters.Parameters).ToList();

            var advParams = globals.AdvancedParameters.Parameters;

            var processor = new AdvancedClauseProcessor(advancedColumns);
            advParams = processor.PreprocessAdvancedParameter(advParams);

            var whereText = string.Empty;
            //make sure there is a semicolon if there is already a where text clause 
            if (!string.IsNullOrEmpty(globals.WhereText) && globals.WhereText.Trim().Right(1) != ";") whereText = ";";

            var isReservationReport = globals.ParmValueEquals(WhereCriteria.PREPOST, "1");
            whereText = processor.BuildWhereText(advParams, isReservationReport, whereText);

            globals.WhereText += whereText;

            var advWhereClauses = new List<string>();
            var serviceFeeAdvWhereClauses = new List<string>();
            foreach (var parm in advParams)
            {
                if (excludeTls)
                {
                    if (_tlsExcludedFields.Contains(parm.FieldName)) continue;
                    //Skip roomtype for IBCST520
                    if (globals.ProcessKey == 520 && parm.FieldName == "ROOMTYPE") continue;
                }

                //skip if fieldname or value1 is empty. FP code also checks operator. This should never happen. 
                if (parm.Operator == Operator.Empty || parm.Operator == Operator.NotEmpty || parm.Value1 == ".t." || parm.Value2 == "f")
                {
                    if (string.IsNullOrEmpty(parm.FieldName)) continue;
                }
                else
                {
                    if (string.IsNullOrEmpty(parm.FieldName) || string.IsNullOrEmpty(parm.Value1)) continue;
                }

                //verify that the field is a valid column
                var col = TranslateParameter(parm, advancedColumns);
                if (col == null) continue;

                if (!processor.IsColumnTypeAndReportTypeMatch(isReservationReport, col)) continue;

                var mutatedParameter = processor.FixField(parm, col, globals.UseHibServices);

                var advWhereClause = BuildAdvancedClause(mutatedParameter, col);
                advWhereClauses.Add(advWhereClause);

                if (col.ColTable == "SVCFEE" && globals.UseHibServices) serviceFeeAdvWhereClauses.Add(advWhereClause);
            }

            var andOr = globals.AdvancedParameters.AndOr == AndOr.And ? " AND " : " OR ";

            if (advWhereClauses.Count == 0) return "";

            serviceFeeAdvancedWhereClause = string.Join(andOr, serviceFeeAdvWhereClauses);

            var clauses = string.Join(andOr, advWhereClauses);
            return $"({clauses})";
        }

        public void BuildAdvancedWhereClause(BuildWhere where, IList<AdvancedColumnInformation> advancedColumns, bool excludeTls)
        {
            _paramsBuilder = new ParamsBuilder();
            _pickListParam = new PickListParms(where.ReportGlobals);
            var fixer = new AdvancedParameterFixer();
            if (where.ReportGlobals.AdvancedParameters.Parameters.Count == 0) where.WhereClauseAdvanced = "";

            where.ReportGlobals.AdvancedParameters.Parameters = fixer.RemoveAdvancedFieldNameAndFieldNamePrefix(where.ReportGlobals.AdvancedParameters.Parameters).ToList();

            var advParams = where.ReportGlobals.AdvancedParameters.Parameters;

            var processor = new AdvancedClauseProcessor(advancedColumns);
            advParams = processor.PreprocessAdvancedParameter(advParams);

            var whereText = string.Empty;
            //make sure there is a semicolon if there is already a where text clause 
            if (!string.IsNullOrEmpty(where.ReportGlobals.WhereText) && where.ReportGlobals.WhereText.Trim().Right(1) != ";") whereText = ";";

            var isReservationReport = where.ReportGlobals.ParmValueEquals(WhereCriteria.PREPOST, "1");
            whereText = processor.BuildWhereText(advParams, isReservationReport, whereText);

            where.ReportGlobals.WhereText += whereText;

            var advWhereClauses = new List<string>();
            var serviceFeeAdvWhereClauses = new List<string>();
            foreach (var parm in advParams)
            {
                if (excludeTls)
                {
                    if (_tlsExcludedFields.Contains(parm.FieldName)) continue;
                    //Skip roomtype for IBCST520
                    if (where.ReportGlobals.ProcessKey == 520 && parm.FieldName == "ROOMTYPE") continue;
                }

                //skip if fieldname or value1 is empty. FP code also checks operator. This should never happen. 
                //TODO: what do parm.Value1 = ".t." and parm.Value2 = "f" signify?
                if (parm.Operator == Operator.Empty || parm.Operator == Operator.NotEmpty || parm.Value1 == ".t." || parm.Value2 == "f")
                {
                    if (string.IsNullOrEmpty(parm.FieldName)) continue;
                }
                else
                {
                    if (string.IsNullOrEmpty(parm.FieldName) || string.IsNullOrEmpty(parm.Value1)) continue;
                }

                //verify that the field is a valid column
                var col = TranslateParameter(parm, advancedColumns);
                if (col == null) continue;

                if (!processor.IsColumnTypeAndReportTypeMatch(isReservationReport, col)) continue;

                var mutatedParameter = processor.FixField(parm, col, where.ReportGlobals.UseHibServices);

                var advWhereClause = string.Empty;
                //if parm.Value1 is "U-728,1200,%66" (U-728 picklist contains 1188, 0001188)
                //BuildAdvancedClauseFullSpectrum will 
                //return in ((T1.ACCT = '1188' OR T1.ACCT = '0001188' OR T1.ACCT = '1200' OR T1.ACCT LIKE '%66'))
                // or ( NOT (T1.ACCT = '1188' OR T1.ACCT = '0001188' OR T1.ACCT = '1200' OR T1.ACCT LIKE '%66')) 
                //based on the operator is inList or notInList
                //where BuildAdvancedClause will 
                //return (T1.ACCT in ('U-728','1200','%66')) 
                //or (T1.ACCT NOT LIKE 'U-728','1200','%66')) -- error respectively
                //Current enhancement only requires on ACCT, but we can expend 
                //BuildAdvancedClauseFullSpectrum to handle other picklist such as "AIRLINES, BRK1, BRK2 etc"
                if (Features.AdvancedParameterAcctPicklistCheck.IsEnabled() && parm.AdvancedFieldName == "ACCT")
                {
                    advWhereClause = BuildAdvancedClauseFullSpectrum(where, mutatedParameter, col);
                }
                else
                {
                    advWhereClause = BuildAdvancedClause(mutatedParameter, col);
                }
                
                if (parm.AdvancedFieldName == "CLASSCAT")
                {
                    //if it were segment level, we need to check if classcat in mktsegs
                    var legOrSeg = Convert.ToInt32(where.ReportGlobals.GetParmValue(WhereCriteria.RBAPPLYTOLEGORSEG));
                    if (legOrSeg == (int)SegmentOrLeg.Segment)
                    {
                        var tableName = isReservationReport ? "ibMktSegs" : "hibMktSegs";
                        advWhereClause = $"T1.reckey in (select reckey from {tableName} nolock where {advWhereClause} and trantype <> 'V')";
                    }
                }

                advWhereClauses.Add(advWhereClause);
                

                //we can use it in AppendMissingTableParamPairs then called from GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded 
                var tableRef = new AvancedParameterQueryTableRef();
                tableRef.TableName = col.ColTable;
                tableRef.AdvancedQuerySnip = advWhereClause; //for instance "BOOKRATE*NIGHTS*ROOMS > 10"; "RARRDATE = '2017/04/24'" etc
                tableRef.IsFieldInTripTable = col.ColTable == "TRIPS";
                where.AdvancedParameterQueryTableRefList.Add(tableRef);

                if (col.ColTable == "SVCFEE" && where.ReportGlobals.UseHibServices) serviceFeeAdvWhereClauses.Add(advWhereClause);
            }

            var andOr = where.ReportGlobals.AdvancedParameters.AndOr == AndOr.And ? " AND " : " OR ";

            if (advWhereClauses.Count == 0) where.WhereClauseAdvanced = "";

            where.WhereClauseServiceFeeAdvanced = string.Join(andOr, serviceFeeAdvWhereClauses);

            // we don't want to end up with a where.WhereClauseAdvanced = "()";
            if (!advWhereClauses.Any() && Features.AdvancedClauseBuilderFeatureFlag.IsEnabled()) return;

            var clauses = string.Join(andOr, advWhereClauses);
            where.WhereClauseAdvanced = $"({clauses})";
        }

        private string BuildAdvancedClause(AdvancedParameter parm, AdvancedColumnInformation col)
        {
            var isText = col.ColType == "TEXT" || col.ColType == "DATE" || col.ColType == "DATETIME";
            if (isText)
            {
                if (parm.Operator == Operator.InList || parm.Operator == Operator.NotInList)
                {
                    var items = parm.Value1.Split(',').Select(s => "'" + s.Trim() + "'");
                    parm.Value1 = string.Join(",", items);
                }
                else
                {
                    if (!parm.Value1.IsNullOrWhiteSpace()) parm.Value1 = "'" + parm.Value1 + "'";
                    if (!parm.Value2.IsNullOrWhiteSpace()) parm.Value2 = "'" + parm.Value2 + "'";
                }
                parm.Value1 = parm.Value1.Replace("*", "%");
                parm.Value2 = parm.Value2.Replace("*", "%");
                if (parm.Value1.Contains("%"))
                {
                    //Don't know why we have to change operator to Like or NotLike
                    //parm.Operator = (parm.Operator == Operator.Equal || parm.Operator == Operator.Empty)
                    //                         ? Operator.Like
                    //                         : Operator.NotLike;
                    //but we dont want to change InList %1000,%1001 to NotLike
                    parm.Operator = (parm.Operator == Operator.Equal || parm.Operator == Operator.Empty)
                                        ? Operator.Like
                                        : (parm.Operator != Operator.InList)
                                           ? Operator.NotLike
                                           : Operator.InList;
                }
            }

            switch (parm.Operator)
            {
                case Operator.Like:
                    return $"{parm.AdvancedFieldName} Like {parm.Value1}";
                case Operator.NotLike:
                    return $"{parm.AdvancedFieldName} Not Like {parm.Value1}";
                case Operator.Equal:
                    return $"{parm.AdvancedFieldName} = {parm.Value1}";
                case Operator.GreaterThan:
                    return $"{parm.AdvancedFieldName} > {parm.Value1}";
                case Operator.GreaterOrEqual:
                    return $"{parm.AdvancedFieldName} >= {parm.Value1}";
                case Operator.Between:
                    return string.Format("{0} >= {1} AND {0} <= {2}", parm.AdvancedFieldName, parm.Value1, parm.Value2);
                case Operator.Empty:
                    {
                        switch (col.ColType)
                        {
                            case "CURRENCY":
                            case "NUMERIC":
                                return $"{parm.AdvancedFieldName} = 0";
                            case "DATE":
                                return string.Format("{0} is null or {0} = cast(-53690 as datetime)", parm.AdvancedFieldName); //Translating empty to a date is a problem because MSSQL has no empty date. Can check minimum date or null.
                            default:
                                return $"{parm.AdvancedFieldName} = ''";
                        }
                    }
                case Operator.InList:
                    return $"{parm.AdvancedFieldName} in ({parm.Value1})";
                case Operator.NotBetween:
                    return string.Format("not ({0} >= {1} AND {0} <= {2})", parm.AdvancedFieldName, parm.Value1, parm.Value2);
                case Operator.NotInList:
                    return $"not ({parm.AdvancedFieldName} in ({parm.Value1}))";
                case Operator.NotEmpty:
                    {
                        switch (col.ColType)
                        {
                            case "CURRENCY":
                            case "NUMERIC":
                                return $"{parm.AdvancedFieldName} <> 0";
                            case "DATE":
                                return string.Format("{0} is not null or {0} > cast(-53690 as datetime)", parm.AdvancedFieldName);
                            default: // TEXT
                                return $"{parm.AdvancedFieldName} <> ''";
                        }
                    }
                case Operator.Lessthan:
                    return $"{parm.AdvancedFieldName} < {parm.Value1}";
                case Operator.LessThanOrEqual:
                    return $"{parm.AdvancedFieldName} <= {parm.Value1}";
                case Operator.NotEqual:
                    return $"{parm.AdvancedFieldName} != {parm.Value1}";
                default:
                    return $"{parm.AdvancedFieldName} = {parm.Value1}";
            }
        }

        private string BuildAdvancedClauseFullSpectrum(BuildWhere where, AdvancedParameter parm, AdvancedColumnInformation col)
        {
            //Current implementation parm.AdvancedFieldName is limited to in "ACCT".
            //Future implementation can expend to BREAK1, BREAK2, BREAK3 etc, which it is referred 
            //in picklist.listtype as BRK1, BRk2, BRK3 etc respectively
            var pickName = string.Empty;
            _pickListParam.ProcessList(parm.Value1, pickName, "ACCTS");
            if (_pickListParam.PickList.Any())
            {
                parm.Value1 = _pickListParam.PickName;
                return _paramsBuilder.AddOrListToWhereClause("", _pickListParam.PickList, parm.FieldName, parm.Operator == Operator.NotInList, where.SqlParameters);
            }
            return string.Empty;
        }

        public AdvancedColumnInformation TranslateParameter(AdvancedParameter parm, IList<AdvancedColumnInformation> advancedColumns)
        {
            return advancedColumns.FirstOrDefault(s => s.ColName.EqualsIgnoreCase(parm.FieldName));
        }

    }

}
