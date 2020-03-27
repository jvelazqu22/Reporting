using Domain.Models.ReportPrograms.UserDefinedReport;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class PdFareTf : IColumnValue
    {
        private RawData _mainRec;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
        }

        public string CalculateColValue()
        {
            //Paid Fare Including Taxes and Fees
            return LookupHelpers.GetPaidFare(_mainRec);
        }
    }
}
