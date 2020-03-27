using System.Globalization;
using Domain;
using Domain.Helper;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler
{
    public class TransId : IColumnValue
    {
        private RawData _mainRec;
        private ReportGlobals _globals;
        private ReportLookups _reportLookups;
        private SegmentOrLeg _segmentOrLeg;

        public void SetupParams(ColValRulesParams colValRulesParams)
        {
            _mainRec = colValRulesParams.MainRec;
            _globals = colValRulesParams.Globals;
            _reportLookups = colValRulesParams.ReportLookups;
            _segmentOrLeg = colValRulesParams.SegmentOrLeg;
        }

        public string CalculateColValue()
        {
            if (Features.RoutingUseTripsDerivedDataTable.IsEnabled())
            {
                if (!_globals.Agency.Equals("GSA")) return _mainRec.RecKey.ToString(CultureInfo.InvariantCulture);

                if (_segmentOrLeg == SegmentOrLeg.Leg)
                {
                    return LookupFunctions.LookupTransId(_reportLookups.Legs, _mainRec.RecKey);
                }
                else
                {
                    return LookupFunctions.LookupTransId(_reportLookups.Segs, _mainRec.RecKey);
                }
            }
            else
            {
                //reckey/Transaction ID
                if (!_globals.Agency.Equals("GSA")) return _mainRec.RecKey.ToString(CultureInfo.InvariantCulture);
                if (!Features.GsaTripTransactionIdFeatureFlag.IsEnabled()) return _mainRec.RecKey.ToString(CultureInfo.InvariantCulture);
                return TripDerivedDataLookup.GetTripTransactionId(_mainRec.RecKey, _reportLookups);
            }
        }
    }
}
