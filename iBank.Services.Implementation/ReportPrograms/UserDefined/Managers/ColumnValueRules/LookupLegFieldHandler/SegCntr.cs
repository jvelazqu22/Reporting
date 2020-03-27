namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupLegFieldHandler
{
    public class SegCntr : IColumnValue
    {
        public void SetupParams(ColValRulesParams colValRulesParams)
        {
        }

        public string CalculateColValue()
        {
            return "000";
        }
    }
}
