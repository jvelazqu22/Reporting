using Domain.Interfaces;
using Domain.Orm.Classes;
using iBank.Services.Implementation.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

using iBankDomain.RepositoryInterfaces;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared
{
    [TestClass]
    public class CollapserMapperTests
    {
        [TestMethod]
        public void MapDepartureLeg()
        {
            var legs = MockCollapsibleData.GenerateMockData();
            var departureLeg = legs.First();
            var arrivalLeg = legs.Last();
            var totalMiles = 1000;

            var output = CollapserMapper<MockCollapsibleData>.MapDepartureLeg(departureLeg, arrivalLeg, totalMiles);

            //some departure info that should not change
            Assert.AreEqual(departureLeg.DitCode, output.DitCode);
            Assert.AreEqual(departureLeg.Origin, output.Origin);
            Assert.AreEqual(departureLeg.DepTime, output.DepTime);

            //arrival info that should be mapped on
            Assert.AreEqual(arrivalLeg.ActFare, output.ActFare);
            Assert.AreEqual(arrivalLeg.MiscAmt, output.MiscAmt);
            Assert.AreEqual(arrivalLeg.Destinat, output.Destinat);
            Assert.AreEqual(arrivalLeg.SeqNo, output.SeqNo);

            Assert.AreEqual(totalMiles, output.Miles);
        }

        [TestMethod]
        public void MapArrivalLeg()
        {
            var legs = MockCollapsibleData.GenerateMockData();
            var departureLeg = legs.First();
            var arrivalLeg = legs.Last();
            var totalMiles = 1000;
            var totalSegmentFare = legs.Sum(s => s.ActFare);
            var totalAirCo2 = legs.Sum(s => s.AirCo2);

            var output = CollapserMapper<MockCollapsibleData>.MapArrivalLeg(arrivalLeg, departureLeg, totalMiles, totalSegmentFare, totalAirCo2);

            //some arrival info that should not change
            Assert.AreEqual(arrivalLeg.Destinat, output.Destinat);

            //arrival info that should be mapped on
            Assert.AreEqual(departureLeg.RDepDate, output.RDepDate);
            Assert.AreEqual(departureLeg.DepTime, output.DepTime);
            Assert.AreEqual(departureLeg.Origin, output.Origin);

            Assert.AreEqual(totalMiles, output.Miles);
        }

        [ExpectedException(typeof(Exception))]
        [TestMethod]
        public void RemapAccordingToClassHierarchy_NoAgencySpecified_ThrowException()
        {
            var query = new Mock<IQuery<IList<ClassCategoryInformation>>>();
            query.Setup(x => x.ExecuteQuery()).Returns(new List<ClassCategoryInformation>
                                                           {
                                                                new ClassCategoryInformation
                                                                    {
                                                                        Agency = "DEMO",
                                                                        Category = "E"
                                                                    },
                                                                new ClassCategoryInformation
                                                                    {
                                                                        Agency = "GANT",
                                                                        Category = "Y"
                                                                    }
                                                           });

            var legsInSegment = MockCollapsibleData.GenerateMockData();
            var legToAdd = new MockCollapsibleData();

            legToAdd = CollapserMapper<MockCollapsibleData>.RemapAccordingToClassHierarchy(query.Object, legsInSegment, legToAdd);
        }

        [TestMethod]
        public void RemapAccordingToClassHierarchy_OnlyOneClass_ReturnOriginalLegToAdd()
        {
            var query = new Mock<IQuery<IList<ClassCategoryInformation>>>();
            query.Setup(x => x.ExecuteQuery()).Returns(new List<ClassCategoryInformation>
                                                           {
                                                                new ClassCategoryInformation
                                                                    {
                                                                        Agency = "DEMO",
                                                                        Category = "E"
                                                                    },
                                                                new ClassCategoryInformation
                                                                    {
                                                                        Agency = "GANT",
                                                                        Category = "Y"
                                                                    }
                                                           });

            var legsInSegment = MockCollapsibleData.GenerateMockData();
            foreach (var leg in legsInSegment)
            {
                leg.ClassCode = "A";
            }
            var legToAdd = new MockCollapsibleData { ClassCode = "" };

            legToAdd = CollapserMapper<MockCollapsibleData>.RemapAccordingToClassHierarchy(query.Object, legsInSegment, legToAdd);

            Assert.AreEqual("", legToAdd.ClassCode);
        }

        [TestMethod]
        public void RemapAccordingToClassHierarchy_MultipleClasses_ReturnHighestRankingClass()
        {
            var query = new Mock<IQuery<IList<ClassCategoryInformation>>>();
            query.Setup(x => x.ExecuteQuery()).Returns(new List<ClassCategoryInformation>
                                                           {
                                                                new ClassCategoryInformation
                                                                    {
                                                                        Agency = "DEMO",
                                                                        Category = "E",
                                                                        Heirarchy = 1
                                                                    },
                                                                new ClassCategoryInformation
                                                                    {
                                                                        Agency = "DEMO",
                                                                        Category = "Y",
                                                                        Heirarchy = 2
                                                                    }
                                                           });

            var legsInSegment = MockCollapsibleData.GenerateMockData();
            var legToAdd = new MockCollapsibleData { ClassCode = "Y", Airline = "ZZ" };

            legToAdd = CollapserMapper<MockCollapsibleData>.RemapAccordingToClassHierarchy(query.Object, legsInSegment, legToAdd);

            Assert.AreEqual("E", legToAdd.ClassCode);
            Assert.AreEqual("AC", legToAdd.Airline);
        }

        [TestMethod]
        public void RemapAccordingToClassHierarchy_MultipleClassesAndMultipleLegsHaveSameClassButDifferentAirline_ReturnAirlineForMaxSequenceNumber()
        {
            var query = new Mock<IQuery<IList<ClassCategoryInformation>>>();
            query.Setup(x => x.ExecuteQuery()).Returns(new List<ClassCategoryInformation>
                                                           {
                                                                new ClassCategoryInformation
                                                                    {
                                                                        Agency = "DEMO",
                                                                        Category = "E",
                                                                        Heirarchy = 1
                                                                    },
                                                                new ClassCategoryInformation
                                                                    {
                                                                        Agency = "DEMO",
                                                                        Category = "Y",
                                                                        Heirarchy = 2
                                                                    }
                                                           });

            var legsInSegment = MockCollapsibleData.GenerateMockData();
            legsInSegment[0].ClassCode = "E";
            legsInSegment[0].Airline = "AC";

            legsInSegment[1].ClassCode = "E";
            legsInSegment[1].Airline = "XX";
            var legToAdd = new MockCollapsibleData { ClassCode = "Y", Airline = "ZZ" };

            legToAdd = CollapserMapper<MockCollapsibleData>.RemapAccordingToClassHierarchy(query.Object, legsInSegment, legToAdd);
            
            Assert.AreEqual("XX", legToAdd.Airline);
        }
    }
}
