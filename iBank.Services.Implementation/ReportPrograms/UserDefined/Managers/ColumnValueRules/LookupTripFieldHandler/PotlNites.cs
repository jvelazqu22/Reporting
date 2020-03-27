using Domain.Models.ReportPrograms.UserDefinedReport;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class PotlNites : IColumnValue
    {
        private RawData _mainRec;
        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
        }

        public string CalculateColValue()
        {
            if (!_mainRec.Tripend.HasValue || !_mainRec.Tripstart.HasValue) return string.Empty;
            var nites = (_mainRec.Tripend.Value - _mainRec.Tripstart.Value);
            return (nites.Days < 99999) ? nites.Days.ToString() : "0";
        }
    }
}
