using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Server.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class InvFyr : IColumnValue
    {
        private RawData _mainRec;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
        }

        public string CalculateColValue()
        {
            //10/1/16-9/30/17 = 2017, 10/1/17-9/30/18=2018, etc
            return _mainRec.Invdate.GetValueOrDefault().GetDateFiscalYear();
        }
    }
}
