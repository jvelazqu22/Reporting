using System;
using System.Collections.Generic;
using System.Linq;

using iBank.Entities.ClientEntities;
using iBank.OverdueBroadcastMonitor;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.OverdueBroadcastMonitor
{
    [TestClass]
    public class OverdueBroadcastMapperTests
    {
        [TestMethod]
        public void MapToOverdueBroadcasts()
        {
            var database = "FOOBAR";
            var batchRecords = new List<ibbatch>
                                   {
                                       new ibbatch
                                           {
                                               agency = "FOO",
                                               batchname = "FOO_BATCH",
                                               batchnum = 1,
                                               UserNumber = 2,
                                               nextrun = new DateTime(2016, 1, 2)
                                           },
                                       new ibbatch
                                           {
                                               agency = "BAR",
                                               batchname = "BAR_BATCH",
                                               batchnum = 2,
                                               UserNumber = 3,
                                               nextrun = new DateTime(2017, 5, 6)
                                           }
                                   };
            var sut = new OverdueBroadcastMapper();
            var overdue = sut.MapToOverdueBroadcasts(batchRecords, database);

            Assert.AreEqual(2, overdue.Count);

            var fooRecord = overdue.FirstOrDefault(x => x.batchnum == 1);
            Assert.AreEqual("FOO", fooRecord.agency);
            Assert.AreEqual("FOO_BATCH", fooRecord.batchname);
            Assert.AreEqual(2, fooRecord.UserNumber);
            Assert.AreEqual("FOOBAR", fooRecord.database_name);
            Assert.AreEqual(new DateTime(2016, 1, 2), fooRecord.nextrun);

            var barRecord = overdue.FirstOrDefault(x => x.batchnum == 2);
            Assert.AreEqual("BAR", barRecord.agency);
            Assert.AreEqual("BAR_BATCH", barRecord.batchname);
            Assert.AreEqual(3, barRecord.UserNumber);
            Assert.AreEqual("FOOBAR", barRecord.database_name);
            Assert.AreEqual(new DateTime(2017, 5, 6), barRecord.nextrun);

        }
    }
}
