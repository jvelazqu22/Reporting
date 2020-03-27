using Domain.Helper;
using Domain.Models.ReportPrograms.MissedHotelOpportunitiesReport;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBankDomain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Services;
using iBank.Server.Utilities;
using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Shared.Client;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.MissedHotelOpportunities
{
    public class MissedHotelDataProcessor
    {
        public List<FinalData> GetFinalData(List<RawData> RawDataList, bool GroupByHomeCountry, Dictionary<int, string> routeItineraries, 
            ReportGlobals Globals, bool BreakByAgenId, IMasterDataStore MasterStore, bool AccountBreak, ClientFunctions clientFunctions, 
            IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, UserBreaks UserBreaks, bool ExcludeCarOnly, IList<RawData> _carRawData, 
            bool IncludeTripsWithHotels, IList<RawData> _hotelRawData, BuildWhere BuildWhere)
        {
            List<FinalData> data = new List<FinalData>();

            //Get a list of distinct trip records (remove the multiple legs/segments)
            var distinctTripData = GetDistinctData(RawDataList, AccountBreak, clientFunctions, Globals, getAllMasterAccountsQuery, UserBreaks, false);

            if (!ExcludeCarOnly)
            {
                if (_carRawData.Any())
                {
                    //Get a list of distinct car reservations
                    var distinctCarDataList = GetDistinctData(_carRawData, AccountBreak, clientFunctions, Globals, getAllMasterAccountsQuery, UserBreaks, false);

                    //Add the car data to the main trip data
                    distinctTripData.AddRange(distinctCarDataList);
                }
            }

            if (IncludeTripsWithHotels)
            {
                if (_hotelRawData.Any())
                {
                    if (!BuildWhere.HasRoutingCriteria && !BuildWhere.HasFirstDestination && !BuildWhere.HasFirstOrigin)
                    {
                        var distinctHotelDataList = GetDistinctData(_hotelRawData, AccountBreak, clientFunctions, Globals, getAllMasterAccountsQuery,
                            UserBreaks, true);

                        distinctTripData.AddRange(distinctHotelDataList);                            
                    }                    
                }
                //check each Hotelbkd for all records
               distinctTripData.Where(s => _hotelRawData.Any(r => r.RecLoc == s.RecLoc && r.TripStart == s.TripStart && r.TripEnd == s.TripEnd)).ToList().ForEach(s => s.Hotelbkd = "Yes");
            }

            //Get the home countries
            var homeCountries = HomeCountriesLookup.GetHomeCountries(new CacheService(), MasterStore.MastersQueryDb, Globals.ClientType, Globals.Agency);
            var homeCountry = Globals.GetParmValue(WhereCriteria.HOMECTRY);
            var sourceAbbrList = homeCountries.Where(s => s.Value.EqualsIgnoreCase(homeCountry)).Select(s => s.Key);

            data = (GroupByHomeCountry == true)
                ? GroupFinalDataByHomeCountry(distinctTripData, GroupByHomeCountry, homeCountry, sourceAbbrList, routeItineraries, Globals, BreakByAgenId)
                : DoNotGroupFinalDataByHomeCountry(distinctTripData, GroupByHomeCountry, homeCountry, sourceAbbrList, routeItineraries, Globals, BreakByAgenId);

            return data.OrderBy(s => s.Homectry)
                                .ThenBy(s => s.Acct)
                                .ThenBy(s => s.Acctdesc)
                                .ThenBy(s => s.Break1)
                                .ThenBy(s => s.Break2)
                                .ThenBy(s => s.Break3)
                                .ThenBy(s => BreakByAgenId ? s.Agentid : s.Break3)
                                .ThenBy(s => s.Passlast)
                                .ThenBy(s => s.Passfrst)
                                .ThenBy(s => s.Tripstart).ToList();
        }

        public List<RawData> GetDistinctData(IList<RawData> rawData, bool accountBreak, ClientFunctions clientFunctions, ReportGlobals globals, 
            IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, UserBreaks userBreaks, bool hotelBooked)
        {
            var hotelBkd = "No ";

            var rawDataTemp = rawData.Select(s => new
            {
                s.RecKey,
                s.SourceAbbr,
                s.RecLoc,
                Acct = accountBreak ? s.Acct.Trim() : Constants.NotApplicable,
                Acctdesc = accountBreak ? clientFunctions.LookupCname(getAllMasterAccountsQuery, s.Acct, globals) : Constants.NotApplicable,
                Break1 = !userBreaks.UserBreak1 ? Constants.NotApplicable : string.IsNullOrEmpty(s.Break1.Trim()) ? "NONE".PadRight(30) : s.Break1.Trim(),
                Break2 = !userBreaks.UserBreak2 ? Constants.NotApplicable : string.IsNullOrEmpty(s.Break2.Trim()) ? "NONE".PadRight(30) : s.Break2.Trim(),
                Break3 = !userBreaks.UserBreak3 ? Constants.NotApplicable : string.IsNullOrEmpty(s.Break3.Trim()) ? "NONE".PadRight(16) : s.Break3.Trim(),
                s.AgentId,
                s.Passlast,
                s.Passfrst,
                s.TranType,
                s.InvDate,
                s.TripStart,
                s.TripEnd,
                s.Invoice,
                HotelBkd = hotelBkd
            }).Distinct();

            // This may seem redundatn, but the above distinct does not work unless using an annonymous object, then we can use the
            // RawData object
            return rawDataTemp.Select(s => new RawData
            {
                RecKey = s.RecKey,
                SourceAbbr = s.SourceAbbr,
                RecLoc = s.RecLoc,
                Acct = s.Acct,
                AcctDesc = s.Acctdesc,
                Break1 = s.Break1,
                Break2 = s.Break2,
                Break3 = s.Break3,
                AgentId = s.AgentId,
                Passlast = s.Passlast,
                Passfrst = s.Passfrst,
                TranType = s.TranType,
                InvDate = s.InvDate,
                TripStart = s.TripStart,
                TripEnd = s.TripEnd,
                Invoice = s.Invoice,
                Hotelbkd = s.HotelBkd
            }).ToList();
        }
        
        public IList<GraphData> GetSubReportData(IList<FinalData> finalData)
        {
            var bookedTotal = finalData.Count(s => s.Hotelbkd == "Yes");
            var notBookedTotal = finalData.Count(s => s.Hotelbkd == "No ");

            return new List<GraphData>
            {
                new GraphData {DataDesc = "Trips with hotel", TripCount = bookedTotal},
                new GraphData {DataDesc = "Trips with No Hotel", TripCount = notBookedTotal}
            };
        }

        public List<FinalData> GroupFinalDataByHomeCountry(List<RawData> distinctTripData, bool GroupByHomeCountry, string homeCountry, IEnumerable<string> sourceAbbrList,
            Dictionary<int, string> routeItineraries, ReportGlobals Globals, bool BreakByAgenId)
        {
            List<FinalData> data = new List<FinalData>();

            var anonymousTempData = distinctTripData.Where(r => string.IsNullOrEmpty(homeCountry) || sourceAbbrList.Any(t => t.EqualsIgnoreCase(r.SourceAbbr))).Select(
                    s => new 
                    {
                        Reckey = s.RecKey,
                        Homectry = LookupFunctions.LookupCountryName(LookupFunctions.LookupHomeCountryCode(s.SourceAbbr, Globals, new MasterDataStore()), Globals, new MasterDataStore()),
                        Acct = s.Acct,
                        Acctdesc = s.AcctDesc,
                        Break1 = s.Break1,
                        Break2 = s.Break2,
                        Break3 = s.Break3,
                        Invoice = s.Invoice,
                        Invdate = s.InvDate ?? DateTime.Now,
                        Agentid = s.AgentId,
                        Passlast = s.Passlast,
                        Passfrst = s.Passfrst,
                        Trantype = s.TranType,
                        Tripstart = s.TripStart ?? DateTime.Now,
                        Tripend = s.TripEnd ?? DateTime.Now,
                        Itinerary = routeItineraries.ContainsKey(s.RecKey) ? routeItineraries[s.RecKey] : "",
                        Tripduratn = new MissedHotelCalculations().GetTripDuration(s.TripStart, s.TripEnd),
                        Hotelbkd = s.Hotelbkd
                    }).ToList();

            // This may seem redundatn, but the above distinct does not work unless using an annonymous object, then we can use the
            // FinalData object
            data = anonymousTempData.Select(s => new FinalData
            {
                Reckey = s.Reckey,
                Homectry = s.Homectry,
                Acct = s.Acct,
                Acctdesc = s.Acctdesc,
                Break1 = s.Break1,
                Break2 = s.Break2,
                Break3 = s.Break3,
                Invoice = s.Invoice,
                Invdate = s.Invdate,
                Agentid = s.Agentid,
                Passlast = s.Passlast,
                Passfrst = s.Passfrst,
                Trantype = s.Trantype,
                Tripstart = s.Tripstart,
                Tripend = s.Tripend,
                Itinerary = s.Itinerary,
                Tripduratn = s.Tripduratn,
                Hotelbkd = s.Hotelbkd
            }).ToList();

            return data;
        }

        public List<FinalData> DoNotGroupFinalDataByHomeCountry(List<RawData> distinctTripData, bool GroupByHomeCountry, string homeCountry, IEnumerable<string> sourceAbbrList,
            Dictionary<int, string> routeItineraries, ReportGlobals Globals, bool BreakByAgenId)
        {
            List<FinalData> data = new List<FinalData>();
            var anonymousTempData = distinctTripData.Where(r => string.IsNullOrEmpty(homeCountry) || sourceAbbrList.Any(t => t.EqualsIgnoreCase(r.SourceAbbr))).Select(
                    s => new 
                    {
                        Reckey = s.RecKey,
                        Homectry = Constants.NotApplicable,
                        Acct = s.Acct,
                        Acctdesc = s.AcctDesc,
                        Break1 = s.Break1,
                        Break2 = s.Break2,
                        Break3 = s.Break3,
                        Invoice = s.Invoice,
                        Invdate = s.InvDate ?? DateTime.Now,
                        Agentid = s.AgentId,
                        Passlast = s.Passlast,
                        Passfrst = s.Passfrst,
                        Trantype = s.TranType,
                        Tripstart = s.TripStart ?? DateTime.Now,
                        Tripend = s.TripEnd ?? DateTime.Now,
                        Itinerary = routeItineraries.ContainsKey(s.RecKey) ? routeItineraries[s.RecKey] : "",
                        Tripduratn = new MissedHotelCalculations().GetTripDuration(s.TripStart, s.TripEnd),
                        Hotelbkd = s.Hotelbkd
                    }).Distinct().ToList();

            // This may seem redundatn, but the above distinct does not work unless using an annonymous object, then we can use the
            // FinalData object
            data = anonymousTempData.Select(s => new FinalData
            {
                Reckey = s.Reckey,
                Homectry = s.Homectry,
                Acct = s.Acct,
                Acctdesc = s.Acctdesc,
                Break1 = s.Break1,
                Break2 = s.Break2,
                Break3 = s.Break3,
                Invoice = s.Invoice,
                Invdate = s.Invdate,
                Agentid = s.Agentid,
                Passlast = s.Passlast,
                Passfrst = s.Passfrst,
                Trantype = s.Trantype,
                Tripstart = s.Tripstart,
                Tripend = s.Tripend,
                Itinerary = s.Itinerary,
                Tripduratn = s.Tripduratn,
                Hotelbkd = s.Hotelbkd
            }).ToList();

            return data;
        }
    }
}
