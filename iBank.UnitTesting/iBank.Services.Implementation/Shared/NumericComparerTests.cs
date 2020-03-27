using System;

using iBank.Services.Implementation.Shared;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared
{
    [TestClass]
    public class NumericComparerTests
    {
        [TestMethod]
        public void Compare_BothNumeric_ValOneIsGreater_ReturnOne()
        {
            var x = "2";
            var y = "1";
            var sut = new NumericComparer();

            var output = sut.Compare(x, y);

            Assert.AreEqual(1, output);
        }

        [TestMethod]
        public void Compare_BothNumeric_ValOneIsLess_ReturnNegativeOne()
        {
            var x = "0";
            var y = "1";
            var sut = new NumericComparer();

            var output = sut.Compare(x, y);

            Assert.AreEqual(-1, output);
        }

        [TestMethod]
        public void Compare_BothNumeric_ValuesAreEqual_ReturnZero()
        {
            var x = "2";
            var y = "2";
            var sut = new NumericComparer();

            var output = sut.Compare(x, y);

            Assert.AreEqual(0, output);
        }

        [TestMethod]
        public void Compare_ValTwoNotNumeric_ReturnOne()
        {
            var x = "2";
            var y = "foo";
            var sut = new NumericComparer();

            var output = sut.Compare(x, y);

            Assert.AreEqual(1, output);
        }

        [TestMethod]
        public void Compare_ValOneNotNumeric_ReturnNegativeOne()
        {
            var x = "foo";
            var y = "1";
            var sut = new NumericComparer();

            var output = sut.Compare(x, y);

            Assert.AreEqual(-1, output);
        }

        [TestMethod]
        public void Compare_BothNotNumeric_ValOneGreaterString_ReturnOne()
        {
            var x = "b";
            var y = "a";
            var sut = new NumericComparer();

            var output = sut.Compare(x, y);

            Assert.AreEqual(1, output);
        }

        [TestMethod]
        public void Compare_BothNotNumeric_ValOneLesserString_ReturnNegativeOne()
        {
            var x = "a";
            var y = "b";
            var sut = new NumericComparer();

            var output = sut.Compare(x, y);

            Assert.AreEqual(-1, output);
        }
    }
}
