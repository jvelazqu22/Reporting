using Domain.Helper;
using Domain.Models.ReportPrograms.TopBottomHotelsReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomHotels
{
    public class TopBottomHotelFinalGroupData
    {
        public List<GroupedByHotel> GroupBy(string _groupBy, ref List<GroupedRawData> _groupedRawData, IMasterDataStore MasterStore, ReportGlobals Globals)
        {
            List<GroupedByHotel> groupedByHotel;
            switch (_groupBy)
            {
                case "3":
                    groupedByHotel = _groupedRawData.Where(s => !string.IsNullOrEmpty(s.Category))
                    .GroupBy(s => (string.IsNullOrEmpty(s.Category) ? "[UNKNOWN]" : s.Category) + (string.IsNullOrEmpty(s.GroupColumn2) ? "" : ", " + s.GroupColumn2),
                    (key, recs) =>
                    {
                        var reclist = recs.ToList();
                        return new GroupedByHotel
                        {
                            Category = key,
                            Stays = reclist.Sum(s => s.Stays),
                            Nights = reclist.Sum(s => s.Nights),
                            NzNights = reclist.Sum(s => s.BookRate == 0 ? 0 : s.Nights),
                            HotelCost = reclist.Sum(s => s.HotelCost),
                            BookRate = reclist.Sum(s => s.SumBkRate),
                            BookCnt = reclist.Sum(s => s.BookRate == 0 ? 0 : s.Stays)
                        };
                    }).ToList();
                    break;
                case "5":
                    groupedByHotel = _groupedRawData.Where(s => !string.IsNullOrEmpty(s.Category))
                   .GroupBy(s => new { Category = (string.IsNullOrEmpty(s.Category) ? "[UNKNOWN]" : s.Category) + (string.IsNullOrEmpty(s.GroupColumn2) ? "" : ", " + s.GroupColumn2), Cat2 = s.GroupColumn3 },
                   (key, recs) =>
                   {
                       var reclist = recs.ToList();
                       return new GroupedByHotel
                       {
                           Category = key.Category,
                           Cat2 = LookupFunctions.LookupChains(key.Cat2, MasterStore),
                           Stays = reclist.Sum(s => s.Stays),
                           Nights = reclist.Sum(s => s.Nights),
                           NzNights = reclist.Sum(s => s.BookRate == 0 ? 0 : s.Nights),
                           HotelCost = reclist.Sum(s => s.HotelCost),
                           BookRate = reclist.Sum(s => s.SumBkRate),
                           BookCnt = reclist.Sum(s => s.BookRate == 0 ? 0 : s.Stays)
                       };
                   }).ToList();
                    break;
                case "6":
                case "7":
                    groupedByHotel = _groupedRawData.Where(s => !string.IsNullOrEmpty(s.Category))
                   .GroupBy(s => new { s.Category, Cat2 = s.GroupColumn2 },
                   (key, recs) =>
                   {
                       var reclist = recs.ToList();
                       return new GroupedByHotel
                       {
                           Category = key.Category,
                           Cat2 = _groupBy.Equals("6") ? LookupFunctions.LookupChains(key.Cat2, MasterStore) : key.Cat2,
                           Stays = reclist.Sum(s => s.Stays),
                           Nights = reclist.Sum(s => s.Nights),
                           NzNights = reclist.Sum(s => s.BookRate == 0 ? 0 : s.Nights),
                           HotelCost = reclist.Sum(s => s.HotelCost),
                           BookRate = reclist.Sum(s => s.SumBkRate),
                           BookCnt = reclist.Sum(s => s.BookRate == 0 ? 0 : s.Stays)
                       };
                   }).ToList();
                    break;
                case "8":
                    groupedByHotel = _groupedRawData.Where(s => !string.IsNullOrEmpty(s.Category))
                   .GroupBy(s => new { Category = TopHotelHelpers.LookupCountry(s.Category, s.GroupColumn3, Globals.UserLanguage), Cat2 = s.GroupColumn2 },
                   (key, recs) =>
                   {
                       var reclist = recs.ToList();
                       return new GroupedByHotel
                       {
                           Category = key.Category,
                           Cat2 = key.Cat2,
                           Stays = reclist.Sum(s => s.Stays),
                           Nights = reclist.Sum(s => s.Nights),
                           NzNights = reclist.Sum(s => s.BookRate == 0 ? 0 : s.Nights),
                           HotelCost = reclist.Sum(s => s.HotelCost),
                           BookRate = reclist.Sum(s => s.SumBkRate),
                           BookCnt = reclist.Sum(s => s.BookRate == 0 ? 0 : s.Stays)
                       };
                   }).ToList();
                    groupedByHotel = SetMissingCategoryToUnknown(groupedByHotel);
                    break;
                default:
                    LookUpChainDescription(_groupedRawData, _groupBy, MasterStore);
                    groupedByHotel = _groupedRawData.Where(s => !string.IsNullOrEmpty(s.Category))
                   .GroupBy(s => string.IsNullOrEmpty(s.Category) ? "[UNKNOWN]" : s.Category,
                   (key, recs) =>
                   {
                       var reclist = recs.ToList();
                       return new GroupedByHotel
                       {
                           Category = key,
                           //Category = _groupBy == "2" ? LookupFunctions.LookupChains(key, MasterStore) : key,
                           Stays = reclist.Sum(s => s.Stays),
                           Nights = reclist.Sum(s => s.Nights),
                           NzNights = reclist.Sum(s => s.BookRate == 0 ? 0 : s.Nights),
                           HotelCost = reclist.Sum(s => s.HotelCost),
                           BookRate = reclist.Sum(s => s.SumBkRate),
                           BookCnt = reclist.Sum(s => s.BookRate == 0 ? 0 : s.Stays)
                       };
                   }).ToList();
                    break;
            }

            return groupedByHotel;
        }

        public List<GroupedByHotel> SetMissingCategoryToUnknown(List<GroupedByHotel> groupedByHotel)
        {
            foreach(var record in groupedByHotel)
                if (string.IsNullOrWhiteSpace(record.Category))
                    record.Category = "[UNKNOWN]";

            return groupedByHotel;
        }

        public void LookUpChainDescription(List<GroupedRawData> _groupedRawData, string _groupBy, IMasterDataStore MasterStore)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if(_groupBy.Equals("2"))
            {
                var categoryList = _groupedRawData.Select(s => s.Category).Distinct();
                foreach(var category in categoryList)
                {
                    var categoryDescription = LookupFunctions.LookupChains(category, MasterStore);
                    dict.Add(category, categoryDescription);
                }
                foreach(var record in _groupedRawData)
                {
                    string categoryDescription = string.Empty;
                    var wasKeyFound = dict.TryGetValue(record.Category, out categoryDescription);
                    if (wasKeyFound)
                        record.Category = categoryDescription;
                }
            }
        }

        public List<FinalData> GetFinalDataList(string _groupBy, List<GroupedByHotel> groupedByHotel, string _sortBy, ReportGlobals Globals, int howMany, IMasterDataStore MasterStore)
        {
            List<FinalData> FinalDataList = new List<FinalData>();

            if ("5,6,7,8".Contains(_groupBy))
            {
                //FIRST PASS GIVES US THE TOTALS BY CHAIN WITHIN CITY / METRO. 
                var passOne = groupedByHotel.Select(s => new
                {
                    Category = string.IsNullOrEmpty(s.Category) ? "[Unknown]" : s.Category,
                    s.Cat2,
                    Stays2 = s.Stays,
                    Nights2 = s.Nights,
                    Hotelcost2 = s.HotelCost,
                    Bookrate2 = s.BookRate,
                    Bookcnt2 = s.BookCnt,
                    Nznights2 = s.NzNights,
                    Avgbook2 = s.BookCnt == 0 ? 0 : s.BookRate / s.BookCnt
                }).ToList();

                //NEXT PASS GIVES US THE TOTALS BY CITY / METRO / COUNTRY. 
                var passTwo = groupedByHotel.GroupBy(s => s.Category, (key, recs) =>
                {
                    var reclist = recs.ToList();
                    return new FinalData
                    {
                        Category = key,
                        Stays = reclist.Sum(s => s.Stays),
                        Nights = reclist.Sum(s => s.Nights),
                        Hotelcost = reclist.Sum(s => s.HotelCost),
                        Bookrate = reclist.Sum(s => s.BookRate),
                        Bookcnt = reclist.Sum(s => s.BookCnt),
                        Nznights = reclist.Sum(s => s.NzNights),
                        Avgbook = reclist.Sum(s => s.BookCnt) == 0 ? 0 : reclist.Sum(s => s.BookRate) / reclist.Sum(s => s.BookCnt)
                    };
                }).ToList();

                passTwo = TopHotelHelpers.SortFinalData(_sortBy, passTwo, Globals.ParmValueEquals(WhereCriteria.RBSORTDESCASC, "1"));
                if (howMany > 0)
                    passTwo = passTwo.Take(howMany).ToList();

                FinalDataList =
                    passOne.Join(passTwo, o => o.Category, t => t.Category, (o, t) => new FinalData
                    {
                        Category = o.Category,
                        Cat2 = o.Cat2,
                        Stays = t.Stays,
                        Nights = t.Nights,
                        Hotelcost = t.Hotelcost,
                        Bookrate = t.Bookrate,
                        Bookcnt = t.Bookcnt,
                        Nznights = t.Nznights,
                        Avgbook = t.Avgbook,
                        Stays2 = o.Stays2,
                        Nights2 = o.Nights2,
                        Hotelcost2 = o.Hotelcost2,
                        Bookrate2 = o.Bookrate2,
                        Bookcnt2 = o.Bookcnt2,
                        Nznights2 = o.Nznights2,
                        Avgbook2 = o.Avgbook2
                    }).ToList();

                if (!Globals.ParmValueEquals(WhereCriteria.RBSORTDESCASC, "2") && _sortBy != "5")
                    FinalDataList = ApplyDescendingOrder(_sortBy, FinalDataList);
                else
                    FinalDataList = ApplyAscedingOrder(_sortBy, FinalDataList);

                if ("6,7".Contains(_groupBy))
                    foreach (var row in FinalDataList)
                        row.Category = LookupFunctions.LookupMetro(row.Category, MasterStore) + " (" + row.Category + ")";
            }
            else
            {
                FinalDataList = groupedByHotel.Select(s => new FinalData
                {
                    Category = s.Category,
                    Stays = s.Stays,
                    Nights = s.Nights,
                    Hotelcost = s.HotelCost,
                    Bookrate = s.BookRate,
                    Bookcnt = s.BookCnt,
                    Nznights = s.NzNights,
                    Avgbook = s.BookCnt == 0 ? 0 : s.BookRate / s.BookCnt
                }).ToList();
            }

            return FinalDataList;
        }

        public List<FinalData> ApplyAscedingOrder(string _sortBy, List<FinalData> FinalDataList)
        {
            switch (_sortBy)
            {
                case "1":
                    return FinalDataList.OrderBy(s => s.Hotelcost)
                            .ThenBy(s => s.Hotelcost2)
                            .ThenBy(s => s.Category)
                            .ToList();
                case "2":
                    return FinalDataList.OrderBy(s => s.Stays)
                            .ThenBy(s => s.Category)
                            .ThenBy(s => s.Stays2)
                            .ToList();
                case "3":
                    return FinalDataList.OrderBy(s => s.Nights)
                            .ThenBy(s => s.Category)
                            .ThenBy(s => s.Nights2)
                            .ToList();
                case "4":
                    return FinalDataList.OrderBy(s => s.Avgbook)
                            .ThenBy(s => s.Category)
                            .ThenBy(s => s.Avgbook2)
                            .ToList();
                case "5":
                    return FinalDataList.OrderBy(s => s.Category)
                            .ThenBy(s => s.Cat2)
                            .ToList();
                default:
                    return FinalDataList;
            }
        }

        public List<FinalData> ApplyDescendingOrder(string _sortBy, List<FinalData> FinalDataList)
        {
            switch (_sortBy)
            {
                case "1":
                    return FinalDataList.OrderByDescending(s => s.Hotelcost)
                            .ThenByDescending(s => s.Hotelcost2)
                            .ThenBy(s => s.Category)
                            .ToList();
                case "2":
                    return FinalDataList.OrderByDescending(s => s.Stays)
                            .ThenBy(s => s.Category)
                            .ThenByDescending(s => s.Stays2)
                            .ToList();
                case "3":
                    return FinalDataList.OrderByDescending(s => s.Nights)
                            .ThenBy(s => s.Category)
                            .ThenByDescending(s => s.Nights2)
                            .ToList();
                case "4":
                    return FinalDataList.OrderByDescending(s => s.Avgbook)
                            .ThenBy(s => s.Category)
                            .ThenByDescending(s => s.Avgbook2)
                            .ToList();
                default:
                    return FinalDataList;
            }
        }

    }
}
