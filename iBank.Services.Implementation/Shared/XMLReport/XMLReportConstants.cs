using System.Collections.Generic;

namespace iBank.Services.Implementation.Shared.XMLReport
{
    public static class XMLReportConstants
    {
        public static List<string> DataTypeList = new List<string> { "Reservation", "Backoffice" };
        public static List<string> OutputToList = new List<string> { "ONLINE", "OFFLINE", "EFFECTS" };
        //not enums between 14-21
        public static List<string> DateRangeTypeList = new List<string> { "Trip Departure", "Invoice", "Booked", "Leg Departure","Leg Arrival", "Car Rental", "Hotel Checkin", "Transaction", "On the road - air", "On the road - car",
                            "On the road - hotel", "Auth status", "Post Date", "", "", "", "", "", "", "", "Last update"};
        public static List<string> DomesticIntList = new List<string> { "", "Domestic Only", "International Only", "Transborder Only", "Exclude Domestic", "Exclude International", "Exclude Transborder" };

    }
}
