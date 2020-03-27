using System;
using System.Collections.Generic;

using Domain.Helper;

using iBank.Server.Utilities.ReportCriteriaHandlers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Server.Utilities.ReportCriteriaHandlers
{
    [TestClass]
    public class MultiUdidParameterRetrieverTests
    {
        [TestMethod]
        public void GetMultiUdidParametersFromReportCriteria_NoWildcards()
        {
            var sut = new MultiUdidParameterRetriever();
            var crit = new List<ReportCriteria>
            {
                new ReportCriteria { VarName = "MUD01", VarValue = "10" },
                new ReportCriteria { VarName = "MUDFLD01", VarValue = "MUD:10" },
                new ReportCriteria { VarName = "MUDOPER01", VarValue = "=" },
                new ReportCriteria { VarName = "MUDTEXT01", VarValue = "domestic" },
                new ReportCriteria { VarName = "MUDANDOR", VarValue = "1" }
            };

            var output = sut.GetMultiUdidParametersFromReportCriteria(crit);
            
            Assert.AreEqual(1, output.Count);
            Assert.AreEqual("10", output[0].FieldName);
            Assert.AreEqual("domestic", output[0].Value1);
            Assert.AreEqual(Operator.Equal, output[0].Operator);
        }
    }
}
