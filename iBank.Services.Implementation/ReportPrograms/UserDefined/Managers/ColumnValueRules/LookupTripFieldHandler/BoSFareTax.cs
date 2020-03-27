using System.Globalization;
using Domain.Models.ReportPrograms.UserDefinedReport;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class BoSFareTax : IColumnValue
    {
        private RawData _mainRec;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
        }

        public string CalculateColValue()
        {
            //Taxes of the Segment
            return LookupHelpers.GetTotalTaxes(_mainRec).ToString(CultureInfo.InvariantCulture);
        }
    }
}
