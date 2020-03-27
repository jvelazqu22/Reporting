using Domain.Constants;
using Domain.Models.ReportPrograms.CarTopBottomAccountsReport;
using Domain.Orm.Classes;
using iBank.Services.Implementation.Shared;
using iBankDomain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Linq;
using iBank.Services.Implementation.Shared.Client;

namespace iBank.Services.Implementation.ReportPrograms.CarTopBottomAccounts
{
    public class CarTopBottomAccountRawDataCalculator
    {
        ClientFunctions _clientFunctions;
        public CarTopBottomAccountRawDataCalculator(ClientFunctions clientFunctions)
        {
            _clientFunctions = clientFunctions;
        }

        public List<RawData> GetSummaryReservationRawData(IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, List<RawData> rawDataList, string groupBy, IQuery<IList<MasterAccountInformation>> getAllParentAccountsQuery)
        {
            List<RawData> results = new List<RawData>();
            results = rawDataList
                .GroupBy(s => new { s.SourceAbbr, s.Acct, s.ABookRat },
                    (key, g) =>
                    {
                        var rawData = new RawData();
                        rawData.SourceAbbr = g.First().SourceAbbr;
                        rawData.Acct = g.First().Acct;
                        rawData.GroupAccount = GetAcct(getAllMasterAccountsQuery, groupBy, g.First().Acct, rawData.SourceAbbr, getAllParentAccountsQuery);
                        rawData.ABookRat = g.First().ABookRat;
                        rawData.Rentals = g.Count();
                        rawData.Days = g.Sum(s => s.Days);
                        rawData.CarCost = g.Sum(s => s.ABookRat * s.Days);
                        rawData.sumbkrate = g.Sum(s => s.ABookRat);
                        rawData.BookCnt = g.Sum(s => s.ABookRat == 0 ? 0 : s.Rentals);
                        return rawData;
                    }).ToList();

            return results;
        }

        public List<RawData> GetSummaryBakcOfficeRawData(IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, List<RawData> rawDataList, string groupBy, IQuery<IList<MasterAccountInformation>> getAllParentAccountsQuery)
        {
            List<RawData> results = new List<RawData>();

            results = rawDataList.GroupBy(s => new { s.SourceAbbr, s.Acct, s.ABookRat },
                    (key, g) =>
                    {
                        var rawData = new RawData();
                        rawData.SourceAbbr = g.First().SourceAbbr;
                        rawData.Acct = g.First().Acct;
                        rawData.GroupAccount = GetAcct(getAllMasterAccountsQuery, groupBy, g.First().Acct, rawData.SourceAbbr, getAllParentAccountsQuery);
                        rawData.ABookRat = g.First().ABookRat;
                        rawData.Rentals = g.Sum(s => s.CPlusMin);
                        rawData.Days = g.Sum(s => s.Days * s.CPlusMin);
                        rawData.CarCost = g.Sum(s => s.ABookRat * s.Days);
                        rawData.sumbkrate = g.Sum(s => s.ABookRat);
                        rawData.BookCnt = g.Sum(s => s.ABookRat == 0 ? 0 : s.Rentals);
                        return rawData;
                    }).ToList();

            return results;
        }

        public string GetAcct(IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, string groupBy, string acct, string sourceAbbr, IQuery<IList<MasterAccountInformation>> getAllParentAccountsQuery)
        {
            switch (groupBy)
            {
                case ReportFilters.CAR_TOP_BOTTOM_ACCOUNTS_RPT_GROUP_BY_DATA_SOURCE_PARAM_VALUE:
                    return sourceAbbr;
                case ReportFilters.CAR_TOP_BOTTOM_ACCOUNTS_RPT_GROUP_BY_PARENT_ACCOUNT_PARAM_VALUE:
                    return _clientFunctions.LookupParent(getAllMasterAccountsQuery, acct, getAllParentAccountsQuery).AccountId;
                default:
                    return acct;
            }
        }

        public int GetRawListTotalRentals(List<RawData> rawDataList)
        {
            int results = 0;
            foreach (var item in rawDataList.ToList())
                results += item.Rentals;
            return results;
        }

        public int GetRawListTotalDays(List<RawData> rawDataList)
        {
            int results = 0;
            foreach (var item in rawDataList.ToList())
                results += item.Days;
            return results;
        }

        public decimal GetRawListTotalCarCosts(List<RawData> rawDataList)
        {
            decimal results = 0;
            foreach (var item in rawDataList.ToList())
                results += item.CarCost;
            return results;
        }
    }
}
