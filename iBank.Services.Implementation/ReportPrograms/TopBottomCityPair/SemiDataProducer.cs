using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.TopBottomCityPairReport;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomCityPair
{
    public class SemiDataProducer
    {
        private List<RawData> rawDataList;
        public SemiDataProducer(List<RawData> list)
        {
            rawDataList = list;
        }
        
        public List<SemiFinalData> ProduceSemiData()
        {
            var temp = rawDataList.GroupBy(x => new
            {
                x.Origin,
                x.Destinat,
                x.Mode,
                x.Airline
            }).Select(x => new SemiFinalData
            {
                Origin = x.Key.Origin,
                Destinat = x.Key.Destinat,
                Airline = x.Key.Airline,
                Mode = x.Key.Mode,
                Segments = x.Sum(s => s.Plusmin),
                Cost = x.Sum(s => s.Plusmin * Math.Abs(s.ActFare)),
                Miles = x.Sum(s => s.Plusmin * Math.Abs(s.Miles)),
                AirCO2 = x.Sum(s => s.AirCo2),
                Numticks = x.Sum(s => s.Plusmin * s.NumTicks),
                OnlineTkts = x.Sum(s => s.Plusmin * s.OnlineTkts),
                AgentTkts = x.Sum(s => s.Plusmin * s.AgentTkts),
                OnlineSegs = x.Sum(q => q.Bktool == "ONLINE" ? 1 : 0),
                AgentSegs = x.Sum(s => s.Plusmin) - x.Sum(q => q.Bktool == "ONLINE" ? 1 : 0),
                OnlineCost = x.Sum(q => q.Bktool == "ONLINE" ? q.Plusmin * Math.Abs(q.ActFare) : 0),
                AgentCost = x.Sum(q => q.Bktool == "ONLINE" ? 0 : q.Plusmin * Math.Abs(q.ActFare))

            }).ToList();

            return temp;
        }
    }
}
