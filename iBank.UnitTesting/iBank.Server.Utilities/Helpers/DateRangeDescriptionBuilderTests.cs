using System;
using System.Collections.Generic;
using Domain.Helper;
using Domain.Orm.Classes;
using iBank.Server.Utilities.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Server.Utilities.Helpers
{
    [TestClass]
    public class DateRangeDescriptionBuilderTests
    {
        #region Test variable setup
        private string _invoiceKey = "xDateDescInv";
        private string _invoiceTranslation = "An invoice translation from [xxxxx] to [xxxxx]";
        private string _bookDatesKey = "xDateDescBooked";
        private string _bookDatesTranslation = "A book date translation from [xxxxx] to [xxxxx]";
        private string _routingDepartKey = "xDateDescDep";
        private string _routingDepartTranslation = "A departure translation from [xxxxx] to [xxxxx]";
        private string _arrivalKey = "xDateDescArr";
        private string _arrivalTranslation = "An arrival translation from [xxxxx] to [xxxxx]";
        private string _rentalKey = "xDateDescRent";
        private string _rentalTranslation = "A rental translation from [xxxxx] to [xxxxx]";
        private string _checkInKey = "xDateDescCheckin";
        private string _checkInTranslation = "A checkin translation from [xxxxx] to [xxxxx]";
        private string _transactionKey = "xDateDescTrans";
        private string _transactionTranslation = "A transaction translation from [xxxxx] to [xxxxx]";
        private string _onRoadKey = "xDateDescOnRoad";
        private string _onRoadTranslation = "An on the road translation from [xxxxx] to [xxxxx]";
        private string _authKey = "xDateDescStatus";
        private string _authTranslation = "An auth translation from [xxxxx] to [xxxxx]";
        private string _postKey = "xDateDescPosted";
        private string _postTranslation = "A post translation from [xxxxx] to [xxxxx]";
        private string _lastUpdateKey = "xDateDescLastUpdated";
        private string _lastUpdateTranslation = "A last update translation from [xxxxx] to [xxxxx]";
        private string _tripDepartKey = "xDateDescTripDep";
        private string _tripDepartTranslation = "A trip depart translation from [xxxxx] to [xxxxx]";
        
        private DateTime _begin = new DateTime(2017, 3, 4, 13, 2, 0);
        private DateTime _end = new DateTime(2020, 7, 8, 15, 5, 0);

        private string _userDateFormat = "dd-mm-yyyy";
        private string _hourMinuteFormat = "HH:mm";

        private List<LanguageVariableInfo> _translations;

        [TestInitialize]
        public void Init()
        {
            _translations = new List<LanguageVariableInfo>
            {
                new LanguageVariableInfo {VariableName = _invoiceKey, Translation = _invoiceTranslation},
                new LanguageVariableInfo {VariableName = _bookDatesKey, Translation = _bookDatesTranslation},
                new LanguageVariableInfo {VariableName = _routingDepartKey, Translation = _routingDepartTranslation},
                new LanguageVariableInfo {VariableName = _arrivalKey, Translation = _arrivalTranslation},
                new LanguageVariableInfo {VariableName = _rentalKey, Translation = _rentalTranslation},
                new LanguageVariableInfo {VariableName = _checkInKey, Translation = _checkInTranslation},
                new LanguageVariableInfo {VariableName = _transactionKey, Translation = _transactionTranslation},
                new LanguageVariableInfo {VariableName = _onRoadKey, Translation = _onRoadTranslation},
                new LanguageVariableInfo {VariableName = _authKey, Translation = _authTranslation},
                new LanguageVariableInfo {VariableName = _postKey, Translation = _postTranslation},
                new LanguageVariableInfo {VariableName = _lastUpdateKey, Translation = _lastUpdateTranslation},
                new LanguageVariableInfo {VariableName = _tripDepartKey, Translation = _tripDepartTranslation}
            };
        }
        #endregion

        [TestMethod]
        public void Build_DateRangeParameterNotParseable_ReturnEmptyString()
        {
            var dateRangeParam = "a";
            var sut = new DateRangeDescriptionBuilder(_translations, _userDateFormat);

            var result = sut.Build(_begin, _end, dateRangeParam);

            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void Build_NoDateRangeParameter_NoTranslation_DefaultToDefaultTripDepartureDateCaptionWithUserFormattedDates()
        {
            var dateRangeParam = "";
            var expected = $"Trip Departures from {_begin.ToString(_userDateFormat)} to {_end.ToString(_userDateFormat)}";
            _translations.RemoveAll(x => x.VariableName.Equals(_tripDepartKey));
            var sut = new DateRangeDescriptionBuilder(_translations, _userDateFormat);

            var result = sut.Build(_begin, _end, dateRangeParam);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Build_NoDateRangeParameter_TranslationExists_DefaultToTranslatedTripDepartureDateCaptionWithUserFormattedDates()
        {
            var dateRangeParam = "";
            var expected = $"A trip depart translation from {_begin.ToString(_userDateFormat)} to {_end.ToString(_userDateFormat)}";
            var sut = new DateRangeDescriptionBuilder(_translations, _userDateFormat);

            var result = sut.Build(_begin, _end, dateRangeParam);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Build_InvoiceDate_TranslationExists_ReturnTranslatedCaptionWithUserFormattedDates()
        {
            var dateRangeParam = GetEnumValAsString(DateType.InvoiceDate);
            var expected = $"An invoice translation from {_begin.ToString(_userDateFormat)} to {_end.ToString(_userDateFormat)}";
            var sut = new DateRangeDescriptionBuilder(_translations, _userDateFormat);

            var result = sut.Build(_begin, _end, dateRangeParam);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Build_InvoiceDate_NoTranslation_ReturnDefaultCaptionWithUserFormattedDates()
        {
            var dateRangeParam = GetEnumValAsString(DateType.InvoiceDate);
            var expected = $"Invoice dates from {_begin.ToString(_userDateFormat)} to {_end.ToString(_userDateFormat)}";
            _translations.RemoveAll(x => x.VariableName.Equals(_invoiceKey));
            var sut = new DateRangeDescriptionBuilder(_translations, _userDateFormat);

            var result = sut.Build(_begin, _end, dateRangeParam);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Build_BookedDate_TranslationExists_ReturnTranslatedCaptionWithUserFormattedDates()
        {
            var dateRangeParam = GetEnumValAsString(DateType.BookedDate);
            var expected = $"A book date translation from {_begin.ToString(_userDateFormat)} to {_end.ToString(_userDateFormat)}";
            var sut = new DateRangeDescriptionBuilder(_translations, _userDateFormat);

            var result = sut.Build(_begin, _end, dateRangeParam);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Build_BookedDate_NoTranslation_ReturnDefaultCaptionWithUserFormattedDates()
        {
            var dateRangeParam = GetEnumValAsString(DateType.BookedDate);
            var expected = $"Booked dates from {_begin.ToString(_userDateFormat)} to {_end.ToString(_userDateFormat)}";
            _translations.RemoveAll(x => x.VariableName.Equals(_bookDatesKey));
            var sut = new DateRangeDescriptionBuilder(_translations, _userDateFormat);

            var result = sut.Build(_begin, _end, dateRangeParam);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Build_RoutingDepartureDate_TranslationExists_ReturnTranslatedCaptionWithUserFormattedDates()
        {
            var dateRangeParam = GetEnumValAsString(DateType.RoutingDepartureDate);
            var expected = $"A departure translation from {_begin.ToString(_userDateFormat)} to {_end.ToString(_userDateFormat)}";
            var sut = new DateRangeDescriptionBuilder(_translations, _userDateFormat);

            var result = sut.Build(_begin, _end, dateRangeParam);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Build_RoutingDepartureDate_NoTranslation_ReturnDefaultCaptionWithUserFormattedDates()
        {
            var dateRangeParam = GetEnumValAsString(DateType.RoutingDepartureDate);
            var expected = $"Departures from {_begin.ToString(_userDateFormat)} to {_end.ToString(_userDateFormat)}";
            _translations.RemoveAll(x => x.VariableName.Equals(_routingDepartKey));
            var sut = new DateRangeDescriptionBuilder(_translations, _userDateFormat);

            var result = sut.Build(_begin, _end, dateRangeParam);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Build_ArrivalDate_TranslationExists_ReturnTranslatedCaptionWithUserFormattedDates()
        {
            var dateRangeParam = GetEnumValAsString(DateType.RoutingArrivalDate);
            var expected = $"An arrival translation from {_begin.ToString(_userDateFormat)} to {_end.ToString(_userDateFormat)}";
            var sut = new DateRangeDescriptionBuilder(_translations, _userDateFormat);

            var result = sut.Build(_begin, _end, dateRangeParam);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Build_ArrivalDate_NoTranslation_ReturnDefaultCaptionWithUserFormattedDates()
        {
            var dateRangeParam = GetEnumValAsString(DateType.RoutingArrivalDate);
            var expected = $"Arrivals from {_begin.ToString(_userDateFormat)} to {_end.ToString(_userDateFormat)}";
            _translations.RemoveAll(x => x.VariableName.Equals(_arrivalKey));
            var sut = new DateRangeDescriptionBuilder(_translations, _userDateFormat);

            var result = sut.Build(_begin, _end, dateRangeParam);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Build_RentalDate_TranslationExists_ReturnTranslatedCaptionWithUserFormattedDates()
        {
            var dateRangeParam = GetEnumValAsString(DateType.CarRentalDate);
            var expected = $"A rental translation from {_begin.ToString(_userDateFormat)} to {_end.ToString(_userDateFormat)}";
            var sut = new DateRangeDescriptionBuilder(_translations, _userDateFormat);

            var result = sut.Build(_begin, _end, dateRangeParam);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Build_RentalDate_NoTranslation_ReturnDefaultCaptionWithUserFormattedDates()
        {
            var dateRangeParam = GetEnumValAsString(DateType.CarRentalDate);
            var expected = $"Rental Dates from {_begin.ToString(_userDateFormat)} to {_end.ToString(_userDateFormat)}";
            _translations.RemoveAll(x => x.VariableName.Equals(_rentalKey));
            var sut = new DateRangeDescriptionBuilder(_translations, _userDateFormat);

            var result = sut.Build(_begin, _end, dateRangeParam);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Build_HotelDate_TranslationExists_ReturnTranslatedCaptionWithUserFormattedDates()
        {
            var dateRangeParam = GetEnumValAsString(DateType.HotelCheckInDate);
            var expected = $"A checkin translation from {_begin.ToString(_userDateFormat)} to {_end.ToString(_userDateFormat)}";
            var sut = new DateRangeDescriptionBuilder(_translations, _userDateFormat);

            var result = sut.Build(_begin, _end, dateRangeParam);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Build_HotelDate_NoTranslation_ReturnDefaultCaptionWithUserFormattedDates()
        {
            var dateRangeParam = GetEnumValAsString(DateType.HotelCheckInDate);
            var expected = $"Check-in Dates from {_begin.ToString(_userDateFormat)} to {_end.ToString(_userDateFormat)}";
            _translations.RemoveAll(x => x.VariableName.Equals(_checkInKey));
            var sut = new DateRangeDescriptionBuilder(_translations, _userDateFormat);

            var result = sut.Build(_begin, _end, dateRangeParam);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Build_TransactionDate_TranslationExists_ReturnTranslatedCaptionWithUserFormattedDates()
        {
            var dateRangeParam = GetEnumValAsString(DateType.TransactionDate);
            var expected = $"A transaction translation from {_begin.ToString(_userDateFormat)} to {_end.ToString(_userDateFormat)}";
            var sut = new DateRangeDescriptionBuilder(_translations, _userDateFormat);

            var result = sut.Build(_begin, _end, dateRangeParam);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Build_TransactionDate_NoTranslation_ReturnDefaultCaptionWithUserFormattedDates()
        {
            var dateRangeParam = GetEnumValAsString(DateType.TransactionDate);
            var expected = $"Transaction Dates from {_begin.ToString(_userDateFormat)} to {_end.ToString(_userDateFormat)}";
            _translations.RemoveAll(x => x.VariableName.Equals(_transactionKey));
            var sut = new DateRangeDescriptionBuilder(_translations, _userDateFormat);

            var result = sut.Build(_begin, _end, dateRangeParam);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Build_OnRoadSpecialDate_TranslationExists_ReturnTranslatedCaptionWithUserFormattedDates()
        {
            var dateRangeParam = GetEnumValAsString(DateType.OnTheRoadDatesSpecial);
            var expected = $"An on the road translation from {_begin.ToString(_userDateFormat)} to {_end.ToString(_userDateFormat)}";
            var sut = new DateRangeDescriptionBuilder(_translations, _userDateFormat);

            var result = sut.Build(_begin, _end, dateRangeParam);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Build_OnRoadSpecialDate_NoTranslation_ReturnDefaultCaptionWithUserFormattedDates()
        {
            var dateRangeParam = GetEnumValAsString(DateType.OnTheRoadDatesSpecial);
            var expected = $"On-the-Road Dates from {_begin.ToString(_userDateFormat)} to {_end.ToString(_userDateFormat)}";
            _translations.RemoveAll(x => x.VariableName.Equals(_onRoadKey));
            var sut = new DateRangeDescriptionBuilder(_translations, _userDateFormat);

            var result = sut.Build(_begin, _end, dateRangeParam);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Build_OnRoadCarDate_TranslationExists_ReturnTranslatedCaptionWithUserFormattedDates()
        {
            var dateRangeParam = GetEnumValAsString(DateType.OnTheRoadDatesCarRental);
            var expected = $"An on the road translation from {_begin.ToString(_userDateFormat)} to {_end.ToString(_userDateFormat)}";
            var sut = new DateRangeDescriptionBuilder(_translations, _userDateFormat);

            var result = sut.Build(_begin, _end, dateRangeParam);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Build_OnRoadCarDate_NoTranslation_ReturnDefaultCaptionWithUserFormattedDates()
        {
            var dateRangeParam = GetEnumValAsString(DateType.OnTheRoadDatesCarRental);
            var expected = $"On-the-Road Dates from {_begin.ToString(_userDateFormat)} to {_end.ToString(_userDateFormat)}";
            _translations.RemoveAll(x => x.VariableName.Equals(_onRoadKey));
            var sut = new DateRangeDescriptionBuilder(_translations, _userDateFormat);

            var result = sut.Build(_begin, _end, dateRangeParam);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Build_OnRoadHotelDate_TranslationExists_ReturnTranslatedCaptionWithUserFormattedDates()
        {
            var dateRangeParam = GetEnumValAsString(DateType.OnTheRoadDatesHotel);
            var expected = $"An on the road translation from {_begin.ToString(_userDateFormat)} to {_end.ToString(_userDateFormat)}";
            var sut = new DateRangeDescriptionBuilder(_translations, _userDateFormat);

            var result = sut.Build(_begin, _end, dateRangeParam);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Build_OnRoadHotelDate_NoTranslation_ReturnDefaultCaptionWithUserFormattedDates()
        {
            var dateRangeParam = GetEnumValAsString(DateType.OnTheRoadDatesHotel);
            var expected = $"On-the-Road Dates from {_begin.ToString(_userDateFormat)} to {_end.ToString(_userDateFormat)}";
            _translations.RemoveAll(x => x.VariableName.Equals(_onRoadKey));
            var sut = new DateRangeDescriptionBuilder(_translations, _userDateFormat);

            var result = sut.Build(_begin, _end, dateRangeParam);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Build_AuthStatusDate_TranslationExists_ReturnTranslatedCaptionWithUserFormattedDates()
        {
            var dateRangeParam = GetEnumValAsString(DateType.AuthorizationStatusDate);
            var expected = $"An auth translation from {_begin.ToString(_userDateFormat)} to {_end.ToString(_userDateFormat)}";
            var sut = new DateRangeDescriptionBuilder(_translations, _userDateFormat);

            var result = sut.Build(_begin, _end, dateRangeParam);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Build_AuthStatusDate_NoTranslation_ReturnDefaultCaptionWithUserFormattedDates()
        {
            var dateRangeParam = GetEnumValAsString(DateType.AuthorizationStatusDate);
            var expected = $"Authorization Status Dates from {_begin.ToString(_userDateFormat)} to {_end.ToString(_userDateFormat)}";
            _translations.RemoveAll(x => x.VariableName.Equals(_authKey));
            var sut = new DateRangeDescriptionBuilder(_translations, _userDateFormat);

            var result = sut.Build(_begin, _end, dateRangeParam);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Build_PostedDate_TranslationExists_ReturnTranslatedCaptionWithUserFormattedDates()
        {
            var dateRangeParam = GetEnumValAsString(DateType.PostDate);
            var expected = $"A post translation from {_begin.ToString(_userDateFormat)} to {_end.ToString(_userDateFormat)}";
            var sut = new DateRangeDescriptionBuilder(_translations, _userDateFormat);

            var result = sut.Build(_begin, _end, dateRangeParam);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Build_PostedDate_NoTranslation_ReturnDefaultCaptionWithUserFormattedDates()
        {
            var dateRangeParam = GetEnumValAsString(DateType.PostDate);
            var expected = $"Posted Dates from {_begin.ToString(_userDateFormat)} to {_end.ToString(_userDateFormat)}";
            _translations.RemoveAll(x => x.VariableName.Equals(_postKey));
            var sut = new DateRangeDescriptionBuilder(_translations, _userDateFormat);

            var result = sut.Build(_begin, _end, dateRangeParam);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Build_LastUpdatedDate_TranslationExists_ReturnTranslatedCaptionWithUserFormattedDates()
        {
            var formatWithTime = $"{_userDateFormat} {_hourMinuteFormat}";
            var dateRangeParam = GetEnumValAsString(DateType.LastUpdate);
            var expected = $"A last update translation from {_begin.ToString(formatWithTime)} to {_end.ToString(formatWithTime)}";
            var sut = new DateRangeDescriptionBuilder(_translations, _userDateFormat);

            var result = sut.Build(_begin, _end, dateRangeParam);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Build_LastUpdatedDate_NoTranslation_ReturnDefaultCaptionWithUserFormattedDates()
        {
            var formatWithTime = $"{_userDateFormat} {_hourMinuteFormat}";
            var dateRangeParam = GetEnumValAsString(DateType.LastUpdate);
            var expected = $"Last Updated Dates from {_begin.ToString(formatWithTime)} to {_end.ToString(formatWithTime)}";
            _translations.RemoveAll(x => x.VariableName.Equals(_lastUpdateKey));
            var sut = new DateRangeDescriptionBuilder(_translations, _userDateFormat);

            var result = sut.Build(_begin, _end, dateRangeParam);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Build_DepartureDate_TranslationExists_ReturnTranslatedCaptionWithUserFormattedDates()
        {
            var dateRangeParam = GetEnumValAsString(DateType.DepartureDate);
            var expected = $"A trip depart translation from {_begin.ToString(_userDateFormat)} to {_end.ToString(_userDateFormat)}";
            var sut = new DateRangeDescriptionBuilder(_translations, _userDateFormat);

            var result = sut.Build(_begin, _end, dateRangeParam);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Build_DepartureDate_NoTranslation_ReturnDefaultCaptionWithUserFormattedDates()
        {
            var dateRangeParam = GetEnumValAsString(DateType.DepartureDate);
            var expected = $"Trip Departures from {_begin.ToString(_userDateFormat)} to {_end.ToString(_userDateFormat)}";
            _translations.RemoveAll(x => x.VariableName.Equals(_tripDepartKey));
            var sut = new DateRangeDescriptionBuilder(_translations, _userDateFormat);

            var result = sut.Build(_begin, _end, dateRangeParam);

            Assert.AreEqual(expected, result);
        }

        private string GetEnumValAsString(DateType dateType)
        {
            return ((int) dateType).ToString();
        }

    }
}
