using System.Collections.Generic;
using System.Linq;
using iBank.Services.Implementation.Shared;
using iBank.Server.Utilities.Classes;
using System;

using Domain.Models.ReportPrograms.ExecutiveSummaryByHomeCountryReport;
using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Repository.SQL.Repository;
using iBank.Services.Implementation.Utilities;
using iBank.Server.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.ExecutiveSummaryByHomeCountry
{
    public class ExecutiveSummaryByHomeCountryDataProcessor
    {
        MasterFunctions masterFunctions = new MasterFunctions();

        public void AddAirAndRailData(List<string> sourceAbbrs, List<AirRawData> RawDataList, ReportGlobals Globals, List<FinalData> FinalDataList)
        {
            var tempQuery = sourceAbbrs.Any()
                ? RawDataList.Where(s => sourceAbbrs.Contains(s.SourceAbbr.Trim()))
                : RawDataList;

            var temp = tempQuery
                .Select(s => new AirRawData
                {
                    HomeCtry = masterFunctions.LookupHomeCountryName(s.SourceAbbr.Trim(), Globals, new GetHomeCountriesByAgencyForSharerClientQuery(new MasterDataStore().MastersQueryDb, Globals.Agency.Trim()),
                        new GetHomeCountriesByAgencyForNonSharerClientQuery(new MasterDataStore().MastersQueryDb, Globals.Agency.Trim()), new GetAllCountriesQuery(new MasterDataStore().MastersQueryDb)),
                    //HomeCtry = LookupFunctions.LookupHomeCountryName(s.SourceAbbr.Trim(), Globals, MasterStore),
                    PlusMin = s.PlusMin,
                    ValCarr = s.ValCarr,
                    Valcarmode = s.Valcarmode,
                    DepDate = s.DepDate,
                    ArrDate = s.ArrDate,
                    AirChg = s.AirChg,
                    StndChg =
                        Math.Abs(s.StndChg) < Math.Abs(s.AirChg) || s.StndChg == 0 || (s.StndChg > 0 && s.AirChg < 0)
                            ? s.AirChg
                            : s.StndChg,
                    OffrdChg = s.OffrdChg > 0 && s.AirChg < 0
                        ? 0 - s.OffrdChg
                        : s.OffrdChg == 0 ? s.AirChg : s.OffrdChg
                }).ToList();


            var airData = temp.Where(s => s.ValCarr.Trim().Length != 4 && !s.Valcarmode.EqualsIgnoreCase("R"))
                .GroupBy(s => s.HomeCtry, (key, recs) =>
                {
                    var reclist = recs as IList<AirRawData> ?? recs.ToList();
                    var netTrans = reclist.Sum(s => s.PlusMin);
                    var volume = reclist.Sum(s => s.AirChg);
                    var days = reclist.Sum(s => ((s.ArrDate.GetValueOrDefault() - s.DepDate.GetValueOrDefault()).Days + 1) * s.PlusMin);
                    var standardCharge = reclist.Sum(s => s.StndChg);
                    var savings = reclist.Sum(s => s.StndChg - s.AirChg);
                    var lostAmt = reclist.Sum(s => s.AirChg - s.OffrdChg);
                    return new FinalData
                    {
                        HomeCtry = key,
                        RowType = "A",
                        RowDesc = "Air Travel",
                        NetTrans = netTrans,
                        Volume = volume,
                        Days = days,
                        StandardCharge = standardCharge,
                        Savings = savings,
                        LostAmt = lostAmt,
                        AvgCost = netTrans == 0 ? 0 : MathHelper.Round(volume / netTrans, 2),
                        AvgDays = netTrans == 0 ? 0 : MathHelper.Round(((double)days / netTrans), 1),
                        SvngsPct = standardCharge == 0 ? 0 : MathHelper.Round(100 * (double)(savings / standardCharge), 2),
                        LossPct = volume == 0 ? 0 : MathHelper.Round(100 * (double)(lostAmt / volume), 2)
                    };
                }).ToList();

            var railData = temp.Where(s => s.ValCarr.Trim().Length == 4 || s.Valcarmode.EqualsIgnoreCase("R"))
                .GroupBy(s => s.HomeCtry, (key, recs) =>
                {
                    var reclist = recs as IList<AirRawData> ?? recs.ToList();
                    var netTrans = reclist.Sum(s => s.PlusMin);
                    var volume = reclist.Sum(s => s.AirChg);
                    var days = reclist.Sum(s => ((s.ArrDate.GetValueOrDefault() - s.DepDate.GetValueOrDefault()).Days + 1) * s.PlusMin);
                    var standardCharge = reclist.Sum(s => s.StndChg);
                    var savings = reclist.Sum(s => s.StndChg - s.AirChg);
                    var lostAmt = reclist.Sum(s => s.AirChg - s.OffrdChg);
                    return new FinalData
                    {
                        HomeCtry = key,
                        RowType = "D",
                        RowDesc = "Rail Travel",
                        NetTrans = netTrans,
                        Volume = volume,
                        Days = days,
                        StandardCharge = standardCharge,
                        Savings = savings,
                        LostAmt = lostAmt,
                        AvgCost = netTrans == 0 ? 0 : MathHelper.Round(volume / netTrans, 2),
                        AvgDays = netTrans == 0 ? 0 : MathHelper.Round((double)days / netTrans, 1),
                        SvngsPct = standardCharge == 0 ? 0 : MathHelper.Round(100 * (double)(savings / standardCharge), 2),
                        LossPct = volume == 0 ? 0 : MathHelper.Round(100 * (double)(lostAmt / volume), 2)
                    };
                }).ToList();
            FinalDataList.AddRange(airData);
            FinalDataList.AddRange(railData);
        }

        public void AddHotelData(List<string> sourceAbbrs, List<HotelRawData> hotelRawDataList, ReportGlobals Globals, List<FinalData> FinalDataList)
        {
            var tempQuery = sourceAbbrs.Any()
                ? hotelRawDataList.Where(s => sourceAbbrs.Contains(s.SourceAbbr.Trim()))
                : hotelRawDataList;

            var hotelTemp = tempQuery
                .Select(s => new HotelRawData
                {
                    HomeCtry = masterFunctions.LookupHomeCountryName(s.SourceAbbr.Trim(), Globals, new GetHomeCountriesByAgencyForSharerClientQuery(new MasterDataStore().MastersQueryDb, Globals.Agency.Trim()),
                        new GetHomeCountriesByAgencyForNonSharerClientQuery(new MasterDataStore().MastersQueryDb, Globals.Agency.Trim()), new GetAllCountriesQuery(new MasterDataStore().MastersQueryDb)),
                    //HomeCtry = LookupFunctions.LookupHomeCountryName(s.SourceAbbr.Trim(), Globals, MasterStore),
                    HPlusMin = s.HPlusMin,
                    Rooms = s.Rooms,
                    Nights = s.Nights,
                    BookRate = s.BookRate
                })
                .GroupBy(s => s.HomeCtry, (key, recs) =>
                {
                    var reclist = recs as IList<HotelRawData> ?? recs.ToList();
                    var netTrans = reclist.Sum(s => s.HPlusMin);
                    var nights = reclist.Sum(s => s.Rooms * s.Nights * s.HPlusMin);
                    var volume = reclist.Sum(s => s.BookRate * s.Nights * s.Rooms);


                    return new FinalData
                    {
                        HomeCtry = key,
                        RowType = "B",
                        RowDesc = "Hotel Bookings",
                        NetTrans = netTrans,
                        Days = nights,
                        Volume = volume,
                        AvgCost = netTrans == 0 ? 0 : MathHelper.Round(volume / nights, 2),
                        AvgDays = netTrans == 0 ? 0 : MathHelper.Round((double)nights / netTrans, 1),
                    };
                });
            FinalDataList.AddRange(hotelTemp);
        }

        public void AddCarData(List<string> sourceAbbrs, List<CarRawData> carRawDataList, ReportGlobals Globals, List<FinalData> FinalDataList)
        {
            var tempQuery = sourceAbbrs.Any()
                ? carRawDataList.Where(s => sourceAbbrs.Contains(s.SourceAbbr.Trim()))
                : carRawDataList;

            var carTemp = tempQuery
                .Select(s => new CarRawData
                {
                    HomeCtry = masterFunctions.LookupHomeCountryName(s.SourceAbbr.Trim(), Globals, new GetHomeCountriesByAgencyForSharerClientQuery(new MasterDataStore().MastersQueryDb, Globals.Agency.Trim()),
                        new GetHomeCountriesByAgencyForNonSharerClientQuery(new MasterDataStore().MastersQueryDb, Globals.Agency.Trim()), new GetAllCountriesQuery(new MasterDataStore().MastersQueryDb)),
                    //HomeCtry = LookupFunctions.LookupHomeCountryName(s.SourceAbbr.Trim(), Globals, MasterStore),
                    CPlusMin = s.CPlusMin,
                    Days = s.Days,
                    ABookRat = s.ABookRat
                })
                .GroupBy(s => s.HomeCtry, (key, recs) =>
                {
                    var reclist = recs as IList<CarRawData> ?? recs.ToList();
                    var netTrans = reclist.Sum(s => s.CPlusMin);
                    var days = reclist.Sum(s => s.Days * s.CPlusMin);
                    var volume = reclist.Sum(s => s.ABookRat * s.Days);


                    return new FinalData
                    {
                        HomeCtry = key,
                        RowType = "C",
                        RowDesc = "Car Rentals",
                        NetTrans = netTrans,
                        Volume = volume,
                        Days = days,
                        AvgCost = netTrans == 0 ? 0 : MathHelper.Round(volume / days, 2),
                        AvgDays = netTrans == 0 ? 0 : MathHelper.Round((double)days / netTrans, 1),
                    };
                });

            FinalDataList.AddRange(carTemp);
        }

        public void AddServiceFees(List<SvcFeeRawData> _svcRawDataList, ReportGlobals Globals, List<FinalData> FinalDataList)
        {
            var svcFeeTemp = _svcRawDataList.Select(s => new SvcFeeRawData
            {
                HomeCtry = masterFunctions.LookupHomeCountryName(s.SourceAbbr.Trim(), Globals, new GetHomeCountriesByAgencyForSharerClientQuery(new MasterDataStore().MastersQueryDb, Globals.Agency.Trim()),
                        new GetHomeCountriesByAgencyForNonSharerClientQuery(new MasterDataStore().MastersQueryDb, Globals.Agency.Trim()), new GetAllCountriesQuery(new MasterDataStore().MastersQueryDb)),
                //HomeCtry = LookupFunctions.LookupHomeCountryName(s.SourceAbbr, Globals, MasterStore),
                SvcAmt = s.SvcAmt
            }).GroupBy(s => s.HomeCtry, (key, recs) =>
            {
                var reclist = recs as IList<SvcFeeRawData> ?? recs.ToList();
                var netTrans = reclist.Sum(s => s.SvcAmt < 0 ? -1 : 1);
                var volume = reclist.Sum(s => s.SvcAmt);
                return new FinalData
                {
                    HomeCtry = key,
                    RowType = "E",
                    RowDesc = "Other",
                    NetTrans = netTrans,
                    Volume = volume,
                    AvgCost = netTrans == 0 ? 0 : MathHelper.Round(volume / netTrans, 2)
                };
            });
            FinalDataList.AddRange(svcFeeTemp);
        }

    }
}