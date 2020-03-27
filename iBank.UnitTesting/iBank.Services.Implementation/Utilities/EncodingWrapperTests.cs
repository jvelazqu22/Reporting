using System;

using iBank.Services.Implementation.Utilities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.Utilities
{
    [TestClass]
    public class EncodingWrapperTests
    {
        [TestMethod]
        public void GetEncoding_NotJapaneseLanguage_ReturnWindowsEncoding()
        {
            var language = "EN";
            var sut = new EncodingWrapper();

            var output = sut.GetEncoding(language);

            Assert.AreEqual("Windows-1252", output.WebName);
        }

        [TestMethod]
        public void GetEncoding_JapaneseLanguage_ReturnUnicodeEncoding()
        {
            var language = "JP";
            var sut = new EncodingWrapper();

            var output = sut.GetEncoding(language);

            Assert.AreEqual("utf-8", output.WebName);
        }
    }
}
