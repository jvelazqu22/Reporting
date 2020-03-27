using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Services.Implementation.Utilities;
using System;
using System.Globalization;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupMktSegsFieldHandler
{
    public class GrosAvgCpm : IColumnValue
    {
        private MarketSegmentRawData _marketSegmentRawData;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _marketSegmentRawData = colValRulesParams.MarketSegmentRawData;
        }

        public string CalculateColValue()
        {
            //cbuf2 = "(iif(curMktsegs.miles != 0, abs(round(curMktsegs.grossamt/curMktsegs.miles,2)), 0.00))"
            return _marketSegmentRawData.Miles != 0 && _marketSegmentRawData.Grossamt.HasValue 
                ? Math.Abs(MathHelper.Round(_marketSegmentRawData.Grossamt.Value / _marketSegmentRawData.Miles, 2)).ToString(CultureInfo.InvariantCulture) 
                : "0";
        }
    }
}
