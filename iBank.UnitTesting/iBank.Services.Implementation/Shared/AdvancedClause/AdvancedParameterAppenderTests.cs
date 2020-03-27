using Domain.Helper;
using iBank.Services.Implementation.Shared.AdvancedClause;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Globalization;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared.AdvancedClause
{
    [TestClass]
    public class AdvancedParameterAppenderTests
    {
        [TestMethod]
        public void AppendMissingTableParamPairs_ReservationReport_MissingTable_AddInCorrectly_AddPrefixesToSelectClause_AddPrefixesToWhereClause()
        {
            var fromClause = "ibtrips T1 WITH (nolock), iblegs T2 WITH (nolock)";
            var keyWhereClause = "T1.reckey = T2.reckey and airline != 'ZZ' and ";
            var advancedParams = new List<AdvancedParameter> { new AdvancedParameter { AdvancedFieldName = "numcars" } };
            var selectClause = "T1.reckey, agency, rdepdate";
            var whereClause = "and reckey = -1 and agency = 'DEMO' and (ditcode = 'd') and agency in ('a', 'b', 'c')";
            var buildWhere = new BuildWhere(null);
            buildWhere.ReportGlobals.AdvancedParameters.Parameters = advancedParams;

            var sut = new AdvancedParameterAppender(fromClause, keyWhereClause, selectClause, whereClause);
            sut.AppendMissingTableParamPairs(buildWhere);

            Assert.AreEqual("ibtrips T1 WITH (nolock), iblegs T2 WITH (nolock), ibcar T4", sut.FromClause);
            Assert.AreEqual("T1.reckey = T2.reckey and airline != 'ZZ' and  T1.reckey = T4.reckey and ", sut.KeyWhereClause);
            Assert.AreEqual("T1.reckey, T1.agency, T2.rdepdate", sut.SelectClause);
            Assert.AreEqual("and T1.reckey = -1 and T1.agency = 'DEMO' and (T2.ditcode = 'd') and T1.agency in ('a', 'b', 'c')", sut.WhereClause);
        }

        [TestMethod]
        public void AppendMissingTableParamPairs_BackOfficeReport_MissingTable_AddInCorrectly_AddPrefixesToSelectClause()
        {
            var fromClause = "hibtrips T1 WITH (nolock), hiblegs T2 WITH (nolock)";
            var keyWhereClause = "";
            var advancedParams = new List<AdvancedParameter> { new AdvancedParameter { AdvancedFieldName = "numcars" } };
            var selectClause = "T1.reckey, agency, rdepdate";
            var buildWhere = new BuildWhere(null);
            buildWhere.ReportGlobals.AdvancedParameters.Parameters = advancedParams;

            var sut = new AdvancedParameterAppender(fromClause, keyWhereClause, selectClause, "");
            sut.AppendMissingTableParamPairs(buildWhere);

            Assert.AreEqual("hibtrips T1 WITH (nolock), hiblegs T2 WITH (nolock), hibcars T4", sut.FromClause);
            Assert.AreEqual(" T1.reckey = T4.reckey and ", sut.KeyWhereClause);
            Assert.AreEqual("T1.reckey, T1.agency, T2.rdepdate", sut.SelectClause);
        }

        [TestMethod]
        public void AppendMissingTableParamPairs_BackOfficeReport_MissingTable_EmptyKeyWhereClause_AddInCorrectly()
        {
            var fromClause = "hibtrips T1 WITH (nolock)";
            var keyWhereClause = "";
            var advancedParams = new List<AdvancedParameter>
            {
                new AdvancedParameter { AdvancedFieldName = "origin" },
                new AdvancedParameter { AdvancedFieldName = "destinat" }
            };
            var buildWhere = new BuildWhere(null);
            buildWhere.ReportGlobals.AdvancedParameters.Parameters = advancedParams;

            var sut = new AdvancedParameterAppender(fromClause, keyWhereClause, "", "");
            sut.AppendMissingTableParamPairs(buildWhere);

            Assert.AreEqual("hibtrips T1 WITH (nolock), hiblegs T2", sut.FromClause);
            Assert.AreEqual(" T1.reckey = T2.reckey and ", sut.KeyWhereClause);
        }

        [TestMethod]
        public void AppendMissingTableParamPairs_BackOfficeReport_MissingTable_EmptyKeyWhereClause_NoPrefix_AddInCorrectly()
        {
            var fromClause = "hibtrips WITH (nolock)";
            var keyWhereClause = "";
            var advancedParams = new List<AdvancedParameter>
                                     {
                                         new AdvancedParameter { AdvancedFieldName = "origin" },
                                         new AdvancedParameter { AdvancedFieldName = "destinat" }
                                     };
            var selectClause = "reckey, agency";
            var buildWhere = new BuildWhere(null);
            buildWhere.ReportGlobals.AdvancedParameters.Parameters = advancedParams;

            var sut = new AdvancedParameterAppender(fromClause, keyWhereClause, selectClause, "");
            sut.AppendMissingTableParamPairs(buildWhere);

            Assert.AreEqual("hibtrips T1 WITH (nolock), hiblegs T2", sut.FromClause);
            Assert.AreEqual(" T1.reckey = T2.reckey and ", sut.KeyWhereClause);
            Assert.AreEqual("T1.reckey, T1.agency", sut.SelectClause);
        }

        [TestMethod]
        public void AppendMissingTableParamPairs_UseTableRef_ReservationReport_MissingTable_AddInCorrectly_AddPrefixesToSelectClause_AddPrefixesToWhereClause()
        {
            var fromClause = "ibtrips T1 WITH (nolock), iblegs T2 WITH (nolock)";
            var keyWhereClause = "T1.reckey = T2.reckey and airline != 'ZZ' and ";
            var advancedParams = new List<AdvancedParameter> { new AdvancedParameter { AdvancedFieldName = "numcars" } };
            var selectClause = "T1.reckey, agency, rdepdate";
            var whereClause = "and reckey = -1 and agency = 'DEMO' and (ditcode = 'd') and agency in ('a', 'b', 'c')";
            var buildWhere = new BuildWhere(null);
            buildWhere.AdvancedParameterQueryTableRefList = new List<AvancedParameterQueryTableRef>
            {
                new AvancedParameterQueryTableRef {TableName = "CAR", AdvancedQuerySnip ="ditcode = 'd'"},
                new AvancedParameterQueryTableRef {TableName = "TRIPS", AdvancedQuerySnip ="agency in ('a', 'b', 'c')"}
            };
            buildWhere.ReportGlobals.AdvancedParameters.Parameters = advancedParams;

            var sut = new AdvancedParameterAppender(fromClause, keyWhereClause, selectClause, whereClause);
            sut.AppendMissingTableParamPairs(buildWhere);

            Assert.AreEqual("ibtrips T1 WITH (nolock), iblegs T2 WITH (nolock), ibcar T4", sut.FromClause, true, CultureInfo.CurrentCulture);
            Assert.AreEqual("T1.reckey = T2.reckey and airline != 'ZZ' and  T1.reckey = T4.reckey and ", sut.KeyWhereClause, true, CultureInfo.CurrentCulture);
            Assert.AreEqual("T1.reckey, T1.agency, T2.rdepdate", sut.SelectClause, true, CultureInfo.CurrentCulture);
            Assert.AreEqual("and T1.reckey = -1 and T1.agency = 'DEMO' and (T2.ditcode = 'd') and T1.agency in ('a', 'b', 'c')", sut.WhereClause, true, CultureInfo.CurrentCulture);
        }

        [TestMethod]
        public void AppendMissingTableParamPairs_UseTableRef_BackOfficeReport_MissingTable_AddInCorrectly_AddPrefixesToSelectClause()
        {
            var fromClause = "hibtrips T1 WITH (nolock), hiblegs T2 WITH (nolock)";
            var keyWhereClause = "";
            var advancedParams = new List<AdvancedParameter> { new AdvancedParameter { AdvancedFieldName = "numcars" } };
            var selectClause = "T1.reckey, agency, rdepdate";
            var buildWhere = new BuildWhere(null);
            buildWhere.AdvancedParameterQueryTableRefList = new List<AvancedParameterQueryTableRef>
            {
                new AvancedParameterQueryTableRef {TableName = "CAR", AdvancedQuerySnip ="numcars = 2"},
            };
            buildWhere.ReportGlobals.AdvancedParameters.Parameters = advancedParams;

            var sut = new AdvancedParameterAppender(fromClause, keyWhereClause, selectClause, "");
            sut.AppendMissingTableParamPairs(buildWhere);

            Assert.AreEqual("hibtrips T1 WITH (nolock), hiblegs T2 WITH (nolock), hibcars T4", sut.FromClause, true, CultureInfo.CurrentCulture);
            Assert.AreEqual(" T1.reckey = T4.reckey and ", sut.KeyWhereClause, true, CultureInfo.CurrentCulture);
            Assert.AreEqual("T1.reckey, T1.agency, T2.rdepdate", sut.SelectClause, true, CultureInfo.CurrentCulture);
        }

        [TestMethod]
        public void AppendMissingTableParamPairs_UseTableRef_BackOfficeReport_MissingTable_EmptyKeyWhereClause_AddInCorrectly()
        {
            var fromClause = "hibtrips T1 WITH (nolock)";
            var keyWhereClause = "";
            var advancedParams = new List<AdvancedParameter>
            {
                new AdvancedParameter { AdvancedFieldName = "origin" },
                new AdvancedParameter { AdvancedFieldName = "destinat" }
            };
            var buildWhere = new BuildWhere(null);
            buildWhere.AdvancedParameterQueryTableRefList = new List<AvancedParameterQueryTableRef>
            {
                new AvancedParameterQueryTableRef {TableName = "LEGS", AdvancedQuerySnip ="origin = 'xxx'"},
                new AvancedParameterQueryTableRef {TableName = "LEGS", AdvancedQuerySnip ="destinat = 'yyy'"}
            };
            buildWhere.ReportGlobals.AdvancedParameters.Parameters = advancedParams;

            var sut = new AdvancedParameterAppender(fromClause, keyWhereClause, "", "");
            sut.AppendMissingTableParamPairs(buildWhere);

            Assert.AreEqual("hibtrips T1 WITH (nolock), hiblegs T2", sut.FromClause, true, CultureInfo.CurrentCulture);
            Assert.AreEqual(" T1.reckey = T2.reckey and ", sut.KeyWhereClause, true, CultureInfo.CurrentCulture);
        }

        [TestMethod]
        public void AppendMissingTableParamPairs_UseTableRef_BackOfficeReport_MissingTable_EmptyKeyWhereClause_NoPrefix_AddInCorrectly()
        {
            var fromClause = "hibtrips WITH (nolock)";
            var keyWhereClause = "";
            var advancedParams = new List<AdvancedParameter>
                                     {
                                         new AdvancedParameter { AdvancedFieldName = "origin" },
                                         new AdvancedParameter { AdvancedFieldName = "destinat" }
                                     };
            var selectClause = "reckey, agency";
            var buildWhere = new BuildWhere(null);
            buildWhere.ReportGlobals.AdvancedParameters.Parameters = advancedParams;

            var sut = new AdvancedParameterAppender(fromClause, keyWhereClause, selectClause, "");
            sut.AppendMissingTableParamPairs(buildWhere);

            Assert.AreEqual("hibtrips T1 WITH (nolock), hiblegs T2", sut.FromClause, true, CultureInfo.CurrentCulture);
            Assert.AreEqual(" T1.reckey = T2.reckey and ", sut.KeyWhereClause, true, CultureInfo.CurrentCulture);
            Assert.AreEqual("T1.reckey, T1.agency", sut.SelectClause, true, CultureInfo.CurrentCulture);
        }


        [TestMethod]
        public void AppendMissingTableParamPairs_UseTableRef_ReservationReport_AddPrefixesToWhereClause()
        {
            var fromClause = "ibtrips T1 WITH (nolock), iblegs T2 WITH (nolock)";
            var keyWhereClause = "T1.reckey = T2.reckey and airline != 'ZZ' and ";
            var advancedParams = new List<AdvancedParameter> { new AdvancedParameter { AdvancedFieldName = "HOTCOST", Operator = Operator.GreaterThan, Type = "CURRENCY", FieldName = "HOTCOST", Value1 = "10" } };
            var selectClause = "T1.reckey, agency, rdepdate";
            var whereClause = "T1.reckey = T2.reckey and airline != 'ZZ' and agency = 'DEMO'";
            var buildWhere = new BuildWhere(null);
            buildWhere.AdvancedParameterQueryTableRefList = new List<AvancedParameterQueryTableRef>
            {
                new AvancedParameterQueryTableRef {TableName = "CAR", AdvancedQuerySnip ="bookrate*nights*rooms"}
            };
            buildWhere.ReportGlobals.AdvancedParameters.Parameters = advancedParams;

            var sut = new AdvancedParameterAppender(fromClause, keyWhereClause, selectClause, whereClause);
            sut.AppendMissingTableParamPairs(buildWhere);

            Assert.AreEqual("ibtrips T1 WITH (nolock), iblegs T2 WITH (nolock), ibcar T4", sut.FromClause, true, CultureInfo.CurrentCulture);
            Assert.AreEqual("T1.reckey = T2.reckey and airline != 'ZZ' and  T1.reckey = T4.reckey and ", sut.KeyWhereClause, true, CultureInfo.CurrentCulture);
            Assert.AreEqual("T1.reckey, T1.agency, T2.rdepdate", sut.SelectClause, true, CultureInfo.CurrentCulture);
            Assert.AreEqual("T1.reckey = T2.reckey and T2.airline != 'ZZ' and T1.agency = 'DEMO'", sut.WhereClause, true, CultureInfo.CurrentCulture);
        }

        [TestMethod]
        public void AppendMissingTableParamPairs_UseTableRef_BackOfficeReport_MissingTable_UseHibservices_MatchAllPrefix()
        {
            var fromClause = "hibtrips T1 WITH (nolock), hiblegs T2 WITH (nolock)";
            var keyWhereClause = "";
            var advancedParams = new List<AdvancedParameter> { new AdvancedParameter { AdvancedFieldName = "scardnum" } };
            var selectClause = "T1.reckey, agency, rdepdate";
            var buildWhere = new BuildWhere(null);
            buildWhere.AdvancedParameterQueryTableRefList = new List<AvancedParameterQueryTableRef>
            {
                new AvancedParameterQueryTableRef {TableName = "SVCFEE", AdvancedQuerySnip ="sfcardnum like '%1008%'"},
            };
            buildWhere.ReportGlobals.AdvancedParameters.Parameters = advancedParams;
            buildWhere.ReportGlobals.UseHibServices = true;

            var sut = new AdvancedParameterAppender(fromClause, keyWhereClause, selectClause, "");
            sut.AppendMissingTableParamPairs(buildWhere);

            Assert.AreEqual("hibtrips T1 WITH (nolock), hiblegs T2 WITH (nolock), hibservices T6A", sut.FromClause, true, CultureInfo.CurrentCulture);
            Assert.AreEqual(" T1.reckey = T6A.reckey and ", sut.KeyWhereClause, true, CultureInfo.CurrentCulture);
            Assert.AreEqual("T1.reckey, T1.agency, T2.rdepdate", sut.SelectClause, true, CultureInfo.CurrentCulture);
        }

        [TestMethod]
        public void AppendMissingTableParamPairs_UseTableRef_BackOfficeReport_MissingTable_NotUseHibservices_MatchAllPrefix()
        {
            var fromClause = "hibtrips T1 WITH (nolock), hiblegs T2 WITH (nolock)";
            var keyWhereClause = "";
            var advancedParams = new List<AdvancedParameter> { new AdvancedParameter { AdvancedFieldName = "scardnum" } };
            var selectClause = "T1.reckey, agency, rdepdate";
            var buildWhere = new BuildWhere(null);
            buildWhere.AdvancedParameterQueryTableRefList = new List<AvancedParameterQueryTableRef>
            {
                new AvancedParameterQueryTableRef {TableName = "SVCFEE", AdvancedQuerySnip ="sfcardnum like '%1008%'"},
            };
            buildWhere.ReportGlobals.AdvancedParameters.Parameters = advancedParams;
            buildWhere.ReportGlobals.UseHibServices = false;

            var sut = new AdvancedParameterAppender(fromClause, keyWhereClause, selectClause, "");
            sut.AppendMissingTableParamPairs(buildWhere);

            Assert.AreEqual("hibtrips T1 WITH (nolock), hiblegs T2 WITH (nolock), hibsvcfees T6A", sut.FromClause, true, CultureInfo.CurrentCulture);
            Assert.AreEqual(" T1.recloc = T6A.recloc and ", sut.KeyWhereClause, true, CultureInfo.CurrentCulture);
            Assert.AreEqual("T1.reckey, T1.agency, T2.rdepdate", sut.SelectClause, true, CultureInfo.CurrentCulture);
        }


        [TestMethod]
        public void AppendMissingTableParamPairs_UseTableRef_BackOfficeReport_MissingTable_AddFieldInTrips_NoChangeOnFromClause()
        {
            var fromClause = "hibtrips T1 WITH (nolock), hiblegs T2 WITH (nolock)";
            var keyWhereClause = "";
            var advancedParams = new List<AdvancedParameter> { new AdvancedParameter { AdvancedFieldName = "cardnum" } };
            var selectClause = "";
            var buildWhere = new BuildWhere(null);
            buildWhere.AdvancedParameterQueryTableRefList = new List<AvancedParameterQueryTableRef>
            {
                new AvancedParameterQueryTableRef {TableName = "TRIPS", AdvancedQuerySnip ="cardnum like '%1008%'"},
            };
            buildWhere.ReportGlobals.AdvancedParameters.Parameters = advancedParams;

            var sut = new AdvancedParameterAppender(fromClause, keyWhereClause, selectClause, "");
            sut.AppendMissingTableParamPairs(buildWhere);

            Assert.AreEqual(fromClause, sut.FromClause, true, CultureInfo.CurrentCulture);
        }

        [TestMethod]
        public void AppendMissingTableParamPairs_NoneUseTableRef_BackOfficeReport_MissingTable_AddFieldInTrips_NoChangeOnFromClause()
        {
            var fromClause = "hibtrips T1 WITH (nolock), hiblegs T2 WITH (nolock)";
            var keyWhereClause = "";
            var advancedParams = new List<AdvancedParameter> { new AdvancedParameter { AdvancedFieldName = "cardnum" } };
            var selectClause = "";
            var buildWhere = new BuildWhere(null);
            buildWhere.ReportGlobals.AdvancedParameters.Parameters = advancedParams;

            var sut = new AdvancedParameterAppender(fromClause, keyWhereClause, selectClause, "");
            sut.AppendMissingTableParamPairs(buildWhere);

            Assert.AreEqual(fromClause, sut.FromClause, true, CultureInfo.CurrentCulture);
        }
    }
}
