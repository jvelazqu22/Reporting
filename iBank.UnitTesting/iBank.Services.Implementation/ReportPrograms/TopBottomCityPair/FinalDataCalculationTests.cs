using Domain.Models.ReportPrograms.TopBottomCityPairReport;
using iBank.Services.Implementation.ReportPrograms.TopBottomCityPair;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.TopBottomCityPair
{
    [TestClass]
    public class FinalDataCalculationTests
    {
        [TestMethod]
        public void GroupByOrgDest_UseTickCnt()
        {
            //Arrange
            FinalDataProducer producer = new FinalDataProducer();
            List<FinalData> list = new List<FinalData>
            {
                new FinalData() {Origin= "ATL", Destinat="EWR", Airline="DL", Cost=100.00m, Segments=1, Numticks=2, Miles=200, Cpcost=0.00m, Cpsegs = 0, CpNumticks = 0 },
                new FinalData() {Origin= "ATL", Destinat="EWR", Airline="DL", Cost=200.00m, Segments=2, Numticks=4, Miles=500, Cpcost=0.00m, Cpsegs = 0, CpNumticks = 0 },
                new FinalData() {Origin= "ATL", Destinat="EWR", Airline="UA", Cost=300.00m, Segments=10, Numticks=6, Miles=200, Cpcost=0.00m, Cpsegs = 0, CpNumticks = 0 },
                new FinalData() {Origin= "ATL", Destinat="EWR", Airline="UA", Cost=400.00m, Segments=14, Numticks=8, Miles=500, Cpcost=0.00m, Cpsegs = 0, CpNumticks = 0 },
            };

            //Act
            var exp = producer.GroupByOrgDest(list, true);


            //Assert
            for (int i=0; i< exp.Count; i++)
            {
                Assert.AreEqual(50.00m, exp[i].CpAvgcost);
            }
        }
        [TestMethod]
        public void GroupByOrgDest_NotUseTickCnt()
        {
            //Arrange
            FinalDataProducer producer = new FinalDataProducer();
            List<FinalData> list = new List<FinalData>
            {
                new FinalData() {Origin= "ATL", Destinat="EWR", Airline="DL", Cost=100.00m, Segments=1, Numticks=2, Miles=200, Cpcost=0.00m, Cpsegs = 0, CpNumticks = 0 },
                new FinalData() {Origin= "ATL", Destinat="EWR", Airline="DL", Cost=200.00m, Segments=2, Numticks=4, Miles=500, Cpcost=0.00m, Cpsegs = 0, CpNumticks = 0 },
                new FinalData() {Origin= "ATL", Destinat="EWR", Airline="UA", Cost=1000.00m, Segments=10, Numticks=6, Miles=200, Cpcost=0.00m, Cpsegs = 0, CpNumticks = 0 },
                new FinalData() {Origin= "ATL", Destinat="EWR", Airline="UA", Cost=2000.00m, Segments=20, Numticks=8, Miles=500, Cpcost=0.00m, Cpsegs = 0, CpNumticks = 0 },
            };

            //Act
            var exp = producer.GroupByOrgDest(list, false);


            //Assert
            for (int i = 0; i < exp.Count; i++)
            {
                Assert.AreEqual(100.00m, exp[i].CpAvgcost);
            }
        }

        [TestMethod]
        public void CalculateTotalCount_UseTickCnt()
        {
            //Arrange
            FinalDataProducer producer = new FinalDataProducer();
            List<FinalData> list = new List<FinalData>
            {
                new FinalData() {Origin= "ATL", Destinat="EWR", Airline="DL", Cost=100.00m, Segments=1, CpNumticks=2, Miles=200 },
                new FinalData() {Origin= "ATL", Destinat="EWR", Airline="DL", Cost=200.00m, Segments=2, CpNumticks=4, Miles=500 },
                new FinalData() {Origin= "ATL", Destinat="EWR", Airline="UA", Cost=300.00m, Segments=10, CpNumticks=6, Miles=200 },
                new FinalData() {Origin= "ATL", Destinat="EWR", Airline="UA", Cost=400.00m, Segments=14, CpNumticks=8, Miles=500 },
            };

            //Act
            var exp = producer.CalculateTotalCount(list, true);

            //Assert
            Assert.AreEqual(20, exp);

        }
        [TestMethod]
        public void CalculateTotalCount_NotUseTickCnt()
        {
            //Arrange
            FinalDataProducer producer = new FinalDataProducer();
            List<FinalData> list = new List<FinalData>
            {
                new FinalData() {Origin= "ATL", Destinat="EWR", Airline="DL", Cost=100.00m, Cpsegs=1, CpNumticks=2, Miles=200 },
                new FinalData() {Origin= "ATL", Destinat="EWR", Airline="DL", Cost=200.00m, Cpsegs=2, CpNumticks=4, Miles=500 },
                new FinalData() {Origin= "ATL", Destinat="EWR", Airline="UA", Cost=300.00m, Cpsegs=10, CpNumticks=6, Miles=200 },
                new FinalData() {Origin= "ATL", Destinat="EWR", Airline="UA", Cost=400.00m, Cpsegs=14, CpNumticks=8, Miles=500 },
            };

            //Act
            var exp = producer.CalculateTotalCount(list, false);

            //Assert
            Assert.AreEqual(27, exp);

        }
    }
}
