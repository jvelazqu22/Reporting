using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using Domain.Exceptions;

using iBank.Entities.ClientEntities;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

using Moq;

using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.UserDefined;
using iBank.UnitTesting.TestingHelpers.MockHelpers;

using iBankDomain.RepositoryInterfaces;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.UserDefinedReport
{
    [TestClass]
    public class UserReportDefinitionRetrieverTests
    {

        [TestMethod]
        public void LoadUserReportColumnData_PassTwoColumnsWithNotAllowedField_ReturnOneColumnMatchTrue()
        {
            //Arrange
            var globals = new ReportGlobals();
            var userrpt2 = new List<userrpt2>
            {
                new userrpt2 {colname="ACOMMISN", agency="a", badfld="N", badhilite="", badoper="", badvalue="", colorder=1, crname="c", goodfld="", goodhilite="", goodoper="",goodvalue ="", grpbreak=0, horAlign="", pagebreak=false, subtotal=false, udidhdg1="1", udidhdg2="", udidtype=1},
                new userrpt2 {colname="ACOMMISN2", agency="a", badfld="N", badhilite="", badoper="", badvalue="", colorder=1, crname="c", goodfld="", goodhilite="", goodoper="",goodvalue ="", grpbreak=0, horAlign="", pagebreak=false, subtotal=false, udidhdg1="2", udidhdg2="", udidtype=1}
            };
            var columnList = new List<collist2>
            {
                new collist2 { colname = "ACOMMISN" },
                new collist2 { colname = "ACOMMISN2"}
            };
            var collist2Query = new Mock<IQuery<IList<collist2>>>();
            collist2Query.Setup(x=>x.ExecuteQuery()).Returns(columnList);

            var userRptData = new List<userrpts> {new userrpts() { crname = "test", reportkey=1, agency="a", userid="1",
                crtitle ="t", crtype="d", crsubtit="d", theme="d", lastused=DateTime.MinValue, segmentleg=1, UserNumber=1, nodetail=false, tripsumlvl=0, pgfoottext="test"} };
            var mockDb = new Mock<IClientQueryable>();
            mockDb.Setup(x => x.UserRpt).Returns(MockHelper.GetListAsQueryable(userRptData).Object);
            var mockStore = new Mock<IClientDataStore>();
            mockStore.Setup(x => x.ClientQueryDb).Returns(mockDb.Object);

            var mockUpdater = new Mock<IUserReportUpdater>();
            
            var urdRetriever = new UserReportDefinitionRetriever(globals, collist2Query.Object, mockUpdater.Object);
            var report = urdRetriever.LoadUserReportInformation(mockStore.Object, 1);

            var queryrpt2 = new Mock<IQuery<IList<userrpt2>>>();
            queryrpt2.Setup(x => x.ExecuteQuery()).Returns(userrpt2);            

            var actCol = 1;
            //Action
            var columns = urdRetriever.LoadUserReportColumnData(queryrpt2.Object);

            //Assert
            Assert.AreEqual(columns.Count, actCol);
        }


        public void LoadUserReportColumnData_PassTwoSameColumns_ReturnSuppressDuplicatesIsTrue()
        {
            //Arrange
            var globals = new ReportGlobals();
            var urcColumns = new List<UserReportColumnInformation>
            {
                new UserReportColumnInformation {Name="B"},
                new UserReportColumnInformation {Name="B"}
            };
            var columnList = new List<collist2>
            {
                new collist2 { colname = "A" },
                new collist2 { colname = "B"}
            };
            var collist2Query = new Mock<IQuery<IList<collist2>>>();
            collist2Query.Setup(x => x.ExecuteQuery()).Returns(columnList);

            var userRptData = new List<userrpts> {new userrpts() { crname = "test", reportkey=1, agency="a", userid="1",
                crtitle ="t", crtype="d", crsubtit="d", theme="d", lastused=DateTime.MinValue, segmentleg=1, UserNumber=1, nodetail=false, tripsumlvl=0, pgfoottext="test"} };
            var mockDb = new Mock<IClientQueryable>();
            mockDb.Setup(x => x.UserRpt).Returns(MockHelper.GetListAsQueryable(userRptData).Object);
            var mockStore = new Mock<IClientDataStore>();
            mockStore.Setup(x => x.ClientQueryDb).Returns(mockDb.Object);           

            var urdRetriever = new UserReportDefinitionRetriever(globals, collist2Query.Object);
            var report = urdRetriever.LoadUserReportInformation(mockStore.Object, 1);
           
            //Action
            var sut = urdRetriever.SuppressDuplicates(urcColumns);

            //Assert
            Assert.IsTrue(sut);
        }

        [TestMethod]
        public void LoadUserReportColumnData_PassTwoColumnsBothAllowed_ReturnTwoColumnsMatchTrue()
        {
            //Arrange
            var globals = new ReportGlobals();
            var userrpt2 = new List<userrpt2>
            {
                new userrpt2 {colname="A", agency="a", badfld="N", badhilite="", badoper="", badvalue="", colorder=1, crname="c", goodfld="", goodhilite="", goodoper="",goodvalue ="", grpbreak=0, horAlign="", pagebreak=false, subtotal=false, udidhdg1="1", udidhdg2="", udidtype=1},
                new userrpt2 {colname="B", agency="a", badfld="N", badhilite="", badoper="", badvalue="", colorder=1, crname="c", goodfld="", goodhilite="", goodoper="",goodvalue ="", grpbreak=0, horAlign="", pagebreak=false, subtotal=false, udidhdg1="2", udidhdg2="", udidtype=1}
            };
            var columnList = new List<collist2>
            {
                new collist2 { colname = "A" },
                new collist2 { colname = "B"}
            };
            var collist2Query = new Mock<IQuery<IList<collist2>>>();
            collist2Query.Setup(x => x.ExecuteQuery()).Returns(columnList);

            var userRptData = new List<userrpts> {new userrpts() { crname = "test", reportkey=1, agency="a", userid="1",
                crtitle ="t", crtype="d", crsubtit="d", theme="d", lastused=DateTime.MinValue, segmentleg=1, UserNumber=1, nodetail=false, tripsumlvl=0, pgfoottext="test"} };
            var mockDb = new Mock<IClientQueryable>();
            mockDb.Setup(x => x.UserRpt).Returns(MockHelper.GetListAsQueryable(userRptData).Object);
            var mockStore = new Mock<IClientDataStore>();
            mockStore.Setup(x => x.ClientQueryDb).Returns(mockDb.Object);

            var mockUpdater = new Mock<IUserReportUpdater>();

            var urdRetriever = new UserReportDefinitionRetriever(globals, collist2Query.Object, mockUpdater.Object);
            var report = urdRetriever.LoadUserReportInformation(mockStore.Object, 1);

            var queryrpt2 = new Mock<IQuery<IList<userrpt2>>>();
            queryrpt2.Setup(x => x.ExecuteQuery()).Returns(userrpt2);

            var actCol = 2;
            //Action
            var columns = urdRetriever.LoadUserReportColumnData(queryrpt2.Object);

            //Assert
            Assert.AreEqual(columns.Count, actCol);
        }

        [TestMethod]
        public void LoadUserReportInformation_VerifyUpdateLastUsed_isCalled()
        {
            //Arrange
            var globals = new ReportGlobals();
            var userrpt2 = new List<userrpt2>
            {
                new userrpt2 {colname="ACOMMISN", agency="a", badfld="N", badhilite="", badoper="", badvalue="", colorder=1, crname="c", goodfld="", goodhilite="", goodoper="",goodvalue ="", grpbreak=0, horAlign="", pagebreak=false, subtotal=false, udidhdg1="1", udidhdg2="", udidtype=1},
                new userrpt2 {colname="ACOMMISN2", agency="a", badfld="N", badhilite="", badoper="", badvalue="", colorder=1, crname="c", goodfld="", goodhilite="", goodoper="",goodvalue ="", grpbreak=0, horAlign="", pagebreak=false, subtotal=false, udidhdg1="2", udidhdg2="", udidtype=1}
            };
            var columnList = new List<collist2>
            {
                new collist2 { colname = "ACOMMISN" },
                new collist2 { colname = "ACOMMISN2"}
            };
            var collist2Query = new Mock<IQuery<IList<collist2>>>();
            collist2Query.Setup(x => x.ExecuteQuery()).Returns(columnList);

            var userRcd = new userrpts()
            {
                crname = "test",
                reportkey = 1,
                agency = "a",
                userid = "1",
                crtitle = "t",
                crtype = "d",
                crsubtit = "d",
                theme = "d",
                lastused = DateTime.MinValue,
                segmentleg = 1,
                UserNumber = 1,
                nodetail = false,
                tripsumlvl = 0,
                pgfoottext = "test"
            };
            var userRptData = new List<userrpts> { userRcd };
            var mockDb = new Mock<IClientQueryable>();
            mockDb.Setup(x => x.UserRpt).Returns(MockHelper.GetListAsQueryable(userRptData).Object);
            var mockStore = new Mock<IClientDataStore>();
            mockStore.Setup(x => x.ClientQueryDb).Returns(mockDb.Object);
            var mockUpdater = new Mock<IUserReportUpdater>();
            var sut = new UserReportDefinitionRetriever(globals, collist2Query.Object, mockUpdater.Object);

            //Act
            var report = sut.LoadUserReportInformation(mockStore.Object, 1);

            //Assert
            mockUpdater.Verify(x => x.UpdateLastUsed(userRcd, mockStore.Object));
        }

        [ExpectedException(typeof(ReportNotFoundException))]
        [TestMethod]
        public void LoadUserReportInformation_BlankUserRcd_ReportNameIsnull()
        {
            //Arrange
            var globals = new ReportGlobals();
            var userrpt2 = new List<userrpt2>
            {
                new userrpt2 {colname="ACOMMISN", agency="a", badfld="N", badhilite="", badoper="", badvalue="", colorder=1, crname="c", goodfld="", goodhilite="", goodoper="",goodvalue ="", grpbreak=0, horAlign="", pagebreak=false, subtotal=false, udidhdg1="1", udidhdg2="", udidtype=1},
                new userrpt2 {colname="ACOMMISN2", agency="a", badfld="N", badhilite="", badoper="", badvalue="", colorder=1, crname="c", goodfld="", goodhilite="", goodoper="",goodvalue ="", grpbreak=0, horAlign="", pagebreak=false, subtotal=false, udidhdg1="2", udidhdg2="", udidtype=1}
            };
            var columnList = new List<collist2>
            {
                new collist2 { colname = "ACOMMISN" },
                new collist2 { colname = "ACOMMISN2"}
            };
            var collist2Query = new Mock<IQuery<IList<collist2>>>();
            collist2Query.Setup(x => x.ExecuteQuery()).Returns(columnList);

            var userRcd = new userrpts();

            var userRptData = new List<userrpts> { userRcd };
            var mockDb = new Mock<IClientQueryable>();
            mockDb.Setup(x => x.UserRpt).Returns(MockHelper.GetListAsQueryable(userRptData).Object);
            var mockStore = new Mock<IClientDataStore>();
            mockStore.Setup(x => x.ClientQueryDb).Returns(mockDb.Object);
            var mockUpdater = new Mock<IUserReportUpdater>();
            var sut = new UserReportDefinitionRetriever(globals, collist2Query.Object, mockUpdater.Object);

            //Act
            var report = sut.LoadUserReportInformation(mockStore.Object, 1);
        }

    }
}
