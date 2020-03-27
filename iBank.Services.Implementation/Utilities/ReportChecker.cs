using Domain.Helper;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities.Classes;
using com.ciswired.libraries.CISLogger;

namespace iBank.Services.Implementation.Utilities
{
    public class ReportChecker
    {
        private static readonly com.ciswired.libraries.CISLogger.ILogger LOG =
            new LogManagerWrapper().GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public IMasterDataStore MasterDataStore { get; set; }

        public ReportChecker()
        {
            MasterDataStore = new MasterDataStore();
        }

        public ReportChecker(IMasterDataStore ds)
        {
            MasterDataStore = ds;
        }

        public bool IsOriginCriteriaPopulated(ReportGlobals globals)
        {
            var metro = globals.GetParmValue(WhereCriteria.INMETROORGS);
            if (!string.IsNullOrEmpty(metro)) return true;
            
            var country = globals.GetParmValue(WhereCriteria.INORIGCOUNTRY);
            if (!string.IsNullOrEmpty(country)) return true;
            
            var region = globals.GetParmValue(WhereCriteria.INORIGREGION);
            if (!string.IsNullOrEmpty(region)) return true;
            
            var airportCode = globals.GetParmValue(WhereCriteria.INORGS);
            if (!string.IsNullOrEmpty(airportCode)) return true;
            
            return false;
        }

        public bool IsDestinationCriteriaPopulated(ReportGlobals globals)
        {
            var metro = globals.GetParmValue(WhereCriteria.INMETRODESTS);
            if (!string.IsNullOrEmpty(metro)) return true;

            var country = globals.GetParmValue(WhereCriteria.INDESTCOUNTRY);
            if (!string.IsNullOrEmpty(country)) return true;

            var region = globals.GetParmValue(WhereCriteria.INDESTREGION);
            if (!string.IsNullOrEmpty(region)) return true;

            var airportCode = globals.GetParmValue(WhereCriteria.INDESTS);
            if (!string.IsNullOrEmpty(airportCode)) return true;
            
            return false;
        }
        
        public bool IsAppliedToSegment(ReportGlobals globals)
        {
            var textFlightSegments = globals.GetParmValue(WhereCriteria.TXTFLTSEGMENTS);
            var applyToLegOrSeg = globals.GetParmValue(WhereCriteria.RBAPPLYTOLEGORSEG);

            //if searching by flight segment it will always need to be looking at seg level
            if (!string.IsNullOrEmpty(textFlightSegments)) return true;

            if (applyToLegOrSeg != "2") return false;
            
            return IsOriginCriteriaPopulated(globals) || IsDestinationCriteriaPopulated(globals);
        }
    }
}
