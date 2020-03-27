using Domain.Models.ReportPrograms.WeeklyTravelerActivity;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.WeeklyTravelerActivity
{
    public class WeeklyTravelerActivityData
    {
        public  List<ExcelExportData> GetExportData(List<FinalData> FinalDataList, List<UdidData> ReportSettingsUdidInfoList)
        {
            var results = new List<ExcelExportData>();

            var grouped = FinalDataList.GroupBy(s => s.Reckey);
            foreach (var group in grouped)
            {
                //get information related to all records
                var firstRec = group.FirstOrDefault();
                if (firstRec == null) continue;

                var newRec = new ExcelExportData
                {
                    PassFrst = firstRec.PassFrst,
                    PassLast = firstRec.PassLast,
                    Ticket = firstRec.Ticket,
                    InvDate = firstRec.InvDate,
                    RecLoc = firstRec.RecLoc,
                    BreaksFld = firstRec.BreaksFld
                };

                //exclude any udids that weren't found (the udid no will have been set to -1)
                var udidNumbers = ReportSettingsUdidInfoList.Select(s => s.UdidNo).OrderBy(s => s).ToList();

                foreach (var item in group)
                {
                    newRec.Day1 += (newRec.Day1.IsNullOrWhiteSpace() || item.Day1Locn.IsNullOrWhiteSpace() ? "" : " - ") + item.Day1Locn;
                    newRec.Day2 += (newRec.Day2.IsNullOrWhiteSpace() || item.Day2Locn.IsNullOrWhiteSpace() ? "" : " - ") + item.Day2Locn;
                    newRec.Day3 += (newRec.Day3.IsNullOrWhiteSpace() || item.Day3Locn.IsNullOrWhiteSpace() ? "" : " - ") + item.Day3Locn;
                    newRec.Day4 += (newRec.Day4.IsNullOrWhiteSpace() || item.Day4Locn.IsNullOrWhiteSpace() ? "" : " - ") + item.Day4Locn;
                    newRec.Day5 += (newRec.Day5.IsNullOrWhiteSpace() || item.Day5Locn.IsNullOrWhiteSpace() ? "" : " - ") + item.Day5Locn;
                    newRec.Day6 += (newRec.Day6.IsNullOrWhiteSpace() || item.Day6Locn.IsNullOrWhiteSpace() ? "" : " - ") + item.Day6Locn;
                    newRec.Day7 += (newRec.Day7.IsNullOrWhiteSpace() || item.Day7Locn.IsNullOrWhiteSpace() ? "" : " - ") + item.Day7Locn;

                    if (udidNumbers.Count == 0 || !udidNumbers.Contains((short)item.UdidNo)) continue;

                    var index = udidNumbers.IndexOf((short)item.UdidNo) + 1;

                    switch (index)
                    {
                        case 1:
                            newRec.Udid1 = item.UdidText;
                            break;
                        case 2:
                            newRec.Udid2 = item.UdidText;
                            break;
                        case 3:
                            newRec.Udid3 = item.UdidText;
                            break;
                        case 4:
                            newRec.Udid4 = item.UdidText;
                            break;
                        case 5:
                            newRec.Udid5 = item.UdidText;
                            break;
                        case 6:
                            newRec.Udid6 = item.UdidText;
                            break;
                        case 7:
                            newRec.Udid7 = item.UdidText;
                            break;
                        case 8:
                            newRec.Udid8 = item.UdidText;
                            break;
                        case 9:
                            newRec.Udid9 = item.UdidText;
                            break;
                        case 10:
                            newRec.Udid10 = item.UdidText;
                            break;
                        default:
                            throw new Exception(string.Format("Invalid UDID #{0}, Label {1}", item.UdidNo, item.UdidLabel));
                    }
                }
                results.Add(newRec);
            }
            return results;
        }
        public List<string> GetExportFields(CommaDelimitedStringCollection ReportSettingsUdidsString, List<UdidData> ReportSettingsUdidInfoList, DateTime BeginDate)
        {
            var fieldList = new List<string> { "passlast", "passfrst", "ticket", "invdate", "recloc", "breaksfld as rptbreaks" };
            if (ReportSettingsUdidsString.Count > 0)
            {
                //Create one column for each report setting UDID
                var udidCounter = 1;
                foreach (var udidInfo in ReportSettingsUdidInfoList)
                {
                    var udidLabel = SharedProcedures.FixDbfColumnName(udidInfo.UdidLabel).ToLowerInvariant();
                    var i = fieldList.Count(s => s == udidLabel); // Check for duplicate columns
                    fieldList.Add("Udid" + udidCounter++ + " as " + udidLabel + (i == 0 ? string.Empty : "_" + i));
                }
            }
            //Note: no language translation for this report, as of this writing on 2016.02.17
            //Will have to be refactored if translation is added.
            fieldList.Add("day1 as " + BeginDate.DayOfWeek.ToString().ToLowerInvariant());
            fieldList.Add("day2 as " + BeginDate.AddDays(1).DayOfWeek.ToString().ToLowerInvariant());
            fieldList.Add("day3 as " + BeginDate.AddDays(2).DayOfWeek.ToString().ToLowerInvariant());
            fieldList.Add("day4 as " + BeginDate.AddDays(3).DayOfWeek.ToString().ToLowerInvariant());
            fieldList.Add("day5 as " + BeginDate.AddDays(4).DayOfWeek.ToString().ToLowerInvariant());
            fieldList.Add("day6 as " + BeginDate.AddDays(5).DayOfWeek.ToString().ToLowerInvariant());
            fieldList.Add("day7 as " + BeginDate.AddDays(6).DayOfWeek.ToString().ToLowerInvariant());

            return fieldList;
        }

        public string GetCrystalReportName(List<UdidData> reportSettingsUdidDataList, bool suppressReportBreaks)
        {
            var crystalReportName = (reportSettingsUdidDataList == null || reportSettingsUdidDataList.Count == 0) ? "ibWeeklyActivity2" : "ibWeeklyActivity";
            if (suppressReportBreaks) crystalReportName += "A";

            return crystalReportName;
        }

    }
}
