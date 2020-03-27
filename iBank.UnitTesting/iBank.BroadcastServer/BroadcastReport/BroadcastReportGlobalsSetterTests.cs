using Domain.Helper;
using iBank.BroadcastServer.BroadcastReport;
using iBank.Server.Utilities.Classes;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using System.Collections.Generic;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

namespace iBank.UnitTesting.iBank.BroadcastServer.BroadcastReport
{
    [TestClass]
    public class BroadcastReportGlobalsSetterTests
    {
        [TestMethod]
        public void SetAccountValues_IsNotIn()
        {
            var queueRecord = new bcstque4 { acctlist = "[NOT]1188" };
            var globals = new ReportGlobals();
            var clientDb = new Mock<IClientQueryable>();
            var masterDb = new Mock<IMastersQueryable>();
            var sut = new BroadcastReportGlobalsSetter(globals, masterDb.Object, clientDb.Object);

            sut.SetAccountValues(queueRecord);

            Assert.AreEqual(true, globals.IsParmValueOn(WhereCriteria.NOTINACCT));
            Assert.AreEqual("1188", globals.GetParmValue(WhereCriteria.INACCT));
        }

        [TestMethod]
        public void SetAccountValues_IsIn()
        {
            var queueRecord = new bcstque4 { acctlist = "1188" };
            var globals = new ReportGlobals();
            var clientDb = new Mock<IClientQueryable>();
            var masterDb = new Mock<IMastersQueryable>();
            var sut = new BroadcastReportGlobalsSetter(globals, masterDb.Object, clientDb.Object);

            sut.SetAccountValues(queueRecord);

            Assert.AreEqual(false, globals.IsParmValueOn(WhereCriteria.NOTINACCT));
            Assert.AreEqual("", globals.GetParmValue(WhereCriteria.ACCT));
            Assert.AreEqual("1188", globals.GetParmValue(WhereCriteria.INACCT));
        }

        [TestMethod]
        public void RemoveSingleQuotesFromAdvanceDateParameters_DateStringsHaveSingleQuotes_RemoveSingleQuotes()
        {
            // Arrange
            var globals = new ReportGlobals()
            {
                AdvancedParameters = new AdvancedParameters()
                {
                    Parameters = new List<AdvancedParameter>()
                    {
                        new AdvancedParameter() { Type = "DATE", Value1 = "'1/20/2016'", Value2 = "8/21/2016" },
                        new AdvancedParameter() { Type = "DATE", Value1 = "3/20/2017", Value2 = "'7/4/2016'" },
                    }
                }
            };
            var clientDb = new Mock<IClientQueryable>();
            var masterDb = new Mock<IMastersQueryable>();
            var broadcastReportGlobalsSetter = new BroadcastReportGlobalsSetter(globals, masterDb.Object, clientDb.Object);

            // Act
            broadcastReportGlobalsSetter.RemoveSingleQuotesFromAdvanceDateParameters(globals);

            // Assert
            Assert.AreEqual(globals.AdvancedParameters.Parameters[0].Value1, "1/20/2016");
            Assert.AreEqual(globals.AdvancedParameters.Parameters[0].Value2, "8/21/2016");
            Assert.AreEqual(globals.AdvancedParameters.Parameters[1].Value1, "3/20/2017");
            Assert.AreEqual(globals.AdvancedParameters.Parameters[1].Value2, "7/4/2016");
        }
    }
}
