using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Orm.iBankMastersQueries.ReportServer;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.UnitTesting.TestingHelpers.MockHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace iBank.UnitTesting.Domain.ORM.MasterQueries.ReportServer
{
    [TestClass]
    public class GetPendingReportsQueryTests
    {
        [TestMethod]
        public void PendingReportExists_ReturnPendingRecord()
        {
            var pendingDataSet = new List<PendingReports>
            {
                //dotnet and pending
                new PendingReports
                {
                    ReportId = "123",
                    Agency = "foo",
                    IsDotNet = true,
                    IsRunning = false,
                    RowVersion = new byte[] {0x20},
                    UserNumber = 123
                },
                //dotnet and already running
                new PendingReports
                {
                    ReportId = "456",
                    Agency = "bar",
                    IsDotNet = true,
                    IsRunning = true,
                    RowVersion = new byte[] {0x40},
                    UserNumber = 456
                },
                //not dotnet
                new PendingReports
                {
                    ReportId = "789",
                    Agency = "foobar",
                    IsDotNet = false,
                    IsRunning = false,
                    RowVersion = new byte[] {0x60},
                    UserNumber = 789
                }
            };

            var handoffDataSet = new List<reporthandoff>
            {
                new reporthandoff
                {
                    reportid = "123",
                    agency = "foo",
                    cfbox = "a",
                    usernumber = 123
                },
                new reporthandoff
                {
                    reportid = "456",
                    agency = "bar",
                    cfbox = "a",
                    usernumber = 456
                },
                new reporthandoff
                {
                    reportid = "789",
                    agency = "foobar",
                    cfbox = "a",
                    usernumber = 789
                }
            };

            var db = new Mock<IMastersQueryable>();
            db.Setup(x => x.PendingReports).Returns(MockHelper.GetListAsQueryable(pendingDataSet).Object);
            db.Setup(x => x.ReportHandoff).Returns(MockHelper.GetListAsQueryable(handoffDataSet).Object);

            var demoUsers = new List<int> {123};
            var serverNumber = 1;
            var query = new GetPendingReportsQuery(db.Object, demoUsers, serverNumber);

            var output = query.ExecuteQuery();

            Assert.AreEqual(true, output.Count == 1);

            var val = output.FirstOrDefault(x => x.ReportId.Equals("123"));
            Assert.IsTrue(val != null);
            Assert.AreEqual("FOO", val.Agency);
            Assert.AreEqual(123, val.UserNumber);
            Assert.AreEqual(true, val.IsDemoUser);
            Assert.AreEqual(true, val.IsDotNet);
            CollectionAssert.AreEqual(new byte[] {0x20}, val.RowVersion);
            Assert.AreEqual(serverNumber, val.ServerNumber);
        }

        [TestMethod]
        public void MultipleRecordsInReportHandoff_ApplyDistinct()
        {
            var pendingDataSet = new List<PendingReports>
            {
                new PendingReports
                {
                    ReportId = "123",
                    Agency = "foo",
                    IsDotNet = true,
                    IsRunning = false,
                    RowVersion = new byte[] {0x20},
                    UserNumber = 123
                }
            };

            var handoffDataSet = new List<reporthandoff>
            {
                new reporthandoff
                {
                    reportid = "123",
                    agency = "foo",
                    cfbox = "a",
                    usernumber = 123
                },
                new reporthandoff
                {
                    reportid = "123",
                    agency = "foo",
                    cfbox = "a",
                    usernumber = 123
                }
            };

            var db = new Mock<IMastersQueryable>();
            db.Setup(x => x.PendingReports).Returns(MockHelper.GetListAsQueryable(pendingDataSet).Object);
            db.Setup(x => x.ReportHandoff).Returns(MockHelper.GetListAsQueryable(handoffDataSet).Object);

            var demoUsers = new List<int> { 123 };
            var serverNumber = 1;
            var query = new GetPendingReportsQuery(db.Object, demoUsers, serverNumber);

            var output = query.ExecuteQuery();

            Assert.AreEqual(true, output.Count == 1);
        }
    }
}
