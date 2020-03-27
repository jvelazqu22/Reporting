using Domain.Models.BroadcastServer;
using Domain.Orm.Classes;
using iBank.BroadcastServer.Email;
using iBank.Entities.MasterEntities;

using iBankDomain.RepositoryInterfaces;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace iBank.UnitTesting.iBank.BroadcastServer.Email
{
    [TestClass]
    public class BroadcastEmailInformationRetrieverTests
    {
        [TestMethod]
        public void GetBroadcastSenderEmailInfo_SenderNameAndEmailInBroadcast_ReturnValuesInBroadcast()
        {
            var batch = new bcstque4 { bcsenderemail = "broadcsat_email@foo.com", bcsendername = "broadcast name" };
            var query = new Mock<IQuery<BroadcastEmailMasterInfo>>();
            query.Setup(x => x.ExecuteQuery()).Returns(new BroadcastEmailMasterInfo { SenderEmail = "master_email@foo.com", SenderName = "master name" });
            var config = new BroadcastServerInformation { SenderEmailAddress = "configemail@foo.com", SenderName = "config name" };
            var sut = new BroadcastEmailInformationRetriever();

            var output = sut.GetBroadcastSenderEmailInfo(batch, query.Object, config);

            Assert.AreEqual(batch.bcsenderemail, output.SenderAddress);
            Assert.AreEqual(batch.bcsendername, output.SenderName);
        }

        [TestMethod]
        public void GetBroadcastSenderEmailInfo_SenderNameAndEmailNotInBroadcastInMasterAgency_ReturnValuesInMasterAgency()
        {
            var batch = new bcstque4 { bcsenderemail = "", bcsendername = "" };
            var query = new Mock<IQuery<BroadcastEmailMasterInfo>>();
            var masterAgencyEmail = "master_email@foo.com";
            var masterAgencyName = "master name";
            query.Setup(x => x.ExecuteQuery()).Returns(new BroadcastEmailMasterInfo { SenderEmail = masterAgencyEmail, SenderName = masterAgencyName });
            var config = new BroadcastServerInformation { SenderEmailAddress = "configemail@foo.com", SenderName = "config name" };
            var sut = new BroadcastEmailInformationRetriever();

            var output = sut.GetBroadcastSenderEmailInfo(batch, query.Object, config);

            Assert.AreEqual(masterAgencyEmail, output.SenderAddress);
            Assert.AreEqual(masterAgencyName, output.SenderName);
        }

        [TestMethod]
        public void GetBroadcastSenderEmailInfo_SenderNameAndEmailNotInBroadcastOrMasterAgency_ReturnValuesInConfig()
        {
            var batch = new bcstque4 { bcsenderemail = "", bcsendername = "" };
            var query = new Mock<IQuery<BroadcastEmailMasterInfo>>();
            query.Setup(x => x.ExecuteQuery()).Returns(new BroadcastEmailMasterInfo { SenderEmail = "", SenderName = "" });
            var config = new BroadcastServerInformation { SenderEmailAddress = "configemail@foo.com", SenderName = "config name" };
            var sut = new BroadcastEmailInformationRetriever();

            var output = sut.GetBroadcastSenderEmailInfo(batch, query.Object, config);

            Assert.AreEqual("configemail@foo.com", output.SenderAddress);
            Assert.AreEqual("config name", output.SenderName);
        }
    }
}
