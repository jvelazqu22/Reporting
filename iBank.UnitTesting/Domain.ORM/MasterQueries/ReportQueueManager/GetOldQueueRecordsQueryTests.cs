using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Orm.iBankMastersQueries.ReportQueueManager;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.UnitTesting.TestingHelpers.MockHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace iBank.UnitTesting.Domain.ORM.MasterQueries.ReportQueueManager
{
    [TestClass]
    public class GetOldQueueRecordsQueryTests
    {
        private IMastersQueryable _queryDb;
        [TestInitialize]
        public void Init()
        {
            var db = new Mock<IMastersQueryable>();
            var values = new List<PendingReports>
            {
                new PendingReports { ReportId = "123", TimeStamp = new DateTime(2018, 1, 24) },
                new PendingReports { ReportId = "456", TimeStamp = new DateTime(2018, 1, 15) }
            };
            db.Setup(x => x.PendingReports).Returns(MockHelper.GetListAsQueryable(values).Object);
            _queryDb = db.Object;
        }

        [TestMethod]
        public void OldRecordsExists_ReturnOldRecords()
        {
            var threshold = new DateTime(2018, 1, 20);
            var query = new GetOldQueueRecordsQuery(_queryDb, threshold);

            var output = query.ExecuteQuery();

            Assert.AreEqual(true, output.Count == 1);
            Assert.AreEqual(true, output.Any(x => x.ReportId.Equals("456")));
        }

        [TestMethod]
        public void NoOldRecordsExist_ReturnZeroRecords()
        {
            var threshold = new DateTime(2015, 1, 1);
            var query = new GetOldQueueRecordsQuery(_queryDb, threshold);

            var output = query.ExecuteQuery();

            Assert.AreEqual(true, output.Count == 0);
        }
    }
}
