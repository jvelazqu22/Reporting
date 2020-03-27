using System;
using System.Collections.Generic;

using Domain.Models.ReportPrograms.UserDefinedReport;

using iBank.Services.Implementation.ReportPrograms.UserDefined;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.UserDefinedReport
{
    [TestClass]
    public class LookupHelpersTests
    {
        [TestMethod]
        public void GetPredominantFareBasisByMiles_DataExists()
        {
            var data = new List<LegRawData>
                           {
                               new LegRawData { RecKey = 1, Miles = 10, Farebase = "low" },
                               new LegRawData { RecKey = 1, Miles = 20, Farebase = "high" },
                               new LegRawData { RecKey = 2, Miles = 40, Farebase = "no" }
                           };

            var output = LookupHelpers.GetPredominantFareBasisByHighMiles(1, data);

            Assert.AreEqual("high", output);
        }

        [TestMethod]
        public void GetPredominantFareBasisByMiles_NoDataExists_ReturnEmptyString()
        {
            var data = new List<LegRawData>
                           {
                               new LegRawData { RecKey = 1, Miles = 10, Farebase = "low" },
                               new LegRawData { RecKey = 1, Miles = 20, Farebase = "high" },
                               new LegRawData { RecKey = 2, Miles = 40, Farebase = "no" }
                           };

            var output = LookupHelpers.GetPredominantFareBasisByHighMiles(5, data);

            Assert.AreEqual("", output);
        }

        [TestMethod]
        public void GetBaseFareBreakout_BaseFareExists_BaseFareHasMoreThanTwoValuesPastDecimal_ReturnValueWithTwoDecimals()
        {
            var data = new List<LegRawData>
                           {
                               new LegRawData { RecKey = 1 },
                               new LegRawData { RecKey = 1 },
                               new LegRawData { RecKey = 1 }
                           };
            var baseFare = 15.0000M;
            var reckey = 1;

            var output = LookupHelpers.GetBaseFareBreakout(reckey, baseFare, data);

            Assert.AreEqual("5.00", output);
        }

        [TestMethod]
        public void GetBaseFareBreakout_BaseDoesNotExist_ReturnZero()
        {
            var data = new List<LegRawData>
                           {
                               new LegRawData { RecKey = 1 },
                               new LegRawData { RecKey = 1 },
                               new LegRawData { RecKey = 1 }
                           };
            decimal? baseFare = null;
            var reckey = 1;

            var output = LookupHelpers.GetBaseFareBreakout(reckey, baseFare, data);

            Assert.AreEqual("0", output);
        }

        [TestMethod]
        public void GetBaseFareBreakout_BaseFareExists_NoLegsExist_ReturnZero()
        {
            var data = new List<LegRawData>
                           {
                               new LegRawData { RecKey = 1 },
                               new LegRawData { RecKey = 1 },
                               new LegRawData { RecKey = 1 }
                           };
            var baseFare = 15.0000M;
            var reckey = 2;

            var output = LookupHelpers.GetBaseFareBreakout(reckey, baseFare, data);

            Assert.AreEqual("0", output);
        }

        [TestMethod]
        public void GetTotalTaxes_RecordIsNotNull_ReturnSummationOfValues_RoundTwoValuesPastDecimal()
        {
            var rec = new RawData { Tax1 = 1.0000M, Tax2 = 2.0000M, Tax3 = 3.0000M, Tax4 = 4.0000M };

            var output = LookupHelpers.GetTotalTaxes(rec);

            Assert.AreEqual(10.00M, output);
        }

        [TestMethod]
        public void GetTotalTaxes_RecordIsNull_ReturnZero()
        {
            RawData rec = null;

            var output = LookupHelpers.GetTotalTaxes(rec);

            Assert.AreEqual(0M, output);
        }
    }
}
