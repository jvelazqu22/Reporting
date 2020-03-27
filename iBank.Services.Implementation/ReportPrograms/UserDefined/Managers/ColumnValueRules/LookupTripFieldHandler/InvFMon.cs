using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class InvFMon : IColumnValue
    {
        private RawData _mainRec;
        private ReportGlobals _globals;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
            _globals = colValRulesParams.Globals;
        }

        public string CalculateColValue()
        {
            //First of Month - Invoice Date (3/18/17 would be 3/1/17)
            return _mainRec.Invdate.GetValueOrDefault().GetDateFirstOfMonth(_globals.DateDisplay);
        }
    }
}
