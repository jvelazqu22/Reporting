using Domain.Models.ReportPrograms.UserDefinedReport;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupMktSegsFieldHandler
{
    public class BiCityPair : IColumnValue
    {
        private MarketSegmentRawData _marketSegmentRawData;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _marketSegmentRawData = colValRulesParams.MarketSegmentRawData;
        }

        public string CalculateColValue()
        {
            //TODO code may need to change - new lookup function for Bi Directional O&D Code 
            return _marketSegmentRawData.Mktsegboth;
        }
    }
}
