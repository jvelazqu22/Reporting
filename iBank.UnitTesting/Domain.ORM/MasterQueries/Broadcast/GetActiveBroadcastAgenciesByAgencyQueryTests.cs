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
    public class GetActiveBroadcastAgenciesByAgencyQueryTests
    {
        [TestMethod]
        public void Success_SingleAgencyNameMatch()
        {
            var data = new List<mstragcy>
            {
                new mstragcy
                    {
                        bcactive = true,
                        active = true,
                        agency = "MockAgency",
                        databasename = "Db",
                        tzoffset = -1
                    }
            }.AsQueryable();

            var mockSet = new Mock<IQueryable<mstragcy>>();
            mockSet.SetupIQueryable(data);

            var mockDb = new Mock<IMastersQueryable>();
            mockDb.Setup(x => x.MstrAgcy).Returns(mockSet.Object);

            var query = new GetActiveBroadcastAgenciesByAgencyQuery(mockDb.Object, "MockAgency");
            var result = query.ExecuteQuery();

            Assert.AreEqual(true, result[0].Active);
            Assert.AreEqual("MOCKAGENCY", result[0].Agency);
            Assert.AreEqual("db", result[0].DatabaseName);
            Assert.AreEqual(-1, result[0].TimeZoneOffset);
        }

        [TestMethod]
        public void Success_MultipleAgencyMatch()
        {
            var data = new List<mstragcy>
            {
                new mstragcy
                    {
                        bcactive = true,
                        active = true,
                        agency = "MockAgency",
                        databasename = "Db",
                        tzoffset = -1
                    },
                new mstragcy
                    {
                        bcactive = true,
                        active = true,
                        agency = "MockAgency",
                        databasename = "db2",
                        tzoffset = 1
                    },
                new mstragcy
                    {
                        bcactive = true,
                        active = true,
                        agency = "NotMockAgency",
                        databasename = "db3",
                        tzoffset = 2
                    }
            }.AsQueryable();

            var mockSet = new Mock<IQueryable<mstragcy>>();
            mockSet.SetupIQueryable(data);

            var mockDb = new Mock<IMastersQueryable>();
            mockDb.Setup(x => x.MstrAgcy).Returns(mockSet.Object);

            var query = new GetActiveBroadcastAgenciesByAgencyQuery(mockDb.Object, "MockAgency");
            var result = query.ExecuteQuery();

            Assert.AreEqual(2, result.Count);

            //first agency
            Assert.AreEqual(true, result[0].Active);
            Assert.AreEqual("MOCKAGENCY", result[0].Agency);
            Assert.AreEqual("db", result[0].DatabaseName);
            Assert.AreEqual(-1, result[0].TimeZoneOffset);

            //second agency
            Assert.AreEqual(true, result[1].Active);
            Assert.AreEqual("MOCKAGENCY", result[1].Agency);
            Assert.AreEqual("db2", result[1].DatabaseName);
            Assert.AreEqual(1, result[1].TimeZoneOffset);
        }

        [TestMethod]
        public void Fail_AgencyNotBroadcastActive()
        {
            var data = new List<mstragcy>
            {
                new mstragcy
                    {
                        bcactive = false,
                        active = true,
                        agency = "MockAgency",
                        databasename = "Db",
                        tzoffset = -1
                    }
            }.AsQueryable();

            var mockSet = new Mock<IQueryable<mstragcy>>();
            mockSet.SetupIQueryable(data);

            var mockDb = new Mock<IMastersQueryable>();
            mockDb.Setup(x => x.MstrAgcy).Returns(mockSet.Object);

            var query = new GetActiveBroadcastAgenciesByAgencyQuery(mockDb.Object, "MockAgency");
            var result = query.ExecuteQuery();

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Fail_AgencyNameMismatch()
        {
            var data = new List<mstragcy>
            {
                new mstragcy
                    {
                        bcactive = true,
                        active = true,
                        agency = "NotMockAgency",
                        databasename = "Db",
                        tzoffset = -1
                    }
            }.AsQueryable();

            var mockSet = new Mock<IQueryable<mstragcy>>();
            mockSet.SetupIQueryable(data);

            var mockDb = new Mock<IMastersQueryable>();
            mockDb.Setup(x => x.MstrAgcy).Returns(mockSet.Object);

            var query = new GetActiveBroadcastAgenciesByAgencyQuery(mockDb.Object, "MockAgency");
            var result = query.ExecuteQuery();

            Assert.AreEqual(0, result.Count);
        }

    }
}
