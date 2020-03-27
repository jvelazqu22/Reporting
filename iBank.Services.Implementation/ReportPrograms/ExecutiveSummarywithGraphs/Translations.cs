using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.ExecutiveSummarywithGraphs
{
    public class Translations
    {
        public string xAirCharges { get; set; }
        public string xMiles { get; set; }
        public string xKilometers { get; set; }
        public string xNbrCarsRented { get; set; }
        public string xNbrDaysRented { get; set; }
        public string xNbrOfRoomNights { get; set; }
        public string xNbrHotelBookings { get; set; }
        public string xNbrAirTrans { get; set; }
        public string xNbrRailTrans { get; set; }
        public string xRailCharges { get; set; }
        public string xCarRentalVol { get; set; }
        public string xHotelBkgVol { get; set; }
        public string xRailVolBooked { get; set; }
        public string xAirVolBooked { get; set; }
        public string xAbbrMthsofYear { get; set; }
        public string xKgs { get; set; }
        public string xLbs { get; set; }

        public string xAirCarRanges { get; set; }
        public string xAirHotelRanges { get; set; }
        public string xAirRange { get; set; }
        public string xResData { get; set; }
        public string xBackOffData { get; set; }
        public string xCarHotelRange { get; set; }
        public string xCarRange { get; set; }
        public string xHotelRange { get; set; }
        public string xSvgsBasedOnBaseFare { get; set; }

        public Translations(ReportGlobals globals)
        {
            xAirCharges = LookupFunctions.LookupLanguageTranslation("xAirCharges", "Air Charges", globals.LanguageVariables);
            xMiles = LookupFunctions.LookupLanguageTranslation("xMiles", "Miles", globals.LanguageVariables);
            xKilometers = LookupFunctions.LookupLanguageTranslation("xKilometers", "Kilometers", globals.LanguageVariables);
            xNbrCarsRented = LookupFunctions.LookupLanguageTranslation("xNbrCarsRented", "# of Cars Rented:", globals.LanguageVariables);
            xNbrDaysRented = LookupFunctions.LookupLanguageTranslation("xNbrDaysRented", "# of Days Rented:", globals.LanguageVariables);
            xNbrOfRoomNights = LookupFunctions.LookupLanguageTranslation("xNbrOfRoomNights", "# of Room Nights", globals.LanguageVariables);
            xNbrHotelBookings = LookupFunctions.LookupLanguageTranslation("xNbrHotelBookings", "# of Hotel Bookings", globals.LanguageVariables);
            xNbrAirTrans = LookupFunctions.LookupLanguageTranslation("xNbrAirTrans", "# of Air Transactions", globals.LanguageVariables);
            xNbrRailTrans = LookupFunctions.LookupLanguageTranslation("xNbrRailTrans", "# of Rail Transactions", globals.LanguageVariables);
            xRailCharges = LookupFunctions.LookupLanguageTranslation("xRailCharges", "Rail Charges", globals.LanguageVariables);
            xCarRentalVol = LookupFunctions.LookupLanguageTranslation("xCarRentalVol", "Car Rental Volume", globals.LanguageVariables);
            xHotelBkgVol = LookupFunctions.LookupLanguageTranslation("xHotelBkgVol", "Hotel Bookings Volume", globals.LanguageVariables);
            xRailVolBooked = LookupFunctions.LookupLanguageTranslation("xRailVolBooked", "Rail Volume Booked", globals.LanguageVariables);
            xAirVolBooked = LookupFunctions.LookupLanguageTranslation("xAirVolBooked", "Air Volume Booked", globals.LanguageVariables);
            xAbbrMthsofYear = LookupFunctions.LookupLanguageTranslation("lt_AbbrMthsofYear", "Jan,Feb,Mar,Apr,May,Jun,Jul,Aug,Sep,Oct,Nov,Dec", globals.LanguageVariables);
            xKgs = LookupFunctions.LookupLanguageTranslation("xKgs", "Kgs.", globals.LanguageVariables);
            xLbs = LookupFunctions.LookupLanguageTranslation("xLbs", "Lbs.", globals.LanguageVariables);

            xAirCarRanges = LookupFunctions.LookupLanguageTranslation("xAirCarRanges", "Air Travel and Car Rentals are", globals.LanguageVariables);
            xAirHotelRanges = LookupFunctions.LookupLanguageTranslation("xAirHotelRanges", "Air Travel and Hotel Bookings are", globals.LanguageVariables);
            xAirRange = LookupFunctions.LookupLanguageTranslation("xAirRange", "Air Travel is", globals.LanguageVariables);
            xResData = LookupFunctions.LookupLanguageTranslation("xResData", "Reservation Data", globals.LanguageVariables);
            xBackOffData = LookupFunctions.LookupLanguageTranslation("xBackOffData", "Back Office Data", globals.LanguageVariables);
            xCarHotelRange = LookupFunctions.LookupLanguageTranslation("xCarHotelRange", "Car Rentals and Hotel Bookings are", globals.LanguageVariables);
            xCarRange = LookupFunctions.LookupLanguageTranslation("xCarRange", "Car Rentals are", globals.LanguageVariables);
            xHotelRange = LookupFunctions.LookupLanguageTranslation("xHotelRange", "Hotel Bookings are", globals.LanguageVariables);
            xSvgsBasedOnBaseFare = LookupFunctions.LookupLanguageTranslation("xSvgsBasedOnBaseFare", "Savings / Loss Amounts based on Base Fare", globals.LanguageVariables);
        }
    }
}
