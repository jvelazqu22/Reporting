using Domain.Models.ReportPrograms.UserDefinedReport;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class TktUsed : IColumnValue
    {
        private RawData _mainRec;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
        }

        public string CalculateColValue()
        {
            //# of Tickets (Refund = -1, Exchange = 0, Regular = 1) 
            return LookupHelpers.GetNumberOfTickets(_mainRec.Trantype.Trim(), _mainRec.Exchange);
        }
    }
}
