using System;

using iBank.BroadcastServer.Email;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.BroadcastServer.Email
{
    [TestClass]
    public class LogoRetrieverTests
    {
        private string _htmlVersion;

        private readonly string _placeholder = "^agency_logo_url^";

        [TestInitialize]
        public void Init()
        {
            _htmlVersion = $"This is the html version with placeholder: {_placeholder}";
        }

        [TestMethod]
        public void ReplaceHTMLLogoPlaceholder_LogoStringNotEmpty_ReplacePlaceholderWithLogo()
        {
            var logoText = "foo";
            var imageMarkup = "<img src";
            var newVersion = LogoRetriever.ReplaceHTMLLogoPlaceholder(_htmlVersion, logoText);

            Assert.AreEqual(false, newVersion.Contains(_placeholder));
            Assert.AreEqual(true, newVersion.Contains(logoText));
            Assert.AreEqual(true, newVersion.Contains(imageMarkup));
        }

        [TestMethod]
        public void ReplaceHTMLLogoPlaceholder_LogoStringEmpty_ReplacePlaceholderWithSpace()
        {
            var logoText = "";
            var space = "&nbsp;";
            var newVersion = LogoRetriever.ReplaceHTMLLogoPlaceholder(_htmlVersion, logoText);

            Assert.AreEqual(false, newVersion.Contains(_placeholder));
            Assert.AreEqual(true, newVersion.Contains(space));
        }
    }
}
