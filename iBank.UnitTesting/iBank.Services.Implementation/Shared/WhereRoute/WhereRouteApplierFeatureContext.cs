using System;
using System.Collections.Generic;

using Domain.Interfaces;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared.WhereRoute
{
    public class WhereRouteApplierFeatureContext
    {
        public IList<DataForContext> Data { get; set; } = new List<DataForContext>();
        public IList<string> OriginCriteria { get; set; } = new List<string>();
        public IList<string> DestinationCriteria { get; set; } = new List<string>();
        public bool ReturnAllLegs { get; set; }
        public bool NotInList { get; set; }

        public void AddData(int reckey, int seqno, string origin, string destination)
        {
            Data.Add(new DataForContext { RecKey = reckey, SeqNo = seqno, Origin = origin, Destinat = destination });
        }
    }

    public class DataForContext : IRouteWhere
    {
        public string DitCode { get; set; }
        public string Origin { get; set; }
        public string Destinat { get; set; }
        public string Mode { get; set; }
        public string Airline { get; set; }
        public int RecKey { get; set; }
        public int SeqNo { get; set; }
        public DateTime? RDepDate { get; set; }
        public DateTime? RArrDate { get; set; }
    }
}
