using Domain.Interfaces;
using System;
using System.Collections.Generic;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared
{
    public class MockCollapsibleData : ICollapsible
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
        public DateTime? RArrDate { get; set; }
        public string ArrTime { get; set; }
        public decimal AirCo2 { get; set; } = 0;
        public decimal AltCarCo2 { get; set; } = 0m;
        public decimal AltRailCo2 { get; set; } = 0m;

        public static IList<MockCollapsibleData> GenerateMockData()
        {
            var data = new List<MockCollapsibleData>();
            var departureLeg = new MockCollapsibleData
            {
                RecKey = 1,
                DitCode = "I",
                Miles = 100,
                Origin = "BOS",
                Destinat = "JFK",
                ActFare = 10M,
                MiscAmt = 1M,
                Connect = "X",
                SeqNo = 1,
                RDepDate = new DateTime(2016, 8, 1),
                DepTime = "04:00",
                Airline = "AC",
                fltno = "123",
                ClassCode = "E",
                Seg_Cntr = 0,
                RArrDate = new DateTime(2016, 8, 2),
                ArrTime = "05:00",
                DomIntl = "I"
            };
            data.Add(departureLeg);

            var connectingLeg = new MockCollapsibleData
            {
                RecKey = 1,
                DitCode = "M",
                Miles = 200,
                Origin = "JFK",
                Destinat = "YYZ",
                ActFare = 20M,
                MiscAmt = 2M,
                Connect = "X",
                SeqNo = 2,
                RDepDate = new DateTime(2016, 8, 2),
                DepTime = "08:00",
                Airline = "XO",
                fltno = "456",
                ClassCode = "Y",
                Seg_Cntr = 0,
                RArrDate = new DateTime(2016, 8, 3),
                ArrTime = "06:00",
                DomIntl = "X"
            };
            data.Add(connectingLeg);

            var arrivalLeg = new MockCollapsibleData
            {
                RecKey = 1,
                DitCode = "Y",
                Miles = 300,
                Origin = "YYZ",
                Destinat = "LAX",
                ActFare = 30M,
                MiscAmt = 3M,
                Connect = "O",
                SeqNo = 3,
                RDepDate = new DateTime(2016, 8, 3),
                DepTime = "14:00",
                Airline = "MM",
                fltno = "789",
                ClassCode = "Z",
                Seg_Cntr = 0,
                RArrDate = new DateTime(2016, 8, 4),
                ArrTime = "09:00",
                DomIntl = "D"
            };
            data.Add(arrivalLeg);

            return data;
        }
    }
}