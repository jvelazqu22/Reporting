using System;

using Domain.Helper;

using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.TripChanges.SharedClasses;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.TripChanges.SharedClasses
{
    [TestClass]
    public class TripChangesCalculationsTests
    {
        private readonly TripChangesCalculations _calc = new TripChangesCalculations();

        private ReportGlobals _globals;

        [TestInitialize]
        public void Init()
        {
            _globals = new ReportGlobals();
        }

        [TestMethod]
        public void GetBeginDate2_GlobalDateHasValue_ReturnValueInGlobalDate()
        {
            var date = new DateTime(2016, 1, 2).ToIBankDateFormat();
            _globals.SetParmValue(WhereCriteria.BEGDATE3, date);

            var output = _calc.GetBeginDate2(_globals);

            Assert.AreEqual(date.ToDateFromiBankFormattedString(), output);
        }

        [TestMethod]
        public void GetBeginDate2_ValueNotInGlobalDate_ReturnValueInChangestamp()
        {
            string date = new DateTime(2016, 1, 2).ToIBankDateFormat();
            _globals.SetParmValue(WhereCriteria.BEGDATE3, "");
            _globals.SetParmValue(WhereCriteria.CHANGESTAMP, date);

            var output = _calc.GetBeginDate2(_globals);

            Assert.AreEqual(date.ToDateFromiBankFormattedString(), output);
        }

        [TestMethod]
        public void GetEndDate2_GlobalDateHasValue_ReturnValueInGlobalDate()
        {
            var date = new DateTime(2016, 1, 2);
            _globals.SetParmValue(WhereCriteria.ENDDATE3, date.ToIBankDateFormat());

            var output = _calc.GetEndDate2(_globals);

            Assert.AreEqual(date, output);
        }
        [TestMethod]
        public void GetEndDate2_GlobalDateHasNoValue_ReturnValueInGlobalChangestamp2()
        {
            var date = new DateTime(2016, 1, 2);
            _globals.SetParmValue(WhereCriteria.ENDDATE3, "");
            _globals.SetParmValue(WhereCriteria.CHANGESTAMP2, date.ToIBankDateFormat());

            var output = _calc.GetEndDate2(_globals);

            Assert.AreEqual(date, output);
        }

        [TestMethod]
        public void IsDateRangeValid_BothDatesHaveValueAndBeginDateIsBeforeEndDate_ReturnTrue()
        {
            var beginDate = new DateTime(2016, 1, 1);
            var endDate = new DateTime(2016, 2, 1);

            var output = _calc.IsDateRangeValid(beginDate, endDate, _globals);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsDateRangeValid_EndDateDoesNotHaveValue_ReturnTrue()
        {
            var beginDate = new DateTime(2016, 1, 1);
            DateTime? endDate = null;

            var output = _calc.IsDateRangeValid(beginDate, endDate, _globals);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsDateRangeValid_BeginDateDoesNotHaveValue_ReturnTrue()
        {
            DateTime? beginDate = null;
            var endDate = new DateTime(2016, 2, 1);

            var output = _calc.IsDateRangeValid(beginDate, endDate, _globals);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsDateRangeValid_BothDatesHaveValueAndBeginDateIsAfterEndDate_ReturnFalseAndChangeErrorMessage()
        {
            var beginDate = new DateTime(2016, 3, 1);
            var endDate = new DateTime(2016, 2, 1);

            var output = _calc.IsDateRangeValid(beginDate, endDate, _globals);

            Assert.AreEqual(false, output);
            Assert.AreEqual(_globals.ReportMessages.RptMsg_DateRange, _globals.ReportInformation.ErrorMessage);
            Assert.AreEqual(2, _globals.ReportInformation.ReturnCode);
        }

        [TestMethod]
        public void ReassignDate_DateOneNullDateTwoNotNull_ReturnDateTwo()
        {
            DateTime? dateOne = null;
            DateTime? dateTwo = new DateTime(2015, 1, 1);

            var output = _calc.ReassignDate(dateOne, dateTwo);

            Assert.AreEqual(dateTwo, output);
        }

        [TestMethod]
        public void ReassignDate_DateOneNullDateTwoNull_ReturnNull()
        {
            DateTime? dateOne = null;
            DateTime? dateTwo = null;

            var output = _calc.ReassignDate(dateOne, dateTwo);

            Assert.AreEqual(null, output);
        }

        [TestMethod]
        public void ReassignDate_DateOneNotNullDateTwoNotNull_ReturnDateOne()
        {
            DateTime? dateOne = new DateTime(2015, 5, 1);
            DateTime? dateTwo = new DateTime(2015, 1, 1);

            var output = _calc.ReassignDate(dateOne, dateTwo);

            Assert.AreEqual(dateOne, output);
        }
    }
}
