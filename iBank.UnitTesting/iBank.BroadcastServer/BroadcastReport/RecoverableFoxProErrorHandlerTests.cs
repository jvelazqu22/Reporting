using System;
using System.Collections.Generic;

using iBank.BroadcastServer.BroadcastReport;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.UnitTesting.TestingHelpers.MockHelpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace iBank.UnitTesting.iBank.BroadcastServer.BroadcastReport
{
    [TestClass]
    public class RecoverableFoxProErrorHandlerTests
    {
        [TestMethod]
        public void IsRecoverableError_ContainsRecoverableError()
        {
            var errorMessage = "[An unexpected error has occurred.[NL][NL]Error No 994255]";
            var mockDb = new Mock<IMastersQueryable>();
            var rec = new errorlog { agency = "foo", errormsg = "Bad stuff happened including COULD NOT CONTINUE SCAN WITH NOLOCK", recordno = 994255 };
            var recList = new List<errorlog> { rec };
            mockDb.Setup(x => x.ErrorLog).Returns(MockHelper.GetListAsQueryable(recList).Object);

            var output = RecoverableFoxProErrorHandler.IsRecoverableError(errorMessage, mockDb.Object);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsRecoverableError_ContainsBadError()
        {
            var errorMessage = "An unexpected error has occurred. Error Number 1234";
            var mockDb = new Mock<IMastersQueryable>();
            var rec = new errorlog { agency = "foo", errormsg = "Bad stuff happened including some stuff we just can't get over", errornbr = 1234 };
            var recList = new List<errorlog> { rec };
            mockDb.Setup(x => x.ErrorLog).Returns(MockHelper.GetListAsQueryable(recList).Object);

            var output = RecoverableFoxProErrorHandler.IsRecoverableError(errorMessage, mockDb.Object);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void IsRecoverableError_NullErrorNumber()
        {
            var errorMessage = "An unexpected error has occurred. Error Number 1234";
            var mockDb = new Mock<IMastersQueryable>();
            var rec = new errorlog { agency = "foo", errormsg = "Bad stuff happened including COULD NOT CONTINUE SCAN WITH NOLOCK", errornbr = 789 };
            var recList = new List<errorlog> { rec };
            mockDb.Setup(x => x.ErrorLog).Returns(MockHelper.GetListAsQueryable(recList).Object);

            var output = RecoverableFoxProErrorHandler.IsRecoverableError(errorMessage, mockDb.Object);

            Assert.AreEqual(false, output);
        }
    }
}
