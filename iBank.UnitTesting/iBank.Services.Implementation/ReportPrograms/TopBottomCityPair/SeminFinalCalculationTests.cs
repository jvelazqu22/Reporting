using Domain.Models.ReportPrograms.TopBottomCityPairReport;
using iBank.Services.Implementation.ReportPrograms.TopBottomCityPair;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.TopBottomCityPair
{
    [TestClass]
    public class SeminFinalCalculationTests
    {
        [TestMethod]
        public void ProduceSemiData_dd()
        {
            //Arrange
            List<RawData> list = new List<RawData>
            {
                new RawData() {Origin = "ABC", Destinat="EFG", ActFare=100.00m, Plusmin=1, Airchg=124.00m, Bktool="ONLINE"},
                new RawData() {Origin = "ABC", Destinat="EFG", ActFare=200.00m, Plusmin=1, Airchg=124.00m, Bktool="AGENT"},
            };
            SemiDataProducer semi = new SemiDataProducer(list);

            //Act
            var exp = semi.ProduceSemiData();

            //Assert
            Assert.AreEqual("ABC", exp[0].Origin);
            Assert.AreEqual("EFG", exp[0].Destinat);
            Assert.AreEqual(1, exp[0].AgentSegs);
            Assert.AreEqual(1, exp[0].OnlineSegs);
            Assert.AreEqual(200.00m, exp[0].AgentCost);
            Assert.AreEqual(100.00m, exp[0].OnlineCost);
            Assert.AreEqual(300.00m, exp[0].Cost);
            Assert.AreEqual(2, exp[0].Segments);

        }
    }
}
