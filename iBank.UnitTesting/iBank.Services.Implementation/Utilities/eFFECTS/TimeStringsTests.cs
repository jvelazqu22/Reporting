using iBank.Services.Implementation.Utilities.eFFECTS;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace iBank.UnitTesting.iBank.Services.Implementation.Utilities.eFFECTS
{
    [TestClass]
    public class TimeStringsTests
    {
        [TestMethod]
        public void PropertiesCheck()
        {
            // 1/2/2016 3:55:59
            var date = new DateTime(2016, 1, 2, 3, 55, 59);
            var ts = new TimeStrings(date);

            Assert.AreEqual("2016", ts.Year);
            Assert.AreEqual("01", ts.Month);
            Assert.AreEqual("02", ts.Day);
            Assert.AreEqual("03", ts.Hour);
            Assert.AreEqual("55", ts.Min);
            Assert.AreEqual("59", ts.Sec);
        }
    }
}
