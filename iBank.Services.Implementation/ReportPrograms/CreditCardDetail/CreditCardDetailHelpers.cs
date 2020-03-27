namespace iBank.Services.Implementation.ReportPrograms.CreditCardDetail
{
    public static class CreditCardDetailHelpers
    {
        public static string LookupPurchaseType(string recType)
        {
            switch (recType)
            {
                case "AIR":
                    return "Air Ticket";
                case "CAR":
                    return "Car Rental";
                case "HOTEL":
                    return "Hotel Booking";
                case "MISC":
                    return "Miscellaneous";
                case "SVCFEE":
                    return "Service Fee";
                default:
                    return "Unspecified";
            }
        }

        public static string BuildDateDesc(string dateDesc, string ccCompany)
        {
            switch (ccCompany)
            {
                case "MC":
                    return "Mastercard - " + dateDesc;
                case "AX":
                    return "American Express - " + dateDesc;
                case "DC":
                    return "Diner's Club - " + dateDesc;
                case "DI":
                    return "Discover - " + dateDesc;
                case "TP":
                    return "UATP Card - " + dateDesc;
                case "VI":
                    return "Visa - " + dateDesc;
                case "JC":
                    return "JCB Visa - " + dateDesc;
                default:
                    return dateDesc;
            }
        }
    }
}
