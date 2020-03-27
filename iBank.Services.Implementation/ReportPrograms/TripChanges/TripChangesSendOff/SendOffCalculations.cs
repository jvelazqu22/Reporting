using CODE.Framework.Core.Utilities.Extensions;
using Domain.Helper;
using Domain.Models.ReportPrograms.TripChangesSendOffReport;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.TripChanges.TripChangesSendOff
{
    public class SendOffCalculations
    {
        public string GetCrystalReportName(bool suppressChangeDetails)
        {
            return suppressChangeDetails ? "ibSendOff2" : "ibSendOff";
        }

        public bool IsDepartureDateRange(ReportGlobals globals)
        {
            return globals.GetParmValue(WhereCriteria.DATERANGE) == "4";
        }

        public bool IncludeCancelledTrips(ReportGlobals globals, string whereClauseChanges, bool buildWhereIncludeCancelled)
        {
            var tripCancelYn = globals.GetParmValue(WhereCriteria.CANCELCODE);
            if (string.IsNullOrEmpty(tripCancelYn))
            {
                return string.IsNullOrEmpty(whereClauseChanges) || buildWhereIncludeCancelled;
            }
            if (tripCancelYn == "Y")
            {
                return true;
            }
            if (tripCancelYn == "N")
            {
                return false;
            }
            // Default to true
            return true;
        }

        public int GetUdidOnReport(ReportGlobals globals, WhereCriteria udidOnReport)
        {
            return globals.GetParmValue(udidOnReport).TryIntParse(0);
        }

        public string GetUdidDescription(int reckey, List<UdidData> udidData, int reportUdidNumber)
        {
            var description = "";

            if (reportUdidNumber > 0)
            {
                var udidInfo = udidData.FirstOrDefault(s => s.RecKey == reckey);
                description = udidInfo != null ? udidInfo.UdidText.Trim() : "";
            }

            return description.PadRight(80);
        }

        public string RemovePipe(string fieldValue)
        {
            // Remove leading or trailing '|' characters
            return fieldValue.Trim().Trim('|');
        }

        public string AddPipe(string valOne, string valTwo)
        {
            return string.Format("{0}|{1}", valOne, valTwo.Trim());
        }

        public string AddPipe(string valOne, DateTime? dateOne)
        {
            return string.Format("{0}|{1}", valOne, dateOne);
        }

        public string GetUdidWhereText(int udidOnReport, string udidLabel)
        {
            if (string.IsNullOrEmpty(udidLabel))
            {
                udidLabel = "Udid # " + udidOnReport + " text:";
            }

            if (!udidLabel.EndsWith(":"))
            {
                udidLabel = udidLabel + ":";
            }

            return udidLabel;
        }

        public string GetBeginHour(ReportGlobals globals)
        {
            var beginHour = globals.GetParmValue(WhereCriteria.BEGHOUR).PadLeft(2, '0');
            int beginHr;
            int.TryParse(beginHour, out beginHr);

            if (!(beginHr >= 1 && beginHr <= 12))
            {
                beginHour = "12";
            }

            return beginHour;
        }

        public string GetBeginMinute(ReportGlobals globals)
        {
            var beginMinute = globals.GetParmValue(WhereCriteria.BEGMINUTE).PadLeft(2, '0');
            int beginMin;
            int.TryParse(beginMinute, out beginMin);

            if (!(beginMin >= 0 && beginMin <= 59))
            {
                beginMinute = "00";
            }

            return beginMinute;
        }

        public string GetBeginAmOrPm(ReportGlobals globals)
        {
            return globals.ParmValueEquals(WhereCriteria.BEGAMPM, "2") ? "PM" : "AM";
        }

        public string GetDateWhereText(string beginHour, string beginMinute, string beginAmOrPm, DateTime beginDate, DateTime endDate)
        {
            if (beginHour != "12" || beginMinute != "00" || beginAmOrPm != "AM")
            {
                return "Changes from " + beginDate.ToShortDateString() + " at " + beginHour +
                                     ":" + beginMinute + " " + beginAmOrPm + " to " +
                                     endDate.ToShortDateString() + " 11:59:59 PM; ";
            }
            else
            {
                return "Changes from " + beginDate.ToShortDateString() + " to " +
                                     endDate.ToShortDateString();
            }
        }

        public void SetNoDataError(ReportGlobals globals)
        {
            globals.ReportInformation.ReturnCode = 2;
            globals.ReportInformation.ErrorMessage = globals.ReportMessages.RptMsg_NoData;
        }

        public string GetUdidColumnOneName(string udidLabelOne)
        {
            if (!string.IsNullOrEmpty(udidLabelOne))
            {
                return SharedProcedures.FixDbfColumnName(udidLabelOne);
            }
            else
            {
                return "UdidText1";
            }
        }

        public string GetUdidColumnTwoName(string udidLabelTwo, string udidColumnOneName)
        {
            var udid2ColumnName = "UdidText2";

            if (!string.IsNullOrEmpty(udidLabelTwo))
            {
                udid2ColumnName = SharedProcedures.FixDbfColumnName(udidLabelTwo);
            }

            if (udidColumnOneName == udid2ColumnName)
            {
                if (udid2ColumnName.Length == 10)
                {
                    udid2ColumnName = udid2ColumnName.EndsWith("2")
                        ? udid2ColumnName.Left(9) + "X"
                        : udid2ColumnName.Trim() + "2";
                }
                else
                {
                    udid2ColumnName = udid2ColumnName + "2";
                }
            }

            return udid2ColumnName;
        }

        /// <summary>
        /// For CSV/Excel export the change description needs to be shortened
        /// </summary>
        /// <param name="changeDescription"></param>
        /// <returns></returns>
        public string ShortenChangeDescription(string changeDescription)
        {
            return changeDescription.Replace("Changed", "Chngd")
                .Replace("Change", "Chng")
                .Replace("Destination", "Dest")
                .Replace("Origin", "Org")
                .Replace("Validating Carrier", "Val Carr")
                .Replace("Departure Date", "Dep Date")
                .Replace("Arrival Date", "Arr Date")
                .Replace("ITINERARY", "Itin")
                .Replace("Seat Assignment", "Seat")
                .Replace("Flight #", "Flt #")
                .Replace("Departure Time", "Dep Time")
                .Replace("Arrival Time", "Arr Time")
                .Replace("Hotel Property", "Hotel");
        }

        public int GetUseChangeFields(int useChangeFields, string originalChangeDescription, string changeDescriptionOne, string changeDescriptionTwo)
        {
            switch (useChangeFields)
            {
                case 1:
                    if (changeDescriptionOne.Length > 50 &&
                        (changeDescriptionOne.Length + originalChangeDescription.Trim().Length > 250))
                        useChangeFields = 2;
                    break;
                case 2:
                    if (changeDescriptionTwo.Length > 50 &&
                        (changeDescriptionTwo.Length + originalChangeDescription.Trim().Length > 250))
                        useChangeFields = 3;
                    break;
            }

            return useChangeFields;
        }

        public bool ChangeDescriptionsNotPresent(string changeDescriptionOne, string changeDescriptionTwo, string changeDescriptionThree,
                                                       string originalChangeDescription)
        {
            return (string.IsNullOrEmpty(changeDescriptionOne.Trim()) || !originalChangeDescription.ContainsIgnoreCase(changeDescriptionOne))
                && (string.IsNullOrEmpty(changeDescriptionTwo.Trim()) || !originalChangeDescription.ContainsIgnoreCase(changeDescriptionTwo))
                && (string.IsNullOrEmpty(changeDescriptionThree.Trim()) || !originalChangeDescription.ContainsIgnoreCase(changeDescriptionThree));
        }

        public List<string> GetExportFields(bool includeEmailAddress, bool useAirportCodes, bool consolidateChanges, int displayFields,
            string udidOneColumnName, string udidTwoColumnName)
        {
            var exportFields = new List<string>();

            exportFields.Add("reckey");
            exportFields.Add("mtggrpnbr");
            exportFields.Add("passlast");
            exportFields.Add("passfrst");
            if (includeEmailAddress)
            {
                exportFields.Add("emailaddr");
            }

            exportFields.Add("origin");

            if (useAirportCodes)
            {
                exportFields.Add("destdesc as destinat");
                exportFields.Add("fstdestdes as firstdest");
            }
            else
            {
                if (!consolidateChanges)
                {
                    exportFields.Add("origdesc");
                }
                exportFields.Add("destdesc");
                exportFields.Add("fstdestdes");
            }

            exportFields.Add("rdepdate");
            exportFields.Add("airline");
            exportFields.Add("alinedesc");
            exportFields.Add("fltno");
            exportFields.Add("arrtime");
            exportFields.Add("deptime");
            exportFields.Add("sorttime");
            exportFields.Add("recloc");
            exportFields.Add("ticketed");
            exportFields.Add("bookdate");

            //The FoxPro code was splitting the changedesc into differnet fields if it exceeded 250 characters but they were never part of the export. 
            //I think this is a bug in FoxPro, so C# version will actually include the split columns in the export
            switch (displayFields)
            {
                case 1:
                    exportFields.Add("changedesc");
                    exportFields.Add("changstamp");
                    break;
                case 2:
                    exportFields.Add("changedesc");
                    exportFields.Add("changstamp");
                    exportFields.Add("changedesc2");
                    exportFields.Add("changstamp2");
                    break;
                case 3:
                    exportFields.Add("changedesc");
                    exportFields.Add("changstamp");
                    exportFields.Add("changedesc2");
                    exportFields.Add("changstamp2");
                    exportFields.Add("changedesc3");
                    exportFields.Add("changstamp3");
                    break;
                default:
                    exportFields.Add("changedesc");
                    exportFields.Add("changstamp");
                    break;
            }

            exportFields.Add("udidnbr1");
            exportFields.Add("udidtext1 AS " + udidOneColumnName);

            exportFields.Add("udidnbr2");
            exportFields.Add("udidtext2 AS " + udidTwoColumnName);

            return exportFields;
        }
    }
}
