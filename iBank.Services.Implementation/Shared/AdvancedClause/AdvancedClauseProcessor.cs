using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

using CODE.Framework.Core.Utilities;

using Domain.Helper;

using iBank.Server.Utilities;

using Domain.Orm.Classes;

namespace iBank.Services.Implementation.Shared.AdvancedClause
{
    public class AdvancedClauseProcessor
    {
        private readonly AdvancedParameterFixer _fixer = new AdvancedParameterFixer();
        private readonly List<string> _dowColumns = new List<string> { "RENTDTDOW", "RETDATEDOW", "DATEINDOW", "CHKOUTDOW", "DEPDTDOW", "ARRDTDOW", "ARRDATEDOW", "INVDTDOW", "BOOKDTDOW" };

        private readonly List<string> _tkTables = new List<string>
        {
            "TKTRIPS","TKSEGS","TTRTRIPS","TTRSEGS"
        };

        private readonly List<string> _exchangeFields = new List<string> { "EXCHANGE", "TREFUNDABL", "HINVBYAGCY", "AINVBYAGCY" };
        private readonly List<string> _yesValues = new List<string> { "Y", "YES", "1", "TRUE", "T" };
        public IList<AdvancedColumnInformation> AdvancedColumns { get; set; }

        public AdvancedClauseProcessor(IList<AdvancedColumnInformation> advancedColumns)
        {
            AdvancedColumns = advancedColumns;
        }

        public List<AdvancedParameter> PreprocessAdvancedParameter(List<AdvancedParameter> advParms)
        {
            foreach (var parm in advParms)
            {
                var colInfo = TranslateParameter(parm);
                if (colInfo == null) continue;

                if ((colInfo.ColType == "CURRENCY" || colInfo.ColType == "NUMERIC") && parm.Operator != Operator.InList && parm.Operator != Operator.NotInList)
                {
                    //remove any characters that aren't digits or decimals. 
                    parm.Value1 = Regex.Replace(parm.Value1, @"[^-?\d+\.]", string.Empty);
                    parm.Value2 = Regex.Replace(parm.Value2, @"[^-?\d+\.]", string.Empty);
                }
                //Populate it so it can be used in AppendMissingTableParamPairs
                parm.TableName = colInfo.ColTable;
                //TODO: Check for valid date. Fix if possible. Not sure this is still necessary.
                //It seems that the AdvancedCriteria dates can come in as normal dates. TBD: is this a 
                //safe assumption?
                if (colInfo.ColType == "DATE")
                {
                    //parm.Value1 = SharedProcedures.GetDate(parm.Value1);
                    //parm.Value2 = SharedProcedures.GetDate(parm.Value2);
                    DateTime d;
                    if (!DateTime.TryParse(parm.Value1, out d))
                    {
                        var temp = parm.Value1.ToDateFromiBankFormattedString();
                        if (temp.HasValue)
                            parm.Value1 = temp.Value.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
                    }
                    if (!DateTime.TryParse(parm.Value2, out d))
                    {
                        var temp = parm.Value2.ToDateFromiBankFormattedString();
                        if (temp.HasValue)
                            parm.Value2 = temp.Value.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
                    }
                }
            }

            return advParms;
        }

        public string BuildWhereText(List<AdvancedParameter> advParams, bool isReservationReport, string whereText)
        {
            foreach (var parm in advParams)
            {
                //skip if fieldname or value1 is empty. FP code also checks operator. This should never happen. 
                if (parm.Operator == Operator.Empty || parm.Operator == Operator.NotEmpty)
                {
                    if (string.IsNullOrEmpty(parm.FieldName))
                    {
                        continue;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(parm.FieldName) || string.IsNullOrEmpty(parm.Value1))
                    {
                        continue;
                    }
                }

                //verify that the field is a valid column
                var col = TranslateParameter(parm); // AdvancedColumnList.FirstOrDefault(s => s.ColName == parm.FieldName);
                if (col == null) continue;

                //if history, and preview only
                if (col.Usage == "PREVIEW" && !isReservationReport) continue;

                //* 04/23/08 -  THE COLUMN "SAVINGS CODE" IS NOT AVAILBLE IN "PREVIEW" DATA.
                //*  PEOPLE USE IT ANYWAY AND DON'T KNOW WHY IT IS IGNORED FOR USER SQL.
                //*  SO, WHEN THE REPORT IS FOR PREVIEW, WE WILL USE THE REASCODE COLUMN IN THIS CRITIERIA INSTEAD OF SAVINGCODE
                if (col.Usage == "HISTORY" && isReservationReport && parm.FieldName != "SAVINGCODE") continue;

                //* 04/23/08 - HERE IS WHERE WE SWAP REASCODE FOR SAVINGCODE
                if (isReservationReport && parm.FieldName == "SAVINGCODE")
                {
                    parm.FieldName = "REASCODE";
                }

                if (parm.Operator != Operator.Empty && parm.Operator != Operator.NotEmpty)
                {
                    if (col.ColType == "DATE")
                    {
                        //NOTE: AdvancedClause dates can be, or might always be, a dateformatted string.
                        //format the dates in Value1,2
                        //parm.Value1 = SharedProcedures.GetDate(parm.Value1);
                        //parm.Value2 = SharedProcedures.GetDate(parm.Value2);
                        DateTime d;
                        if (!DateTime.TryParse(parm.Value1, out d))
                        {
                            var temp = parm.Value1.ToDateFromiBankFormattedString();
                            if (temp.HasValue)
                                parm.Value1 = temp.Value.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
                        }
                        if (!DateTime.TryParse(parm.Value2, out d))
                        {
                            var temp = parm.Value2.ToDateFromiBankFormattedString();
                            if (temp.HasValue)
                                parm.Value2 = temp.Value.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
                        }
                    }
                }

                if (_dowColumns.Contains(parm.FieldName))
                {
                    parm.Value1 = _fixer.TranslateValue1DayOfWeekNumericToCharacter(parm);
                }

                switch (parm.Operator)
                {
                    case Operator.NotBetween:
                    case Operator.Between:
                        whereText += string.Format("{0}{1} {2} and {3};", col.BigName, parm.Operator.ToFriendlyString(), parm.Value1, parm.Value2);
                        break;
                    case Operator.Empty:
                        whereText += string.Format("{0} DOES NOT EXIST;", col.BigName);
                        break;
                    case Operator.NotEmpty:
                        whereText += string.Format("{0} EXISTS;", col.BigName);
                        break;
                    case Operator.Equal:
                    case Operator.GreaterThan:
                    case Operator.GreaterOrEqual:
                    case Operator.InList:
                    case Operator.NotInList:
                    case Operator.Lessthan:
                    case Operator.LessThanOrEqual:
                    case Operator.NotEqual:
                        whereText += string.Format("{0} {1} {2};", col.BigName, parm.Operator.ToFriendlyString(), parm.Value1);
                        break;
                }
            }

            return whereText;
        }

        public string MapServiceFeeField(AdvancedColumnInformation col, AdvancedParameter param)
        {
            switch (col.ColName.ToUpper())
            {
                //These cases should be handled differently. Otherwise use advcolname
                case "SCARDNUM": //fixing SCARDNUM to SFCARDNUM
                    return "SFCARDNUM";
                case "SDESCRIPT":
                    return "SVCDESC";
                case "SSVCFEE":
                    return "SVCAMT";
                default:
                    return param.AdvancedFieldName;
            }
        }

        public AdvancedParameter FixField(AdvancedParameter parm, AdvancedColumnInformation col, bool useHibServices)
        {
            //map to avoid mutating the original object
            var newParam = new AdvancedParameter();
            Mapper.Map(parm, newParam);
            
            if (col.ColTable.ToUpper() == "SVCFEE" && useHibServices)
            {
                newParam.FieldName = MapServiceFeeField(col, newParam);
            }
            else if (col.ColTable.ToUpper() == "SVCFEE")
            {
                newParam.FieldName = newParam.AdvancedFieldName;
            }

            if (_tkTables.Contains(col.ColTable) || col.ColTable == "HOTEL" || col.ColTable == "AUTO")
            {
                newParam.FieldName = col.AdvancedColName;
            }

            //various tables have columns that need to be renamed in some way. 
            if (col.ColTable == "TTRLOG")
            {
                newParam.FieldName = FixTLogColumnName(newParam.FieldName);
            }
            if (col.ColTable == "TTREMLOG")
            {
                newParam.FieldName = FixTELogColumnName(newParam.FieldName);
            }
            if (col.ColTable == "AXITRACS")
            {
                newParam.FieldName = FixAXTColumnName(newParam.FieldName);
            }
            if (col.ColTable == "AXICALLS")
            {
                newParam.FieldName = FixAXCColumnName(newParam.FieldName);
            }
            if (col.ColTable == "AXIPRODT")
            {
                newParam.FieldName = FixAXPColumnName(newParam.FieldName);
            }
            if (col.ColTable == "CCTRANS")
            {
                newParam.FieldName = FixCCColumnName(newParam.FieldName);
            }
            if (newParam.FieldName == "ACONFIRMNO")
            {
                newParam.FieldName = "CONFIRMNO";
            }

            if (_dowColumns.Contains(newParam.FieldName))
            {
                newParam.Value1 = _fixer.TranslateValue1DayOfWeekCharacterToNumeric(newParam);
            }
            newParam.AdvancedFieldName = _fixer.FixParameterAdvancedFieldName(newParam);

            //TODO: comp2exp function, which handles "COMPUTED" fields. Line 451
            //TODO: Handle FoxPro expression fields. Line 460
            //TODO: Handle "datediff" stuff, which is FoxSql. Line 468.

            //SPECIAL HANDLING OF "EXCHANGE"
            if (_exchangeFields.Contains(col.ColName))
            {
                newParam.Value1 = _yesValues.Contains(newParam.Value1) ? "1" : "0";
            }

            //SPECIAL HANDLING OF "CONNECT"
            if (col.ColName == "CONNECT")
            {
                newParam.Value1 = (newParam.Value1 == "Y" || newParam.Value1 == "X") ? "X" : "O";
            }

            //TODO: TLS Handling. Uses Fox functions. Line 499.

            //TODO: There's code here for handling wildcards...not sure if we need this, or if we should do it differently. Line 555.
            //It's possible we can get away with just doing a "fieldname.contains(value) for all fields. 

            #region Special Cases 

            if (col.ColName.EqualsIgnoreCase("EXCHANGE") || col.ColName.EqualsIgnoreCase("TREFUNDABL") || col.ColName.EqualsIgnoreCase("HINVBYAGCY") || col.ColName.EqualsIgnoreCase("AINVBYAGCY"))
            {
                if (newParam.Value1.EqualsIgnoreCase("Y") || newParam.Value1.EqualsIgnoreCase("YES") ||
                    newParam.Value1.EqualsIgnoreCase("1") || newParam.Value1.EqualsIgnoreCase("T") ||
                    newParam.Value1.EqualsIgnoreCase("TRUE"))
                {
                    newParam.Value1 = "1";
                }
                else
                {
                    newParam.Value1 = "0";
                }
            }

            if (col.ColName.EqualsIgnoreCase("CONNECT"))
            {
                if (newParam.Value1.EqualsIgnoreCase("Y") || newParam.Value1.EqualsIgnoreCase("X"))
                {
                    newParam.Value1 = "X";
                }
                else
                {
                    newParam.Value1 = "O";
                }
            }
            #endregion

            //Line 615, inlist processing of SQL TEXT-type columns where they perform a LIKE comparison instead of =. 
            //The FP code replaces * for a %, and if there is no wildcard, adds one at the end to force a LIKE comparison.
            if (new[] { "RESULT", "EMAILLOG", "EMAILADDR" }.Any(s => s == col.ColName))
            {
                newParam.Value1 = newParam.Value1.Replace('*', '%');
                if (!newParam.Value1.Contains("%")) newParam.Value1 = newParam.Value1 + "%";
            }

            newParam = FixAmbiguousFields(newParam, col);

            return newParam;
        }

        public bool IsColumnTypeAndReportTypeMatch(bool isReservationReport, AdvancedColumnInformation col)
        {
            if (col.Usage == "PREVIEW" && !isReservationReport) return false;

            if (col.Usage == "HISTORY" && isReservationReport) return false;

            return true;
        }

        private AdvancedColumnInformation TranslateParameter(AdvancedParameter parm)
        {
            return AdvancedColumns.FirstOrDefault(s => s.ColName.EqualsIgnoreCase(parm.FieldName));
        }

        private string FixTColumnName(string colName)
        {
            if (colName == "TPCC") return "PSEUDOCITY";
            if (colName == "TEXPIREDAT") return "EXPIREDATE";
            if (colName == "TREISSUEDA") return "REISSUEDATE";
            if (colName == "TREFUNDTYP") return "REFUNDTYPE";
            if (colName == "TREISSUEFE") return "REISSUEFEE";
            if (colName == "TRESTRICTN") return "RESTRICTNS";
            if (colName == "MCOFLAG") return "MCOFLAG";
            if (colName == "ITINCANCEL") return "ITINCANCEL";
            if (colName == "TAX2") return "TAX2";
            // Strip the leading "T". 
            // Original code doesn't check the leading characters.
            return colName.TrimStart("T".ToCharArray());
        }

        private string FixTLogColumnName(string colName)
        {
            // most can just return substring
            if (colName == "TLPCC") return "PCC";
            if (colName == "TLTICKET") return "TICKET";
            if (colName == "TLRECLOC") return "RECLOC";
            if (colName == "TLGDS") return "GDS";
            if (colName == "TLTKTDATE") return "TKTDATE";
            if (colName == "TLEVENTDATE") return "DATESENT";
            if (colName == "TLLOGTYPE") return "LOGTYPE";
            if (colName == "TLSUBLOGTY") return "SUBLOGTYPE";
            if (colName == "TLREFUNDBL") return "REFUNDABLE";
            if (colName == "TLRESULT") return "RESULT";
            return colName.Substring(2); // JUST STRIP THE LEADING "TL"
        }

        private string FixTELogColumnName(string colName)
        {
            if (colName == "TELEMAILAD") return "EMAILADDR";
            if (colName == "TELTICKET") return "TICKET";
            if (colName == "TELDATSENT") return "DATESENT";
            if (colName == "TELMSGTYPE") return "MSGTYPE";
            if (colName == "TELEMLOG") return "EMAILLOG";
            return colName.Substring(3); // JUST STRIP THE LEADING "TEL"
        }

        private string FixAXTColumnName(string colName)
        {
            // ToDo: Do we really want to prefix aliases?
            // var p = "T51.";
            const string p = "";
            // ALWAYS USE TABLE ALIAS "T51." FOR AXITRACS TABLE
            if (colName == "AXISITE") return p + "AXISITE";
            if (colName == "AXTPASSLAS") return p + "PASSLAST";
            if (colName == "AXTPASSFRS") return p + "PASSFRST";
            if (colName == "AXTTERMCOD") return p + "TERMCODE";
            if (colName == "AXTTRANDAT") return p + "TRANDATE";
            if (colName == "AXTTRANTYP") return p + "TRANTYPE";
            return p + colName.Substring(3); // JUST STRIP THE LEADING "AXT"
        }

        private string FixAXCColumnName(string colName)
        {
            // ToDo: Do we really want to prefix aliases?
            // var p = "T52.";
            const string p = "";
            // ALWAYS USE TABLE ALIAS "T52." FOR AXICALLS TABLE
            if (colName == "AXCPAXEMAL") return p + "PAXEMAIL";
            if (colName == "AXCCALRTYP") return p + "CALLERTYPE";
            if (colName == "AXCRESTYPE") return p + "RESTYPE";
            if (colName == "AXCSVCTYPE") return p + "SVCTYPE";
            if (colName == "AXCUSERID") return p + "USERID";
            return p + colName.Substring(3); // JUST STRIP THE LEADING "AXC"
        }

        private string FixAXPColumnName(string colName)
        {
            // ToDo: Do we really want to prefix aliases?
            // var p = "T57.";
            const string p = "";
            // ALWAYS USE TABLE ALIAS "T57." FOR AXICALLS TABLE
            if (colName == "AXPSITE") return p + "AXISITE";
            if (colName == "AXPPASSLAS") return p + "PASSLAST";
            if (colName == "AXPPASSFRS") return p + "PASSFRST";
            if (colName == "AXPTRANDAT") return p + "TRANDATE";
            if (colName == "AXPUSERID") return p + "USERID";
            if (colName == "AXPACCT") return p + "ACCT";
            if (colName == "AXPRECLOC") return p + "RECLOC";
            if (colName == "AXPGDS") return p + "GDS";
            if (colName == "AXPBKTOOL") return p + "BKTOOL";
            return p + colName.Substring(3); // JUST STRIP THE LEADING "AXC"
        }

        private string FixCCColumnName(string colName)
        {
            if (colName == "CCSRCABBR") return "SOURCEABBR";
            if (colName == "CCCARDNUM") return "CARDNUM";
            if (colName == "CCACCTNBR") return "ACCTNBR";
            if (colName == "CCTRANTYPE") return "TRANTYPE";
            if (colName == "CCMONEYTYP") return "MONEYTYPE";
            return colName; // THE REST ARE THE ACTUAL COLUMN NAMES.
        }

        private AdvancedParameter FixAmbiguousFields(AdvancedParameter parm, AdvancedColumnInformation col)
        {
            parm.FieldName = _fixer.HandleAmbiguousFields(parm.FieldName, col.ColTable);
            parm.AdvancedFieldName = _fixer.HandleAmbiguousFields(parm.AdvancedFieldName, col.ColTable);

            return parm;
        }
    }
}
