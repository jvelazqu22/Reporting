using System;
using iBank.Server.Utilities.Classes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Server.Utilities.Classes
{
    [TestClass]
    public class iBankDateFormattedStringTests
    {
        [TestMethod]
        public void Parse_StringIsNull_ReturnNull()
        {
            string s = null;

            var result = iBankDateFormattedString.Parse(s);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Parse_ContainsOnlyDateComponents_ReturnDate()
        {
            var s = "DT:2016,4,1";
            var expected = new DateTime(2016, 4, 1);

            var result = iBankDateFormattedString.Parse(s);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Parse_ContainsOnlyDateComponents_WantsEndOfDay_ReturnDateWithEndOfDay()
        {
            var s = "DT:2016,4,1";
            var expected = new DateTime(2016, 4, 1, 23, 59, 59);

            var result = iBankDateFormattedString.Parse(s, true);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Parse_ContainsDateAndTimeComponents_TimeIsPastNoon_ReturnDateTime()
        {
            var s = "DT:2016,4,1 T:23:25";
            var expected = new DateTime(2016, 4, 1, 23, 25, 0);

            var result = iBankDateFormattedString.Parse(s);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Parse_ContainsDateAndTimeComponents_TimeIsBeforeNoon_ReturnDateTime()
        {
            var s = "DT:2016,4,1 T:5:9";
            var expected = new DateTime(2016, 4, 1, 5, 9, 0);

            var result = iBankDateFormattedString.Parse(s);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Parse_YearIsOnlyTwoDigits__AssumeTwentyFirstCentury_ReturnDateTime()
        {
            var s = "DT:16,4,1 T:23:25";
            var expected = new DateTime(2016, 4, 1, 23, 25, 0);

            var result = iBankDateFormattedString.Parse(s);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Parse_DoesNotContainDateDelimiter_ReturnsNull()
        {
            var s = " T:23:25";

            var result = iBankDateFormattedString.Parse(s);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Parse_DateComponentIsMalformed_ReturnsNull()
        {
            var s = "DT:2016,4,1, T:5:9";

            var result = iBankDateFormattedString.Parse(s);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Parse_TimeComponentIsMalformed_ReturnsNull()
        {
            var s = "DT:2016,4,1 T:5:9:";

            var result = iBankDateFormattedString.Parse(s);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Parse_NotAbleToParseYear_ReturnsNull()
        {
            var s = "DT:a,4,1 T:5:9";

            var result = iBankDateFormattedString.Parse(s);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Parse_NotAbleToParseMonth_ReturnsNull()
        {
            var s = "DT:2016,a,1 T:5:9";

            var result = iBankDateFormattedString.Parse(s);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Parse_NotAbleToParseDay_ReturnsNull()
        {
            var s = "DT:2016,4,a T:5:9";

            var result = iBankDateFormattedString.Parse(s);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Parse_NotAbleToParseHour_ReturnsNull()
        {
            var s = "DT:2016,4,1 T:a:9";

            var result = iBankDateFormattedString.Parse(s);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Parse_NotAbleToParseMinute_ReturnsNull()
        {
            var s = "DT:2016,4,1 T:5:a";

            var result = iBankDateFormattedString.Parse(s);

            Assert.IsNull(result);
        }
    }
}
