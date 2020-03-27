using Domain.Models.ReportPrograms.UserDefinedReport;
using System.Globalization;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupMktSegsRailTickerFieldHandler
{
    public class RalArrDate : IColumnValue
    {
        private MiscSegSharedRawData _miscSegSharedRawData;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _miscSegSharedRawData = colValRulesParams.MiscSegSharedRawData;
        }

        public string CalculateColValue()
        {
            //cbuf2 = "curRalsegs.msarrdate"
            return _miscSegSharedRawData.Msarrdate.GetValueOrDefault().ToString(CultureInfo.InvariantCulture);
        }
    }
}
