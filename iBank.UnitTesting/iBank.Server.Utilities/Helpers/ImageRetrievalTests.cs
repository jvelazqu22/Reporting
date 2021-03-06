﻿using System.Collections.Generic;

using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Helpers;
using iBank.UnitTesting.TestingHelpers.MockHelpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

namespace iBank.UnitTesting.iBank.Server.Utilities.Helpers
{
    [TestClass]
    public class ImageRetrievalTests
    {
        private IMasterDataStore _masterDataStore;

        private IClientDataStore _clientDataStore;

        [TestInitialize]
        public void Init()
        {
            var mockClientDb = new Mock<IClientQueryable>();
            var mockMasterDb = new Mock<IMastersQueryable>();

            var validStyleGroupExtra = new StyleGroupExtras { SGroupNbr = 1, ClientCode = "FOO", FieldFunction = "RPTLOGO_IMAGENBR", FieldData = "1" };
            var validButNoImage = new StyleGroupExtras { SGroupNbr = 2, ClientCode = "FOO", FieldFunction = "RPTLOGO_IMAGENBR", FieldData = "2" };
            var invalidStyleGroupExtra = new StyleGroupExtras { SGroupNbr = 3, ClientCode = "FOO", FieldFunction = "RPTLOGO_IMAGENBR", FieldData = "Invalid" };
            mockClientDb.Setup(x => x.StyleGroupExtra).Returns( MockHelper.GetListAsQueryable(new List<StyleGroupExtras> { validStyleGroupExtra, validButNoImage, invalidStyleGroupExtra }).Object);

            var encodedData =
                @"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";
            var validClientImage = new ClientImages { Agency = "FOO", ImageNbr = 1, ImageName = "Image Name", EncodedData = encodedData };
            var defaultClientImage = new ClientImages { Agency = "[defRpt]", ImageNbr = 2, ImageName = "default", EncodedData = encodedData };
            mockMasterDb.Setup(x => x.ClientImages).Returns(MockHelper.GetListAsQueryable(new List<ClientImages> { validClientImage, defaultClientImage }).Object);

            var mockMasterStore = new Mock<IMasterDataStore>();
            mockMasterStore.Setup(x => x.MastersQueryDb).Returns(mockMasterDb.Object);
            var mockClientStore = new Mock<IClientDataStore>();
            mockClientStore.Setup(x => x.ClientQueryDb).Returns(mockClientDb.Object);

            _masterDataStore = mockMasterStore.Object;
            _clientDataStore = mockClientStore.Object;
        }

        [TestMethod]

        public void GetStandardReportImage_StyleGroupExistsImageNumberValid_ReturnAgencyImage()
        {
            var sut = new ImageRetrieval(_masterDataStore, _clientDataStore);
            var styleGroupNumber = 1;
            var agency = "FOO";

            var output = sut.GetStandardReportImage(styleGroupNumber, agency);

            Assert.AreEqual("Image Name", output.ImageName);
        }

        [TestMethod]

        public void GetStandardReportImage_StyleGroupDoesntExist_ReturnDefaultImage()
        {
            var sut = new ImageRetrieval(_masterDataStore, _clientDataStore);
            var styleGroupNumber = 5;
            var agency = "FOO";

            var output = sut.GetStandardReportImage(styleGroupNumber, agency);

            Assert.AreEqual("default", output.ImageName);
        }

        [TestMethod]

        public void GetStandardReportImage_StyleGroupExistsImageNumberNotValid_ReturnDefaultImage()
        {
            var sut = new ImageRetrieval(_masterDataStore, _clientDataStore);
            var styleGroupNumber = 3;
            var agency = "FOO";

            var output = sut.GetStandardReportImage(styleGroupNumber, agency);

            Assert.AreEqual("default", output.ImageName);
        }

        [TestMethod]

        public void GetStandardReportImage_StyleGroupExistsImageNumberDoesntExist_ReturnDefaultImage()
        {
            var sut = new ImageRetrieval(_masterDataStore, _clientDataStore);
            var styleGroupNumber = 2;
            var agency = "FOO";

            var output = sut.GetStandardReportImage(styleGroupNumber, agency);

            Assert.AreEqual("default", output.ImageName);
        }
    }

}
