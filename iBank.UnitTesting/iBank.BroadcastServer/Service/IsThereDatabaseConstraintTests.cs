using System;
using System.Collections.Generic;
using Domain.Helper;
using Domain.Services;
using iBank.BroadcastServer.Helper;
using iBank.Entities.AdministrationEntities;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.UnitTesting.TestingHelpers.MockHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace iBank.UnitTesting.iBank.BroadcastServer.Service
{
    [TestClass]
    public class IsThereDatabaseConstraintTests
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

        private List<broadcast_servers> bcstServers = new List<broadcast_servers>
        {
            new broadcast_servers{server_function_id = 6, server_number = 1},
            new broadcast_servers{server_function_id = 6, server_number = 2}
        };

        private Mock<IMastersQueryable> InitializeRegular()
        {
            var mockDb = new Mock<IMastersQueryable>();

            var bcstque4 = new List<bcstque4>
                           {
                                runningBcstque4Foo,
                                runningBcstque4Boo,
                                pendingBcstque4Boo,
                                pendingBcstque4Foo,
                                pendingBcstque4BooDooShare
                           };


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
                                pendingBcstque4Foo,
                                runningBcstque4Foo,
                                pendingBcstque4Boo,
                           };

            var batch =

            mockDb.Setup(x => x.BcstQue4).Returns(MockHelper.GetListAsQueryable(bcstque4).Object);
            mockDb.Setup(x => x.MstrAgcy).Returns(MockHelper.GetListAsQueryable(mstragcy).Object);
            mockDb.Setup(x => x.JunctionAgcyCorp).Returns(MockHelper.GetListAsQueryable(junctionAgcyCorp).Object);

            return mockDb;
        }

        private Mock<IMastersQueryable> InitializeMix()
        {
            var mockDb = new Mock<IMastersQueryable>();

            var bcstque4 = new List<bcstque4>
                           {
                                pendingBcstque4BooDooShare,
                                runningBcstque4Foo,
                                runningBcstque4BooDooShare
                           };

            var batch =

            mockDb.Setup(x => x.BcstQue4).Returns(MockHelper.GetListAsQueryable(bcstque4).Object);
            mockDb.Setup(x => x.MstrAgcy).Returns(MockHelper.GetListAsQueryable(mstragcy).Object);
            mockDb.Setup(x => x.JunctionAgcyCorp).Returns(MockHelper.GetListAsQueryable(junctionAgcyCorp).Object);

            return mockDb;
        }

        [TestMethod]
        public void IsThereDatabaseConstraint_RegularNeedsBooRunningFooBoo_ReturnTrue()
        {
            //arrange
            //assume there are 3 long-running servers, 1, 2, and 3.
            var currentServerNumber = 1; //the other ones are 2, 3

            var otherLongRunningServerCache = new CacheService();
            CacheKeys key = CacheKeys.BroadcastLongRunningServers;
            otherLongRunningServerCache.Set(key, new List<int> { 2, 3 }, DateTime.Now.AddDays(1));

            var mockDb = InitializeRegular();
            var masterStore = new Mock<IMasterDataStore>();
            masterStore.Setup(x => x.MastersQueryDb).Returns(mockDb.Object);

            //act

            var longrunningHandler = new LongRunningHandler();
            var sut = longrunningHandler.IsThereDatabaseConstraint(masterStore.Object, otherLongRunningServerCache, currentServerNumber, pendingBcstque4Boo);

            //assert Constraint
            Assert.AreEqual(true, sut);
        }

        [TestMethod]
        public void IsThereDatabaseConstraint_ShareNeedsBooDooRunningFoo_ReturnFalse()
        {
            //arrange
            //assume there are 3 long-running servers, 1, 2, and 3.
            var currentServerNumber = 1; //the other ones are 2, 3

            var otherLongRunningServerCache = new CacheService();
            CacheKeys key = CacheKeys.BroadcastLongRunningServers;
            otherLongRunningServerCache.Set(key, new List<int> { 2, 3 }, DateTime.Now.AddDays(1));

            var mockDb = InitializeShare();
            var masterStore = new Mock<IMasterDataStore>();
            masterStore.Setup(x => x.MastersQueryDb).Returns(mockDb.Object);

            //act Need Boo and Doo, running Foo
            var longrunningHandler = new LongRunningHandler();
            var sut = longrunningHandler.IsThereDatabaseConstraint(masterStore.Object, otherLongRunningServerCache, currentServerNumber, pendingBcstque4BooDooShare);

            //assert not Constraint
            Assert.AreEqual(false, sut);
        }


        [TestMethod]           
        public void IsThereDatabaseConstraint_ShareNeedsBooDooRunningFooBoo_ReturnTrue()
        {
            //arrange
            //assume there are 3 long-running servers, 1, 2, and 3.
            var currentServerNumber = 1; //the other ones are 2, 3

            var otherLongRunningServerCache = new CacheService();
            CacheKeys key = CacheKeys.BroadcastLongRunningServers;
            otherLongRunningServerCache.Set(key, new List<int> { 2, 3 }, DateTime.Now.AddDays(1));

            var mockDb = InitializeMix();
            var masterStore = new Mock<IMasterDataStore>();
            masterStore.Setup(x => x.MastersQueryDb).Returns(mockDb.Object);

            //act Need Foo, Boo and Doo, running Foo, Boo
            var longrunningHandler = new LongRunningHandler();
            var sut = longrunningHandler.IsThereDatabaseConstraint(masterStore.Object, otherLongRunningServerCache, currentServerNumber, pendingBcstque4BooDooShare);

            //assert Constraint
            Assert.AreEqual(true, sut);
        }
    }
}
