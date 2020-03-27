using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Utilities.eFFECTS;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;

using Domain.Helper;

using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.UnitTesting.TestingHelpers.MockHelpers;

namespace iBank.UnitTesting.iBank.Services.Implementation.Utilities.eFFECTS
{
    [TestClass]
    public class XmlReportHandlerTests
    {
        private Mock<IMastersQueryable> MockMasterDb;

        private Mock<IClientQueryable> MockClientDb;

        [TestInitialize]
        public void Init()
        {
            var udrData = new xmluserrpts
            {
                reportkey = 1,
                exportType = "footype",
                crname = "fooname"
            };
            var mockClientSet = new Mock<IQueryable<xmluserrpts>>();
            mockClientSet.SetupIQueryable(new List<xmluserrpts> { udrData }.AsQueryable());
            MockClientDb = new Mock<IClientQueryable>();
            MockClientDb.Setup(x => x.XmlUserRpt).Returns(mockClientSet.Object);


            var stdData = new xmlrpts
            {
                reportkey = 2,
                exportType = "bartype",
                crname = "barname"
            };
            var mockMasterSet = new Mock<IQueryable<xmlrpts>>();
            mockMasterSet.SetupIQueryable(new List<xmlrpts> { stdData }.AsQueryable());
            MockMasterDb = new Mock<IMastersQueryable>();
            MockMasterDb.Setup(x => x.XmlRpt).Returns(mockMasterSet.Object);
        }

        [TestMethod]
        public void GetXmlReportInfo_UserDefinedReportAndExists_ReturnsFilledObject()
        {
            var handler = new XmlReportHandler();
            var reportKey = 1;
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.UDRKEY, reportKey.ToString());

            var output = handler.GetXmlReportInfo(globals, MockClientDb.Object, MockMasterDb.Object);

            Assert.AreEqual("footype", output.ExportType);
            Assert.AreEqual("fooname", output.CrName);
        }

        [TestMethod]
        public void GetXmlReportInfo_UserDefinedReportNotExists_ReturnsEmptyObject()
        {
            var handler = new XmlReportHandler();
            var reportKey = 4;
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.UDRKEY, reportKey.ToString());

            var output = handler.GetXmlReportInfo(globals, MockClientDb.Object, MockMasterDb.Object);

            Assert.AreEqual("", output.ExportType);
            Assert.AreEqual("", output.CrName);
        }

        [TestMethod]
        public void GetXmlReportInfo_StandardReportAndExists_ReturnsFilledObject()
        {
            var handler = new XmlReportHandler();
            var reportKey = -2;
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.UDRKEY, reportKey.ToString());

            var output = handler.GetXmlReportInfo(globals, MockClientDb.Object, MockMasterDb.Object);

            Assert.AreEqual("bartype", output.ExportType);
            Assert.AreEqual("barname", output.CrName);
        }

        [TestMethod]
        public void GetXmlReportInfo_StandardReportNotExists_ReturnsEmptyObject()
        {
            var handler = new XmlReportHandler();
            var reportKey = -10;
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.UDRKEY, reportKey.ToString());

            var output = handler.GetXmlReportInfo(globals, MockClientDb.Object, MockMasterDb.Object);

            Assert.AreEqual("", output.ExportType);
            Assert.AreEqual("", output.CrName);
        }
    }
}
