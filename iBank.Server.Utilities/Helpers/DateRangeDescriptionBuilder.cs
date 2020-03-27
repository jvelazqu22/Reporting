using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Domain.Helper;
using Domain.Orm.Classes;

namespace iBank.Server.Utilities.Helpers
{
    public class DateRangeDescriptionBuilder
    {
        private readonly List<LanguageVariableInfo> _translations;
        private readonly string _userDateFormat;

        public DateRangeDescriptionBuilder(List<LanguageVariableInfo> translations, string userDateFormat)
        {
            _translations = translations;
            _userDateFormat = userDateFormat;
        }

        public string Build(DateTime begin, DateTime end, string dateRangeParam)
        {
            //TravelGlance has no DATERANGE parameter and should be "Trip Departures from" instead of just blank
            //In case there are other reports like this...
            if (dateRangeParam.IsNullOrWhiteSpace()) return TranslatedDateDesc("xDateDescTripDep", "Trip Departures from", begin, end);

            if (!int.TryParse(dateRangeParam, out var dateRangeType)) return "";

            switch (dateRangeType)
            {
                case (int)DateType.InvoiceDate:
                    return TranslatedDateDesc("xDateDescInv", "Invoice dates from", begin, end);
                case (int)DateType.BookedDate:
                    return TranslatedDateDesc("xDateDescBooked", "Booked dates from", begin, end);
                case (int)DateType.RoutingDepartureDate:
                    return TranslatedDateDesc("xDateDescDep", "Departures from", begin, end);
                case (int)DateType.RoutingArrivalDate:
                    return TranslatedDateDesc("xDateDescArr", "Arrivals from", begin, end);
                case (int)DateType.CarRentalDate:
                    return TranslatedDateDesc("xDateDescRent", "Rental Dates from", begin, end);
                case (int)DateType.HotelCheckInDate:
                    return TranslatedDateDesc("xDateDescCheckin", "Check-in Dates from", begin, end);
                case (int)DateType.TransactionDate:
                    return TranslatedDateDesc("xDateDescTrans", "Transaction Dates from", begin, end);
                case (int)DateType.OnTheRoadDatesSpecial:
                case (int)DateType.OnTheRoadDatesCarRental:
                case (int)DateType.OnTheRoadDatesHotel:
                    return TranslatedDateDesc("xDateDescOnRoad", "On-the-Road Dates from", begin, end);
                case (int)DateType.AuthorizationStatusDate:
                    return TranslatedDateDesc("xDateDescStatus", "Authorization Status Dates from", begin, end);
                case (int)DateType.PostDate:
                    return TranslatedDateDesc("xDateDescPosted", "Posted Dates from", begin, end);
                case (int)DateType.LastUpdate:
                    return TranslatedDateDesc("xDateDescLastUpdated", "Last Updated Dates from", begin, end, true);
                default:
                    return TranslatedDateDesc("xDateDescTripDep", "Trip Departures from", begin, end);
            }
        }
        
        private string TranslatedDateDesc(string translationKey, string defaultDescription, DateTime begin, DateTime end, 
            bool includeHoursAndMinutes = false)
        {
            var translation = GetLanguageTranslation(translationKey, "");
            var beginDate = GetUserFormattedDate(begin, includeHoursAndMinutes);
            var endDate = GetUserFormattedDate(end, includeHoursAndMinutes);

            return translation.IsNullOrWhiteSpace()
                ? $"{defaultDescription} {beginDate} to {endDate}"
                : ReplaceDatePlaceholders(translation, beginDate, endDate);
        }

        private string GetLanguageTranslation(string varName, string defaultCaption)
        {
            var translation = _translations.FirstOrDefault(s => s.VariableName.Equals(varName, StringComparison.InvariantCultureIgnoreCase));
            return translation == null ? defaultCaption : translation.Translation;
        }

        private string GetUserFormattedDate(DateTime dt, bool includeHoursAndMinutes)
        {
            return dt.ToString(includeHoursAndMinutes ? $"{_userDateFormat} HH:mm" : _userDateFormat);
        }

        /// <summary>
        /// Replaces the date placeholders in a translation.
        /// A date description translation looks like "Departure date from [xxxxx] to [xxxxx]"
        /// </summary>
        /// <param name="dateDescriptionTranslation"></param>
        /// <param name="beginDate">The begin date.</param>
        /// <param name="endDate">The end date.</param>
        /// <returns></returns>
        private string ReplaceDatePlaceholders(string dateDescriptionTranslation, string beginDate, string endDate)
        {
            var dateReplaceRegX = new Regex(@"(\[xxxxx\])");

            var afterReplacingBeginPlaceholder = dateReplaceRegX.Replace(dateDescriptionTranslation, beginDate, 1);
            return dateReplaceRegX.Replace(afterReplacingBeginPlaceholder, endDate, 1);
        }
    }
}
