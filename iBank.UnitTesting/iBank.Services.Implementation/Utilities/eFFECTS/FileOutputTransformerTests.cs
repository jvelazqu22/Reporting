using System;

using iBank.Services.Implementation.Utilities.eFFECTS;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.Utilities.eFFECTS
{
    [TestClass]
    public class FileOutputTransformerTests
    {
        [TestMethod]
        public void TruncateFileName_OverLimit_ReturnShortenedName()
        {
            var originalPath = @"C:\foo\somereallylongname.txt";
            var maxCharacters = 7;

            var output = FileOutputTransformer.TruncateFileName(originalPath, maxCharacters);

            Assert.AreEqual(@"C:\foo\som.txt", output);
        }

        [TestMethod]
        public void TruncateFileName_UnderLimit_ReturnOriginalName()
        {
            var originalPath = @"C:\foo\somereallylongname.txt";
            var maxCharacters = 300;

            var output = FileOutputTransformer.TruncateFileName(originalPath, maxCharacters);

            Assert.AreEqual(originalPath, output);
        }

        [TestMethod]
        public void TruncateFileName_EqualToLimit_ReturnOriginalName()
        {
            var originalPath = @"C:\foo\equal.txt";
            var maxCharacters = 9;

            var output = FileOutputTransformer.TruncateFileName(originalPath, maxCharacters);

            Assert.AreEqual(originalPath, output);
        }
    }
}
