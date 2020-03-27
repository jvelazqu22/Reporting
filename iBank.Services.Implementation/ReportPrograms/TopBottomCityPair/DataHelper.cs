using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.TopBottomCityPairReport;
using iBank.Repository.SQL.Repository;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomCityPair
{
    public class DataHelper
    {
        public string GetSubTitle(bool bothWay)
        {
            if (bothWay) return "City Pairs Represent Trips in Both Directions";
            else return "City Pairs Represent Trips in Each Direction Separately";
        }

        public List<string> GetExportFields(bool isMetric, bool isCarbon)
        {
            var fieldList = new List<string>();
            //sortby 1
            fieldList.Add("origin");
            fieldList.Add("orgdesc");
            fieldList.Add("destinat");
            fieldList.Add("destdesc");
            fieldList.Add("cpsegs");
            fieldList.Add("cpnumticks");
            fieldList.Add("cppctttl");
            fieldList.Add("cpcost");
            fieldList.Add("cpavgcost");
            fieldList.Add("cpmiles");
            if (isCarbon)
                fieldList.Add("cpairco2");
            if (isMetric) fieldList.Add("cpcst_km");
            else fieldList.Add("cpcst_mile");
            fieldList.Add("airline");
            fieldList.Add("alinedesc");
            fieldList.Add("segments");
            fieldList.Add("pctttl");
            fieldList.Add("numticks");
            fieldList.Add("cost");
            fieldList.Add("miles");
            if (isMetric) fieldList.Add("cst_km");
            else fieldList.Add("cst_mile");
            if (isCarbon)
                fieldList.Add("airco2");


            return fieldList;
        }

        public List<RawData> AllocateAirCharge(List<RawData> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                var totalMiles = list.Where(x => x.RecKey == item.RecKey).Sum(s => s.Miles);
                if(totalMiles > 0)
                {
                    var tnFactor = MathHelper.Round((decimal)item.Miles / totalMiles, 12);
                    item.ActFare = MathHelper.Round((decimal)item.BaseFare * tnFactor, 2);
                }
            }

            return list;
        }

        public List<RawData> AllocateAirCharge(List<RawData> allLegsList, List<RawData> resultSegList)
        {
            for (int i = 0; i < resultSegList.Count; i++)
            {
                var item = resultSegList[i];
                var totalMiles = allLegsList.Where(x => x.RecKey == item.RecKey).Sum(s => s.Miles);
                if (totalMiles > 0)
                {
                    var tnFactor = MathHelper.Round((decimal)item.Miles / totalMiles, 12);
                    item.ActFare = MathHelper.Round((decimal)item.BaseFare * tnFactor, 2);
                }
            }

            return resultSegList;
        }

        public List<RawData> CalculateOneWayBothWay(List<RawData> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                string origin = list[i].Origin;
                string destinat = list[i].Destinat;
                int compareResult = string.Compare(origin, destinat);
                if (compareResult > 0)
                {
                    list[i].Origin = destinat;
                    list[i].Destinat = origin;
                }
            }
            return list;
        }

        public List<RawData> ApplyCityPair(List<RawData> list)
        {

            /*** 05/07/2004 - EXPLANATION OF plusmin*abs(actfare)) BELOW:  ACTFARE COMES **
             ** INTO iBank AS POSITIVE, EVEN FOR CREDITS.  HOWEVER, THE ROUTINE ABOVE   **
             ** THAT ALLOCATES AIRCHG FOR RECORDS WHERE THE ACTFARE IS ZERO WILL        **
             ** ESTABLISH THE PROPER SIGN, SO , WE NEED TO SET THE CORRECT SIGN BY      **
             ** USING THE ABS() FUNCTION.                                               **


             ** 05/20/2014 - ADD ABILITY TO GROUP CITY PAIRS BY METRO, INSTEAD OF AIRPORT CODE. **
             ** IF THIS OPTION CHOSEN, WE NEED TO SUBSTITUTE METROS FOR AIRPORTS RIGHT HERE,    **
             ** BEFORE WE SUMMARIZE.                                                            **
             */

            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                string metrOrg = LookupFunctions.LookupMetroCode(new MasterDataStore(), item.Origin, item.Mode);
                string metrDest = LookupFunctions.LookupMetroCode(new MasterDataStore(), item.Destinat, item.Mode);
                item.Origin = metrOrg;
                item.Destinat = metrDest;
            }
            return list;
        }

        public List<RawData> CalculateAdopt(List<RawData> list)
        {
            var recKeyList = list.Select(s => new
            {
                RecKey = s.RecKey,
                Numsegs = 1.0000
            }).GroupBy(s => new {
                RecKey = s.RecKey,
                Numsegs = 1.0000
            }).Select(s => new
            {
                RecKey = s.Key.RecKey,
                Numsegs = s.Sum(x => x.Numsegs)
            }).ToList();

            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                int segs = (int)recKeyList.Where(x => x.RecKey == item.RecKey).Select(x => x.Numsegs).Sum();
                decimal tkts = MathHelper.Round((decimal)1 / segs, 3);
                item.NumTicks = segs != 0
                    ? tkts
                    : 0.00m;
                item.OnlineTkts = segs != 0 && item.Bktool == "ONLINE"
                    ? tkts
                    : 0.00m;
                item.AgentTkts = segs != 0 && item.Bktool != "ONLINE"
                    ? tkts
                    : 0.00m;
            }

            return list;
        }

        public string GetOrderBy(DataTypes.SortBy sortBy, bool isUseTickCnt)
        {
            string result = "";
            switch (sortBy)
            {
                case DataTypes.SortBy.VOLUME_BOOKED:
                    result = "Cpcost";
                    break;
                case DataTypes.SortBy.AVG_COST_PER_SEGMENT_OR_TRIP:
                    result = "CpAvgcos";
                    break;
                case DataTypes.SortBy.NO_OF_SEGMENT_OR_TRIP:
                    result = isUseTickCnt
                        ? "CpNumticks"
                        : "Cpsegs";
                    break;
                case DataTypes.SortBy.CITY_PAIR:
                    result = "Orgdesc";
                    break;
            }

            return result;
        }

    }
}
