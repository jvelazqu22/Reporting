using Domain.Models.ReportPrograms.UserDefinedReport;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupMktSegsRailTickerFieldHandler
{
    public class RalMeals : IColumnValue
    {
        private MiscSegSharedRawData _miscSegSharedRawData;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _miscSegSharedRawData = colValRulesParams.MiscSegSharedRawData;
        }

        public string CalculateColValue()
        {
            //cbuf2 = "curRalsegs.mealdesc"
            return _miscSegSharedRawData.Mealdesc;
        }
    }
}
