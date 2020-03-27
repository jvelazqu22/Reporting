using Domain.Helper;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;

using System.Collections.Generic;
using System.Linq;

using Domain.Models.ReportPrograms.ClassOfServiceReport;
using Domain.Orm.Classes;

using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using iBank.Services.Implementation.Shared.Client;
using iBankDomain.RepositoryInterfaces;

namespace iBank.Services.Implementation.ReportPrograms.ClassOfService
{
    public class ClassOfSvcDataProcessor
    {
        private readonly ClassofSvcCalculations _calc = new ClassofSvcCalculations();
        public IEnumerable<FinalData> MapRawToFinalData(IList<RawData> rawData, bool accountBreak, bool isByAirline, bool isByHomeCountry, bool useClassCategory, ReportGlobals globals,
            UserBreaks userBreaks, ClientFunctions clientFunctions, IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, 
            IMasterDataStore store)
        {
            var finalList = rawData.Select(x => new FinalData
            {
                RecKey = x.RecKey,
                Acct = !accountBreak ? Constants.NotApplicable : x.Acct,
                Acctdesc = !accountBreak ? Constants.NotApplicable : clientFunctions.LookupCname(getAllMasterAccountsQuery, x.Acct, globals),
                Break1 = !userBreaks.UserBreak1 ? Constants.NotApplicable
                        : string.IsNullOrEmpty(x.Break1.Trim()) ? "NONE".PadRight(30) : x.Break1.Trim(),
                Break2 = !userBreaks.UserBreak2 ? Constants.NotApplicable
                        : string.IsNullOrEmpty(x.Break2.Trim()) ? "NONE".PadRight(30) : x.Break2.Trim(),
                Break3 = !userBreaks.UserBreak3 ? Constants.NotApplicable
                        : string.IsNullOrEmpty(x.Break3.Trim()) ? "NONE".PadRight(16) : x.Break3.Trim(),
                Carrname = !isByAirline ? "NA" : LookupFunctions.LookupAline(store, x.Airline, x.Mode),

                Homectry = !isByHomeCountry ? "NA" : LookupFunctions.LookupHomeCountryName(x.SourceAbbr, globals, store),
                Airline = !isByAirline ? "NA" : x.Airline,
                Class = !useClassCategory ? x.ClassCode : clientFunctions.LookupClassCategoryDescription(x.ClassCat, globals.Agency, new iBankClientQueryable(globals.AgencyInformation.ServerName, globals.AgencyInformation.DatabaseName)),
                Segcost = x.ActFare,
                Segs = x.Plusmin,
                Domsegs = (x.DitCode == "D") ? x.Plusmin : 0,
                Domsegcost = (x.DitCode == "D") ? x.ActFare : 0m,
                Trnsegs = (x.DitCode == "T") ? x.Plusmin : 0,
                Trnsegcost = (x.DitCode == "T") ? x.ActFare : 0m,
                Intsegs = (x.DitCode == "T" || x.DitCode == "D") ? 0 : x.Plusmin,
                Intsegcost = (x.DitCode == "T" || x.DitCode == "D") ? 0m : x.ActFare,
            });
            return finalList;
        }

        public IEnumerable<IGrouping<GroupedData, FinalData>> GroupFinalDataForCarrierClass(IList<FinalData> finalData)
        {
            return finalData.GroupBy(s => new GroupedData
            {
                Acct = s.Acct,
                Break1 = s.Break1,
                Break2 = s.Break2,
                Break3 = s.Break3,
                Airline = s.Airline,
                Carrname = s.Carrname,
                Homectry = s.Homectry,
                Class = s.Class
            });
        }

        public List<FinalData> MapGroupedDataToFinalData(List<FinalData> FinalList)
        {
            var finalList = FinalList.GroupBy(s => new { s.Acct, s.Break1, s.Break2, s.Break3, s.Airline, s.Carrname, s.Homectry, s.Class }, 
                (key, g) =>
            {
                var finalData = new FinalData();
                finalData.Acct = key.Acct;
                finalData.Acctdesc = g.First().Acctdesc;
                finalData.Break1 = key.Break1;
                finalData.Break2 = key.Break2;
                finalData.Break3 = key.Break3;
                finalData.Airline = key.Airline;
                finalData.Carrname = key.Carrname;
                finalData.Homectry = key.Homectry;
                finalData.Class = key.Class;
                finalData.Segcost = g.Sum(x => x.Segcost);
                finalData.Segs = g.Sum(x => x.Segs);
                finalData.Domsegs = g.Sum(x => x.Domsegs);
                finalData.Domsegcost = g.Sum(x => x.Domsegcost);
                finalData.Trnsegs = g.Sum(x => x.Trnsegs);
                finalData.Trnsegcost = g.Sum(x => x.Trnsegcost);
                finalData.Intsegs = g.Sum(x => x.Intsegs);
                finalData.Intsegcost = g.Sum(x => x.Intsegcost);
                finalData.RecKey = g.First().RecKey;
                return finalData;
            }).OrderBy(x => x.Acct)
            .ThenBy(x => x.Acctdesc)
            .ThenBy(x => x.Break1)
            .ThenBy(x => x.Break2)
            .ThenBy(x => x.Break3)
            .ThenBy(x => x.Carrname)
            .ThenBy(x => x.Homectry)
            .ThenBy(x => x.Class);

            return finalList.ToList();
        }

        public IEnumerable<FinalData> MapGroupedDataToCarrierClass(IEnumerable<IGrouping<GroupedData, FinalData>> groupedData)
        {
            return groupedData.Select(s => new FinalData
            {
                Acct = s.Key.Acct,
                Break1 = s.Key.Break1,
                Break2 = s.Key.Break2,
                Break3 = s.Key.Break3,
                Airline = s.Key.Airline,
                Carrname = s.Key.Carrname,
                Homectry = s.Key.Homectry,
                Class = s.Key.Class,
                Segcost = s.Sum(x => x.Segcost),
                Segs = s.Sum(x => x.Segs)
            });
        }

        public IList<FinalData> ReplaceFinalDataItemsWithCarrierItems(IList<FinalData> finalData, IList<FinalData> carrierData)
        {
            foreach(var item in finalData)
            {
                var matchingData = _calc.GetMatchingCarrierData(item, carrierData).ToList();
                var segCost = matchingData.Sum(x => x.Segcost);
                var segs = matchingData.Sum(x => x.Segs);

                item.Segs = segs;
                item.Carrsegs = _calc.GetTotalNumberOfSegsExcludingClass(item, carrierData);
                item.Segcost = segCost;
                item.Carrcost = _calc.GetTotalSegsCostExcludingClass(item, carrierData);
            }

            return finalData;
        }

        public IEnumerable<SubReportFinalData> MapFinalDataToSubReport(IList<FinalData> finalData)
        {
            return finalData.Select(s => new SubReportFinalData
            {
                Class = s.Class,
                Segcost = s.Segcost,
                Segs = s.Segs,
                Domsegs = s.Domsegs,
                Domsegcost = s.Domsegcost,
                Trnsegs = s.Trnsegs,
                Trnsegcost = s.Trnsegcost,
                Intsegs = s.Intsegs,
                Intsegcost = s.Intsegcost
            });
        }

        public List<SubReportFinalData> MapGroupedDataToSubReportData(IEnumerable<SubReportFinalData> subReportData)
        {
            var list = subReportData.GroupBy(s => new { s.Class },
                (key, g) =>
                {
                    var subReportFinalData = new SubReportFinalData();
                    subReportFinalData.Class = key.Class;
                    subReportFinalData.Segcost = g.Sum(x => x.Segcost);
                    subReportFinalData.Segs = g.Sum(x => x.Segs);
                    subReportFinalData.Domsegcost = g.Sum(x => x.Domsegcost);
                    subReportFinalData.Domsegs = g.Sum(x => x.Domsegs);
                    subReportFinalData.Trnsegcost = g.Sum(x => x.Trnsegcost);
                    subReportFinalData.Trnsegs = g.Sum(x => x.Trnsegs);
                    subReportFinalData.Intsegcost = g.Sum(x => x.Intsegcost);
                    subReportFinalData.Intsegs = g.Sum(x => x.Intsegs);
                    return subReportFinalData;
                }).OrderBy(x => x.Class);

            return list.ToList();
        }

        public IList<SubReportFinalData> SumTotalSegData(IList<SubReportFinalData> subReportData)
        {
            for (var i = 0; i < subReportData.Count; i++)
            {
                var item = subReportData[i];
                item.Totsegcost = subReportData.Select(x => x.Segcost).Sum();
                item.Totsegs = subReportData.Select(x => x.Segs).Sum();
            }

            return subReportData;
        }

        public IList<RawData> FilterByHomeCountry(IList<RawData> rawData, string inHomeCountry, string notCountry, ReportGlobals globals)
        {
            if (string.IsNullOrEmpty(inHomeCountry) && string.IsNullOrEmpty(notCountry)) return rawData;

            var rowsToRemove = new List<RawData>();
            
            // ReSharper disable once PossibleNullReferenceException
            var arrHomeCtry = inHomeCountry.Split(',');

            for (var i = 0; i < arrHomeCtry.Length; i++)
            {
                var ctry = arrHomeCtry[i].Trim();
                foreach (var row in rawData)
                {
                    var cctry = LookupFunctions.LookupHomeCountryCode(row.SourceAbbr, globals, new MasterDataStore()).Trim();
                    //need to remove the countries
                    if (cctry == notCountry || cctry != ctry) rowsToRemove.Add(row);
                }
            }
            rowsToRemove.ForEach(row => rawData.Remove(row));

            return rawData;
        }
    }
}