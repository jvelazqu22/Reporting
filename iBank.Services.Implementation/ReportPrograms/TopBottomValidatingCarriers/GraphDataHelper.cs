using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.TopBottomValidatingCarriers;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomValidatingCarriers
{
    public class GraphDataHelper
    {
        public List<GraphFinalData> ConvertToGraphFinalData(List<FinalData> reportData, bool isCtryCode, string orderBy)
        {
            var query = reportData.Select(x => new GraphFinalData
            {
                RecNumber = reportData.Count,
                CatDesc = isCtryCode ? x.HomeCtry : x.Carrdesc,
                Data1 = orderBy == "Amt" || orderBy == "Amt2"
                       ? x.Amt
                       : orderBy == "Trips" || orderBy == "Trips2"
                           ? x.Trips
                           : orderBy == "AvgCost" || orderBy == "AvgCost"
                               ? x.Avgcost
                               : x.Amt,
                Data2 = orderBy == "Amt" || orderBy == "Amt2"
                       ? x.Amt2
                       : orderBy == "Trips" || orderBy == "Trips2"
                           ? x.Trips2
                           : orderBy == "AvgCost" || orderBy == "AvgCost"
                               ? x.Avgcost2
                               : x.Amt2
            }).GroupBy(s => new
            {
                CatDesc = s.CatDesc,
            }).Select(s => new GraphFinalData
            {
                CatDesc = s.Key.CatDesc,
                Data1 = s.Sum(x => x.Data1),
                Data2 = s.Sum(x => x.Data2)
            });

            switch (orderBy)
            {
                case "Amt":
                case "AvgCost":
                case "Trips":
                    query = query.OrderBy(s => s.Data1).ToList();
                    break;
                default:
                    query = query.OrderBy(s => s.Data2).ToList();
                    break;
            }

            return query.Reverse().ToList();
        }

        public string GetGraphTitle(DataTypes.SortBy sortBy)
        {
            string title;

            switch (sortBy)
            {
                case DataTypes.SortBy.VOLUME_BOOKED:
                    title = "Total Volume";
                    break;
                case DataTypes.SortBy.AVG_COST_PER_TRIP:
                    title = "Avg Cost per Trip";
                    break;
                case DataTypes.SortBy.NO_OF_TRIPS:
                    title = "# of Trips";
                    break;
                default:
                    title = "Total Volume";
                    break;
            }
            return title;
        }

        public string GetGraphDataType(DataTypes.SortBy sortBy)
        {
            string dataType = "C";
            switch (sortBy)
            {
                case DataTypes.SortBy.AVG_COST_PER_TRIP:
                case DataTypes.SortBy.NO_OF_TRIPS:
                    dataType = "N";
                    break;
                default:
                    dataType = "C";
                    break;
            }
            return dataType;
        }
    }
}
