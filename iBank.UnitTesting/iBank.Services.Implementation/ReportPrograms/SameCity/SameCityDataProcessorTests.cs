using System;
using System.Collections.Generic;

using Domain.Models.ReportPrograms.SameCityReport;

using iBank.Services.Implementation.ReportPrograms.SameCity;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.SameCity
{
    [TestClass]
    public class SameCityDataProcessorTests
    {
        private readonly SameCityDataProcessor _processor = new SameCityDataProcessor();

        [TestMethod]
        public void RemoveMatchingCreditAndInvoiceRecords()
        {
            var finalData = new List<FinalData>
            {

            };
        }
    }
}
