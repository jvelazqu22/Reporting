using System;

using iBank.Services.Implementation.Shared.BuildWhereHelpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared.BuildWhereHelpers
{
    [TestClass]
    public class WhereTextBuilderTests
    {
        [TestMethod]
        public void AddToWhereText_NoPickName_NoIsNotIn()
        {
            var whereText = "existing text; ";
            var pickName = "";
            var displayName = "Display Name";
            var notIn = false;
            var stringToAdd = "foo";
            var sut = new WhereTextBuilder();

            var output = sut.AddToWhereText(whereText, pickName, displayName, notIn, stringToAdd);

            Assert.AreEqual("existing text; Display Name = foo; ", output);
        }

        [TestMethod]
        public void AddToWhereText_NoPickName_IsNotIn()
        {
            var whereText = "existing text; ";
            var pickName = "";
            var displayName = "Display Name";
            var notIn = true;
            var stringToAdd = "foo";
            var sut = new WhereTextBuilder();

            var output = sut.AddToWhereText(whereText, pickName, displayName, notIn, stringToAdd);

            Assert.AreEqual("existing text; Display Name <> foo; ", output);
        }

        [TestMethod]
        public void AddToWhereText_PickName_NoIsNotIn()
        {
            var whereText = "existing text; ";
            var pickName = " pickname";
            var displayName = "Display Name";
            var notIn = false;
            var stringToAdd = "foo";
            var sut = new WhereTextBuilder();

            var output = sut.AddToWhereText(whereText, pickName, displayName, notIn, stringToAdd);

            Assert.AreEqual("existing text; Display Name pickname = foo; ", output);
        }

        [TestMethod]
        public void AddToWhereText_PickName_IsNotIn()
        {
            var whereText = "existing text; ";
            var pickName = " pickname";
            var displayName = "Display Name";
            var notIn = true;
            var stringToAdd = "foo";
            var sut = new WhereTextBuilder();

            var output = sut.AddToWhereText(whereText, pickName, displayName, notIn, stringToAdd);

            Assert.AreEqual("existing text; Display Name pickname <> foo; ", output);
        }
    }
}
