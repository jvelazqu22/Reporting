using Domain.Models.ReportPrograms.UserDefinedReport;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class InvYr : IColumnValue
    {
        private RawData _mainRec;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
        }

        public string CalculateColValue()
        {
            return _mainRec.Invdate.GetValueOrDefault().Year.ToString().PadLeft(4, '0');
        }
    }
}
