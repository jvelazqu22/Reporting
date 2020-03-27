using System.Collections.Generic;
using System.Linq;

using Domain.Orm.iBankMastersQueries.BroadcastQueries;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.UnitTesting.TestingHelpers.MockHelpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace iBank.UnitTesting.Domain.ORM.MasterQueries.Broadcast
{
    [TestClass]
    public class IsBroadcastReportQueryTest
    {
        [TestMethod]
        public void IsBroadcastReportQuery_True()
        {
            var data = new List<reporthandoff>
                           {
                                new reporthandoff
                                {
                                    reportid = "1",
                                    parmname = "DOTNET_BCST",
                                    parmvalue = "YES"
                                }
                           }.AsQueryable();

            var mockSet = new Mock<IQueryable<reporthandoff>>();
            mockSet.SetupIQueryable(data);
            
            var mockDb = new Mock<IMastersQueryable>();
            mockDb.Setup(x => x.ReportHandoff).Returns(mockSet.Object);

            var query = new IsBroadcastReportQuery(mockDb.Object, "1");
            var result = query.ExecuteQuery();

            Assert.AreEqual(true, result);
        }

        [TestMethod]
        public void IsBroadcastReportQuery_False()
        {
            var data = new List<reporthandoff>
                           {
                                new reporthandoff
                                {
                                    reportid = "1",
                                    parmname = "DOTNET_BCST",
                                    parmvalue = "NO"
                                }
                           }.AsQueryable();

            var mockSet = new Mock<IQueryable<reporthandoff>>();
            mockSet.SetupIQueryable(data);

            var mockDb = new Mock<IMastersQueryable>();
            mockDb.Setup(x => x.ReportHandoff).Returns(mockSet.Object);

            var query = new IsBroadcastReportQuery(mockDb.Object, "1");
            var result = query.ExecuteQuery();

            Assert.AreEqual(false, result);
        }
    }
}
