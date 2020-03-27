using System.Collections.Generic;

using Domain.Helper;
using Domain.Interfaces;

using iBank.Server.Utilities.Classes;

namespace iBank.UnitTesting.iBank.Services.Implementation.Utilities.ClientData
{
    public class UdidFilterContext
    {
        public IList<MockDataUdidFilter> MockData { get; set; }

        public AdvancedParameters MultiUdidParameters { get; set; }
        public List<UdidRecord> Udids { get; set; }

        public IList<MockDataUdidFilter> Results { get; set; }
        public ReportGlobals Globals { get; set; }

        public UdidFilterContext()
        {
            MockData = new List<MockDataUdidFilter>();
            MultiUdidParameters = new AdvancedParameters();
            Results = new List<MockDataUdidFilter>();
            Udids = new List<UdidRecord>();
            Globals = new ReportGlobals();
        }
    }

    public class MockDataUdidFilter : IRecKey
    {
        public MockDataUdidFilter(int reckey)
        {
            RecKey = reckey;
        }

        public int RecKey { get; set; }
    }
}
