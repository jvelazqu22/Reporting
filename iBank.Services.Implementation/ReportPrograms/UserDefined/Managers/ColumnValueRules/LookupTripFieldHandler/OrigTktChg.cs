using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class OrigTktChg : IColumnValue
    {
        private RawData _mainRec;
        private ReportLookups _reportLookups;
        private BuildWhere _buildWhere;
        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
            _reportLookups = colValRulesParams.ReportLookups;
            _buildWhere = colValRulesParams.BuildWhere;
        }

        public string CalculateColValue()
        {
            return _reportLookups.LookupOriginalTicket(_mainRec.Origticket, "AMT", _buildWhere.WhereClauseTrip);
        }
    }
}
