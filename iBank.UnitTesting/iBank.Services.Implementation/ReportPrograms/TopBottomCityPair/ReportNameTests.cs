using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using iBank.Services.Implementation.ReportPrograms.TopBottomCityPair;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.TopBottomCityPair
{
    [TestClass]
    public class ReportNameTests
    {
        [TestMethod]
        public void GetGetCrystalReportName_NotOnlineAdoptNotExcludeMileageNoTktCountNoCarbon_retureibTopCityPair()
        {
            //Arrange
            TopCityPairHelper helper = new TopCityPairHelper();
            var exp = "ibTopCityPair";

            //Act
            var actual = helper.GetCrystalReportName(false, false, false, false);

            //Assert
            Assert.AreEqual(exp, actual);
        }

        [TestMethod]
        public void GetGetCrystalReportName_NotOnlineAdoptIsExcludeMileageNoTktCountNoCarbon_retureibTopCityPair2()
        {
            //Arrange
            TopCityPairHelper helper = new TopCityPairHelper();
            var exp = "ibTopCityPair2";

            //Act
            var actual = helper.GetCrystalReportName(false, true, false, false);

            //Assert
            Assert.AreEqual(exp, actual);
        }
        [TestMethod]
        public void GetGetCrystalReportName_NotOnlineIsExcludeMileageisUseTtkCountNoCarbon_returnibTopCityPair2A()
        {
            //Arrange
            TopCityPairHelper helper = new TopCityPairHelper();
            var exp = "ibTopCityPair2A";

            //Act
            var actual = helper.GetCrystalReportName(false, true, true, false);

            //Assert
            Assert.AreEqual(exp, actual);
        }
        [TestMethod]
        public void GetGetCrystalReportName_NotOnlineAdoptNotExcludeMileageIsTktCountNoCarbon_returnibTopCityPairA()
        {
            //Arrange
            TopCityPairHelper helper = new TopCityPairHelper();
            var exp = "ibTopCityPairA";

            //Act
            var actual = helper.GetCrystalReportName(false, false, true, false);

            //Assert
            Assert.AreEqual(exp, actual);
        }

        [TestMethod]
        public void GetGetCrystalReportName_NotOnlineAdoptNotExcludeMileageIsTktCountISCarbon_returnibTopCityPairACarb()
        {
            //Arrange
            TopCityPairHelper helper = new TopCityPairHelper();
            var exp = "ibTopCityPairACarb";

            //Act
            var actual = helper.GetCrystalReportName(false, false, true, true);

            //Assert
            Assert.AreEqual(exp, actual);
        }

        [TestMethod]
        public void GetGetCrystalReportName_NotOnlineAdoptNotExcludeMileageNotTktCountIsCarbon_returnibTopCityPairACarb()
        {
            //Arrange
            TopCityPairHelper helper = new TopCityPairHelper();
            var exp = "ibTopCityPairCarb";

            //Act
            var actual = helper.GetCrystalReportName(false, false, false, true);

            //Assert
            Assert.AreEqual(exp, actual);
        }

        [TestMethod]
        public void GetGetCrystalReportName_IsOnlineAdoptNotExcludeMileageNotTktCountNotCarbon_returnibTopCityPairOnlineAdopt()
        {
            //Arrange
            TopCityPairHelper helper = new TopCityPairHelper();
            var exp = "ibTopCityPairOnlineAdopt";

            //Act
            var actual = helper.GetCrystalReportName(true, false, false, false);

            //Assert
            Assert.AreEqual(exp, actual);
        }
        [TestMethod]
        public void GetGetCrystalReportName_IsOnlineAdoptNotExcludeMileageIsTktCountNotCarbon_returnibTopCityPairOnlineAdoptA()
        {
            //Arrange
            TopCityPairHelper helper = new TopCityPairHelper();
            var exp = "ibTopCityPairOnlineAdoptA";

            //Act
            var actual = helper.GetCrystalReportName(true, false, true, false);

            //Assert
            Assert.AreEqual(exp, actual);
        }

    }
}
