using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Server.Utilities.Classes;
using System;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupMktSegsFieldHandler
{
    public class MkDuration : IColumnValue
    {
        private MarketSegmentRawData _marketSegmentRawData;
        private RawData _mainRec;
        private ReportGlobals _globals;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _marketSegmentRawData = colValRulesParams.MarketSegmentRawData;
            _mainRec = colValRulesParams.MainRec;
            _globals = colValRulesParams.Globals;
        }

        public string CalculateColValue()
        {
            var defaultDate = new DateTime(1900, 1, 1, 0, 0, 0);
            if (_globals.Agency.Equals("GSA"))
            {
                if (_mainRec.DepDate == null || _mainRec.Arrdate == null) return string.Empty;
                if (defaultDate == _mainRec.DepDate || defaultDate == _mainRec.Arrdate) return string.Empty;
            }
            //MKDURATION = (h)ibMktSegs.flduration
            return _marketSegmentRawData.Flduration.ToString();
        }
    }
}
