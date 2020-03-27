using iBank.Services.Implementation.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace iBank.UnitTesting.iBank.Services.Implementation.Utilities
{
    [TestClass]
    public class DateConverterTests
    {
        [TestMethod]
        public void ReplaceEmptyDate_NullPossibleDateAndNonNullControlDate_ReturnControlDate()
        {
            DateTime? possibleDate = null;
            DateTime? controlDate = new DateTime(1900, 1, 1);

            possibleDate = DateConverter.ReplaceEmptyDate(possibleDate, controlDate);

            Assert.AreEqual(controlDate, possibleDate);
        }

        [TestMethod]
        public void ReplaceEmptyDate_NonNullPossibleDateAndNonNullControlDate_ReturnPossibleDate()
        {
            DateTime? possibleDate = new DateTime(2000, 1, 1);
            DateTime? controlDate = new DateTime(1900, 1, 1);

            possibleDate = DateConverter.ReplaceEmptyDate(possibleDate, controlDate);

            var expected = new DateTime(2000, 1, 1);

            Assert.AreEqual(expected, possibleDate);
        }

        [TestMethod]
        public void ReplaceEmptyDate_NullPossibleDateAndNullControlDate_ReturnNull()
        {
            DateTime? possibleDate = null;
            DateTime? controlDate = null;

            possibleDate = DateConverter.ReplaceEmptyDate(possibleDate, controlDate);

            Assert.AreEqual(null, possibleDate);
        }

        [TestMethod]
        public void ReplaceEmptyDate_NonNullPossibleDateAndNullControlDate_ReturnNull()
        {
            DateTime? possibleDate = new DateTime(2000, 1, 1);
            DateTime? controlDate = null;

            possibleDate = DateConverter.ReplaceEmptyDate(possibleDate, controlDate);

            var expected = new DateTime(2000, 1, 1);

            Assert.AreEqual(expected, possibleDate);
        }
    }
}
