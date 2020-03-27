using CODE.Framework.Core.Utilities.Extensions;

using Domain.Helper;

using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.Utilities
{
    public class GlobalsCalculator
    {
        public ReportGlobals Globals { get; set; }
        public GlobalsCalculator(ReportGlobals globals)
        {
            Globals = globals;
        }
        public bool UseConnectingLegs()
        {
            return Globals.IsParmValueOn(WhereCriteria.CBUSECONNECTLEGS);
        }

        public bool IncludePageBreakByDate()
        {
            return Globals.IsParmValueOn(WhereCriteria.CBINCLUDEPGBRKBYDATE);
        }

        public bool IncludePassengerCountByFlight()
        {
            return Globals.IsParmValueOn(WhereCriteria.CBINCLUDEPAXCOUNTBYFLT);
        }

        public bool IncludeAllLegs()
        {
            return Globals.IsParmValueOn(WhereCriteria.CBINCLUDEALLLEGS);
        }

        public bool IsReservationReport()
        {
            return Globals.ParmValueEquals(WhereCriteria.PREPOST, "1");
        }

        public bool IsBothWays()
        {
            return Globals.ParmValueEquals(WhereCriteria.RBONEWAYBOTHWAYS, "1");
        }

        public int GetUdidNumber()
        {
            var udidNumber = Globals.GetParmValue(WhereCriteria.UDIDNBR);
            return udidNumber.TryIntParse(0);
        }

        public bool IncludeVoids()
        {
            return Globals.IsParmValueOn(WhereCriteria.CBINCLVOIDS);
        }

        public bool UseBaseFare()
        {
            return Globals.IsParmValueOn(WhereCriteria.CBUSEBASEFARE);
        }

        public string GetReasonList()
        {
            return Globals.GetParmValue(WhereCriteria.INREASCODE);
        }

        public string GetReasonCode()
        {
            return Globals.GetParmValue(WhereCriteria.REASCODE);
        }

        public bool IsNotInReason()
        {
            return Globals.IsParmValueOn(WhereCriteria.NOTINREAS);
        }

        public bool IgnoreBreakSettings()
        {
            return Globals.IsParmValueOn(WhereCriteria.CBIGNOREBRKSETTINGS);
        }

        public string GetGroupBy()
        {
            return Globals.GetParmValue(WhereCriteria.GROUPBY);
        }

        public bool UseClassCategory()
        {
            return Globals.IsParmValueOn(WhereCriteria.CBUSECLASSCATS);
        }

        public bool ShowBreakByDomesticInternational()
        {
            return Globals.IsParmValueOn(WhereCriteria.CBSHOWBRKBYDOMINTL);
        }

        public bool IncludeOneWay()
        {
            return Globals.IsParmValueOn(WhereCriteria.CBINCLUDEONEWAY);
        }

        public string GetSortBy()
        {
            return Globals.GetParmValue(WhereCriteria.SORTBY);
        }

        public bool IsAppliedToLegLevelData()
        {            
            return Globals.GetParmValue(WhereCriteria.RBAPPLYTOLEGORSEG) == "1" || Globals.IsParmValueOn(WhereCriteria.CBUSECONNECTLEGS);
        }
    }
}
