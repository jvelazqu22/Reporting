using Domain.Helper;
using Domain.Models.ReportPrograms.TopBottomCars;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomCars
{
    public class TopBottomCarsFinalData
    {
        private TopCarHelpers _topCarHelpers = new TopCarHelpers();

        public List<FinalData> GetFinalDataList(string _groupBy, List<GroupedRawData> _groupedRawData, int howMany, string _sortBy, ReportGlobals Globals, IMasterDataStore MasterStore,
            ref decimal TotCnt, ref decimal TotDays, ref decimal TotNzDays, ref decimal TotCost, ref decimal TotRate, ref decimal TotBookCnt,
            ref decimal TotCnt2, ref decimal TotDays2, ref decimal TotNzDays2, ref decimal TotCost2, bool _secondRange, List<GroupedRawData> _groupedRawData2)
        {
            if (_groupBy.Equals("4"))
                return GroupByRentalCitiesWithABreakdownByCarCompanies(_groupBy, _groupedRawData, howMany, _sortBy, Globals, MasterStore, ref TotCnt, ref TotDays,
                    ref TotNzDays, ref TotCost, ref TotRate, ref TotBookCnt, ref TotCnt2, ref TotDays2, ref TotNzDays2, ref TotCost2, _secondRange, _groupedRawData2);
            else
                return DefaultGroupBy(_groupBy, _groupedRawData, howMany, _sortBy, Globals, ref TotCnt, ref TotDays, ref TotNzDays, ref TotCost, ref TotRate,
                            ref TotBookCnt, ref TotCnt2, ref TotDays2, ref TotNzDays2, ref TotCost2, _secondRange, _groupedRawData2);
        }

        public List<FinalData> DefaultGroupBy(string _groupBy, List<GroupedRawData> _groupedRawData, int howMany, string _sortBy,
            ReportGlobals Globals, ref decimal TotCnt, ref decimal TotDays, ref decimal TotNzDays, ref decimal TotCost, ref decimal TotRate,
            ref decimal TotBookCnt, ref decimal TotCnt2, ref decimal TotDays2, ref decimal TotNzDays2, ref decimal TotCost2, bool _secondRange, List<GroupedRawData> _groupedRawData2)
        {
            List<FinalData> FinalDataList = new List<FinalData>();
            var groupedByCar = _topCarHelpers.GroupData(_groupBy, _groupedRawData);

            FinalDataList = groupedByCar.Select(s => new FinalData
            {
                Category = s.Category,
                Days = s.Days,
                Rentals = s.Rentals,
                Carcost = s.CarCost,
                Bookrate = s.BookRate,
                Bookcnt = s.BookCnt,
                Nzdays = s.NzDays,
                Avgbook = s.BookCnt == 0 ? 0 : s.BookRate / s.BookCnt
            }).ToList();

            TotCnt = FinalDataList.Sum(s => s.Rentals);
            TotDays = FinalDataList.Sum(s => s.Days);
            TotNzDays = FinalDataList.Sum(s => s.Nzdays);
            TotCost = FinalDataList.Sum(s => s.Carcost);
            TotRate = FinalDataList.Sum(s => s.Bookrate);
            TotBookCnt = FinalDataList.Sum(s => s.Bookcnt);
            if (_secondRange)
            {

                List<FinalData> FinalDataList2 = new List<FinalData>();
                var groupedByCar2 = _topCarHelpers.GroupData(_groupBy, _groupedRawData2);

                FinalDataList2 = groupedByCar2.Select(s => new FinalData
                {
                    Category = s.Category,
                    Days = s.Days,
                    Rentals = s.Rentals,
                    Carcost = s.CarCost,
                    Bookrate = s.BookRate,
                    Bookcnt = s.BookCnt,
                    Nzdays = s.NzDays,
                    Avgbook = s.BookCnt == 0 ? 0 : s.BookRate / s.BookCnt
                }).ToList();
                _topCarHelpers.JoinRanges(FinalDataList, FinalDataList2);

                TotCnt2 = FinalDataList.Sum(s => s.Rentals2);
                TotDays2 = FinalDataList.Sum(s => s.Days2);
                TotNzDays2 = FinalDataList.Sum(s => s.Nzdays2);
                TotCost2 = FinalDataList.Sum(s => s.Carcost2);
            }

            FinalDataList = _topCarHelpers.SortFinalData(_sortBy, FinalDataList, Globals.ParmValueEquals(WhereCriteria.RBSORTDESCASC, "1"));

            if (howMany > 0 && !_sortBy.Equals("5"))
                FinalDataList = FinalDataList.Take(howMany).ToList();

            return FinalDataList;
        }

        public List<FinalData> GroupByRentalCitiesWithABreakdownByCarCompanies(string _groupBy, List<GroupedRawData> _groupedRawData, int howMany, string _sortBy,
                ReportGlobals Globals, IMasterDataStore MasterStore, ref decimal TotCnt, ref decimal TotDays, ref decimal TotNzDays, ref decimal TotCost, ref decimal TotRate,
                ref decimal TotBookCnt, ref decimal TotCnt2, ref decimal TotDays2, ref decimal TotNzDays2, ref decimal TotCost2, bool _secondRange, List<GroupedRawData> _groupedRawData2)
        {
            List<FinalData> FinalDataList = new List<FinalData>();
            var groupedByCar = _topCarHelpers.GroupData(_groupBy, _groupedRawData);

            //FIRST PASS GIVES US THE TOTALS BY CHAIN WITHIN CITY / METRO. 
            var passOne = groupedByCar.Select(s => new
            {
                Category = string.IsNullOrEmpty(s.Category) ? "[Unknown]" : s.Category,
                s.Company,
                Rentals2 = s.Rentals,
                Days2 = s.Days,
                Carcost2 = s.CarCost,
                Bookrate2 = s.BookRate,
                Bookcnt2 = s.BookCnt,
                Nzdays2 = s.NzDays,
                Avgbook2 = s.BookCnt == 0 ? 0 : s.BookRate / s.BookCnt
            }).ToList();

            //NEXT PASS GIVES US THE TOTALS BY CITY / METRO / COUNTRY. 
            var passTwo = groupedByCar.GroupBy(s => s.Category, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new FinalData
                {
                    Category = key,
                    Rentals = reclist.Sum(s => s.Rentals),
                    Days = reclist.Sum(s => s.Days),
                    Carcost = reclist.Sum(s => s.CarCost),
                    Bookrate = reclist.Sum(s => s.BookRate),
                    Bookcnt = reclist.Sum(s => s.BookCnt),
                    Nzdays = reclist.Sum(s => s.NzDays),
                    Avgbook =
                        reclist.Sum(s => s.BookCnt) == 0
                            ? 0
                            : reclist.Sum(s => s.BookRate) / reclist.Sum(s => s.BookCnt)
                };
            }).ToList();

            passTwo = _topCarHelpers.SortFinalData(_sortBy, passTwo, Globals.ParmValueEquals(WhereCriteria.RBSORTDESCASC, "1"));

            if (howMany > 0 && !_sortBy.Equals("5"))
                passTwo = passTwo.Take(howMany).ToList();

            FinalDataList =
                passOne.Join(passTwo, o => o.Category, t => t.Category, (o, t) => new FinalData
                {
                    Category = o.Category,
                    Company = o.Company,
                    Rentals = t.Rentals,
                    Days = t.Days,
                    Carcost = t.Carcost,
                    Bookrate = t.Bookrate,
                    Bookcnt = t.Bookcnt,
                    Nzdays = t.Nzdays,
                    Avgbook = t.Avgbook,
                    Rentals2 = o.Rentals2,
                    Days2 = o.Days2,
                    Carcost2 = o.Carcost2,
                    Bookrate2 = o.Bookrate2,
                    Bookcnt2 = o.Bookcnt2,
                    Nzdays2 = o.Nzdays2,
                    Avgbook2 = o.Avgbook2
                }).ToList();


            FinalDataList = _topCarHelpers.SortJoinedData(Globals.ParmValueEquals(WhereCriteria.RBSORTDESCASC, "2"), _sortBy, FinalDataList);

            if ("6,7".Contains(_groupBy))
                foreach (var row in FinalDataList)
                    row.Category = LookupFunctions.LookupMetro(row.Category, MasterStore) + " (" + row.Category + ")";

            TotCnt = passTwo.Sum(s => s.Rentals);
            TotDays = passTwo.Sum(s => s.Days);
            TotNzDays = passTwo.Sum(s => s.Nzdays);
            TotCost = passTwo.Sum(s => s.Carcost);
            TotRate = passTwo.Sum(s => s.Bookrate);
            TotBookCnt = passTwo.Sum(s => s.Bookcnt);

            return FinalDataList;
        }
    }
}
