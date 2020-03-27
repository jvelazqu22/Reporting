using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared
{
    [TestClass]
    public class ReportDelayerTests
    {
        [TestMethod]
        public void DelayReportTest()
        {
            //mock of savedrpt1
            var mockSavedRpt1Queryable = new Mock<IQueryable<savedrpt1>>();

            var mockDb = new Mock<IClientQueryable>();
            mockDb.Setup(x => x.SavedRpt1).Returns(mockSavedRpt1Queryable.Object);

            var mockClientQueryDb = new Mock<IClientQueryable>();
            mockClientQueryDb.Setup(x => x.Clone()).Returns(new Mock<IClientQueryable>());
        }
    }
}
