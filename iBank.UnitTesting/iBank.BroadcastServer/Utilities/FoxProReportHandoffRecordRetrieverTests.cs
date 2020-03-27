using Domain.Helper;
using iBank.BroadcastServer.Utilities;
using iBank.Server.Utilities.ReportCriteriaHandlers;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

using iBank.Entities.MasterEntities;

namespace iBank.UnitTesting.iBank.BroadcastServer.Utilities
{
    [TestClass]
    public class FoxProReportHandoffRecordRetrieverTests
    {
        private FoxProReportHandoffRecordRetriever _retriever = null;

        [TestInitialize]
        public void Init()
        {
            var handler = new ReportHandoffRecordHandler("foo-1", "EN", "1", 2, "DEMO", DateTime.Now);
            _retriever = new FoxProReportHandoffRecordRetriever(handler);
        }

        [TestMethod]
        public void GetReportParameterRecordsExceptReportPeriodDates()
        {
            var reportParameters = new Dictionary<int, ReportCriteria>
            {
                { 1, new ReportCriteria { VarName = "BEGDATE", VarValue = "DT:2016,1,1" } },
                { 2, new ReportCriteria { VarName = "ENDDATE", VarValue = "DT:2016,2,1"} },
                { 3, new ReportCriteria { VarName = "INCLUDE", VarValue = "foo" } }
            };

            var expected = new reporthandoff
                               {
                                   parmname = "INCLUDE",
                                   parmvalue = "foo"
                               };
            

            var output = _retriever.GetReportParameterRecordsExceptReportPeriodDates(reportParameters).ToList();

            Assert.AreEqual(1, output.Count);
            Assert.AreEqual(expected.parmname, output[0].parmname);
            Assert.AreEqual(expected.parmvalue, output[0].parmvalue);

        }
    }
}
