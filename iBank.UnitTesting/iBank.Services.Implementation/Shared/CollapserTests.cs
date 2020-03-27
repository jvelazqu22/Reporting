using System;

using iBank.Services.Implementation.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

using Domain.Interfaces;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared
{
    [TestClass]
    public class CollapserTests
    {
        private List<CollapsibleData> _legData = new List<CollapsibleData>();
        [TestInitialize]
        public void Init()
        {
            _legData = new List<CollapsibleData>
            {
                new CollapsibleData
                {
                    RecKey = 1,
                    Origin = "BNA",
                    Destinat = "IAH",
                    Airline = "UA",
                    fltno = "5701",
                    RDepDate = new DateTime(2016, 10, 18),
                    DepTime = "10:00",
                    SeqNo = 1,
                    Miles = 663,
                    DitCode = "D",
                    ActFare = 0.00M,
                    MiscAmt = 0.00M,
                    Connect = "X",
                    ClassCode = "T"
                },
                new CollapsibleData
                {
                    RecKey = 1,
                    Origin = "IAH",
                    Destinat = "SAT",
                    Airline = "UA",
                    fltno = "5715",
                    RDepDate = new DateTime(2016, 10, 18),
                    DepTime = "14:30",
                    SeqNo = 2,
                    Miles = 192,
                    DitCode = "D",
                    ActFare = 134.92M,
                    MiscAmt = 0.00M,
                    Connect = "O",
                    ClassCode = "T"
                },
                new CollapsibleData
                {
                    RecKey = 1,
                    Origin = "SAT",
                    Destinat = "IAH",
                    Airline = "UA",
                    fltno = "1408",
                    RDepDate = new DateTime(2016, 10, 23),
                    DepTime = "08:20",
                    SeqNo = 3,
                    Miles = 192,
                    DitCode = "D",
                    ActFare = 0.00M,
                    MiscAmt = 0.00M,
                    Connect = "X",
                    ClassCode = "S"
                },
                new CollapsibleData
                {
                    RecKey = 1,
                    Origin = "IAH",
                    Destinat = "BNA",
                    Airline = "UA",
                    fltno = "5969",
                    RDepDate = new DateTime(2016, 10, 23),
                    DepTime = "10:55",
                    SeqNo = 4,
                    Miles = 663,
                    DitCode = "D",
                    ActFare = 153.15M,
                    MiscAmt = 0.00M,
                    Connect = "O",
                    ClassCode = "S"
                },
            };
        }

        [TestMethod]
        public void SetSegCounter_OneSegment()
        {
            var data = new List<MockCollapsibleData>();
            data.AddRange(MockCollapsibleData.GenerateMockData());

            Collapser<MockCollapsibleData>.SetSegCounter(data);

            Assert.AreEqual(1, data[0].Seg_Cntr);
            Assert.AreEqual(1, data[1].Seg_Cntr);
            Assert.AreEqual(1, data[2].Seg_Cntr);
        }

        [TestMethod]
        public void SetSegCounter_TwoSegments()
        {
            var data = new List<MockCollapsibleData>();
            data.AddRange(MockCollapsibleData.GenerateMockData());
            data.AddRange(MockCollapsibleData.GenerateMockData());

            for (var i = 3; i < data.Count; i++)
            {
                data[i].SeqNo = i + 1;
            }

            data = data.OrderBy(x => x.SeqNo).ToList();

            Collapser<MockCollapsibleData>.SetSegCounter(data);

            Assert.AreEqual(1, data[0].Seg_Cntr);
            Assert.AreEqual(1, data[1].Seg_Cntr);
            Assert.AreEqual(1, data[2].Seg_Cntr);

            Assert.AreEqual(2, data[3].Seg_Cntr);
            Assert.AreEqual(2, data[4].Seg_Cntr);
            Assert.AreEqual(2, data[5].Seg_Cntr);
        }
        
        [TestMethod]
        public void Collapse_CollapseBothOption_TripHasFourLegsWithTwoSegments_ReturnTwoSegments()
        {
            var output = Collapser<CollapsibleData>.Collapse(_legData, Collapser<CollapsibleData>.CollapseType.Both);

            Assert.AreEqual(2, output.Count);
            
            var segmentOne = output.First(x => x.Seg_Cntr == 1);
            Assert.AreEqual("BNA", segmentOne.Origin);
            Assert.AreEqual("SAT", segmentOne.Destinat);
            Assert.AreEqual(855, segmentOne.Miles);
            Assert.AreEqual("UA", segmentOne.Airline);
            Assert.AreEqual("5715", segmentOne.fltno);
            Assert.AreEqual(new DateTime(2016, 10, 18), segmentOne.RDepDate);
            Assert.AreEqual("10:00", segmentOne.DepTime);
            Assert.AreEqual(134.92M, segmentOne.ActFare);

            var segmentTwo = output.First(x => x.Seg_Cntr == 2);
            Assert.AreEqual("SAT", segmentTwo.Origin);
            Assert.AreEqual("BNA", segmentTwo.Destinat);
            Assert.AreEqual(855, segmentTwo.Miles);
            Assert.AreEqual("UA", segmentTwo.Airline);
            Assert.AreEqual("5969", segmentTwo.fltno);
            Assert.AreEqual(new DateTime(2016, 10, 23), segmentTwo.RDepDate);
            Assert.AreEqual("08:20", segmentTwo.DepTime);
            Assert.AreEqual(153.15M, segmentTwo.ActFare);

        }
    }

    [Serializable]
    internal class CollapsibleData : ICollapsible
    {
        public int RecKey { get; set; }
        public string DitCode { get; set; }
        public string DomIntl { get; set; }
        public int Miles { get; set; }
        public string Mode { get; set; }
        public string Origin { get; set; }
        public string Destinat { get; set; }
        public decimal ActFare { get; set; }
        public decimal MiscAmt { get; set; }
        public string Connect { get; set; }
        public int SeqNo { get; set; }
        public DateTime? RDepDate { get; set; }
        public string DepTime { get; set; }
        public string Airline { get; set; }
        public string fltno { get; set; }
        public string ClassCode { get; set; }
        public int Seg_Cntr { get; set; }
        public decimal AirCo2 { get; set; }
        public decimal AltCarCo2 { get; set; } = 0m;
        public decimal AltRailCo2 { get; set; } = 0m;
    }

}
