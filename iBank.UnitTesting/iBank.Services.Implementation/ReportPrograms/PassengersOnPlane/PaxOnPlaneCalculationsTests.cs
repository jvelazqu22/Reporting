using System;

using Domain.Helper;

using iBank.Services.Implementation.ReportPrograms.PassengersOnPlane;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Domain.Models.ReportPrograms.PassengersOnPlaneReport;

using iBank.Server.Utilities.Classes;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.PassengersOnPlane
{
    [TestClass]
    public class PaxOnPlaneCalculationsTests
    {
        private readonly PaxOnPlaneCalculations _calc = new PaxOnPlaneCalculations();
        [TestMethod]
        public void GetCrystalReportName_NoPrintBreakInfo_ReturnRegularReport()
        {
            var isPrintBreakInfoInBodyOn = false;

            var crystalName = _calc.GetCrystalReportName(isPrintBreakInfoInBodyOn);

            Assert.AreEqual("ibPaxOnPlane", crystalName);
        }

        [TestMethod]
        public void GetCrystalReportName_PrintBreakInfo_ReturnAlternateReport()
        {
            var isPrintBreakInfoInBodyOn = true;

            var crystalName = _calc.GetCrystalReportName(isPrintBreakInfoInBodyOn);

            Assert.AreEqual("ibPaxOnPlane2", crystalName);
        }

        [TestMethod]
        public void GetBreakCombo_NoBreak_Return10CharacterEmptyString()
        {
            var breaks = new UserBreaks
                             {
                                 UserBreak1 = false,
                                 UserBreak2 = false,
                                 UserBreak3 = false
                             };
            var bBreakInfoInReport = false;
            var row = new RawData();
            var expected = new string(' ', 10);

            var output = _calc.GetBreakCombo(row, breaks, bBreakInfoInReport);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void GetBreakCombo_NoUserBreaksButBreakInfoInReport_ReturnDataInBreaks()
        {
            var breaks = new UserBreaks
            {
                UserBreak1 = false,
                UserBreak2 = false,
                UserBreak3 = false
            };
            var bBreakInfoInReport = true;
            var row = new RawData
                          {
                            Break1 = "foo",
                            Break2 = "bar",
                            Break3 = "baz"
                          };
            var expected = "foo/bar/baz";

            var output = _calc.GetBreakCombo(row, breaks, bBreakInfoInReport);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void GetBreakCombo_UserBreak1TrueNoPutBreakInfoInBody_ReturnDataInBreak1()
        {
            var breaks = new UserBreaks
            {
                UserBreak1 = true,
                UserBreak2 = false,
                UserBreak3 = false
            };
            var bBreakInfoInReport = false;
            var row = new RawData
            {
                Break1 = "foo",
                Break2 = "bar",
                Break3 = "baz"
            };
            var expected = "foo";

            var output = _calc.GetBreakCombo(row, breaks, bBreakInfoInReport);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void GetBreakCombo_UserBreak1And2TrueNoPutBreakInfoInBody_ReturnDataInBreak1()
        {
            var breaks = new UserBreaks
            {
                UserBreak1 = true,
                UserBreak2 = true,
                UserBreak3 = false
            };
            var bBreakInfoInReport = false;
            var row = new RawData
            {
                Break1 = "foo",
                Break2 = "bar",
                Break3 = "baz"
            };
            var expected = "foo/bar";

            var output = _calc.GetBreakCombo(row, breaks, bBreakInfoInReport);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void GetNumberOfPassengers_GoodPositiveValue_ReturnValue()
        {
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.NBRPASSENGERS, "2");

            var expected = 2;

            var output = _calc.GetNumberOfPassengers(globals);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void GetNumberOfPassengers_GoodNegativeValue_Return5()
        {
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.NBRPASSENGERS, "-1");

            var expected = 5;

            var output = _calc.GetNumberOfPassengers(globals);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void GetNumberOfPassengers_BadValue_Return5()
        {
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.NBRPASSENGERS, "foo");

            var expected = 5;

            var output = _calc.GetNumberOfPassengers(globals);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void GetDomesticInternationalValue_GoodValue_ReturnValue()
        {
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.DOMINTL, "2");

            var expected = 2;

            var output = _calc.GetDomesticInternationalValue(globals);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void GetDomesticInternationalValue_BadValue_Return0()
        {
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.DOMINTL, "foo");

            var expected = 0;

            var output = _calc.GetDomesticInternationalValue(globals);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void GetDomesticInternationalWhereText_NotInSwitch_ReturnEmpty()
        {
            var val = 100;
            var expected = "";

            var output = _calc.GetDomesticInternationalWhereText(val);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void GetDomesticInternationalWhereText_ValueIs2_ReturnDomesticOnly()
        {
            var val = 2;
            var expected = "Domestic Only ;";

            var output = _calc.GetDomesticInternationalWhereText(val);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void GetDomesticInternationalWhereText_ValueIs3_ReturnInternationalOnly()
        {
            var val = 3;
            var expected = "International Only ;";

            var output = _calc.GetDomesticInternationalWhereText(val);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void GetDomesticInternationalWhereText_ValueIs4_ReturnTransborderOnly()
        {
            var val = 4;
            var expected = "Transborder Only ;";

            var output = _calc.GetDomesticInternationalWhereText(val);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void GetDomesticInternationalWhereText_ValueIs5_ReturnExcludeDomestic()
        {
            var val = 5;
            var expected = "Exclude Domestic ;";

            var output = _calc.GetDomesticInternationalWhereText(val);

            Assert.AreEqual(expected, output);
        }
        [TestMethod]
        public void GetDomesticInternationalWhereText_ValueIs6_ReturnExcludeInternational()
        {
            var val = 6;
            var expected = "Exclude International ;";

            var output = _calc.GetDomesticInternationalWhereText(val);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void GetDomesticInternationalWhereText_ValueIs7_ReturnExcludeTransborder()
        {
            var val = 7;
            var expected = "Exclude Transborder ;";

            var output = _calc.GetDomesticInternationalWhereText(val);

            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void IsIgnoreBreakSettingsOn_IsOn_ReturnTrue()
        {
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.CBIGNOREBRKSETTINGS, "ON");

            var output = _calc.IsIgnoreBreakSettingsOn(globals);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsIgnoreBreakSettingsOn_IsOff_ReturnFalse()
        {
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.CBIGNOREBRKSETTINGS, "OFF");

            var output = _calc.IsIgnoreBreakSettingsOn(globals);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void IsPrintBreakInfoInBodyOn_IsOn_ReturnTrue()
        {
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.CBPRINTBRKINFOINBODY, "ON");

            var output = _calc.IsPrintBreakInfoInBodyOn(globals);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsPrintBreakInfoInBodyOn_IsOff_ReturnFalse()
        {
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.CBPRINTBRKINFOINBODY, "OFF");

            var output = _calc.IsIgnoreBreakSettingsOn(globals);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void IsGantAgency_IsGant_ReturnTrue()
        {
            var agency = "gant";

            var output = _calc.IsGantAgency(agency);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsGantAgency_NotGant_ReturnFalse()
        {
            var agency = "foo";

            var output = _calc.IsGantAgency(agency);

            Assert.AreEqual(false, output);
        }

        
    }
}
