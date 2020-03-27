using Domain.Models.TransactionSummary;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;

using System.Collections.Generic;
using System.Linq;

using Domain.Models.ReportPrograms.TransactionSummary;
using Domain.Orm.Classes;

using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Shared.Client;
using iBankDomain.RepositoryInterfaces;

namespace iBank.Services.Implementation.ReportPrograms.TransactionSummary
{
    public class TransactionSummaryFinalDataCalculator
    {
        public List<FinalData> GetFinalDataListFromRawData(List<RawData> RawData, bool cbsShowClientDetail, bool logGen1, ClientFunctions clientFunctions, 
            IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, ReportGlobals Globals, IMasterDataStore store)
        {
            return cbsShowClientDetail
                ? GetFinalDataWithCbsClientDetail(RawData, logGen1, clientFunctions, getAllMasterAccountsQuery, Globals, store)
                : GetFinalDataWithOutCbsClientDetail(RawData, logGen1, Globals, store);
        }

        private List<FinalData> GetFinalDataWithCbsClientDetail(List<RawData> RawDataList, bool logGen1, ClientFunctions clientFunctions, 
            IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, ReportGlobals Globals, IMasterDataStore store)
        {
            List<FinalData> FinalDataList = new List<FinalData>();

            FinalDataList = RawDataList.Select(s => new FinalData
            {
                SourceAbbr = logGen1 ? s.SourceAbbr : "N/A",
                Acct = s.Acct,
                Year = s.Year,
                Month = s.Month,
                YearMth = s.Year * 100 + s.Month,
                PrevCount = s.Source.EqualsIgnoreCase("PREVIEW") ? s.Catcount : 0,
                HistCount = s.Source.EqualsIgnoreCase("HISTORY") ? s.Catcount : 0,
                BothCount = s.Source.EqualsIgnoreCase("HISTPREV") ? s.Catcount : 0,
                TrackerCnt = s.Source.EqualsIgnoreCase("TRACKER") ? s.Catcount : 0,
                ChgMgmtCnt = s.Source.EqualsIgnoreCase("CHNGMGMT") ? s.Catcount : 0,
                AuthCount = s.Source.EqualsIgnoreCase("TRIPAUTH") ? s.Catcount : 0,
                PcmCount = s.Source.EqualsIgnoreCase("PCMBILL") && s.Category.EqualsIgnoreCase("I") ? s.Catcount : 0
            })
            .GroupBy(s => new { s.SourceAbbr, s.Acct, s.AcctDesc, s.Year, s.Month, s.YearMth }, (key, g) =>
            {
                var temp = g.ToList();
                return new FinalData
                {
                    SourceAbbr = key.SourceAbbr,
                    SourceDesc =
                        logGen1
                            ? LookupFunctions.LookupMasterAgencySource(store, key.SourceAbbr, Globals.Agency)
                            : "N/A",
                    Acct = key.Acct,
                    AcctDesc = clientFunctions.LookupCname(getAllMasterAccountsQuery, key.Acct, Globals),
                    Year = key.Year,
                    Month = key.Month,
                    MthName = key.Month.MonthNameFromNumber(),
                    YearMth = key.YearMth,
                    PrevCount = temp.Sum(s => s.PrevCount),
                    HistCount = temp.Sum(s => s.HistCount),
                    BothCount = temp.Sum(s => s.BothCount),
                    TrackerCnt = temp.Sum(s => s.TrackerCnt),
                    ChgMgmtCnt = temp.Sum(s => s.ChgMgmtCnt),
                    AuthCount = temp.Sum(s => s.AuthCount),
                    PcmCount = temp.Sum(s => s.PcmCount),

                };
            }).ToList();

            return FinalDataList;
        }

        private List<FinalData> GetFinalDataWithOutCbsClientDetail(List<RawData> RawDataList, bool logGen1, ReportGlobals Globals, 
            IMasterDataStore store)
        {
            List<FinalData> FinalDataList = new List<FinalData>();
            FinalDataList = RawDataList.Select(s => new FinalData
            {
                SourceAbbr = logGen1 ? s.SourceAbbr : "N/A",
                Year = s.Year,
                Month = s.Month,
                YearMth = s.Year * 100 + s.Month,
                PrevCount = s.Source.EqualsIgnoreCase("PREVIEW") ? s.Catcount : 0,
                HistCount = s.Source.EqualsIgnoreCase("HISTORY") ? s.Catcount : 0,
                BothCount = s.Source.EqualsIgnoreCase("HISTPREV") ? s.Catcount : 0,
                TrackerCnt = s.Source.EqualsIgnoreCase("TRACKER") ? s.Catcount : 0,
                ChgMgmtCnt = s.Source.EqualsIgnoreCase("CHNGMGMT") ? s.Catcount : 0,
                AuthCount = s.Source.EqualsIgnoreCase("TRIPAUTH") ? s.Catcount : 0,
                PcmCount = s.Source.EqualsIgnoreCase("PCMBILL") && s.Category.EqualsIgnoreCase("I") ? s.Catcount : 0
            }).GroupBy(s => new { s.SourceAbbr, s.Year, s.Month, s.YearMth }, (key, g) =>
            {
                var temp = g.ToList();
                return new FinalData
                {
                    SourceAbbr = key.SourceAbbr,
                    SourceDesc =
                        logGen1 ? LookupFunctions.LookupMasterAgencySource(store, key.SourceAbbr, Globals.Agency) : "N/A",
                    Year = key.Year,
                    Month = key.Month,
                    YearMth = key.YearMth,
                    MthName = key.Month.MonthNameFromNumber(),
                    PrevCount = temp.Sum(s => s.PrevCount),
                    HistCount = temp.Sum(s => s.HistCount),
                    BothCount = temp.Sum(s => s.BothCount),
                    TrackerCnt = temp.Sum(s => s.TrackerCnt),
                    ChgMgmtCnt = temp.Sum(s => s.ChgMgmtCnt),
                    AuthCount = temp.Sum(s => s.AuthCount),
                    PcmCount = temp.Sum(s => s.PcmCount),

                };
            }).ToList();

            return FinalDataList;
        }
    }
}
