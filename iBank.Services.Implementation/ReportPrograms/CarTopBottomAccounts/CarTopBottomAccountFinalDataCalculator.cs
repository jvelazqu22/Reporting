using Domain.Constants;

using iBank.Services.Implementation.Shared;

using System.Collections.Generic;
using System.Linq;

using Domain.Models.ReportPrograms.CarTopBottomAccountsReport;

using iBank.Server.Utilities.Classes;

using Domain.Orm.Classes;

using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Shared.Client;
using iBankDomain.RepositoryInterfaces;

namespace iBank.Services.Implementation.ReportPrograms.CarTopBottomAccounts
{
    public class CarTopBottomAccountFinalDataCalculator
    {
        ClientFunctions _clientFunctions;
        public CarTopBottomAccountFinalDataCalculator(ClientFunctions clientFunctions)
        {
            _clientFunctions = clientFunctions;
        }

        public string GetAccountDescription(IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, IMasterDataStore store, 
            IQuery<IList<MasterAccountInformation>> getAllParentAccountsQuery, string acct, ReportGlobals globals, string groupBy, string sourceAbbr, string agency, string ddGrpField)
        {
            switch (groupBy)
            {
                case ReportFilters.CAR_TOP_BOTTOM_ACCOUNTS_RPT_GROUP_BY_DATA_SOURCE_PARAM_VALUE:
                    return ddGrpField == ReportFilters.CAR_TOP_BOTTOM_ACCOUNTS_RPT_GROUPING_FIELD_ACCT_NUM_PARAM_VALUE
                        ? sourceAbbr.Trim()
                        : LookupFunctions.LookupSourceDescription(store, sourceAbbr.Trim(), agency.Trim());
                case ReportFilters.CAR_TOP_BOTTOM_ACCOUNTS_RPT_GROUP_BY_PARENT_ACCOUNT_PARAM_VALUE:
                    var parent = _clientFunctions.LookupParent(getAllMasterAccountsQuery, acct, getAllParentAccountsQuery);
                    return ddGrpField == ReportFilters.CAR_TOP_BOTTOM_ACCOUNTS_RPT_GROUPING_FIELD_ACCT_NUM_PARAM_VALUE
                        ? parent.AccountDescription + " (" + parent.AccountId.Trim() + ")"
                        : parent.AccountDescription;
                default:
                    return ddGrpField == ReportFilters.CAR_TOP_BOTTOM_ACCOUNTS_RPT_GROUPING_FIELD_ACCT_NUM_PARAM_VALUE
                        ? _clientFunctions.LookupCname(getAllMasterAccountsQuery, acct, globals) + " (" + acct.Trim() + ")"
                        : _clientFunctions.LookupCname(getAllMasterAccountsQuery, acct, globals);
            }
        }

        public string GetAccountNameForExportRawData(IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, IMasterDataStore store,
            IQuery<IList<MasterAccountInformation>> getAllParentAccountsQuery, string acct, ReportGlobals globals, string groupBy, string sourceAbbr, string agency)
        {
            switch (groupBy)
            {
                case ReportFilters.CAR_TOP_BOTTOM_ACCOUNTS_RPT_GROUP_BY_DATA_SOURCE_PARAM_VALUE:
                    return LookupFunctions.LookupSourceDescription(store, sourceAbbr.Trim(), agency.Trim());
                case ReportFilters.CAR_TOP_BOTTOM_ACCOUNTS_RPT_GROUP_BY_PARENT_ACCOUNT_PARAM_VALUE:
                    return _clientFunctions.LookupParent(getAllMasterAccountsQuery, acct, getAllParentAccountsQuery).AccountDescription;
                default:
                    return _clientFunctions.LookupCname(getAllMasterAccountsQuery, acct, globals);
            }
        }

        public List<FinalData> GetFinalDataFromRawData(IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, IMasterDataStore store,
            IQuery<IList<MasterAccountInformation>> getAllParentAccountsQuery, List<RawData> rawDataList, ReportGlobals globals, string groupBy, string agency, string ddGrpField, bool exportToCvs)
        {
            List<FinalData> results = new List<FinalData>();
            if (groupBy == ReportFilters.HOTEL_TOP_BOTTOM_ACCOUNTS_RPT_GROUP_BY_PARENT_ACCOUNT_PARAM_VALUE)
            {
                results = rawDataList.GroupBy(s => new { s.GroupAccount },
                    (key, g) =>
                    {
                        var finalData = new FinalData();
                        finalData.SourceAbbr = g.First().SourceAbbr;
                        if (exportToCvs)
                        {
                            finalData.Account = g.First().SourceAbbr;
                        }
                        else
                        {
                            finalData.Account = GetAccountDescription(getAllMasterAccountsQuery, store, getAllParentAccountsQuery, g.First().Acct, globals, groupBy, g.First().SourceAbbr, agency, ddGrpField);
                        }
                        finalData.acctname = GetAccountNameForExportRawData(getAllMasterAccountsQuery, store, getAllParentAccountsQuery, g.First().Acct, globals, groupBy, g.First().SourceAbbr, agency);
                        finalData.Bookrate = g.Sum(s => s.sumbkrate);
                        finalData.Rentals = g.Sum(s => s.Rentals);
                        finalData.Days = g.Sum(s => s.Days);
                        finalData.Carcost = g.Sum(s => s.CarCost);
                        finalData.sumbkrate = g.Sum(s => s.sumbkrate);
                        finalData.Bookcnt = g.Sum(s => s.ABookRat == 0 ? 0 : s.Rentals);
                        finalData.AccountNumber = g.First().Acct;
                        return finalData;
                    }).ToList();
            }
            else
            {
                results = rawDataList.GroupBy(s => new { s.Acct },
                    (key, g) =>
                    {
                        var finalData = new FinalData();
                        finalData.SourceAbbr = g.First().SourceAbbr;
                        if (exportToCvs)
                        {
                            finalData.Account = groupBy == ReportFilters.CAR_TOP_BOTTOM_ACCOUNTS_RPT_GROUP_BY_DATA_SOURCE_PARAM_VALUE ? finalData.SourceAbbr : g.First().Acct;
                        }
                        else
                        {
                            finalData.Account = GetAccountDescription(getAllMasterAccountsQuery, store, getAllParentAccountsQuery, g.First().Acct, globals, groupBy, g.First().SourceAbbr, agency, ddGrpField);
                        }
                        finalData.acctname = GetAccountNameForExportRawData(getAllMasterAccountsQuery, store, getAllParentAccountsQuery, g.First().Acct, globals, groupBy, g.First().SourceAbbr, agency);
                        finalData.Bookrate = g.Sum(s => s.sumbkrate);
                        finalData.Rentals = g.Sum(s => s.Rentals);
                        finalData.Days = g.Sum(s => s.Days);
                        finalData.Carcost = g.Sum(s => s.CarCost);
                        finalData.sumbkrate = g.Sum(s => s.sumbkrate);
                        finalData.Bookcnt = g.Sum(s => s.ABookRat == 0 ? 0 : s.Rentals);
                        finalData.AccountNumber = g.First().Acct;
                        return finalData;
                    }).ToList();
            }
            foreach(var item in results)
            {
                item.avgbook = item.Bookcnt == 0 ? 0 : item.sumbkrate / item.Bookcnt;
                item.VolumeBooked = item.Days == 0 ? 0 : item.Carcost / item.Days;
            }

            return results;
        }

        public decimal GetFinalListTotalRate(List<FinalData> finalDataList)
        {
            decimal results = 0;
            foreach (var item in finalDataList.ToList())
                results += item.sumbkrate;
            return results;
        }

        public decimal GetFinalListTotalBookCount(List<FinalData> finalDataList)
        {
            decimal results = 0;
            foreach (var item in finalDataList.ToList())
                results += item.Bookcnt;
            return results;
        }
    }
}
