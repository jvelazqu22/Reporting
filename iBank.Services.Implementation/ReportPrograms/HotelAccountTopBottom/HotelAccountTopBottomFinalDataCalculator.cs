using Domain.Constants;
using Domain.Models.ReportPrograms.HotelAccountTopBottom;
using Domain.Orm.Classes;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBankDomain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Linq;

using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Shared.Client;

namespace iBank.Services.Implementation.ReportPrograms.HotelAccountTopBottom
{
    public class HotelAccountTopBottomFinalDataCalculator
    {
        ClientFunctions _clientFunctions;
        public HotelAccountTopBottomFinalDataCalculator(ClientFunctions clientFunctions)
        {
            _clientFunctions = clientFunctions;
        }

        public string GetAccountDescription(IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, IMasterDataStore store,
            IQuery<IList<MasterAccountInformation>> getAllParentAccountsQuery, string acct, ReportGlobals globals, string groupBy, string sourceAbbr, string agency, string ddGrpField)
        {
            switch (groupBy)
            {
                case ReportFilters.HOTEL_TOP_BOTTOM_ACCOUNTS_RPT_GROUP_BY_DATA_SOURCE_PARAM_VALUE:
                    return ddGrpField == ReportFilters.HOTEL_TOP_BOTTOM_ACCOUNTS_RPT_GROUPING_FIELD_ACCT_NUM_PARAM_VALUE
                        ? sourceAbbr.Trim()
                        : LookupFunctions.LookupSourceDescription(store, sourceAbbr.Trim(), agency.Trim());
                case ReportFilters.HOTEL_TOP_BOTTOM_ACCOUNTS_RPT_GROUP_BY_PARENT_ACCOUNT_PARAM_VALUE:
                    var parent = _clientFunctions.LookupParent(getAllMasterAccountsQuery, acct, getAllParentAccountsQuery);
                    return ddGrpField == ReportFilters.HOTEL_TOP_BOTTOM_ACCOUNTS_RPT_GROUPING_FIELD_ACCT_NUM_PARAM_VALUE
                        ? parent.AccountDescription + " (" + parent.AccountId.Trim() + ")"
                        : parent.AccountDescription;
                default:
                    return ddGrpField == ReportFilters.HOTEL_TOP_BOTTOM_ACCOUNTS_RPT_GROUPING_FIELD_ACCT_NUM_PARAM_VALUE
                        ? _clientFunctions.LookupCname(getAllMasterAccountsQuery, acct, globals) + " (" + acct.Trim() + ")"
                        : _clientFunctions.LookupCname(getAllMasterAccountsQuery, acct, globals);
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
                        finalData.Stays = g.Sum(s => s.Stays);
                        finalData.Nights = g.Sum(s => s.nights);
                        finalData.Hotelcost = g.Sum(s => s.hotelcost);
                        finalData.Bookrate = g.Sum(s => s.sumbkrate);
                        finalData.Bookcnt = g.Sum(s => s.BookRate == 0 ? 0 : s.Stays);
                        finalData.AveBookRate = finalData.Bookcnt == 0 ? 0 : finalData.Bookrate / finalData.Bookcnt;
                        finalData.sumbkrate = g.Sum(s => s.sumbkrate);
                        finalData.AccountNumber = g.First().Acct;
                        return finalData;
                    }).ToList();
            }else
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
                        finalData.Stays = g.Sum(s => s.Stays);
                        finalData.Nights = g.Sum(s => s.nights);
                        finalData.Hotelcost = g.Sum(s => s.hotelcost);
                        finalData.Bookrate = g.Sum(s => s.sumbkrate);
                        finalData.Bookcnt = g.Sum(s => s.BookRate == 0 ? 0 : s.Stays);
                        finalData.AveBookRate = finalData.Bookcnt == 0 ? 0 : finalData.Bookrate / finalData.Bookcnt;
                        finalData.sumbkrate = g.Sum(s => s.sumbkrate);
                        finalData.AccountNumber = g.First().Acct;
                        return finalData;
                    }).ToList();
            }
            return results;
        }
        
        public string GetAccountNameForExportRawData(IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, IMasterDataStore store,
            IQuery<IList<MasterAccountInformation>> getAllParentAccountsQuery, string acct, ReportGlobals globals, string groupBy, string sourceAbbr, string agency)
        {
            switch (groupBy)
            {
                case ReportFilters.HOTEL_TOP_BOTTOM_ACCOUNTS_RPT_GROUP_BY_DATA_SOURCE_PARAM_VALUE:
                    return LookupFunctions.LookupSourceDescription(store, sourceAbbr.Trim(), agency.Trim());
                case ReportFilters.HOTEL_TOP_BOTTOM_ACCOUNTS_RPT_GROUP_BY_PARENT_ACCOUNT_PARAM_VALUE:
                    return _clientFunctions.LookupParent(getAllMasterAccountsQuery, acct, getAllParentAccountsQuery).AccountDescription;
                default:
                    return _clientFunctions.LookupCname(getAllMasterAccountsQuery, acct, globals);
            }
        }

        public decimal GetFinalListTotalRate(List<FinalData> finalDataList)
        {
            return finalDataList.Sum(item => item.sumbkrate);
        }

        public decimal GetFinalListTotalBookCount(List<FinalData> finalDataList)
        {
            return finalDataList.Sum(item => item.Bookcnt);
        }

        public void CalculateAveBook(List<FinalData> finalDataList)
        {
            foreach(var record in finalDataList)
            {
                record.AvgBook = record.Bookcnt == 0
                    ? 0
                    : record.Bookrate / record.Bookcnt;
            }
        }
    }
}
