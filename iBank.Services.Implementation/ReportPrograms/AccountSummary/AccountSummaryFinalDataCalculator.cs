using Domain.Constants;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.AccountSummary;
using Domain.Orm.Classes;
using iBank.Services.Implementation.Shared.Client;
using iBankDomain.RepositoryInterfaces;

namespace iBank.Services.Implementation.ReportPrograms.AccountSummary
{
    public class AccountSummaryFinalDataCalculator
    {
        public void AddFinalDataRecordsByPseudoCityBranchOrAgenIdForXlsOrCvs(string groupBy, List<IGrouping<string, RawData>> groupedRecs, List<FinalData> finalDataList)
        {
            foreach (var group in groupedRecs)
            {
                var firstItem = group.FirstOrDefault();
                if (firstItem == null) continue;
                finalDataList.Add(new FinalData
                {
                    Acct = GetAcctString(firstItem, groupBy),
                    AcctDesc = string.Empty,
                    PyTrips = group.Sum(s => s.PyTrips),
                    PyAmt = group.Sum(s => s.PyAmt),
                    CyTrips = group.Sum(s => s.CyTrips),
                    CyAmt = group.Sum(s => s.CyAmt)
                });
            }
        }

        public void AddFinalDataRecordsNogGroupByPseudoCityBranchOrAgenIdForXlsOrCvs(string groupBy, List<IGrouping<string, RawData>> groupedRecs, List<FinalData> finalDataList)
        {
            foreach (var group in groupedRecs)
            {
                var firstItem = group.FirstOrDefault();
                if (firstItem == null) continue;
                finalDataList.Add(new FinalData
                {
                    Acct = string.Empty,
                    AcctDesc = GetAcctString(firstItem, groupBy),
                    PyTrips = group.Sum(s => s.PyTrips),
                    PyAmt = group.Sum(s => s.PyAmt),
                    CyTrips = group.Sum(s => s.CyTrips),
                    CyAmt = group.Sum(s => s.CyAmt)
                });
            }
        }

        public List<FinalData> GetFinalDataFromRawData(IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, IQuery<IList<MasterAccountInformation>> getAllParentAccountsQuery, 
            List<RawData> rawDataList, string outputType, string groupBy, List<IGrouping<string, RawData>> groupedRecs, ClientFunctions clientFunctions, ReportGlobals globals, 
            string columnHeader)
        {
            List<FinalData> finalDataList = new List<FinalData>();
            switch (groupBy)
            {
                case ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_PSEUDOCITY_PARAM_VALUE:
                case ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_BRANCH_PARAM_VALUE:
                case ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_AGEND_ID_PARAM_VALUE:
                    if (outputType == OutputType.XLS || outputType == OutputType.CVS)
                    {
                        AddFinalDataRecordsByPseudoCityBranchOrAgenIdForXlsOrCvs(groupBy, groupedRecs, finalDataList);                    
                    }
                    else
                    {
                        AddFinalDataRecordsNogGroupByPseudoCityBranchOrAgenIdForXlsOrCvs(groupBy, groupedRecs, finalDataList);
                    }
                    break;
                default:
                    foreach (var group in groupedRecs)
                    {
                        var firstItem = group.FirstOrDefault();
                        if (firstItem == null) continue;
                        if (columnHeader == "Parent Account")
                        {
                            AddFinalDataRecordsUsingParentAccountDescription(getAllMasterAccountsQuery, getAllParentAccountsQuery, group, clientFunctions, finalDataList);
                        }
                        else
                        {
                            AddFinalDataRecordsUsingAccountDescription(getAllMasterAccountsQuery, group, clientFunctions, finalDataList, firstItem, globals);
                        }
                    }
                    break;
            }
            return finalDataList;
        }

        public void AddFinalDataRecordsUsingParentAccountDescription(IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, IQuery<IList<MasterAccountInformation>> getAllParentAccountsQuery,
                        IGrouping<string, RawData> group, ClientFunctions clientFunctions, List<FinalData> finalDataList)
        {
            var parent = clientFunctions.LookupParent(getAllMasterAccountsQuery, group.Key, getAllParentAccountsQuery);
            finalDataList.Add(new FinalData
            {
                Acct = parent.AccountId,
                AcctDesc = parent.AccountDescription,
                PyTrips = group.Sum(s => s.PyTrips),
                PyAmt = group.Sum(s => s.PyAmt),
                CyTrips = group.Sum(s => s.CyTrips),
                CyAmt = group.Sum(s => s.CyAmt)
            });
        }

        public void AddFinalDataRecordsUsingAccountDescription(IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, IGrouping<string, RawData> group, 
            ClientFunctions clientFunctions, List<FinalData> finalDataList, RawData firstItem, ReportGlobals globals)
        {
            var finalData = new FinalData();
            finalData.Acct = firstItem.Acct;
            finalData.AcctDesc = clientFunctions.LookupCname(getAllMasterAccountsQuery, firstItem.Acct, globals);
            finalData.PyTrips = group.Sum(s => s.PyTrips);
            finalData.PyAmt = group.Sum(s => s.PyAmt);
            finalData.CyTrips = group.Sum(s => s.CyTrips);
            finalData.CyAmt = group.Sum(s => s.CyAmt);
            finalDataList.Add(finalData);
        }

        public string GetAcctString(RawData item, string groupBy)
        {
            switch (groupBy)
            {
                case ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_PSEUDOCITY_PARAM_VALUE:
                    return item.Pseudocity;
                case ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_BRANCH_PARAM_VALUE:
                    return item.Branch;
                case ReportFilters.ACCOUNT_SUMMARY_RPT_GROUP_BY_AGEND_ID_PARAM_VALUE:
                    return item.AgentId;
            }
            return string.Empty;
        }

        public List<FinalData> RegroupParentAccount(List<FinalData> finalDataList)
        {
            return finalDataList
                .GroupBy(s => new { s.AcctDesc },
                    (key, g) =>
                    {
                        var finalData = new FinalData();
                        finalData.Acct = g.First().Acct;
                        finalData.AcctDesc = g.First().AcctDesc;
                        finalData.PyTrips = g.Sum(x=>x.PyTrips);
                        finalData.PyAmt = g.Sum(s => s.PyAmt);
                        finalData.CyTrips = g.Sum(s => s.CyTrips);
                        finalData.CyAmt = g.Sum(s => s.CyAmt);
                        return finalData;
                    }).ToList();
        }
    }
}
