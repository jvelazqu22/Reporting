using Domain.Models.ReportPrograms.TopBottomCityPairReport;
using iBank.Services.Implementation.ReportPrograms.TopBottomCityPair;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.TopBottomCityPair
{
    /// <summary>
    /// Summary description for RawDataCalculationTests
    /// </summary>
    [TestClass]
    public class RawDataCalculationTests
    {
        public RawDataCalculationTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        [TestMethod]
        public void CalculateAdopt_CalculateOnlineAgent_ReturnEvenAmout()
        {
            //Arrange
            DataHelper helper = new DataHelper();
            List<RawData> list = new List<RawData>
            {
                new RawData() {RecKey=1, Bktool="ONLINE" },
                new RawData() {RecKey=2, Bktool="AGENT"},
            };

            //Act
            list = helper.CalculateAdopt(list);

            //Assert
            Assert.AreEqual(1, list[0].OnlineTkts);
            Assert.AreEqual(1m, list[1].AgentTkts);

        }

        [TestMethod]
        public void AllocateAirCharge_ActFareNotEvenDist_EvenOut()
        {
            //Arrange
            DataHelper helper = new DataHelper();
            List<RawData> list = new List<RawData>
            {
                new RawData() {RecKey=1, Miles = 200, BaseFare = 50 },
                new RawData() {RecKey=1, Miles = 300, BaseFare = 60 },
            };

            //Act
            list = helper.AllocateAirCharge(list);

            //Assert
            Assert.AreEqual(20.00m, list[0].ActFare);
            Assert.AreEqual(36.00m, list[1].ActFare);
        }

        [TestMethod]
        public void CalculateOneWayBothWay_DestOrgAreNotInMinToMax_Reverse()
        {
            //Arrange
            DataHelper helper = new DataHelper();
            List<RawData> list = new List<RawData>
            {
                new RawData() {Origin = "ABC", Destinat="EFG" },
                new RawData() {Origin = "EFG", Destinat="ABC" },
            };

            //Act
            list = helper.CalculateOneWayBothWay(list);

            //Assert
            foreach(RawData data in list)
            {
                Assert.AreEqual("ABC", data.Origin);
                Assert.AreEqual("EFG", data.Destinat);
            }

        }

        //TODO LookupFunctions access db
        //[TestMethod]
        public void ApplyCityPair_UseMetroCode_ReturnListChanged()
        {

            //Arrange
            DataHelper helper = new DataHelper();
            List<RawData> list = new List<RawData>
            {
                new RawData() {Origin = "DEN", Destinat="VIE" },
                new RawData() {Origin = "IAD", Destinat="PVG" },
            };

            //Act
            list = helper.ApplyCityPair(list);

            //Assert
            Assert.AreEqual("DEN", list[0].Origin);
            Assert.AreEqual("VIE", list[0].Destinat);
            Assert.AreEqual("WAS", list[1].Origin);
            Assert.AreEqual("PVG", list[1].Destinat);
        }
    }
}
