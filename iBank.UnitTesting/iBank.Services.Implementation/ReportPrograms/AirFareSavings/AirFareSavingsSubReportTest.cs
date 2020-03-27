using iBank.Services.Implementation.ReportPrograms.AirFareSavings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Domain.Models.ReportPrograms.AirFareSavingsReport;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.AirFareSavings.Calculations
{
    [TestClass]
    public class AirFareSavingsSubReportTest
    {
        // Unit test naming convention:
        // [UnitOfWork_StateUnderTest_ExpectedBehavior]
        // Example: public void Sun_NegativeNumberAsFirstParam_ExceptionThrown

        [TestMethod]
        public void NegotiatedSavings_NonEmptyListOfSummaryDataInformation_SubReportDataIncludesRecords()
        {
            // Arrange
            var airFareSubReport = new AirFareSavingsSubReport();
            var results = new List<SubReportData>();
            List<SummaryDataInformation> aNegoSvngs = new List<SummaryDataInformation>()
            {
                new SummaryDataInformation() { Count = 1, Amount = 2, Description = "item 1" },
                new SummaryDataInformation() { Count = 2, Amount = 3, Description = "item 2" },
            };

            // Act
            new AirFareSavingsSubReportSavings().NegotiatedSavings(aNegoSvngs,airFareSubReport.GetSubReportDataList());
            results = airFareSubReport.GetSubReportDataList();

            // Assert
            Assert.AreEqual(aNegoSvngs.Count, results.Count);
        }

        [TestMethod]
        public void SavingsCodesFirstLossCodesSecondSetUp_SavingCodesAndLossCodesGreaterThanZero_SubReportDataIncludesSaveAndLossCodes()
        {
            // Arrange
            var airFareSubReport = new AirFareSavingsSubReport();
            var results = new List<SubReportData>();
            List<SummaryDataInformation> aSaveCodes = new List<SummaryDataInformation>()
            {
                new SummaryDataInformation() { Count = 1, Amount = 2, Description = "item1" },
                new SummaryDataInformation() { Count = 2, Amount = 3, Description = "item2" },
            };
            List<SummaryDataInformation> aLossCodes = new List<SummaryDataInformation>()
            {
                new SummaryDataInformation() { Count = 3, Amount = 4, Description = "loss1" },
                new SummaryDataInformation() { Count = 4, Amount = 5, Description = "loss2" },
            };

            // Act
            new AirFareSavingsSubReportSavings().SavingsCodesFirstLossCodesSecondSetUp(aSaveCodes, aLossCodes, airFareSubReport.GetSubReportDataList());
            results = airFareSubReport.GetSubReportDataList();

            // Assert
            Assert.AreEqual(aSaveCodes.Count, results.Count);
            for(int index = 0; index < aSaveCodes.Count; index++)
            {
                Assert.AreEqual(aSaveCodes[index].Description, results[index].Savingdesc);
                Assert.AreEqual(aSaveCodes[index].Amount, results[index].Svgamt);
                Assert.AreEqual(aSaveCodes[index].Count, results[index].Svgcount);
                Assert.AreEqual(aLossCodes[index].Description, results[index].Lossdesc);
                Assert.AreEqual(aLossCodes[index].Amount, results[index].Lossamt);
                Assert.AreEqual(aLossCodes[index].Count, results[index].Losscount);
            }
        }

        [TestMethod]
        public void LossCodesFirstSavingsCodesSecondSetUp_ExcludeSavingCodesAndLossCodesGreaterThanZero_SubReportDataDoesNotIncludesSaveCodes()
        {
            // Arrange
            var airFareSubReport = new AirFareSavingsSubReport();
            var results = new List<SubReportData>();
            List<SummaryDataInformation> aSaveCodes = new List<SummaryDataInformation>()
            {
                new SummaryDataInformation() { Count = 1, Amount = 2, Description = "item1" },
                new SummaryDataInformation() { Count = 2, Amount = 3, Description = "item2" },
            };
            List<SummaryDataInformation> aLossCodes = new List<SummaryDataInformation>()
            {
                new SummaryDataInformation() { Count = 3, Amount = 4, Description = "loss1" },
                new SummaryDataInformation() { Count = 4, Amount = 5, Description = "loss2" },
            };

            // Act
            new AirFareSavingsSubReportLosses().LossCodesFirstSavingsCodesSecondSetUp(aSaveCodes, aLossCodes, true, airFareSubReport.GetSubReportDataList());
            results = airFareSubReport.GetSubReportDataList();

            // Assert
            Assert.AreEqual(aSaveCodes.Count, results.Count);
            for (int index = 0; index < aSaveCodes.Count; index++)
            {
                Assert.AreNotEqual(aSaveCodes[index].Description, results[index].Savingdesc);
                Assert.AreNotEqual(aSaveCodes[index].Amount, results[index].Svgamt);
                Assert.AreNotEqual(aSaveCodes[index].Count, results[index].Svgcount);
                Assert.AreEqual(aLossCodes[index].Description, results[index].Lossdesc);
                Assert.AreEqual(aLossCodes[index].Amount, results[index].Lossamt);
                Assert.AreEqual(aLossCodes[index].Count, results[index].Losscount);
            }
        }

        [TestMethod]
        public void LossCodesFirstSavingsCodesSecondSetUp_DontExcludeSavingCodesAndLossCodesGreaterThanZero_SubReportDataIncludesSaveCodes()
        {
            // Arrange
            var airFareSubReport = new AirFareSavingsSubReport();
            var results = new List<SubReportData>();
            List<SummaryDataInformation> aSaveCodes = new List<SummaryDataInformation>()
            {
                new SummaryDataInformation() { Count = 1, Amount = 2, Description = "item1" },
                new SummaryDataInformation() { Count = 2, Amount = 3, Description = "item2" },
            };
            List<SummaryDataInformation> aLossCodes = new List<SummaryDataInformation>()
            {
                new SummaryDataInformation() { Count = 3, Amount = 4, Description = "loss1" },
                new SummaryDataInformation() { Count = 4, Amount = 5, Description = "loss2" },
            };
            List<SummaryDataInformation> aNegoSvngs = new List<SummaryDataInformation>()
            {
                new SummaryDataInformation() { Count = 5, Amount = 7, Description = "negoSvgs1" },
                new SummaryDataInformation() { Count = 6, Amount = 8, Description = "negoSvgs2" },
            };

            // Act
            new AirFareSavingsSubReportLosses().LossCodesFirstSavingsCodesSecondSetUp(aSaveCodes, aLossCodes, false, airFareSubReport.GetSubReportDataList());
            results = airFareSubReport.GetSubReportDataList();

            // Assert
            Assert.AreEqual(aSaveCodes.Count, results.Count);
            for (int index = 0; index < aSaveCodes.Count; index++)
            {
                Assert.AreEqual(aSaveCodes[index].Description, results[index].Savingdesc);
                Assert.AreEqual(aSaveCodes[index].Amount, results[index].Svgamt);
                Assert.AreEqual(aSaveCodes[index].Count, results[index].Svgcount);
                Assert.AreEqual(aLossCodes[index].Description, results[index].Lossdesc);
                Assert.AreEqual(aLossCodes[index].Amount, results[index].Lossamt);
                Assert.AreEqual(aLossCodes[index].Count, results[index].Losscount);
            }
        }

        [TestMethod]
        public void PopulateSubReportDataList_ExcludeSavingsAndNegotiatedSavings_ReturnOnlyLossCodes()
        {
            // Arrange
            var airFareSubReport = new AirFareSavingsSubReport();
            var results = new List<SubReportData>();
            bool excludeSavings = true, negotiatedSavings = true;
            List<SummaryDataInformation> aSaveCodes = new List<SummaryDataInformation>()
            {
                new SummaryDataInformation() { Count = 1, Amount = 2, Description = "item1" },
                new SummaryDataInformation() { Count = 2, Amount = 3, Description = "item2" },
            };
            List<SummaryDataInformation> aLossCodes = new List<SummaryDataInformation>()
            {
                new SummaryDataInformation() { Count = 3, Amount = 4, Description = "loss1" },
                new SummaryDataInformation() { Count = 4, Amount = 5, Description = "loss2" },
            };
            List<SummaryDataInformation> aNegoSvngs = new List<SummaryDataInformation>()
            {
                new SummaryDataInformation() { Count = 5, Amount = 7, Description = "Negotiated Savings" },
                new SummaryDataInformation() { Count = 6, Amount = 8, Description = "Negotiated Savings" },
            };

            // Act
            airFareSubReport.PopulateSubReportDataList(aSaveCodes, aLossCodes, aNegoSvngs, excludeSavings, negotiatedSavings);
            results = airFareSubReport.GetSubReportDataList();

            // Assert
            Assert.AreEqual(aSaveCodes.Count, results.Count);
            for (int index = 0; index < aSaveCodes.Count; index++)
            {
                Assert.AreEqual(aLossCodes[index].Description, results[index].Lossdesc);
                Assert.AreEqual(aLossCodes[index].Amount, results[index].Lossamt);
                Assert.AreEqual(aLossCodes[index].Count, results[index].Losscount);
                Assert.AreNotEqual(aSaveCodes[index].Description, results[index].Savingdesc);
                Assert.AreNotEqual(aSaveCodes[index].Amount, results[index].Svgamt);
                Assert.AreNotEqual(aSaveCodes[index].Count, results[index].Svgcount);
                Assert.AreNotEqual(aNegoSvngs[index].Description, results[index].Savingdesc);
                Assert.AreNotEqual(aNegoSvngs[index].Amount, results[index].Svgamt);
                Assert.AreNotEqual(aNegoSvngs[index].Count, results[index].Svgcount);
            }
        }

        [TestMethod]
        public void PopulateSubReportDataList_IncludeSaveLossAndNegoSvngCodes_ReturnAllValuesAndRecords()
        {
            // Arrange
            var airFareSubReport = new AirFareSavingsSubReport();
            var results = new List<SubReportData>();
            bool excludeSavings = false, negotiatedSavings = false;
            List<SummaryDataInformation> aSaveCodes = new List<SummaryDataInformation>()
            {
                new SummaryDataInformation() { Count = 1, Amount = 2, Description = "item1" },
                new SummaryDataInformation() { Count = 2, Amount = 3, Description = "item2" },
            };
            List<SummaryDataInformation> aLossCodes = new List<SummaryDataInformation>()
            {
                new SummaryDataInformation() { Count = 3, Amount = 4, Description = "loss1" },
                new SummaryDataInformation() { Count = 4, Amount = 5, Description = "loss2" },
            };
            List<SummaryDataInformation> aNegoSvngs = new List<SummaryDataInformation>()
            {
                new SummaryDataInformation() { Count = 5, Amount = 7, Description = "Negotiated Savings" },
                new SummaryDataInformation() { Count = 6, Amount = 8, Description = "Negotiated Savings" },
            };

            // Act
            airFareSubReport.PopulateSubReportDataList(aSaveCodes, aLossCodes, aNegoSvngs, excludeSavings, negotiatedSavings);
            results = airFareSubReport.GetSubReportDataList();

            // Assert
            Assert.AreEqual(aSaveCodes.Count + aNegoSvngs.Count, results.Count);
            for (int index = 0; index < aSaveCodes.Count; index++)
            {
                Assert.AreEqual(aLossCodes[index].Description, results[index].Lossdesc);
                Assert.AreEqual(aLossCodes[index].Amount, results[index].Lossamt);
                Assert.AreEqual(aLossCodes[index].Count, results[index].Losscount);
                Assert.AreEqual(aSaveCodes[index].Description, results[index].Savingdesc);
                Assert.AreEqual(aSaveCodes[index].Amount, results[index].Svgamt);
                Assert.AreEqual(aSaveCodes[index].Count, results[index].Svgcount);
            }
            for (int index = aSaveCodes.Count, index2 = 0; index < aNegoSvngs.Count + aNegoSvngs.Count && index2 < aNegoSvngs.Count; index++, index2++)
            {
                Assert.AreEqual(aNegoSvngs[index2].Description, results[index].Savingdesc);
                Assert.AreEqual(aNegoSvngs[index2].Amount, results[index].Svgamt);
                Assert.AreEqual(aNegoSvngs[index2].Count, results[index].Svgcount);
            }
        }

        [TestMethod]
        public void GetDistinctNegotiatedSavingsModelListFromFinalDataList_PassDuplicateRecords_ShouldGetDistinctRecords()
        {
            // Arrange
            var airFareSubReport = new AirFareSavingsSubReport();
            var results = new List<NegotiatedSavingsModel>();
            List<FinalData> finalList = new List<FinalData>()
            {
                new FinalData { Passfrst = "Jorge",  Reckey = 1, Savingcode ="s1", Reascode = "r1", Airchg = 1, Offrdchg = 2, Stndchg = 3, Lostamt = 4, Savings = 5, Negosvngs = 6, Plusmin = 1, Origacct = "ac1" },
                new FinalData { Passfrst= "Jorge", Reckey = 1, Savingcode ="s1", Reascode = "r1", Airchg = 1, Offrdchg = 2, Stndchg = 3, Lostamt = 4, Savings = 5, Negosvngs = 6, Plusmin = 1, Origacct = "ac1" },
                new FinalData { Passfrst= "Jorge", Reckey = 2, Savingcode ="s1", Reascode = "r1", Airchg = 1, Offrdchg = 2, Stndchg = 3, Lostamt = 4, Savings = 5, Negosvngs = 6, Plusmin = 1, Origacct = "ac1" },
            };

            // Act
            results = new AirFareSavingsSubReportSavings().GetDistinctNegotiatedSavingsModelListFromFinalDataList(finalList);

            // Assert
           Assert.AreEqual(2, results.Count);
        }

        [TestMethod]
        public void GetNegotiatedSavingsSummaryData_PassedANonDistinctReocord_ReturnShouldExcludeNonDistinctRecord()
        {
            // Arrange
            var airFareSubReport = new AirFareSavingsSubReport();
            var results = new List<SummaryDataInformation>();
            List<FinalData> finalList = new List<FinalData>()
            {
                new FinalData { Passfrst = "Jorge",  Reckey = 1, Savingcode ="s1", Reascode = "r1", Airchg = 1, Offrdchg = 2, Stndchg = 3, Lostamt = 4, Savings = 5, Negosvngs = 6, Plusmin = 1, Origacct = "ac1" },
                new FinalData { Passfrst= "Jorge", Reckey = 1, Savingcode ="s1", Reascode = "r1", Airchg = 1, Offrdchg = 2, Stndchg = 3, Lostamt = 4, Savings = 5, Negosvngs = 6, Plusmin = 1, Origacct = "ac1" },
                new FinalData { Passfrst= "Jorge", Reckey = 2, Savingcode ="s1", Reascode = "r1", Airchg = 1, Offrdchg = 2, Stndchg = 3, Lostamt = 4, Savings = 5, Negosvngs = 6, Plusmin = 1, Origacct = "ac1" },
            };

            // Act
            results = new AirFareSavingsSubReportSavings().GetNegotiatedSavingsSummaryData(finalList);

            // Assert
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(12, results[0].Amount);
            Assert.AreEqual(2, results[0].Count);
        }

    }
}
