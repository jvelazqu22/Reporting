using System;
using System.Collections.Generic;
using Domain.Helper;
using Domain.Models;
using Domain.Orm.Classes;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.AdvancedClause;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared.AdvancedClause
{
    [TestClass]
    public class WhereClauseWithAdvanceParamsHandlerTests
    {
        [TestMethod]
        public void WhereClauseWithAdvanceParamsHandler_AllLegUseServiceFeeIsOffPassReference_ServiceFeeAdvancedClauseIsEmpty()
        {
            //Arrange
            var sql = new SqlScript();
            var buildWhere = new BuildWhere(null);
            buildWhere.ReportGlobals = new ReportGlobals();
            buildWhere.ReportGlobals.UseHibServices = false;
            buildWhere.ReportGlobals.AdvancedParameters = new AdvancedParameters
            {
                AndOr = AndOr.And,
                Parameters = new List<AdvancedParameter>()
                        {
                            new AdvancedParameter
                            {
                                AdvancedFieldName = "CARDNUM",
                                FieldName = "SCARDNUM",
                                Type = "Text",
                                Value1 = "XXX"
                            }
                        }
            };
            buildWhere.AdvancedColumnList = new List<AdvancedColumnInformation>()
            {
                new AdvancedColumnInformation() { AdvancedColName = "CARDNUM", ColName="SCARDNUM", ColTable="SVCFEE", ColType="TEXT"},
                new AdvancedColumnInformation() { AdvancedColName = "CARDNUM", ColName="CARDNUM", ColTable="TRIPS", ColType="TEXT"}
            };

            var exp = "";
            //act
            var serviceFeeWhere = string.Empty;
            buildWhere.AddAdvancedClauses(ref serviceFeeWhere);

            //Assert
            Assert.AreEqual(exp, serviceFeeWhere);
        }


        [TestMethod]
        public void WhereClauseWithAdvanceParamsHandler_AllLegUseServiceFeeIsOnNotPassReference_MatchReturnInMainQuery()
        {
            //Arrange
            var handler = new WhereClauseWithAdvanceParamsHandler();
            var sql = new SqlScript() { FieldList = "fieldlist", FromClause = "hibtrips T1 (nolock)", WhereClause="whereclause", KeyWhereClause="keywhereclause"};
            var buildWhere = new BuildWhere(null);
            buildWhere.ReportGlobals = new ReportGlobals();
            buildWhere.ReportGlobals.UseHibServices = true;
            buildWhere.ReportGlobals.AdvancedParameters = new AdvancedParameters
            {
                AndOr = AndOr.And,
                Parameters = new List<AdvancedParameter>()
                        {
                            new AdvancedParameter
                            {
                                AdvancedFieldName = "CARDNUM",
                                FieldName = "SCARDNUM",
                                Type = "Text",
                                Value1 = "XXX"
                            }
                        }
            };
            buildWhere.AdvancedColumnList = new List<AdvancedColumnInformation>()
            {
                new AdvancedColumnInformation() { AdvancedColName = "CARDNUM", ColName="SCARDNUM", ColTable="SVCFEE", ColType="TEXT"},
                new AdvancedColumnInformation() { AdvancedColName = "CARDNUM", ColName="CARDNUM", ColTable="TRIPS", ColType="TEXT"}
            };

            var exp = "whereclause and (SFCARDNUM = 'XXX')";
           
            //act
            //do not pass servicefee reference to force advanced where clause be included in the main query
            buildWhere.AddAdvancedClauses();
            sql.WhereClause = handler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(sql, buildWhere, false);

            //Assert
            Assert.AreEqual(exp.ToUpper(), sql.WhereClause.ToUpper());
        }

        [TestMethod]
        public void WhereClauseWithAdvanceParamsHandler_AllLegUseServiceFeeIsOnPassReference_MatchReturnSubqury()
        {
            //Arrange
            var handler = new WhereClauseWithAdvanceParamsHandler();
            var sql = new SqlScript();
            var buildWhere = new BuildWhere(null); 
            buildWhere.ReportGlobals = new ReportGlobals();
            buildWhere.ReportGlobals.UseHibServices = true;
            buildWhere.ReportGlobals.AdvancedParameters = new AdvancedParameters {
                AndOr = AndOr.And,
                Parameters = new List<AdvancedParameter>()
                        {
                            new AdvancedParameter
                            {
                                AdvancedFieldName = "CARDNUM",
                                FieldName = "SCARDNUM",
                                Type = "Text",
                                Value1 = "XXX"
                            }
                        }
                };
            buildWhere.AdvancedColumnList = new List<AdvancedColumnInformation>()
            {
                new AdvancedColumnInformation() { AdvancedColName = "CARDNUM", ColName="SCARDNUM", ColTable="SVCFEE", ColType="TEXT"},
                new AdvancedColumnInformation() { AdvancedColName = "CARDNUM", ColName="CARDNUM", ColTable="TRIPS", ColType="TEXT"}
            };

            var serviceFeeWhere = string.Empty;
            buildWhere.AddAdvancedClauses(ref serviceFeeWhere);

            var exp = " and T1.reckey in (select DISTINCT reckey from hibservices nolock where SFCARDNUM = 'XXX' and reckey = t1.reckey)";
            //act
            sql.WhereClause = handler.GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(sql, buildWhere, false, serviceFeeWhere);

            //Assert
            Assert.AreEqual(exp, sql.WhereClause);

        }
    }
}
