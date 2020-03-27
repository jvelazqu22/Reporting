using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Server.Utilities.Classes;
using System;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class AdvPurch : IColumnValue
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
            var defaultDate = new DateTime(1900, 1, 1, 0, 0, 0);
            if (_globals.Agency.Equals("GSA") && (defaultDate == _mainRec.DepDate || defaultDate == _mainRec.Invdate.GetValueOrDefault()))
            {
                return string.Empty;
            }
            else
            {
                return (_mainRec.DepDate.GetValueOrDefault() - _mainRec.Invdate.GetValueOrDefault()).Days.ToString();
            }
        }
    }
}
