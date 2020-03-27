using System.Linq;
using Domain;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class SegRouteFirstOriginCode : IColumnValue
    {
        private RawData _mainRec;
        private ReportLookups _reportLookups;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
            _reportLookups = colValRulesParams.ReportLookups;
        }

        public string CalculateColValue()
        {
            if (Features.OriginTranslatingUseMode.IsEnabled())
            {
                var ctrycode = LookupFunctions.LookupSegRouteFirstOrigin(_reportLookups.Segs, _mainRec.RecKey);
                return ctrycode.Contains('-') ? ctrycode.Split('-')[0] : ctrycode;
            }
            else
            {
                return LookupFunctions.LookupSegRouteFirstOrigin(_reportLookups.Segs, _mainRec.RecKey);
            }
        }
    }
}
