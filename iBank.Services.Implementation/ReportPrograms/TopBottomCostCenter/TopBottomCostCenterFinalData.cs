using Domain.Models.ReportPrograms.TopBottomCostCenter;
using System.Collections.Generic;
using System.Linq;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomCostCenter
{
    public class TopBottomCostCenterFinalData
    {
        public List<FinalData> GetFinalList(List<RawData> RawDataList, BuildWhere BuildWhere, List<HotelRawData> HotelRawDataList, List<CarRawData> CarRawDataList, ReportGlobals Globals) 
        {
            TopBottomCostCenterHelpers.FixCharges(RawDataList);

            foreach (var row in RawDataList.Where(s => HotelRawDataList.Select(r => r.RecKey).Distinct().Contains(s.RecKey)))
            {
                row.NoHotel = 0;
            }

            //if there was Routing criteria, we need to remove all records from Car and Hotel that don't have a Trip. 
            if (BuildWhere.HasRoutingCriteria || BuildWhere.HasFirstDestination || BuildWhere.HasFirstOrigin)
            {
                var tripKeys = RawDataList.Select(s => s.RecKey).Distinct();
                CarRawDataList = CarRawDataList.Where(s => tripKeys.Contains(s.RecKey)).ToList();
                HotelRawDataList = HotelRawDataList.Where(s => tripKeys.Contains(s.RecKey)).ToList();
            }

            var FinalDataList = RawDataList.GroupBy(s => s.GrpCol, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new FinalData
                {
                    GrpCol = key.Trim(),
                    Airchg = reclist.Sum(s => s.AirChg),
                    Lostamt = reclist.Sum(s => s.AirChg - s.OffrdChg),
                    Numtrips = reclist.Sum(s => s.Plusmin),
                    Nohotel = reclist.Sum(s => s.NoHotel),
                };
            }).ToList();

            var carFinal = CarRawDataList.GroupBy(s => s.GrpCol, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new FinalData
                {
                    GrpCol = key.Trim(),
                    Rentals = reclist.Sum(s => s.CPlusMin),
                    Days = reclist.Sum(s => s.Days * s.CPlusMin),
                    Carcost = reclist.Sum(s => s.Days * s.ABookRat)
                };
            }).ToList();

            var hotelFinal = HotelRawDataList.GroupBy(s => s.GrpCol, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new FinalData
                {
                    GrpCol = key.Trim(),
                    Stays = reclist.Sum(s => s.HPlusMin),
                    Nights = reclist.Sum(s => s.Nights * s.Rooms * s.HPlusMin),
                    Hotelcost = reclist.Sum(s => s.Nights * s.Rooms * s.BookRate)
                };
            }).ToList();

            return TopBottomCostCenterHelpers.BuildFinalData(FinalDataList, carFinal, hotelFinal);
        }

        public void CalculateAndUpdateTotalParameterValues(ref decimal totChg, ref decimal totLost, ref int totCnt, ref int totCnt2, ref int totCnt3, ref int totStays, 
            ref int totNites, ref decimal totHotCost, ref int totRents, ref int totDays, ref decimal totCarCost, ref decimal totCost, List<FinalData> finalDataList)
        {
            totChg = finalDataList.Sum(s => s.Airchg);
            totLost = finalDataList.Sum(s => s.Lostamt);
            totCnt = finalDataList.Sum(s => s.Numtrips);
            totCnt2 = finalDataList.Sum(s => s.Nohotel);
            totStays = finalDataList.Sum(s => s.Stays);
            totNites = finalDataList.Sum(s => s.Nights);
            totHotCost = finalDataList.Sum(s => s.Hotelcost);
            totRents = finalDataList.Sum(s => s.Rentals);
            totDays = finalDataList.Sum(s => s.Days);
            totCarCost = finalDataList.Sum(s => s.Carcost);
            totCost = finalDataList.Sum(s => s.Totalcost);
        }
    }
}
