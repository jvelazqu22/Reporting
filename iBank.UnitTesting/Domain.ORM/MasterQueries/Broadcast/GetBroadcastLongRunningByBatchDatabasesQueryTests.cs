using System.Collections.Generic;
using Domain.Orm.Classes;
using Domain.Orm.iBankMastersQueries.BroadcastQueries;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.UnitTesting.TestingHelpers.MockHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace iBank.UnitTesting.Domain.ORM.MasterQueries.Broadcast
{
    [TestClass]
    public class GetBroadcastLongRunningByBatchDatabasesQueryTests
    {

        private bcstque4 runningBcstque4Foo = new bcstque4
        {
            batchname = "b1",
            batchnum = 11,
            agency = "Foo",
            svrstatus = "RUNNING",
            svrnumber = 2,
        };

        private bcstque4 runningBcstque4Boo = new bcstque4
        {
            batchname = "b12",
            batchnum = 12,
            agency = "Boo",
            svrstatus = "RUNNING",
            svrnumber = 2,
        };
        private bcstque4 pendingBcstque4Foo = new bcstque4
        {
            batchname = "b13",
            batchnum = 13,
            agency = "Foo",
            svrstatus = "PENDING",
            svrnumber = 0,
            dbname = "Foo"
        };

        private bcstque4 pendingBcstque4Boo = new bcstque4
        {
            batchname = "14",
            batchnum = 14,
            agency = "Boo",
            svrstatus = "PENDING",
            svrnumber = 0
        };

        private bcstque4 pendingBcstque4BooDooShare = new bcstque4
        {
            batchname = "b15",
            batchnum = 15,
            agency = "BooDooShare",
            svrstatus = "PENDING",
            svrnumber = 0,
            dbname = "BooDooShare"
        };

        private bcstque4 runningBcstque4BooDooShare = new bcstque4
        {
            batchname = "b16",
            batchnum = 16,
            agency = "BooDooShare",
            svrstatus = "RUNNING",
            svrnumber = 2
        };

        private List<mstragcy> mstragcy = new List<mstragcy>
            {
                new mstragcy
                    {
                        bcactive = true,
                        active = true,
                        agency = "Foo",
                        databasename = "Foo",
                        tzoffset = -1
                    },
                new mstragcy
                    {
                        bcactive = true,
                        active = true,
                        agency = "Boo",
                        databasename = "Boo",
                        tzoffset = -1
                    },
                new mstragcy
                    {
                        bcactive = true,
                        active = true,
                        agency = "Doo",
                        databasename = "Doo",
                        tzoffset = -1
                    }
            };

        private List<JunctionAgcyCorp> junctionAgcyCorp = new List<JunctionAgcyCorp>
            {
                new JunctionAgcyCorp
                {
                    CorpAcct = "BooDooShare",
                    agency = "Boo"
                },
                new JunctionAgcyCorp
                {
                    CorpAcct = "BooDooShare",
                    agency = "Doo"
                }
            };

        private Mock<IMastersQueryable> InitializeRegular()
        {
            var mockDb = new Mock<IMastersQueryable>();

            var bcstque4 = new List<bcstque4>
                           {
                                runningBcstque4Foo,
                                pendingBcstque4Foo,
                                runningBcstque4Boo,
                                pendingBcstque4Boo,
                                runningBcstque4BooDooShare,
                                pendingBcstque4BooDooShare
                           };

            var batch =

            mockDb.Setup(x => x.BcstQue4).Returns(MockHelper.GetListAsQueryable(bcstque4).Object);
            mockDb.Setup(x => x.MstrAgcy).Returns(MockHelper.GetListAsQueryable(mstragcy).Object);
            mockDb.Setup(x => x.JunctionAgcyCorp).Returns(MockHelper.GetListAsQueryable(junctionAgcyCorp).Object);

            return mockDb;
        }
        private Mock<IMastersQueryable> InitializeShare()
        {
            var mockDb = new Mock<IMastersQueryable>();

            var bcstque4 = new List<bcstque4>
                           {
                                pendingBcstque4BooDooShare,
                                runningBcstque4BooDooShare,
                                pendingBcstque4Foo,
                                runningBcstque4Foo,
                                pendingBcstque4Boo,
                                runningBcstque4Boo
                           };

            var batch =

            mockDb.Setup(x => x.BcstQue4).Returns(MockHelper.GetListAsQueryable(bcstque4).Object);
            mockDb.Setup(x => x.MstrAgcy).Returns(MockHelper.GetListAsQueryable(mstragcy).Object);
            mockDb.Setup(x => x.JunctionAgcyCorp).Returns(MockHelper.GetListAsQueryable(junctionAgcyCorp).Object);

            return mockDb;
        }

        [TestMethod]
        public void GetBroadcastLongRunningByBatchDatabasesQuery_PassRegular_ReturnFoo()
        {
            //arrange
            var mockDb = InitializeRegular();
            int batchNum = pendingBcstque4Foo.batchnum ?? 0;

            IList<DatabaseInformation> exp = new List<DatabaseInformation> {
                new DatabaseInformation { DatabaseName = "Foo" }
            };

            //act
            var sut = new GetBroadcastLongRunningByBatchDatabasesQuery(mockDb.Object, batchNum).ExecuteQuery();
            
            //Assert

            for (var i = 0; i < exp.Count; i++)
            {
                Assert.AreEqual(exp[i].DatabaseName.ToLower(), sut[i].DatabaseName.ToLower());
            }
            Assert.AreEqual(1, sut.Count);

        }

        [TestMethod]
        public void GetBroadcastLongRunningByBatchDatabasesQuery_PassShare_ReturnBooDoo()
        {
            //arrange
            var mockDb = InitializeShare();
            int batchNum = pendingBcstque4BooDooShare.batchnum ?? 0;

            IList<DatabaseInformation> exp = new List<DatabaseInformation> {
                new DatabaseInformation { DatabaseName = "Boo" },
                new DatabaseInformation { DatabaseName = "Doo" }
            };

            //act
            var sut = new GetBroadcastLongRunningByBatchDatabasesQuery(mockDb.Object, batchNum).ExecuteQuery();

            //Assert

            for (var i = 0; i < exp.Count; i++)
            {
                Assert.AreEqual(exp[i].DatabaseName.ToLower(), sut[i].DatabaseName.ToLower());
            }
            Assert.AreEqual(2, sut.Count);

        }
    }
}
