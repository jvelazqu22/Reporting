using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Server.Utilities.Classes;
using System;
using UserDefinedReports;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupLegFieldHandler
{
    public class FlDuration : IColumnValue
    {
        private LegRawData _legRawData;
        private RawData _mainRec;
        private ReportGlobals _globals;
        private UserReportColumnInformation _column;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _legRawData = colValRulesParams.LegRawData;
            _mainRec = colValRulesParams.MainRec;
            _globals = colValRulesParams.Globals;
            _column = colValRulesParams.Column;
        }

        public string CalculateColValue()
        {
            var defaultDate = new DateTime(1900, 1, 1, 0, 0, 0);
            if (_globals.Agency.Equals("GSA"))
            {
                if (_mainRec.DepDate == null || _mainRec.Arrdate == null) return string.Empty;
                if (defaultDate == _mainRec.DepDate || defaultDate == _mainRec.Arrdate) return string.Empty;
            }
            return ReportBuilder.GetValueAsString(_legRawData, _column.Name);
        }
    }
}
