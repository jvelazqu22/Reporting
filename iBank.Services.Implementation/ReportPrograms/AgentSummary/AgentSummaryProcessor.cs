using System.Collections.Generic;
using System.Linq;
using Domain.Helper;
using Domain.Models.ReportPrograms.AgentSummary;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.AgentSummary
{
    public class AgentSummaryProcessor
    {
        public List<FinalData> GetFinalDataList(List<RawData> rawDataList, ReportGlobals globals)
        {
            rawDataList = rawDataList.GroupBy(s => new { s.Agentid, s.Plusmin }, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new RawData
                {
                    Agentid = key.Agentid.Trim(),
                    Plusmin = key.Plusmin,
                    Transacts = reclist.Count,
                    Commission = reclist.Sum(s => s.Commission),
                    Airchg = reclist.Sum(s => s.Airchg)
                };

            }).ToList();

            var finalQuery = rawDataList.GroupBy(s => s.Agentid, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new FinalData
                {
                    Agentid = string.IsNullOrEmpty(key) ? "[None]" : key,
                    Transacts = reclist.Sum(s => s.Transacts),
                    Tickets = reclist.Sum(s => s.Plusmin == 1 ? s.Transacts : 0),
                    Refunds = reclist.Sum(s => s.Plusmin == 1 ? 0 : s.Transacts),
                    Net_trips = reclist.Sum(s => s.Plusmin == 1 ? s.Transacts : 0 - s.Transacts),
                    Commission = reclist.Sum(s => s.Commission),
                    Invoiceamt = reclist.Sum(s => s.Plusmin == 1 ? s.Airchg : 0),
                    Creditamt = reclist.Sum(s => s.Plusmin == 1 ? 0 : s.Airchg),
                    Netvolume = reclist.Sum(s => s.Airchg),
                };
            });

            return GetSortedFinalList(finalQuery.ToList(), globals);
        }

        public List<FinalData> GetSortedFinalList(List<FinalData> finalDataList, ReportGlobals globals)
        {
            switch (globals.GetParmValue(WhereCriteria.SORTBY))
            {
                case "2":
                    finalDataList = finalDataList.OrderByDescending(s => s.Net_trips).ToList();
                    break;
                case "3":
                    finalDataList = finalDataList.OrderByDescending(s => s.Netvolume).ToList();
                    break;
                default:
                    finalDataList = finalDataList.OrderBy(s => s.Agentid).ToList();
                    break;
            }
            return finalDataList;
        }
    }
}
