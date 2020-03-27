using System.Collections.Generic;
using Domain.Models.ReportPrograms.TopBottomHotelsReport;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomHotels
{
    public class TopBottomHotelRawGroupData
    {
        public void UpdateGroupedRawData(string groupBy, List<RawData> RawDataList, ref string subTitle, ref string catDesc1, 
            ref string catDesc2, bool isPreview, bool firstPass, ref List<GroupedRawData> paramGroupedRawData, ref List<GroupedRawData> paramGroupedRawData2)
        {
            List<GroupedRawData> localGroupedRawData = null;
            switch (groupBy)
            {
                case "2":
                    localGroupedRawData =
                        RawDataList.GroupBy(s => new { s.Chaincod, s.Bookrate },
                            (key, recs) => GetGroupedRecord(recs, isPreview, key.Bookrate, key.Chaincod))
                            .ToList();
                    subTitle = "Hotel Chains Bookings";
                    catDesc1 = "Hotel Chain";
                    catDesc2 = "Hotel Chain";
                    break;
                case "3":
                    localGroupedRawData =
                        RawDataList.GroupBy(s => new { s.Hotcity, s.Hotstate, s.Bookrate },
                            (key, recs) => GetGroupedRecord(recs, isPreview, key.Bookrate, key.Hotcity, key.Hotstate))
                            .ToList();
                    subTitle = "Hotel Cities Bookings";
                    catDesc1 = "Hotel City";
                    catDesc2 = "Hotel Cities";
                    break;
                case "4":
                    localGroupedRawData =
                        RawDataList.GroupBy(s => new { s.Hotstate, s.Bookrate },
                            (key, recs) => GetGroupedRecord(recs, isPreview, key.Bookrate, key.Hotstate))
                            .ToList();
                    subTitle = "Hotel States/Countries Bookings";
                    catDesc1 = "Hotel State/Country";
                    catDesc2 = "Hotel States/Countries";
                    break;
                case "5":
                    localGroupedRawData =
                        RawDataList.GroupBy(s => new { s.Hotcity, s.Hotstate, s.Chaincod, s.Bookrate },
                            (key, recs) =>
                                GetGroupedRecord(recs, isPreview, key.Bookrate, key.Hotcity, key.Hotstate, key.Chaincod))
                            .ToList();
                    subTitle = "Hotel Cities Bookings, with Hotel Chain Breakdown";
                    catDesc1 = "Hotel City";
                    catDesc2 = "Hotel Cities";
                    break;
                case "6":
                    localGroupedRawData =
                        RawDataList.GroupBy(s => new { s.Metro, s.Chaincod, s.Bookrate },
                            (key, recs) => GetGroupedRecord(recs, isPreview, key.Bookrate, key.Metro, key.Chaincod)).ToList()
                            .ToList();
                    subTitle = "Hotel Metro Regions, with Hotel Chain Breakdown";
                    catDesc1 = "Metro Region";
                    catDesc2 = "Hotel Chain";
                    break;
                case "7":
                    localGroupedRawData =
                        RawDataList.GroupBy(s => new { s.Metro, s.Hotelnam, s.Bookrate },
                            (key, recs) => GetGroupedRecord(recs, isPreview, key.Bookrate, key.Metro, key.Hotelnam))
                            .ToList();
                    subTitle = "Hotel Metro Regions, with Hotel Property Breakdown";
                    catDesc1 = "Metro Region";
                    catDesc2 = "Hotel Property";
                    break;
                case "8":
                    localGroupedRawData =
                        RawDataList.GroupBy(s => new { s.Metro, s.Hotcity, s.Hotcountry, s.Bookrate },
                            (key, recs) =>
                                GetGroupedRecord(recs, isPreview, key.Bookrate, key.Metro, key.Hotcity, key.Hotcountry))
                            .ToList();
                    subTitle = "Hotel Country, with Hotel City Breakdown";
                    catDesc1 = "Hotel Country";
                    catDesc2 = "Hotel City";
                    break;
                default:
                    localGroupedRawData =
                        RawDataList.GroupBy(s => new { s.Hotelnam, s.Bookrate },
                            (key, recs) => GetGroupedRecord(recs, isPreview, key.Bookrate, key.Hotelnam))
                            .ToList();
                    subTitle = "Hotel Properties Bookings";
                    catDesc1 = "Hotel Property";
                    catDesc2 = "Hotel Properties";
                    break;
            }

            if (firstPass)
                paramGroupedRawData = localGroupedRawData;
            else
                paramGroupedRawData2 = localGroupedRawData;
        }

        private static GroupedRawData GetGroupedRecord(IEnumerable<RawData> recs, bool isPreview, decimal bookRate, string key, string col2 = "", string col3 = "")
        {
            var reclist = recs.ToList();
            return isPreview
                ? new GroupedRawData
                    {
                        Category = key.Trim(),
                        GroupColumn2 = col2.Trim(),
                        GroupColumn3 = col3.Trim(),
                        Stays = reclist.Count,
                        Nights = reclist.Sum(s => s.Nights * s.Rooms),
                        HotelCost = reclist.Sum(s => s.Bookrate * s.Nights * s.Rooms),
                        SumBkRate = reclist.Sum(s => s.Bookrate),
                        BookRate = bookRate
                    }
                : new GroupedRawData
                    {
                        Category = key.Trim(),
                        GroupColumn2 = col2.Trim(),
                        GroupColumn3 = col3.Trim(),
                        Stays = reclist.Sum(s => s.Hplusmin),
                        Nights = reclist.Sum(s => s.Nights * s.Rooms * s.Hplusmin),
                        HotelCost = reclist.Sum(s => s.Bookrate * s.Nights * s.Rooms),
                        SumBkRate = reclist.Sum(s => s.Bookrate),
                        BookRate = bookRate
                    };
        }
    }
}
