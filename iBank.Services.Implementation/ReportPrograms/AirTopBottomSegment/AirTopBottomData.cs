using CODE.Framework.Core.Utilities.Extensions;

using System.Collections.Generic;
using System.Linq;

using Domain.Models.ReportPrograms.AirTopBottomSegment;

namespace iBank.Services.Implementation.ReportPrograms.AirTopBottomSegment
{
    public class AirTopBottomData
    {

        public List<FinalData> SortList(List<FinalData> finalList, string sortedBy, string sortOrder, string howManyString)
        {
            switch (sortedBy)
            {
                case "1":
                    finalList = !sortOrder.Equals("2")
                        ? finalList.OrderByDescending(o => o.Actfare).ThenBy(o => o.Carrdesc).ToList()
                        : finalList.OrderBy(o => o.Actfare).ThenBy(o => o.Carrdesc).ToList();
                    break;
                case "2":
                    finalList = !sortOrder.Equals("2")
                        ? finalList.OrderByDescending(o => o.Avgcost).ThenBy(o => o.Carrdesc).ToList()
                        : finalList.OrderBy(o => o.Avgcost).ThenBy(o => o.Carrdesc).ToList();
                    break;
                case "3":
                    finalList = !sortOrder.Equals("2")
                        ? finalList.OrderByDescending(o => o.Segs).ThenBy(o => o.Carrdesc).ToList()
                        : finalList.OrderBy(o => o.Segs).ThenBy(o => o.Carrdesc).ToList();
                    break;
                default:
                    finalList = !sortOrder.Equals("2")
                        ? finalList.OrderByDescending(o => o.Carrdesc).ToList()
                        : finalList.OrderBy(o => o.Carrdesc).ToList();
                    break;
            }

            var howMany = howManyString.TryIntParse(-1);

            return howMany > 0 ? finalList.Take(howMany).ToList() : finalList;
        }

        public static decimal GetGraphData1(FinalData rec, string sortBy)
        {
            switch (sortBy)
            {
                case "1":
                    return rec.Actfare;
                case "2":
                    return rec.Avgcost;
                case "3":
                    return rec.Segs;
                default:
                    return rec.Actfare;
            }
        }

        public List<string> GetExportFields()
        {
            var fieldList = new List<string>();

            fieldList.Add("airline");
            fieldList.Add("carrdesc");
            fieldList.Add("segs");
            fieldList.Add("actfare");
            fieldList.Add("avgcost");

            return fieldList;
        }
    }
}
