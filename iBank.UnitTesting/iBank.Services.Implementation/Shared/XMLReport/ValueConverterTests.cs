using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Domain.Models.ReportPrograms.XmlExtractReport;
using iBank.Services.Implementation.Shared.XMLReport;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared.XMLReport
{
    [TestClass]
    public class ValueConverterTests
    {
        [TestMethod]

        public void ValueConverter_NoMaskNoIncludeTime_AllMatch()
        {
            var tripData = new RawData {
                Break1 = "break1",
                Depdate = new DateTime(2018, 1, 10, 8, 30, 40),
                Faretax = 4,
                Plusmin = 1,
                Exchange = false
            };

            var dataList = new List<MockData>
            {
                new MockData { value = tripData.Break1, mask = false, includeTime = false},
                new MockData { value = tripData.Depdate, mask = false, includeTime = false},
                new MockData { value = tripData.Faretax, mask = false, includeTime = false},
                new MockData { value = tripData.Plusmin, mask = false, includeTime = false},
                new MockData { value = tripData.Exchange, mask = false, includeTime = false}
            };

            var exp = new List<string>
            {
                "break1", "2018-01-10", "4.00", "1", "N"
            };
            List<string> act = new List<string>();
            foreach(var item in dataList)
            {
                act.Add(ValueConverter.ConvertValue(item.value, item.mask, item.includeTime));
            }

            for (var i = 0; i < act.Count; i++)
            {
                Assert.AreEqual(exp[i], act[i]);
            }
        }

        [TestMethod]
        public void ValueConverter_IncludeTime()
        {
            var Depdate = new DateTime(2018, 1, 10, 8, 30, 40);

            var exp = "2018-01-10T08:30:40";

            var act = ValueConverter.ConvertValue(Depdate, false, true);

            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void ValueConverter_PassNoTimeRequireIncludeTime_ReturnNoTime()
        {
            var Depdate = new DateTime(2018, 1, 10);

            var exp = "2018-01-10T00:00:00";

            var act = ValueConverter.ConvertValue(Depdate, false, true);

            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void ValueConverter_PassNoTimeRequireMaskIncludeTime_ReturnMaskValue()
        {
            var Depdate = new DateTime(2018, 1, 10);

            var exp = "XXXXXXXXXXXXXXXXXXX";

            var act = ValueConverter.ConvertValue(Depdate, true, true);

            Assert.AreEqual(exp, act);
        }


        class MockData
        {
            public object value;
            public bool mask;
            public bool includeTime;
        }
    }
}