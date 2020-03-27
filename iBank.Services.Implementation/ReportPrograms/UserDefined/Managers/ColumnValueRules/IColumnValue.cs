namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules
{
    public interface IColumnValue
    {
        void SetupParams(ColValRulesParams colValRulesParams);
        string CalculateColValue();
    }
}
