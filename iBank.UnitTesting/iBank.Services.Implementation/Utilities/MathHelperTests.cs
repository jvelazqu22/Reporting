using System;

using iBank.Services.Implementation.Utilities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.Utilities
{
    [TestClass]
    public class MathHelperTests
    {
        [TestMethod]
        public void Max_ThreeValues()
        {
            var x = 1;
            var y = 2;
            var z = 3;

            var output = MathHelper.Max(x, y, z);

            Assert.AreEqual(z, output);
        }

        [TestMethod]
        public void Max_FourValues()
        {
            var w = 0;
            var x = 1;
            var y = 2;
            var z = 3;

            var output = MathHelper.Max(w, x, y, z);

            Assert.AreEqual(z, output);
        }

        [TestMethod]
        public void Max_FiveValues()
        {
            var v = -1;
            var w = 0;
            var x = 1;
            var y = 2;
            var z = 3;

            var output = MathHelper.Max(v, w, x, y, z);

            Assert.AreEqual(z, output);
        }
    }
}
