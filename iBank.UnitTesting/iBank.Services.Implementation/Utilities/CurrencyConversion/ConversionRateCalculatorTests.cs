using iBank.Services.Implementation.Utilities.CurrencyConversion;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.Utilities.CurrencyConversion
{
    [TestClass]
    public class ConversionRateCalculatorTests
    {
        [TestMethod]
        public void ConvertValue()
        {
            var originalValue = 3M;
            var sourceRate = 3M;
            var destinationRate = 6M;
            var output = ConversionRateCalculator.ConvertValue(originalValue, sourceRate, destinationRate);

            Assert.AreEqual(6.00M, output);
        }
    }
}
