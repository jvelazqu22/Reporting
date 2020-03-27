using iBank.OverdueBroadcastMonitor;
using iBank.UnitTesting.TestingHelpers.MockHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

using Domain.Helper;

using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

using mstragcy = iBank.Entities.MasterEntities.mstragcy;

namespace iBank.UnitTesting.iBank.OverdueBroadcastMonitor
{
    [TestClass]
    public class NotificationSentFilterTests
    {
        [TestMethod]
        public void FilterOutNotificationSentBroadcasts_NoneExist_ReturnOriginalList()
        {
            var overdueBcsts = new List<ibbatch>
            {
                new ibbatch
                    {
                        agency = "FOO"
                    },
                new ibbatch
                    {
                        agency = "BAR"
                    }
            };
            var notificationSent = new List<overdue_broadcasts>();
            var mockDb = new Mock<IMastersQueryable>();
            mockDb.Setup(x => x.OverdueBroadcasts).Returns(MockHelper.GetListAsQueryable(notificationSent).Object);
            var store = new Mock<IMasterDataStore>();
            store.Setup(x => x.MastersQueryDb).Returns(mockDb.Object);
            var sut = new BroadcastFilter();

            var output = sut.FilterOutPreviouslyCollectedBroadcasts(overdueBcsts, store.Object);

            Assert.AreEqual(2, output.Count);
        }

        [TestMethod]
        public void FilterOutNotificationSentBroadcasts_OneNotifcationExists_ReturnFilteredList()
        {
            var existingDate = new DateTime(2016, 1, 1);
            var overdueBcsts = new List<ibbatch>
            {
                new ibbatch
                    {
                        agency = "FOO",
                        batchname = "FOO_NAME",
                        batchnum = 1,
                        nextrun = existingDate,
                        UserNumber = 1
                    },
                new ibbatch
                    {
                        agency = "BAR",
                        batchname = "BAR_NAME",
                        batchnum = 2,
                        nextrun = new DateTime(2011, 1, 1),
                        UserNumber = 2
                    }
            };
            var notificationSent = new List<overdue_broadcasts>
                                       {
                                           new overdue_broadcasts
                                               {
                                                   agency = "FOO",
                                                   batchname = "FOO_NAME",
                                                   batchnum = 1,
                                                   nextrun = existingDate,
                                                   UserNumber = 1,
                                                   notification_sent = true
                                               }
                                       };
            var mockDb = new Mock<IMastersQueryable>();
            mockDb.Setup(x => x.OverdueBroadcasts).Returns(MockHelper.GetListAsQueryable(notificationSent).Object);
            var store = new Mock<IMasterDataStore>();
            store.Setup(x => x.MastersQueryDb).Returns(mockDb.Object);
            var sut = new BroadcastFilter();

            var output = sut.FilterOutPreviouslyCollectedBroadcasts(overdueBcsts, store.Object);

            Assert.AreEqual(1, output.Count);
            Assert.AreEqual("BAR", output[0].agency);
        }

        [TestMethod]
        public void FilterOutOfflineBroadcasts()
        {
            var overdueBcsts = new List<ibbatch>
            {
                new ibbatch
                    {
                        agency = "FOO",
                        batchname = BroadcastCriteria.OfflineRecord + "FOO_NAME",
                        batchnum = 1,
                        nextrun = new DateTime(2012,1,1),
                        UserNumber = 1
                    },
                new ibbatch
                    {
                        agency = "BAR",
                        batchname = "BAR_NAME",
                        batchnum = 2,
                        nextrun = new DateTime(2011, 1, 1),
                        UserNumber = 2
                    }
            };
            var sut = new BroadcastFilter();

            var output = sut.FilterOutOfflineBroadcasts(overdueBcsts);

            Assert.AreEqual(1, output.Count);
            Assert.AreEqual("BAR", output[0].agency);
        }

        public void FilterOutInactiveAgencies_InactiveAgencyExistsOnDatabase_RemoveInactiveAgencyRecord()
        {
            var overdueBcsts = new List<ibbatch>
            {
                new ibbatch
                    {
                        agency = "FOO",
                        batchname = "FOO_NAME",
                        batchnum = 1,
                        nextrun = new DateTime(2012,1,1),
                        UserNumber = 1
                    },
                new ibbatch
                    {
                        agency = "BAR",
                        batchname = "BAR_NAME",
                        batchnum = 2,
                        nextrun = new DateTime(2011, 1, 1),
                        UserNumber = 2
                    }
            };
            var agencies = new List<mstragcy>
                               {
                                    new mstragcy { agency = "FOO", active = true, databasename = "A" },
                                    new mstragcy { agency = "BAR", active = false, databasename = "A" }
                               };
            var mockDb = new Mock<IMastersQueryable>();
            mockDb.Setup(x => x.MstrAgcy).Returns(MockHelper.GetListAsQueryable(agencies).Object);
            var store = new Mock<IMasterDataStore>();
            store.Setup(x => x.MastersQueryDb).Returns(mockDb.Object);
            var sut = new BroadcastFilter();

            var output = sut.FilterOutInactiveAgencies(overdueBcsts, store.Object, "A");

            Assert.AreEqual(1, output.Count);
            Assert.AreEqual("FOO", output[0].agency);
        }
    }
}
