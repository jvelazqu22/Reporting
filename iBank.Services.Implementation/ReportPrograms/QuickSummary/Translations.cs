using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.QuickSummary
{
    public class Translations
    {
        public Translations(ReportGlobals globals, bool splitRail)
        {
            xAir = splitRail
                ? LookupFunctions.LookupLanguageTranslation("lm_Air", "Air", globals.LanguageVariables)
                : LookupFunctions.LookupLanguageTranslation("ll_BothAirRail", "Air & Rail", globals.LanguageVariables);
            xNbrOfTrips = LookupFunctions.LookupLanguageTranslation("xNbrOfTrips", "# of Trips:", globals.LanguageVariables);
            xAirCharges = LookupFunctions.LookupLanguageTranslation("xAirCharges", "Air Charges", globals.LanguageVariables);
            xSavings = LookupFunctions.LookupLanguageTranslation("xSavings", "Savings:", globals.LanguageVariables);
            xNbrOfExcepts = LookupFunctions.LookupLanguageTranslation("xNbrOfExcepts", "# of Exceptions:", globals.LanguageVariables);
            xLostSvgs = LookupFunctions.LookupLanguageTranslation("xLostSvgs", "Lost Savings:", globals.LanguageVariables);
            xNegoSvgs = LookupFunctions.LookupLanguageTranslation("xNegoSvgs", "Negotiated Savings:", globals.LanguageVariables);
            xRail = LookupFunctions.LookupLanguageTranslation("xRail", "Rail", globals.LanguageVariables);
            xCarRental = LookupFunctions.LookupLanguageTranslation("xCarRental", "Car Rental", globals.LanguageVariables);
            xNbrRentals = LookupFunctions.LookupLanguageTranslation("xNbrRentals", "# of Cars Rented:", globals.LanguageVariables);
            xNbrDays = LookupFunctions.LookupLanguageTranslation("xNbrDays", "# of Days Rented:", globals.LanguageVariables);
            xCostBkRate = LookupFunctions.LookupLanguageTranslation("xCostBkRate", "Cost (Booked Rate):", globals.LanguageVariables);
            xCostPerDay = LookupFunctions.LookupLanguageTranslation("xCostPerDay", "Cost per Day:", globals.LanguageVariables);
            xExcpnAmtLost = LookupFunctions.LookupLanguageTranslation("xExcpnAmtLost", "Exception Amt Lost:", globals.LanguageVariables);
            xHotelBkgs = LookupFunctions.LookupLanguageTranslation("xHotelBkgs", "Hotel Bookings", globals.LanguageVariables);
            xNbrBkngs = LookupFunctions.LookupLanguageTranslation("xNbrBkngs", "# of Bookings:", globals.LanguageVariables);
            xNbrNights = LookupFunctions.LookupLanguageTranslation("xNbrNights", "# of Nights Booked:", globals.LanguageVariables);
            xCostPerNight = LookupFunctions.LookupLanguageTranslation("xCostPerNight", "Cost per RoomNight:", globals.LanguageVariables);
            xRptTotals = LookupFunctions.LookupLanguageTranslation("xRptTotals", "Report Totals", globals.LanguageVariables);
            xTotCharges = LookupFunctions.LookupLanguageTranslation("xTotCharges", "Total Charges:", globals.LanguageVariables);
            xTotExcepts = LookupFunctions.LookupLanguageTranslation("xTotExcepts", "Total Exceptions:", globals.LanguageVariables);
            xTotLost = LookupFunctions.LookupLanguageTranslation("xTotLost", "Total Exception Amt Lost:", globals.LanguageVariables);
        }

        public string xAir { get; set; }
        public string xNbrOfTrips { get; set; }
        public string xAirCharges { get; set; }
        public string xSavings { get; set; }
        public string xNbrOfExcepts { get; set; }
        public string xLostSvgs { get; set; }
        public string xNegoSvgs { get; set; }

        public string xRail { get; set; }

        public string xCarRental { get; set; }
        public string xNbrRentals { get; set; }
        public string xNbrDays { get; set; }
        public string xCostBkRate { get; set; }
        public string xCostPerDay { get; set; }
        public string xExcpnAmtLost { get; set; }

        public string xHotelBkgs { get; set; }
        public string xNbrBkngs { get; set; }
        public string xNbrNights { get; set; }
        public string xCostPerNight { get; set; }

        public string xRptTotals { get; set; }
        public string xTotCharges { get; set; }
        public string xTotExcepts { get; set; }
        public string xTotLost { get; set; }
    }
}
