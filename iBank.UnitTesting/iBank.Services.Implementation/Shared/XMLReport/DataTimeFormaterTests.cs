using System;
using iBank.Services.Implementation.Shared.XMLReport;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared.XMLReport
{
    [TestClass]
    public class DataTimeFormaterTests
    {
        [TestMethod]
        public void DateTimeFormater_PassDateTimeToFormatDateUseStarAsDateDelim_Match()
        {

            var Depdate = new DateTime(2018, 1, 10, 8, 30, 40);

            var exp = "2018*01*10";

            var act = DateTimeFormater.FormatDate(Depdate, "*");

            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void DateTimeFormater_PassDateTimeFormatDateTimeUseStarAsDelim_Match()
        {
            var Depdate = new DateTime(2018, 1, 10, 8, 30, 40);

            var exp = "2018*01*10T08$30$40";

            var act = DateTimeFormater.FormatDateTime(Depdate, "*", "$");

            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void DateTimeFormater_PassTimeAsStringFormatTime_Match()
        {
            var Deptime = "08:30:00";

            var exp = "08:30";

            var act = DateTimeFormater.FormatTime(Deptime);

            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void DateTimeFormater_PassTimeWithoutNoSparater_Match()
        {
            var Deptime = "083004";

            var exp = "08:30";

            var act = DateTimeFormater.FormatTime(Deptime);

            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void DateTimeFormater_PassTimeWithoutNoSparaterWithNoLeadingZero_Match()
        {
            var Deptime = "830";

            var exp = "08:30";

            var act = DateTimeFormater.FormatTime(Deptime);

            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void DateTimeFormater_PassTimeWithoutHasSparaterWithNoLeadingZero_Match()
        {
            var Deptime = "8:30";

            var exp = "08:30";

            var act = DateTimeFormater.FormatTime(Deptime);

            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void DateTimeFormater_PassTimeWithoutHasSparaterWithNoLeadingZeroInBothParts_Match()
        {
            var Deptime = "8:3";

            var exp = "08:03";

            var act = DateTimeFormater.FormatTime(Deptime);

            Assert.AreEqual(exp, act);
        }
    }
}
