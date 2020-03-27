using iBank.Services.Implementation.Shared;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared
{
    [TestClass]
    public class DecimalComparerTests
    {
        [TestMethod]
        public void Compare_BothDecimals_ValOneIsGreater_ReturnOne()
        {
            var x = "2.00";
            var y = "1.00";
            var sut = new DecimalComparer();

            var output = sut.Compare(x, y);

            Assert.AreEqual(1, output);
        }

        [TestMethod]
        public void Compare_BothDecimals_ValOneIsLess_ReturnNegativeOne()
        {
            var x = "0.00";
            var y = "1.00";
            var sut = new DecimalComparer();

            var output = sut.Compare(x, y);

            Assert.AreEqual(-1, output);
        }

        [TestMethod]
        public void Compare_BothDecimals_ValuesAreEqual_ReturnZero()
        {
            var x = "2.00";
            var y = "2.00";
            var sut = new DecimalComparer();

            var output = sut.Compare(x, y);

            Assert.AreEqual(0, output);
        }

        [TestMethod]
        public void Compare_ValTwoNotDecimal_ReturnOne()
        {
            var x = "2.00";
            var y = "foo";
            var sut = new DecimalComparer();

            var output = sut.Compare(x, y);

            Assert.AreEqual(1, output);
        }

        [TestMethod]
        public void Compare_ValOneNotDecimal_ReturnNegativeOne()
        {
            var x = "foo";
            var y = "1.00";
            var sut = new DecimalComparer();

            var output = sut.Compare(x, y);

            Assert.AreEqual(-1, output);
        }

        [TestMethod]
        public void Compare_BothNotDecimals_ValOneGreaterString_ReturnOne()
        {
            var x = "b";
            var y = "a";
            var sut = new DecimalComparer();

            var output = sut.Compare(x, y);

            Assert.AreEqual(1, output);
        }

        [TestMethod]
        public void Compare_BothNotDecimals_ValOneLesserString_ReturnNegativeOne()
        {
            var x = "a";
            var y = "b";
            var sut = new DecimalComparer();

            var output = sut.Compare(x, y);

            Assert.AreEqual(-1, output);
        }
    }
}
