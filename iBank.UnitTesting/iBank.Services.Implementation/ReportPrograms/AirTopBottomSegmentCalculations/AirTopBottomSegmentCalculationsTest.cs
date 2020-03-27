using System;
using System.Collections.Generic;

using Domain.Models.ReportPrograms.AirTopBottomSegment;
using Domain.Orm.Classes;

using iBank.Entities.MasterEntities;

using iBankDomain.RepositoryInterfaces;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using AirTopBottomSegment_AirTopBottomSegmentCalculations = iBank.Services.Implementation.ReportPrograms.AirTopBottomSegment.AirTopBottomSegmentCalculations;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.AirTopBottomSegmentCalculations
{
    [TestClass]
    public class AirTopBottomSegmentCalculationsTest
    {
        // Unit test naming convention:
        // [UnitOfWork_StateUnderTest_ExpectedBehavior]
        // Example: public void Sun_NegativeNumberAsFirstParam_ExceptionThrown

        [TestMethod]
        public void GetCalculatedValues_8ValidRecords_CalculateSumValues()
        {
            // Arrange
            var calculator = new AirTopBottomSegment_AirTopBottomSegmentCalculations();
            var finalDataList = new List<FinalData>()
            {
                new FinalData() { Actfare = Convert.ToDecimal(5543.5000), Airline = "BA", Carrdesc = "BRITISH AIRWAYS", Segs = 1},
                new FinalData() { Actfare = Convert.ToDecimal(5543.5000), Airline = "BA", Carrdesc = "BRITISH AIRWAYS", Segs = 1},
                new FinalData() { Actfare = Convert.ToDecimal(5543.5000), Airline = "BA", Carrdesc = "BRITISH AIRWAYS", Segs = 1},
                new FinalData() { Actfare = Convert.ToDecimal(5543.5000), Airline = "BA", Carrdesc = "BRITISH AIRWAYS", Segs = 1},
                new FinalData() { Actfare = Convert.ToDecimal(5543.5000), Airline = "BA", Carrdesc = "BRITISH AIRWAYS", Segs = 1},
                new FinalData() { Actfare = Convert.ToDecimal(5543.5000), Airline = "BA", Carrdesc = "BRITISH AIRWAYS", Segs = 1},
                new FinalData() { Actfare = Convert.ToDecimal(5543.5000), Airline = "BA", Carrdesc = "BRITISH AIRWAYS", Segs = 1},
                new FinalData() { Actfare = Convert.ToDecimal(5543.5000), Airline = "BA", Carrdesc = "BRITISH AIRWAYS", Segs = 1},
                new FinalData() { Actfare = Convert.ToDecimal(-5543.5000), Airline = "BA", Carrdesc = "BRITISH AIRWAYS", Segs = -1},
            };
            var results = new List<FinalData>();

            // Act
            results = calculator.GetCalculatedValues(finalDataList);

            // Assert
            Assert.AreEqual("BA", results[0].Airline);
            Assert.AreEqual("BRITISH AIRWAYS", results[0].Carrdesc);
            Assert.AreEqual(7, results[0].Segs);
            Assert.AreEqual(Convert.ToDecimal(5543.5 * 7), results[0].Actfare);
            Assert.AreEqual(Convert.ToDecimal((5543.5 * 7) / 7), results[0].Avgcost);
        }
    }
}
