using System;

using iBank.Services.Implementation.Shared;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared
{
    [TestClass]
    public class DateComparerTests
    {
        [TestMethod]
        public void Compare_BothDate_ValOneIsGreater_ReturnOne()
        {
            var x = "1/1/2017";
            var y = "1/1/2016";
            var sut = new DateComparer();

            var output = sut.Compare(x, y);

            Assert.AreEqual(1, output);
        }

        [TestMethod]
        public void Compare_BothDate_ValOneIsLess_ReturnNegativeOne()
        {
            var x = "1/1/2016";
            var y = "1/1/2017";
            var sut = new DateComparer();

            var output = sut.Compare(x, y);

            Assert.AreEqual(-1, output);
        }

        [TestMethod]
        public void Compare_BothDate_ValuesAreEqual_ReturnZero()
        {
            var x = "1/1/2016";
            var y = "1/1/2016";
            var sut = new DateComparer();

            var output = sut.Compare(x, y);

            Assert.AreEqual(0, output);
        }

        [TestMethod]
        public void Compare_ValTwoNotDate_ReturnOne()
        {
            var x = "1/1/2016";
            var y = "foo";
            var sut = new DateComparer();

            var output = sut.Compare(x, y);

            Assert.AreEqual(1, output);
        }

        [TestMethod]
        public void Compare_ValOneNotDate_ReturnNegativeOne()
        {
            var x = "foo";
            var y = "1/1/2016";
            var sut = new DateComparer();

            var output = sut.Compare(x, y);

            Assert.AreEqual(-1, output);
        }

        [TestMethod]
        public void Compare_BothNotDate_ValOneGreaterString_ReturnOne()
        {
            var x = "b";
            var y = "a";
            var sut = new DateComparer();

            var output = sut.Compare(x, y);

            Assert.AreEqual(1, output);
        }

        [TestMethod]
        public void Compare_BothNotDate_ValOneLesserString_ReturnNegativeOne()
        {
            var x = "a";
            var y = "b";
            var sut = new DateComparer();

            var output = sut.Compare(x, y);

            Assert.AreEqual(-1, output);
        }
    }
}
