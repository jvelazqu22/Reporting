using Domain.Constants;
using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.AccountSummary;

namespace iBank.Services.Implementation.ReportPrograms.AccountSummary
{
    public class AccountSummaryRawDataCalculator
    {
         public string GetCrystalReportName(string groupBy)
        {
            var reportName = "ibAcctSum1";
            switch (groupBy)
            {
                case ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_PSEUDOCITY_PARAM_VALUE:
                case ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_BRANCH_PARAM_VALUE:
                case ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_AGEND_ID_PARAM_VALUE:
                    return reportName + "A";
            }
            return reportName;
        }

        public string GetFieldNameBaseOnGroupBy(string groupBy)
        {
            switch (groupBy)
            {
                case ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_PSEUDOCITY_PARAM_VALUE:
                    return "pseudocity";
                case ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_BRANCH_PARAM_VALUE:
                    return "branch";
                case ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_AGEND_ID_PARAM_VALUE:
                    return "agentid";
                default:
                    return "acct";
            }
        }

        public string GetColumnHeaderBaseOnGroupBy(string groupBy)
        {
            switch (groupBy)
            {
                case ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_PSEUDOCITY_PARAM_VALUE:
                    return "Pseudocity";
                case ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_BRANCH_PARAM_VALUE:
                    return "Branch";
                case ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_AGEND_ID_PARAM_VALUE:
                    return "AgentId";
                default:
                    return (groupBy == ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_PARENT_ACCOUNT_PARAM_VALUE)
                        ? "Parent Account"
                        : "Account";
            }
        }

        public List<RawData> GetPreviousYearData(List<RawData> rawDataList)
        {
            foreach (var rawData in rawDataList)
            {
                rawData.PyTrips = rawData.plusmin ?? 0;
                rawData.PyAmt = rawData.airchg ?? 0;
                rawData.CyTrips = 0;
                rawData.CyAmt = 0;
                rawData.Account = rawData.Acct;
            }
            return rawDataList;
        }

        public List<RawData> GetCurrentYearData(List<RawData> rawDataList)
        {
            foreach(var rawData in rawDataList)
            {
                rawData.PyTrips = 0;
                rawData.PyAmt = 0;
                rawData.CyTrips = rawData.plusmin ?? 0;
                rawData.CyAmt = rawData.airchg ?? 0;
                rawData.Account = rawData.Acct;
            }
            return rawDataList;
        }

        public List<IGrouping<string, RawData>> GetGroupRecsBasedOnGroupBy(string groupBy, List<RawData> rawDataList)
        {
            switch (groupBy)
            {
                case ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_PSEUDOCITY_PARAM_VALUE:
                    return rawDataList.GroupBy(s => s.Pseudocity).ToList();
                case ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_BRANCH_PARAM_VALUE:
                    return rawDataList.GroupBy(s => s.Branch).ToList();
                case ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_AGEND_ID_PARAM_VALUE:
                    return rawDataList.GroupBy(s => s.AgentId).ToList();
                default:
                    return rawDataList.GroupBy(s => s.Acct).ToList();
            }
        }

    }
}
