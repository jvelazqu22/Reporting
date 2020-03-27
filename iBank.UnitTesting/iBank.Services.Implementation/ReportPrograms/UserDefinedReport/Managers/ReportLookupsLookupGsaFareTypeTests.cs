using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ReportLokkup;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.UserDefinedReport.Managers
{
    [TestClass]
    public class ReportLookupsLookupGsaFareTypeTests
    {
        [TestMethod]
        public void LookupGsaFareType_FirstRule_Passes()
        {
            // arrange
            var pdrFareBas = "FASR";
            // act
            var result = FareTypeHandler.LookupGsaFareType(pdrFareBas);

            // assert
            Assert.AreEqual("First", result);
        }

        [TestMethod]
        public void LookupGsaFareType_BusinessRule_Passes()
        {
            // arrange
            var pdrFareBas = "CE0RA0MP";
            // act
            var result = FareTypeHandler.LookupGsaFareType(pdrFareBas);

            // assert
            Assert.AreEqual("Business", result);
        }

        [TestMethod]
        public void LookupGsaFareType_DgRule_Passes()
        {
            // arrange
            var pdrFareBas = "VDG";

            // act
            var result = FareTypeHandler.LookupGsaFareType(pdrFareBas);

            // assert
            Assert.AreEqual("DG", result);
        }

        [TestMethod]
        public void LookupGsaFareType_CppRule_Passes()
        {
            // arrange
            var pdrFareBas = "CCB";

            // act
            var result = FareTypeHandler.LookupGsaFareType(pdrFareBas);

            // assert
            Assert.AreEqual("CPP Business", result);
        }

        [TestMethod]
        public void LookupGsaFareType_DashCaRule_Passes()
        {
            // arrange
            var pdrFareBas = "GCADCA";

            // act
            var result = FareTypeHandler.LookupGsaFareType(pdrFareBas);

            // assert
            Assert.AreEqual("Dash CA", result);
        }

        [TestMethod]
        public void LookupGsaFareType_YcaRule_Passes()
        {
            // arrange
            var pdrFareBas = "YCADCA";

            // act
            var result = FareTypeHandler.LookupGsaFareType(pdrFareBas);

            // assert
            Assert.AreEqual("YCA", result);
        }

        [TestMethod]
        public void LookupGsaFareType_CategoryZRule_Passes()
        {
            // arrange
            var pdrFareBas = "GMZDCA";

            // act
            var result = FareTypeHandler.LookupGsaFareType(pdrFareBas);

            // assert
            Assert.AreEqual("Category Z", result);
        }

        [TestMethod]
        public void LookupGsaFareType_OtherRule_Passes()
        {
            // arrange
            string pdrFareBas = null;

            // act
            var result = FareTypeHandler.LookupGsaFareType(pdrFareBas);

            // assert
            Assert.AreEqual("Other", result);
        }
    }
}
