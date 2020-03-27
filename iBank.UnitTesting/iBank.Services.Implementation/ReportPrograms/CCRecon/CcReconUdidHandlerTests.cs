using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using iBank.Services.Implementation.ReportPrograms.CCRecon;
using iBank.Server.Utilities.Classes;
using Domain.Helper;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.CCRecon
{
    [TestClass]
    public class CcReconUdidHandlerTests
    {

        [TestMethod]
        public void GetUdidLabel_Pass1UdidValueIsZero_ShouldBeBlank()
        {
            //arrange
            var label1 = "first label:";
            var blankLabel = string.Empty;
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.UDIDONRPT1, "0");
            globals.SetParmValue(WhereCriteria.UDIDLBL1, label1);

            //act
            var handler = new CcReconUdidHandler(globals);
            handler.SetUdidOnReportProperties();
            var actLabel1 = handler.UdidLabel[0];

            //assert
            Assert.AreEqual(blankLabel, actLabel1);
        }

        [TestMethod]
        public void GetUdidLabel_Pass2UdidBothLabelsSupplied_MatchLabels()
        {
            //arrange
            var label1 = "first label:";
            var label2 = "second label:";
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.UDIDONRPT1, "1");
            globals.SetParmValue(WhereCriteria.UDIDLBL1, label1);
            globals.SetParmValue(WhereCriteria.UDIDONRPT2, "10");
            globals.SetParmValue(WhereCriteria.UDIDLBL2, label2);

            //act
            var handler = new CcReconUdidHandler(globals);
            handler.SetUdidOnReportProperties();
            var actLabel1 = handler.UdidLabel[0];
            var actLabel2 = handler.UdidLabel[1];

            //assert
            Assert.AreEqual(label1, actLabel1);
            Assert.AreEqual(label2, actLabel2);
        }

        [TestMethod]
        public void GetUdidLabel_Pass1UdidLabelIsNotSupplied_MatchDefaultLabel()
        {
            //arrange
            var label1 = string.Empty;
            var defaultLabel1 = "Udid # 1 text:";

            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.UDIDONRPT1, "1");
            globals.SetParmValue(WhereCriteria.UDIDLBL1, label1);

            //act
            var handler = new CcReconUdidHandler(globals);
            handler.SetUdidOnReportProperties();
            var actLabel1 = handler.UdidLabel[0];

            //assert
            Assert.AreEqual(defaultLabel1, actLabel1);
        }
    }
}
