using System.Collections.Generic;

using Domain.Exceptions;

using iBank.Entities.MasterEntities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.UserDefined;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.UserDefinedReport
{
    [TestClass]
    public class ColumnConstructorTests
    {
        [TestMethod]
        public void GetCurrentColumnFromCollist_TypeIsCombdet_InComboTables_ReturnValueMatchedOnName()
        {
            var reportType = "COMBDET";
            var collist = new List<collist2> { new collist2 { colname = "foo", coltable = "TRIPS" } };
            var userReportColumn = new UserReportColumnInformation { Name = "foo" };
            var sut = new ColumnConstructor(userReportColumn);

            var output = sut.GetCurrentColumnFromCollist(reportType, collist);

            Assert.AreEqual("foo", output.colname);
        }

        [ExpectedException(typeof(UserDefinedColumnNotFoundException))]
        [TestMethod]
        public void GetCurrentColumnFromCollist_TypeIsCombdet_NotInComboTables_ThrowExcpetion()
        {
            var reportType = "COMBDET";
            var collist = new List<collist2> { new collist2 { colname = "foo", coltable = "fake" } };
            var userReportColumn = new UserReportColumnInformation { Name = "foo" };
            var sut = new ColumnConstructor(userReportColumn);

            var output = sut.GetCurrentColumnFromCollist(reportType, collist);
        }

        [TestMethod]
        public void GetCurrentColumnFromCollist_TypeIsCalls_ReportTypeIsTracs_InAcctTables_ReturnValueMatchedOnName()
        {
            var reportType = "CALLS";
            var collist = new List<collist2> { new collist2 { colname = "foo", coltable = "ACCTSPCL", rpttype = "TRACS" } };
            var userReportColumn = new UserReportColumnInformation { Name = "foo" };
            var sut = new ColumnConstructor(userReportColumn);

            var output = sut.GetCurrentColumnFromCollist(reportType, collist);

            Assert.AreEqual("foo", output.colname);
        }

        [ExpectedException(typeof(UserDefinedColumnNotFoundException))]
        [TestMethod]
        public void GetCurrentColumnFromCollist_TypeIsCalls_ReportTypeIsNotTracs_InAcctTables_ThrowException()
        {
            var reportType = "CALLS";
            var collist = new List<collist2> { new collist2 { colname = "foo", coltable = "ACCTSPCL", rpttype = "fake" } };
            var userReportColumn = new UserReportColumnInformation { Name = "foo" };
            var sut = new ColumnConstructor(userReportColumn);

            var output = sut.GetCurrentColumnFromCollist(reportType, collist);
        }

        [TestMethod]
        public void GetCurrentColumnFromCollist_TypeIsProdt_ReportTypeIsTracs_InAcctTables_ReturnValueMatchedOnName()
        {
            var reportType = "PRODT";
            var collist = new List<collist2> { new collist2 { colname = "foo", coltable = "ACCTSPCL", rpttype = "TRACS" } };
            var userReportColumn = new UserReportColumnInformation { Name = "foo" };
            var sut = new ColumnConstructor(userReportColumn);

            var output = sut.GetCurrentColumnFromCollist(reportType, collist);

            Assert.AreEqual("foo", output.colname);
        }

        [ExpectedException(typeof(UserDefinedColumnNotFoundException))]
        [TestMethod]
        public void GetCurrentColumnFromCollist_TypeIsProdt_ReportTypeIsNotTracs_InAcctTables_ThrowException()
        {
            var reportType = "PRODT";
            var collist = new List<collist2> { new collist2 { colname = "foo", coltable = "ACCTSPCL", rpttype = "fake" } };
            var userReportColumn = new UserReportColumnInformation { Name = "foo" };
            var sut = new ColumnConstructor(userReportColumn);

            var output = sut.GetCurrentColumnFromCollist(reportType, collist);
        }

        [ExpectedException(typeof(UserDefinedColumnNotFoundException))]
        [TestMethod]
        public void GetCurrentColumnFromCollist_TypeIsProdt_ReportTypeIsTracs_NotInAcctTables_ThrowException()
        {
            var reportType = "PRODT";
            var collist = new List<collist2> { new collist2 { colname = "foo", coltable = "fake", rpttype = "TRACS" } };
            var userReportColumn = new UserReportColumnInformation { Name = "foo" };
            var sut = new ColumnConstructor(userReportColumn);

            var output = sut.GetCurrentColumnFromCollist(reportType, collist);
        }

        [ExpectedException(typeof(UserDefinedColumnNotFoundException))]
        [TestMethod]
        public void GetCurrentColumnFromCollist_TypeIsProdt_ReportTypeIsTracs_InAcctTables_NamesDontMatch_ThrowException()
        {
            var reportType = "PRODT";
            var collist = new List<collist2> { new collist2 { colname = "foo", coltable = "fake", rpttype = "TRACS" } };
            var userReportColumn = new UserReportColumnInformation { Name = "bar" };
            var sut = new ColumnConstructor(userReportColumn);

            var output = sut.GetCurrentColumnFromCollist(reportType, collist);
        }

        [TestMethod]
        public void GetCurrentColumnFromCollist_TypeNotInList_NamesMatch_ReportTypeIsAll_ReturnValueMatchedOnName()
        {
            var reportType = "fake";
            var collist = new List<collist2> { new collist2 { colname = "foo", rpttype = "ALL" } };
            var userReportColumn = new UserReportColumnInformation { Name = "foo" };
            var sut = new ColumnConstructor(userReportColumn);

            var output = sut.GetCurrentColumnFromCollist(reportType, collist);

            Assert.AreEqual("foo", output.colname);
        }

        [TestMethod]
        public void GetCurrentColumnFromCollist_TypeNotInList_NamesMatch_ReportTypeMatches_ReturnValueMatchedOnName()
        {
            var reportType = "bar";
            var collist = new List<collist2> { new collist2 { colname = "foo", rpttype = "bar" } };
            var userReportColumn = new UserReportColumnInformation { Name = "foo" };
            var sut = new ColumnConstructor(userReportColumn);

            var output = sut.GetCurrentColumnFromCollist(reportType, collist);

            Assert.AreEqual("foo", output.colname);
        }

        [ExpectedException(typeof(UserDefinedColumnNotFoundException))]
        [TestMethod]
        public void GetCurrentColumnFromCollist_TypeNotInList_NamesMatch_ReportTypeIsNotAllAndDoesntMatch_ThrowException()
        {
            var reportType = "fake";
            var collist = new List<collist2> { new collist2 { colname = "foo", rpttype = "bar" } };
            var userReportColumn = new UserReportColumnInformation { Name = "foo" };
            var sut = new ColumnConstructor(userReportColumn);

            var output = sut.GetCurrentColumnFromCollist(reportType, collist);
        }

        [ExpectedException(typeof(UserDefinedColumnNotFoundException))]
        [TestMethod]
        public void GetCurrentColumnFromCollist_TypeNotInList_NamesDontMatch_ThrowException()
        {
            var reportType = "fake";
            var collist = new List<collist2> { new collist2 { colname = "foo", rpttype = "ALL" } };
            var userReportColumn = new UserReportColumnInformation { Name = "bar" };
            var sut = new ColumnConstructor(userReportColumn);

            var output = sut.GetCurrentColumnFromCollist(reportType, collist);
        }

        [TestMethod]
        public void AssignHeaders_NotUdid_UserReportHeaderOneIsNotEmpty_SetNewColumnHeadersToUserReportColumnHeaders()
        {
            var userReportColumn = new UserReportColumnInformation { Name = "not a udid", Header1 = "foo", Header2 = "" };
            var newColumn = new UserReportColumnInformation { Name = "new", Header1 = "blah", Header2 = "blah blah" };
            var sut = new ColumnConstructor(userReportColumn);

            sut.AssignHeaders(newColumn, new ReportGlobals());

            Assert.AreEqual("foo", newColumn.Header1);
            Assert.AreEqual("", newColumn.Header2);
        }

        [TestMethod]
        public void AssignHeaders_NotUdid_UserReportHeaderTwoIsNotEmpty_SetNewColumnHeadersToUserReportColumnHeaders()
        {
            var userReportColumn = new UserReportColumnInformation { Name = "not a udid", Header1 = "", Header2 = "foo" };
            var newColumn = new UserReportColumnInformation { Name = "new", Header1 = "blah", Header2 = "blah blah" };
            var sut = new ColumnConstructor(userReportColumn);

            sut.AssignHeaders(newColumn, new ReportGlobals());

            Assert.AreEqual("", newColumn.Header1);
            Assert.AreEqual("foo", newColumn.Header2);
        }

        [TestMethod]
        public void AssignHeaders_IsUdid_UserReportHeadersAreEmptySetNewColumnHeadersToUserReportColumnHeaders()
        {
            var userReportColumn = new UserReportColumnInformation { Name = "a udid", Header1 = "", Header2 = "" };
            var newColumn = new UserReportColumnInformation { Name = "new", Header1 = "blah", Header2 = "blah blah" };
            var sut = new ColumnConstructor(userReportColumn) { IsUdidField = true };

            sut.AssignHeaders(newColumn, new ReportGlobals());

            Assert.AreEqual("", newColumn.Header1);
            Assert.AreEqual("", newColumn.Header2);
        }

        [TestMethod]
        public void AssignHeaders_IsUserLabeledUdid_UserReportHeadersAreEmptySetNewColumnHeadersToUserReportColumnHeaders()
        {
            var userReportColumn = new UserReportColumnInformation { Name = "a udid", Header1 = "", Header2 = "" };
            var newColumn = new UserReportColumnInformation { Name = "new", Header1 = "blah", Header2 = "blah blah" };
            var sut = new ColumnConstructor(userReportColumn) { IsUserLabeledUdid = true };

            sut.AssignHeaders(newColumn, new ReportGlobals());

            Assert.AreEqual("", newColumn.Header1);
            Assert.AreEqual("", newColumn.Header2);
        }

        [ExpectedException(typeof(UserDefinedColumnNotFoundException))]
        [TestMethod]
        public void AssignHeaders_NotUdid_EmptyUserReportHeaders_NullCurrentColumnSupplied_ThrowException()
        {
            var userReportColumn = new UserReportColumnInformation { Name = "col", Header1 = "", Header2 = "" };
            var newColumn = new UserReportColumnInformation { Name = "new", Header1 = "blah", Header2 = "blah blah" };
            var sut = new ColumnConstructor(userReportColumn) { IsUdidField = false, IsUserLabeledUdid = false };

            sut.AssignHeaders(newColumn, new ReportGlobals());
        }
        
        [TestMethod]
        public void AssignHeaders_NotUdid_EmptyUserReportHeaders_AssignColumnHeadersToRetrievedHeaders()
        {
            var userReportColumn = new UserReportColumnInformation { Name = "col", Header1 = "", Header2 = "" };
            var newColumn = new UserReportColumnInformation { Name = "new", Header1 = "blah", Header2 = "blah blah" };
            var globals = new ReportGlobals();
            var currentColumn = new collist2 { colname = "fake", head1 = "foo", head2 = "bar" };
            var sut = new ColumnConstructor(userReportColumn) { IsUdidField = false, IsUserLabeledUdid = false };
            
            sut.AssignHeaders(newColumn, globals, currentColumn);

            Assert.AreEqual("foo", newColumn.Header1);
            Assert.AreEqual("bar", newColumn.Header2);
        }

        [TestMethod]
        public void IsSuppressDuplicates_NotTripLevelField_ReturnFalse()
        {
            var userReportColumn = new UserReportColumnInformation { Name = "col", Header1 = "", Header2 = "" };
            var tableName = "fake";
            var type = "";
            var suppressOptions = "";
            var sut = new ColumnConstructor(userReportColumn);

            var output = sut.IsSuppressDuplicates(tableName, type, suppressOptions);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void IsSuppressDuplicates_TripLevelField_SuppressOptionsNotInSwitch_ReturnFalse()
        {
            var userReportColumn = new UserReportColumnInformation { Name = "col", Header1 = "", Header2 = "" };
            var tableName = "TRIPS";
            var type = "";
            var suppressOptions = "99";
            var sut = new ColumnConstructor(userReportColumn);

            var output = sut.IsSuppressDuplicates(tableName, type, suppressOptions);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void IsSuppressDuplicates_TripLevelField_SuppressOptionsIsOne_TypeIsText_ReturnTrue()
        {
            var userReportColumn = new UserReportColumnInformation { Name = "col", Header1 = "", Header2 = "" };
            var tableName = "TRIPS";
            var type = "TEXT";
            var suppressOptions = "1";
            var sut = new ColumnConstructor(userReportColumn);

            var output = sut.IsSuppressDuplicates(tableName, type, suppressOptions);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsSuppressDuplicates_TripLevelField_SuppressOptionsIsOne_TypeIsDate_ReturnTrue()
        {
            var userReportColumn = new UserReportColumnInformation { Name = "col", Header1 = "", Header2 = "" };
            var tableName = "TRIPS";
            var type = "DATE";
            var suppressOptions = "1";
            var sut = new ColumnConstructor(userReportColumn);

            var output = sut.IsSuppressDuplicates(tableName, type, suppressOptions);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsSuppressDuplicates_TripLevelField_SuppressOptionsIsOne_TypeIsCurrency_ReturnTrue()
        {
            var userReportColumn = new UserReportColumnInformation { Name = "col", Header1 = "", Header2 = "" };
            var tableName = "TRIPS";
            var type = "CURRENCY";
            var suppressOptions = "1";
            var sut = new ColumnConstructor(userReportColumn);

            var output = sut.IsSuppressDuplicates(tableName, type, suppressOptions);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsSuppressDuplicates_TripLevelField_SuppressOptionsIsOne_TypeIsFake_ReturnFalse()
        {
            var userReportColumn = new UserReportColumnInformation { Name = "col", Header1 = "", Header2 = "" };
            var tableName = "TRIPS";
            var type = "FAKE";
            var suppressOptions = "1";
            var sut = new ColumnConstructor(userReportColumn);

            var output = sut.IsSuppressDuplicates(tableName, type, suppressOptions);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void IsSuppressDuplicates_TripLevelField_SuppressOptionsIsTwo_TypeIsText_ReturnTrue()
        {
            var userReportColumn = new UserReportColumnInformation { Name = "col", Header1 = "", Header2 = "" };
            var tableName = "TRIPS";
            var type = "TEXT";
            var suppressOptions = "2";
            var sut = new ColumnConstructor(userReportColumn);

            var output = sut.IsSuppressDuplicates(tableName, type, suppressOptions);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsSuppressDuplicates_TripLevelField_SuppressOptionsIsTwo_TypeIsDate_ReturnTrue()
        {
            var userReportColumn = new UserReportColumnInformation { Name = "col", Header1 = "", Header2 = "" };
            var tableName = "TRIPS";
            var type = "DATE";
            var suppressOptions = "2";
            var sut = new ColumnConstructor(userReportColumn);

            var output = sut.IsSuppressDuplicates(tableName, type, suppressOptions);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsSuppressDuplicates_TripLevelField_SuppressOptionsIsTwo_TypeIsFake_ReturnFalse()
        {
            var userReportColumn = new UserReportColumnInformation { Name = "col", Header1 = "", Header2 = "" };
            var tableName = "TRIPS";
            var type = "FAKE";
            var suppressOptions = "2";
            var sut = new ColumnConstructor(userReportColumn);

            var output = sut.IsSuppressDuplicates(tableName, type, suppressOptions);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void IsSuppressDuplicates_TripLevelField_SuppressOptionsIsThree_TypeIsNumeric_ReturnTrue()
        {
            var userReportColumn = new UserReportColumnInformation { Name = "col", Header1 = "", Header2 = "" };
            var tableName = "TRIPS";
            var type = "NUMERIC";
            var suppressOptions = "3";
            var sut = new ColumnConstructor(userReportColumn);

            var output = sut.IsSuppressDuplicates(tableName, type, suppressOptions);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsSuppressDuplicates_TripLevelField_SuppressOptionsIsThree_TypeIsFake_ReturnFalse()
        {
            var userReportColumn = new UserReportColumnInformation { Name = "col", Header1 = "", Header2 = "" };
            var tableName = "TRIPS";
            var type = "FAKE";
            var suppressOptions = "3";
            var sut = new ColumnConstructor(userReportColumn);

            var output = sut.IsSuppressDuplicates(tableName, type, suppressOptions);

            Assert.AreEqual(false, output);
        }
    }
}
