using System.Collections.Generic;

using iBank.Entities.MasterEntities;
using iBank.OverdueBroadcastMonitor;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.OverdueBroadcastMonitor
{
    [TestClass]
    public class HighAlertAgencyBroadcastHandlerTests
    {
        [TestMethod]
        public void PairOverdueBroadcastsWithHighAlertAgency()
        {
            var highAlertAgencies = new List<broadcast_high_alert_agency>
            {
                new broadcast_high_alert_agency { agency = "FOO", contact = "foo@ciswired.com", id = 1 }
            };
            var overdue = new List<overdue_broadcasts>
            {
                new overdue_broadcasts { agency = "FOO", batchnum = 1 },
                new overdue_broadcasts { agency = "FOO", batchnum = 2 },
                new overdue_broadcasts { agency = "BAR", batchnum = 3 }
            };
            var sut = new HighAlertAgencyBroadcastHandler();
            var output = sut.PairOverdueBroadcastsWithHighAlertAgency(overdue, highAlertAgencies);

            Assert.AreEqual(1, output.Count);
            Assert.AreEqual(2, output["FOO"].OverdueBroadcasts.Count);
            Assert.AreEqual("foo@ciswired.com", output["FOO"].BroadcastHighAlertAgency.contact);
        }
    }
}
