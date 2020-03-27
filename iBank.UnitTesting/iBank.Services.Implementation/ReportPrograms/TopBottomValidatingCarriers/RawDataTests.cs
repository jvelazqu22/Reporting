using iBank.Services.Implementation.ReportPrograms.TopBottomValidatingCarriers;
using iBank.Services.Implementation.Shared;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;

using Domain.Models.ReportPrograms.TopBottomValidatingCarriers;

using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.Client;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.TopBottomValidatingCarriers
{
    [TestClass]
    public class RawDataTests
    {
        private RawDataHelper helper = null;

        public RawDataTests()
        {
            helper = new RawDataHelper(new BuildWhere(new ClientFunctions()), new Mock<IClientQueryable>().Object, new ReportGlobals(), false);
        }
        //TODO LookupFunctions access db
        //[TestMethod]
        public void ApplyHomeCountryFilter_MultipleHomeCtrys_ReturnsUSAOnly()
        {
            // Arrange
            List<RawData> rawList = new List<RawData>{
                    new RawData() { ValCarMode = "R", SourceAbbr = "Demo", ValCarr = "UA", AirChg = 200.00m },
                    new RawData() { ValCarMode = "A", SourceAbbr = "Demo", ValCarr = "AA", AirChg = 500.00m },
                    new RawData() { ValCarMode = "R", SourceAbbr = "DemoCA01", ValCarr = "UA", AirChg = 300.00m }
                };

            string homeCtry = "USA";
            string notCtry = "CAN";
            //Act
            var results = helper.ApplyHomeCountryFilter(rawList, homeCtry, notCtry);

            //Assert
            for (int index = 0; index < results.Count; index++)
            {
                Assert.AreEqual("Demo", results[index].SourceAbbr);
            }
        }

        [TestMethod]
        public void ModeFilter_RawWithRailAndAir_ReturnsRailOnly()
        {
            // Arrange
            List<RawData> rawList = new List<RawData>{
                    new RawData() { ValCarMode = "R", SourceAbbr = "Demo", ValCarr = "UA", AirChg = 200.00m },
                    new RawData() { ValCarMode = "A", SourceAbbr = "Demo", ValCarr = "AA", AirChg = 500.00m },
                    new RawData() { ValCarMode = "R", SourceAbbr = "Demo", ValCarr = "UA", AirChg = 300.00m }
                };
            int mode = 2; //Rail
                          //Act
            var results = helper.ModeFilter(rawList, mode);

            //Assert
            for (int index = 0; index < results.Count; index++)
            {
                Assert.AreEqual("R", results[index].ValCarMode);
            }
        }
    }
}
