using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Server.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class PaxNameAbr : IColumnValue
    {
        private RawData _mainRec;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
        }

        public string CalculateColValue()
        {
            return _mainRec.Passfrst.Trim().Left(1) + ". " + _mainRec.Passlast.Trim();
        }
    }
}
