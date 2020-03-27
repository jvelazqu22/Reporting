using System.Collections.Generic;

using CODE.Framework.Core.Utilities.Extensions;

using Domain.Helper;

using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.AdvanceBookAir
{
    public class AdvanceBookAirCalculations
    {
        public string GetCrystalReportName(bool isSummaryPageOnly, bool baseChartOnTrips)
        {
            if (isSummaryPageOnly)
            {
                return baseChartOnTrips ? "ibAdvBookSum2" : "ibAdvBookSum";
            }
            else
            {
                return baseChartOnTrips ? "ibAdvanceBook2" : "ibAdvanceBook";
            }
        }

        public int GetNumberDaysInAdvance(ReportGlobals globals, bool isRbInAdvanceRecordsEqualTwo)
        {
            var numberOfDays = globals.GetParmValue(WhereCriteria.NBRDAYSINADVANCE).TryIntParse(1);

            if (isRbInAdvanceRecordsEqualTwo)
            {
                if (numberOfDays < 1)
                {
                    numberOfDays = 1;
                }
                if (numberOfDays > 60)
                {
                    numberOfDays = 60;
                }
            }

            return numberOfDays;
        }

        public bool IsRbInAdvanceRecordsEqualTwo(ReportGlobals globals)
        {
            //TODO: what does this mean - give better name once we know
            return globals.ParmValueEquals(WhereCriteria.RBINADVANCERECORDS, "2");
        }

        public string AppendDayMessageToWhereTexgt(bool isReservationReport, int numberOfDays, string whereText)
        {
            var dayMessage = "";
            if (isReservationReport)
            {
                dayMessage = "Only for trips booked less than " + numberOfDays + " days in advance.";
            }
            else
            {
                dayMessage = "Only for trips purchased less than " + numberOfDays + " days in advance.";
            }

            return string.IsNullOrEmpty(whereText) ? dayMessage
                                                   : whereText.Right(1) == ";" ? string.Format(" {0}", dayMessage)
                                                                               : string.Format("; {0}", dayMessage);
        }

        public bool IsAppliedToSegment(ReportGlobals globals)
        {
            return globals.ParmValueEquals(WhereCriteria.RBAPPLYTOLEGORSEG, "1");
        }

        public bool IsBaseChartOnTrips(ReportGlobals globals)
        {
            return globals.IsParmValueOn(WhereCriteria.CBBASECHARTONTRIPS);
        }

        public bool IsSummaryPageOnly(ReportGlobals globals)
        {
            return globals.IsParmValueOn(WhereCriteria.CBSUMPAGEONLY);
        }

        public bool IsDateRange1(ReportGlobals globals)
        {
            //TODO: what does this date range mean
            return globals.ParmValueEquals(WhereCriteria.DATERANGE, "1");
        }

        public IList<string> GetExportFields(bool isDateRange1, UserBreaks userBreaks, bool accountBreak, UserInformation user)
        {
            var fieldList = new List<string>();

            if (accountBreak)
            {
                fieldList.Add("acct");
                fieldList.Add("acctdesc");
            }
            if (userBreaks.UserBreak1)
            {
                var break1 = string.IsNullOrEmpty(user.Break1Name) ? "break_1" : user.Break1Name;
                fieldList.Add($"break1 as {break1}");
            }
            if (userBreaks.UserBreak2)
            {
                var break2 = string.IsNullOrEmpty(user.Break2Name) ? "break_2" : user.Break2Name;
                fieldList.Add($"break2 as {break2}");
            }
            if (userBreaks.UserBreak3)
            {
                var break3 = string.IsNullOrEmpty(user.Break3Name) ? "break_3" : user.Break3Name;
                fieldList.Add($"break3 as {break3}");
            }

            fieldList.Add("bookcat");
            fieldList.Add("catdesc");
            fieldList.Add("passlast");
            fieldList.Add("passfrst");
            fieldList.Add("reckey");
            fieldList.Add(isDateRange1 ? "bookdate" : "bookdate as invdate");

            fieldList.Add("depdate");
            fieldList.Add("bookdays");
            fieldList.Add("seqno");
            fieldList.Add("origin");
            fieldList.Add("orgdesc");
            fieldList.Add("destinat");
            fieldList.Add("destdesc");
            fieldList.Add("airline");
            fieldList.Add("alinedesc");
            fieldList.Add("classcode");
            fieldList.Add("airchg");

            return fieldList;
        }
    }
}
