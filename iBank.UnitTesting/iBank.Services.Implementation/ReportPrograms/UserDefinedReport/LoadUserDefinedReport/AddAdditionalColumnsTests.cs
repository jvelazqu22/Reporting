
using iBank.Services.Implementation.ReportPrograms.UserDefined;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using Domain.Helper;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.UserDefinedReport.LoadUserDefinedReport
{
    [TestClass]
    public class AddAdditionalColumnsTests
    {
        [TestMethod]
        public void AddAdditionalColumns_WhenItIsHotelUserDefinedReportXlsReturnRECKEYandLEGCNTR_ResultMatch()
        {
            //Arrange
            var act = BuildUserReportColumnInformation();
            var exp = new List<UserReportColumnInformation>()
            {
                new UserReportColumnInformation {Name = "PAXNAME", TableName = "TRIPS"},
                new UserReportColumnInformation {Name = "RECKEY", TableName = "TRIPS"},
                new UserReportColumnInformation {Name = "LEGCNTR", TableName = "DUMMY"},
                new UserReportColumnInformation {Name = "SORT", TableName = "DUMMY"}
            };
            var sut = new UserDefinedColumnBuilder();

            //Act
            sut.AddAdditionalColumns(DestinationSwitch.Xls, (int)ReportTitles.HotelUserDefinedReports, act);

            //Assert
            for(int i =0; i<act.Count; i++)
            {
                Assert.AreEqual(exp[i].Name, act[i].Name);
                Assert.AreEqual(exp[i].TableName, act[i].TableName);
            }
        }
        [TestMethod]
        public void AddAdditionalColumns_WhenItIsCombinedUserDefinedReportXlsReturnAdditional4Columns_ResultMatch()
        {
            //Arrange
            var act = BuildUserReportColumnInformation();

            var exp = new List<UserReportColumnInformation>
            {
                new UserReportColumnInformation {Name = "PAXNAME", TableName = "TRIPS"},
                new UserReportColumnInformation {Name = "RECKEY", TableName = "TRIPS"},
                new UserReportColumnInformation {Name = "PLUSMIN", TableName = "TRIPS"},
                new UserReportColumnInformation {Name = "LEGPLUSMIN", TableName = "DUMMY"},
                new UserReportColumnInformation {Name = "LEGCNTR", TableName = "DUMMY"},
                new UserReportColumnInformation {Name = "SORT", TableName = "DUMMY"}
            };
            var sut = new UserDefinedColumnBuilder();

            //Act
            sut.AddAdditionalColumns(DestinationSwitch.Xls, (int)ReportTitles.CombinedUserDefinedReports, act);
            //Assert
            for (int i = 0; i < act.Count; i++)
            {
                Assert.AreEqual(exp[i].Name, act[i].Name);
                Assert.AreEqual(exp[i].TableName, act[i].TableName);
            }
        }


        [TestMethod]
        public void AddAdditionalColumns_WhenItIsCombinedUserDefinedPDFReportOnlyAddRECKEY_ResultMatch()
        {
            //Arrange
            var act = BuildUserReportColumnInformation();
            var exp = new List<UserReportColumnInformation>
            {
                new UserReportColumnInformation {Name = "PAXNAME", TableName = "TRIPS"},
                new UserReportColumnInformation {Name = "RECKEY", TableName = "TRIPS"},
                new UserReportColumnInformation {Name = "LEGCNTR", TableName = "DUMMY"},
                new UserReportColumnInformation {Name = "SORT", TableName = "DUMMY"}
            };
            var sut = new UserDefinedColumnBuilder();

            //Act
            sut.AddAdditionalColumns(DestinationSwitch.ClassicPdf, (int)ReportTitles.CombinedUserDefinedReports, act);
            
            //Assert
            for (var i = 0; i < act.Count; i++)
            {
                Assert.AreEqual(exp[i].Name, act[i].Name);
                Assert.AreEqual(exp[i].TableName, act[i].TableName);
            }
        }
        
        private List<UserReportColumnInformation> BuildUserReportColumnInformation()
        {
            var list = new List<UserReportColumnInformation>
            {
                new UserReportColumnInformation {Name = "PAXNAME", TableName = "TRIPS"}
            };
            return list;
        }
    }
}
