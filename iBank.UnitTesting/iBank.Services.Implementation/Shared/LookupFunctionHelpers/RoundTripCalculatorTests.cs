using System;

using iBank.Services.Implementation.Shared.LookupFunctionHelpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared.LookupFunctionHelpers
{
    [TestClass]
    public class RoundTripCalculatorTests
    {
        [TestMethod]
        public void IsRoundTrip_SameOriginAndDestination_ReturnTrue()
        {
            var routing = "DEN SBA DEN";

            var output = RoundTripCalculator.IsRoundTrip(routing);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsRoundTrip_SameOriginAndDestination_WithSpaceAtEnd_ReturnTrue()
        {
            var routing = "DEN SBA DEN ";

            var output = RoundTripCalculator.IsRoundTrip(routing);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsRoundTrip_DifferentOriginAndDestination_ReturnFalse()
        {
            var routing = "DEN SBA";

            var output = RoundTripCalculator.IsRoundTrip(routing);

            Assert.AreEqual(false, output);
        }
    }
}
