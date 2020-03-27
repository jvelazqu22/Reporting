namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class TripCntr : IColumnValue
    {
        public void SetupParams(ColValRulesParams colValRulesParams)
        {
        }

        public string CalculateColValue()
        {
            return "000000000000";
        }
    }
}
