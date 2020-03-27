using System.Linq;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers;
using iBank.Services.Implementation.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined
{
    public class RowCounter
    {
        public static int GetTotalRowsNeeded(int reckey, UserDefinedParameters userDefinedParams, SwitchManager switchManager, UserReportInformation userReport)
        {
            if (userReport.AreAllColumnsInTripTables) return 1;

            var hotelCount = userDefinedParams.HotelLookup[reckey].Count();
            var carCount = userDefinedParams.CarLookup[reckey].Count();
            var legCount = switchManager.LegSwitch ? userDefinedParams.LegLookup[reckey].Count() : 0;
            var miscSegTourCount = userDefinedParams.MiscSegTourLookup[reckey].Count();
            var miscSegCruiseCount = userDefinedParams.MiscSegCruiseLookup[reckey].Count();
            var miscSegLimoCount = userDefinedParams.MiscSegLimoLookup[reckey].Count();
            var miscSegRailCount = userDefinedParams.MiscSegRailTicketLookup[reckey].Count();
            var svcFeeCount= userDefinedParams.ServiceFeeLookup[reckey].Count();
            var changeLogCount = userDefinedParams.ChangeLogLookup[reckey].Count();
            var marketSegCount = switchManager.MarketSegmentsSwitch ? userDefinedParams.MarketSegmentLookup[reckey].Count() : 0;

            return MathHelper.Max(hotelCount, carCount, legCount, miscSegTourCount, miscSegCruiseCount, miscSegLimoCount, miscSegRailCount, svcFeeCount, changeLogCount, marketSegCount);
        }
    }
}

