using System.Globalization;
using Domain.Models.ReportPrograms.UserDefinedReport;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class InvYrMo : IColumnValue
    {
        private RawData _mainRec;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
        }

        public string CalculateColValue()
        {
            return (_mainRec.Invdate.GetValueOrDefault().Year * 100 + _mainRec.Invdate.GetValueOrDefault().Month).ToString(CultureInfo.InvariantCulture).PadLeft(6, '0');
        }
    }
}
