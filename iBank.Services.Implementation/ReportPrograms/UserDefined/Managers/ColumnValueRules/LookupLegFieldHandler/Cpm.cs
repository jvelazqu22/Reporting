using Domain.Models.ReportPrograms.UserDefinedReport;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupLegFieldHandler
{
    public class Cpm : IColumnValue
    {
        private RawData _mainRec;
        private LegRawData _legRawData;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _legRawData = colValRulesParams.LegRawData;
            _mainRec = colValRulesParams.MainRec;
        }

        public string CalculateColValue()
        {
            return (_legRawData.Miles == 0) ? "0.00" : (_mainRec.PlusMin * (_legRawData.ActFare + _legRawData.MiscAmt) / _legRawData.Miles).ToString();
        }
    }
}
