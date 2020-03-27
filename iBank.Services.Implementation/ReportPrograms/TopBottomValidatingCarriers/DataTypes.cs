namespace iBank.Services.Implementation.ReportPrograms.TopBottomValidatingCarriers
{
    public class DataTypes
    {
        public enum GroupBy
        {
            VALIDATING_CARRIER_ONLY = 1,
            HOME_COUNTRY_VALIDATING_CARRIER,
            VALIDATING_CARRIER_HOME_COUNTRY
        }
        public enum SortBy
        {
            VOLUME_BOOKED = 1,
            AVG_COST_PER_TRIP,
            NO_OF_TRIPS,
            CARRIER_HOME_COUNTRY
        }

        public enum Sort
        {
            Descending=1,
            Ascending
        }
    }
}
