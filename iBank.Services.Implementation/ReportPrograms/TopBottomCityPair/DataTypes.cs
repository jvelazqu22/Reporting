namespace iBank.Services.Implementation.ReportPrograms.TopBottomCityPair
{
    public class DataTypes
    {
        public enum TreatMarkets
        {
            BI_DIRECTIONAL = 1,
            ONE_WAY
        }
        public enum SortBy
        {
            VOLUME_BOOKED = 1,
            AVG_COST_PER_SEGMENT_OR_TRIP,
            NO_OF_SEGMENT_OR_TRIP,
            CITY_PAIR
        }

        public enum Sort
        {
            DESCENDING = 1,
            ASCENDING
        }
    }
}
