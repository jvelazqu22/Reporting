using Domain;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ReportLokkup;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class FareType : IColumnValue
    {
        private RawData _mainRec;
        private ReportLookups _reportLookups;
        private ReportGlobals _globals;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
            _reportLookups = colValRulesParams.ReportLookups;
            _globals = colValRulesParams.Globals;
        }

        public string CalculateColValue()
        {
            if(Features.FareType.IsEnabled())
            {
                if (_mainRec.PrdFareBas == null) return "Other";

                return _globals.Agency.Equals("GSA")
                    ? FareTypeHandler.LookupGsaFareType(_mainRec.PrdFareBas.Trim())
                    : FareTypeHandler.LookupFareType(_mainRec.PrdFareBas.Trim());
            }
            else
            {
                return _mainRec.PrdFareBas != null
                    ? _reportLookups.LookupFareType(_mainRec.PrdFareBas.Trim())
                    : "Other";
            }
        }
    }
}
