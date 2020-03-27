using System.Collections.Generic;

using Domain.Helper;
using Domain.Interfaces;

using iBank.BroadcastServer.Helper;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.UnitTesting.TestingHelpers.MockHelpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace iBank.UnitTesting.iBank.BroadcastServer.Helper
{
    [TestClass]
    public class HighAlertHandlerTests
    {
        //[TestMethod]
        public void NotificationIfHighAlertAgency_AgencyExists_SendEmail()
        {
            var highAlertAgency = new List<broadcast_high_alert_agency>
            {
                new broadcast_high_alert_agency { agency = "FOO", contact = "foo@ciswired.com" }
            };
            var mockDb = new Mock<IMastersQueryable>();
            mockDb.Setup(x => x.BroadcastHighAlertAgency).Returns(MockHelper.GetListAsQueryable(highAlertAgency).Object);
            var store = new Mock<IMasterDataStore>();
            store.Setup(x => x.MastersQueryDb).Returns(mockDb.Object);
            var emailer = new Mock<IEmailer>();
            var queueRec = new bcstque4 { agency = "FOO", batchnum = 1, batchname = "foo_batch", UserNumber = 2 };

            var sut = new HighAlertHandler(emailer.Object);
            sut.NotificationIfHighAlertAgency(store.Object, queueRec, true);

            emailer.Verify(x => x.SendEmail(new EmailInformation()), Times.Once());


        }
    }
}
