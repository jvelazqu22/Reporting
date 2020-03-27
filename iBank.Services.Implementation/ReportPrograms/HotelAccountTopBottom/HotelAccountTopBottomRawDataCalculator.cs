using Domain.Constants;
using Domain.Models.ReportPrograms.HotelAccountTopBottom;
using Domain.Orm.Classes;
using iBank.Services.Implementation.Shared;
using iBankDomain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Linq;
using iBank.Services.Implementation.Shared.Client;

namespace iBank.Services.Implementation.ReportPrograms.HotelAccountTopBottom
{
    public class HotelAccountTopBottomRawDataCalculator
    {
        ClientFunctions _clientFunctions;
        public HotelAccountTopBottomRawDataCalculator(ClientFunctions clientFunctions)
        {
            _clientFunctions = clientFunctions;
        }

        public List<RawData> GetSummaryReservationRawData(IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, List<RawData> rawDataList, 
            string groupBy, IQuery<IList<MasterAccountInformation>> getAllParentAccountsQuery)
        {
            List<RawData> results = new List<RawData>();
            results = rawDataList
                .GroupBy(s => new { s.SourceAbbr, s.Acct, s.BookRate },
                    (key, g) =>
                    {
                    var rawData = new RawData();
                    rawData.SourceAbbr = g.First().SourceAbbr;
                    rawData.Acct = g.First().Acct;
                    rawData.GroupAccount = GetAcct(getAllMasterAccountsQuery, groupBy, g.First().Acct, rawData.SourceAbbr, getAllParentAccountsQuery);
                    rawData.BookRate = g.First().BookRate;
                    rawData.Stays = g.Count();
                    rawData.nights = g.Sum(s => s.nights * s.rooms);
                    rawData.hotelcost = g.Sum(s => s.BookRate * s.nights * s.rooms);
                    rawData.sumbkrate = g.Sum(s => s.BookRate);
                    return rawData;
                    }).ToList();

            return results;
        }

        public List<RawData> GetSummaryBakcOfficeRawData(IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, List<RawData> rawDataList, 
            string groupBy, IQuery<IList<MasterAccountInformation>> getAllParentAccountsQuery)
        {
            List<RawData> results = new List<RawData>();

            results = rawDataList
                .GroupBy(s => new { s.SourceAbbr, s.Acct, s.BookRate },
                    (key, g) =>
                    {
                        var rawData = new RawData();
                        rawData.SourceAbbr = g.First().SourceAbbr;
                        rawData.Acct = g.First().Acct;
                        rawData.GroupAccount = GetAcct(getAllMasterAccountsQuery, groupBy, g.First().Acct, rawData.SourceAbbr, getAllParentAccountsQuery);
                        rawData.BookRate = g.First().BookRate;
                        rawData.Stays = g.Sum(s => s.Hplusmin);
                        rawData.nights = g.Sum(s => s.nights * s.rooms * s.Hplusmin);
                        rawData.hotelcost = g.Sum(s => s.BookRate * s.nights * s.rooms);
                        rawData.sumbkrate = g.Sum(s => s.BookRate);
                        return rawData;
                    }).ToList();

            return results;
        }

        public string GetAcct(IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, string groupBy, string acct, string sourceAbbr, 
            IQuery<IList<MasterAccountInformation>> getAllParentAccountsQuery)
        {
            switch (groupBy)
            {
                case ReportFilters.HOTEL_TOP_BOTTOM_ACCOUNTS_RPT_GROUP_BY_DATA_SOURCE_PARAM_VALUE:
                    return sourceAbbr;
                case ReportFilters.HOTEL_TOP_BOTTOM_ACCOUNTS_RPT_GROUP_BY_PARENT_ACCOUNT_PARAM_VALUE:
                    return _clientFunctions.LookupParent(getAllMasterAccountsQuery, acct, getAllParentAccountsQuery).AccountId;
                default:
                    return acct;
            }
        }

        public int GetRawListTotalStays(List<RawData> rawDataList)
        {
            int results = 0;
            foreach (var item in rawDataList.ToList())
                results += item.Stays;
            return results;
        }

        public int GetRawListTotalNights(List<RawData> rawDataList)
        {
            int results = 0;
            foreach (var item in rawDataList.ToList())
                results += item.nights;
            return results;
        }

        public decimal GetRawListTotalHotelCosts(List<RawData> rawDataList)
        {
            decimal results = 0;
            foreach (var item in rawDataList.ToList())
                results += item.hotelcost;
            return results;
        }

    }
}
