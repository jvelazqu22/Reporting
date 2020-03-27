using Domain.Models.ReportPrograms.UserDefinedReport;
using System;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupLegFieldHandler
{
    public class Miles : IColumnValue
    {
        private LegRawData _legRawData;
        private RawData _mainRec;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _legRawData = colValRulesParams.LegRawData;
            _mainRec = colValRulesParams.MainRec;
        }

        public string CalculateColValue()
        {
            return (Math.Abs(_legRawData.Miles) * _mainRec.PlusMin).ToString();
        }
    }
}
