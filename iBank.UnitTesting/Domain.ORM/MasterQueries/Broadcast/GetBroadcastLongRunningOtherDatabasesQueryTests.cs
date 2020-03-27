﻿using System;
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
    public class GetBroadcastLongRunningOtherDatabasesQueryTests
    {

        private bcstque4 runningBcstque4 = new bcstque4
        {
            batchname = "b1",
            batchnum = 11,
            agency = "Foo",
            svrstatus = "RUNNING",
            svrnumber = 2,
        };

        private bcstque4 runningBcstque4Boo = new bcstque4
        {
            batchname = "b11",
            batchnum = 11,
            agency = "Boo",
            svrstatus = "RUNNING",
            svrnumber = 2,
        };

        private bcstque4 pendingBcstque4Foo = new bcstque4
        {
            batchname = "b2",
            batchnum = 12,
            agency = "Foo",
            svrstatus = "PENDING",
            svrnumber = 0
        };

        private bcstque4 pendingBcstque4Boo = new bcstque4
        {
            batchname = "b3",
            batchnum = 13,
            agency = "Boo",
            svrstatus = "PENDING",
            svrnumber = 0
        };

        private bcstque4 pendingBcstque4Share = new bcstque4
        {
            batchname = "b4",
            batchnum = 14,
            agency = "BooDooShare",
            svrstatus = "PENDING",
            svrnumber = 0
        };

        private bcstque4 runningBcstque4BooDooShare = new bcstque4
        {
            batchname = "b14",
            batchnum = 14,
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

        private Mock<IMastersQueryable> InitializeRegularRunning()
        {
            var mockDb = new Mock<IMastersQueryable>();

            var bcstque4 = new List<bcstque4>
                           {
                                runningBcstque4,
                                pendingBcstque4Foo,
                                pendingBcstque4Boo
                           };

            mockDb.Setup(x => x.BcstQue4).Returns(MockHelper.GetListAsQueryable(bcstque4).Object);
            mockDb.Setup(x => x.MstrAgcy).Returns(MockHelper.GetListAsQueryable(mstragcy).Object);
            mockDb.Setup(x => x.JunctionAgcyCorp).Returns(MockHelper.GetListAsQueryable(junctionAgcyCorp).Object);

            return mockDb;
        }

        private Mock<IMastersQueryable> InitializeCorpAcctRunning()
        {
            var mockDb = new Mock<IMastersQueryable>();

            var bcstque4 = new List<bcstque4>
                           {
                                runningBcstque4BooDooShare,
                                pendingBcstque4Foo,
                                pendingBcstque4Boo
                           };

            mockDb.Setup(x => x.BcstQue4).Returns(MockHelper.GetListAsQueryable(bcstque4).Object);
            mockDb.Setup(x => x.MstrAgcy).Returns(MockHelper.GetListAsQueryable(mstragcy).Object);
            mockDb.Setup(x => x.JunctionAgcyCorp).Returns(MockHelper.GetListAsQueryable(junctionAgcyCorp).Object);

            return mockDb;
        }
        private Mock<IMastersQueryable> InitializeMixedRunning()
        {
            var mockDb = new Mock<IMastersQueryable>();

            var bcstque4 = new List<bcstque4>
                           {
                                runningBcstque4Boo,
                                runningBcstque4BooDooShare,
                                pendingBcstque4Foo,
                                pendingBcstque4Boo
                           };

            mockDb.Setup(x => x.BcstQue4).Returns(MockHelper.GetListAsQueryable(bcstque4).Object);
            mockDb.Setup(x => x.MstrAgcy).Returns(MockHelper.GetListAsQueryable(mstragcy).Object);
            mockDb.Setup(x => x.JunctionAgcyCorp).Returns(MockHelper.GetListAsQueryable(junctionAgcyCorp).Object);

            return mockDb;
        }


        [TestMethod]
        public void GetBroadcastLongRunningOtherDatabasesQuery_RegularAgencyFooIsRunning_ReturnOneDatabase()
        {
            //arrange
            var mockDb = InitializeRegularRunning();
            var otherServerList = new List<int> { 2 };

            IList<DatabaseInformation> exp = new List<DatabaseInformation> { new DatabaseInformation { DatabaseName = "Foo" } };
            //act
            var query = new GetBroadcastLongRunningOtherDatabasesQuery(mockDb.Object, otherServerList);
            var sut = query.ExecuteQuery();

            //Assert
            for (var i = 0; i < exp.Count; i++)
            {
                Assert.AreEqual(exp[i].DatabaseName.ToLower(), sut[i].DatabaseName.ToLower());
            }
            Assert.AreEqual(1, sut.Count);
        }

        [TestMethod]
        public void GetBroadcastLongRunningOtherDatabasesQuery_CorpAcctBooDooIsRunning_ReturnTwoDatabasesBooDoo()
        {
            //arrange
            var mockDb = InitializeCorpAcctRunning();
            var otherServerList = new List<int> { 2 };

            IList<DatabaseInformation> exp = new List<DatabaseInformation> {
                new DatabaseInformation { DatabaseName = "Boo" },
                new DatabaseInformation { DatabaseName = "Doo" }
            };
            //act
            var query = new GetBroadcastLongRunningOtherDatabasesQuery(mockDb.Object, otherServerList);
            var sut = query.ExecuteQuery();

            //Assert
            for (var i = 0; i < exp.Count; i++)
            {
                Assert.AreEqual(exp[i].DatabaseName.ToLower(), sut[i].DatabaseName.ToLower());
            }
            Assert.AreEqual(2, sut.Count);
        }

        [TestMethod]
        public void GetBroadcastLongRunningOtherDatabasesQuery_FixedAgencyBooCorpAcctBooDooAreRunning_ReturnDatabasesBooDoo()
        {
            //arrange
            var mockDb = InitializeMixedRunning();
            var otherServerList = new List<int> { 2 };

            IList<DatabaseInformation> exp = new List<DatabaseInformation> {
                new DatabaseInformation { DatabaseName = "Boo" },
                new DatabaseInformation { DatabaseName = "Doo" }
            };
            //act
            var query = new GetBroadcastLongRunningOtherDatabasesQuery(mockDb.Object, otherServerList);
            var sut = query.ExecuteQuery();

            //Assert

            for (var i = 0; i < exp.Count; i++)
            {
                Assert.AreEqual(exp[i].DatabaseName.ToLower(), sut[i].DatabaseName.ToLower());
            }
            Assert.AreEqual(2, sut.Count);
        }
    }
}