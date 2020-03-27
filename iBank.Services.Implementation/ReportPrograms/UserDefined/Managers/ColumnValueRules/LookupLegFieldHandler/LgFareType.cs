using Domain;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ReportLokkup;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupLegFieldHandler
{
    public class LgFareType : IColumnValue
    {
        private LegRawData _legRawData;
        private ReportLookups _reportLookups;
        private ReportGlobals _globals;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _legRawData = colValRulesParams.LegRawData;
            _reportLookups = colValRulesParams.ReportLookups;
            _globals = colValRulesParams.Globals;
        }

        public string CalculateColValue()
        {
            if (Features.FareType.IsEnabled())
            {
                if (_legRawData.Farebase == null) return "Other";
                return _globals.Agency.Equals("GSA")
                ? FareTypeHandler.LookupGsaFareType((_legRawData.Farebase.Trim()))
                : FareTypeHandler.LookupFareType(_legRawData.Farebase.Trim());
            }
            else
            {
                return _legRawData.Farebase != null
                ? _reportLookups.LookupFareType(_legRawData.Farebase.Trim())
                : "Other";
            }
        }
    }
}
