using System.Collections.Generic;
using System.Linq;
using Domain.Helper;
using Domain.Models.ReportPrograms.ReportLog;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;

namespace iBank.Services.Implementation.ReportPrograms.ReportLog
{
    public class ProcessDataHandler
    {
        public List<FinalData> GetFinalDataAndUpdateCrystalReportName(List<RawData> rawDataList, ReportGlobals globals, ref string crystalReportName, List<ReportLogCriteria> criteria, BuildWhere buildWhere)
        {
            return globals.ParmValueEquals(WhereCriteria.RBRPTVERSION, "1")
                ? GetFinalDataAndUpdateCrystalReportNameForSummaryReport(rawDataList, globals, ref crystalReportName, criteria, buildWhere)
                : GetFinalDataAndUpdateCrystalReportNameForDetailReport(rawDataList, globals, ref crystalReportName, criteria, buildWhere);
        }

        public List<FinalData> GetFinalDataAndUpdateCrystalReportNameForSummaryReport(List<RawData> rawDataList, ReportGlobals globals, ref string crystalReportName, List<ReportLogCriteria> criteria, BuildWhere buildWhere)
        {
            var finalDataList = rawDataList.Select(s => new FinalData
            {
                UserName = s.UserName,
                Caption = LookupProcess.LookupProcessCaption(s.ProcessKey, s.RptProgram, buildWhere)
            })
                .GroupBy(s => new { s.UserName, s.Caption }, (key, recs) => new FinalData
                {
                    UserName = key.UserName,
                    Caption = key.Caption,
                    TimesRun = recs.Count()
                })
                .OrderBy(s => s.Caption)
                .ThenBy(s => s.UserName)
                .ToList();
            crystalReportName += "1";

            return finalDataList;
        }

        public List<FinalData> GetFinalDataAndUpdateCrystalReportNameForDetailReport(List<RawData> rawDataList, ReportGlobals globals, ref string crystalReportName, List<ReportLogCriteria> criteria, BuildWhere buildWhere)
        {
            var finalDataList = new List<FinalData>();

            foreach (var row in rawDataList)
            {
                var newRow = new FinalData
                {
                    Caption = LookupProcess.LookupProcessCaption(row.ProcessKey, row.RptProgram, buildWhere),
                    RptDate = row.RptDate,
                    UserName = row.UserName
                };

                var begDate = string.Empty;
                var endDate = string.Empty;

                var crit = criteria.FirstOrDefault(s => s.RptLogNo == row.RptLogNo && s.VarName.EqualsIgnoreCase("BEGDATE"));

                if (crit != null) begDate = crit.VarValue.Trim();

                crit = criteria.FirstOrDefault(s => s.RptLogNo == row.RptLogNo && s.VarName.EqualsIgnoreCase("ENDDATE"));

                if (crit != null) endDate = crit.VarValue.Trim();

                if (!string.IsNullOrEmpty(begDate) || !string.IsNullOrEmpty(endDate))
                {
                    if (string.IsNullOrEmpty(begDate)) begDate = endDate;
                    if (string.IsNullOrEmpty(endDate)) endDate = begDate;

                    newRow.DateRange = begDate + "-" + endDate;
                }

                crit = criteria.FirstOrDefault(s => s.RptLogNo == row.RptLogNo && s.VarName.EqualsIgnoreCase("INACCT"));

                if (crit != null)
                {
                    newRow.Accts = crit.VarValue;
                    crit = criteria.FirstOrDefault(s => s.RptLogNo == row.RptLogNo && s.VarName.EqualsIgnoreCase("NOTINACCT"));
                    if (crit != null && crit.VarValue.EqualsIgnoreCase("ON")) newRow.Accts = "NOT " + newRow.Accts;
                }

                finalDataList.Add(newRow);
            }

            finalDataList = finalDataList.OrderBy(s => s.Caption)
                .ThenBy(s => s.UserName)
                .ThenBy(s => s.RptDate)
                .ToList();

            crystalReportName += "2";

            return finalDataList;
        }
    }
}
