using iBank.Services.Implementation.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.Core_Functionality
{
    [TestClass]
    public class MathHelperTests
    {
        [TestMethod]
        public void Round_RoundUp5Test()
        {
            //Arrange
            decimal input = 123.125m;
            decimal exp = 123.13m;

            //act
            var result = MathHelper.Round(input);

            //assert
            Assert.AreEqual<decimal>(exp, result);
        }

        [TestMethod]
        public void Round_WontRoundUpIfLessThan4()
        {
            //Arrange
            decimal input = 123.124m;
            decimal exp = 123.12m;

            //act
            var result = MathHelper.Round(input);

            //assert
            Assert.AreEqual<decimal>(exp, result);
        }
        [TestMethod]
        public void Round_OneDigits()
        {
            //Arrange
            double input = 123.125;
            double exp = 123.1;

            //act
            var result = MathHelper.Round(input, 1);

            //assert
            Assert.AreEqual<double>(exp, result);
        }

        [TestMethod]
        public void Round_TwoDigits()
        {
            //Arrange
            decimal input = 123.125m;
            decimal exp = 123.13m;

            //act
            var result = MathHelper.Round(input, 2);

            //assert
            Assert.AreEqual<decimal>(exp, result);
        }
    }
}
