using Domain.Helper;
using iBank.Services.Implementation.Utilities.eFFECTS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace iBank.UnitTesting.iBank.Services.Implementation.Utilities.eFFECTS
{
    [TestClass]
    public class FileHandlerTests
    {
        [TestMethod]
        public void GetZipFileName_ExtensionExists_ReplaceExtensionWithZipExtension()
        {
            var handler = new FileHandler();
            var effectsOutputFile = "foo.txt";
            var obj = new PrivateObject(handler);

            var output = obj.Invoke("GetZipFileName", effectsOutputFile);

            Assert.AreEqual("foo.zip", output);
        }

        [TestMethod]
        public void GetZipFileName_ExtensionNotPresentStringDoesNotEndWithPeriod_AppendZipExtension()
        {
            var handler = new FileHandler();
            var effectsOutputFile = "foo";
            var obj = new PrivateObject(handler);

            var output = obj.Invoke("GetZipFileName", effectsOutputFile);

            Assert.AreEqual("foo.zip", output);
        }

        [TestMethod]
        public void GetZipFileName_ExtensionNotPresentStringEndsWithPeriod_AppendZipNotation()
        {
            var handler = new FileHandler();
            var effectsOutputFile = "foo";
            var obj = new PrivateObject(handler);

            var output = obj.Invoke("GetZipFileName", effectsOutputFile);

            Assert.AreEqual("foo.zip", output);
        }

        [TestMethod]
        public void GetNonMaskedOutputFile_iBankStandardExtractReport_ReturnFileName()
        {
            var handler = new FileHandler();
            var processKey = (int)ReportTitles.iBankStandardExtract;
            var eProfileInfo = new EffectsOutputInformation { Outbox = @"e:\outbox" };
            var fileName = "foo.txt";

            var output = handler.GetNonMaskedOutputFile(processKey, null, eProfileInfo, "", fileName);

            Assert.AreEqual(@"e:\outbox\foo.txt", output);
        }

        [TestMethod]
        public void GetNonMaskedOutputFile_NotExtractReport_ReturnNameComposedOfVariables()
        {
            var handler = new FileHandler();
            var processKey = (int)ReportTitles.iBankStandardExtract + 1;
            var eProfileInfo = new EffectsOutputInformation { Outbox = @"e:\outbox", TradingPartnerName = "tpName" };
            var fileName = "foo.txt";
            var agency = "agency";
            var time = new TimeStrings(new DateTime(2016, 1, 2, 3, 4, 5));

            var output = handler.GetNonMaskedOutputFile(processKey, time, eProfileInfo, agency, fileName);

            Assert.AreEqual(@"e:\outbox\IBagencytpName20160102030405.txt", output);
        }
    }
}
