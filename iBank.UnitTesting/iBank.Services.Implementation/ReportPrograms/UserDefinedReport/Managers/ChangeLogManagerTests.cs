using System;
using Domain.Helper;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.ReportPrograms.UserDefinedReport.Managers
{
    [TestClass]
    public class ChangeLogManagerTests
    {
        [TestMethod]
        public void ChangeLogManager_PassInchangeCodeChangeLogCriteriaPresent_ReturnTrue()
        {
            //Arrange
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.INCHANGECODE, "1");

            var changeLog = new ChangeLogManager(globals);

            //Act
            var exp = changeLog.ChangeLogCriteriaPresent;


            //Assert
            Assert.IsTrue(exp);
        }

        [TestMethod]
        public void ChangeLogManager_PassCancelCodeYChangeLogCriteriaPresent_ReturnTrue()
        {
            //Arrange
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.CANCELCODE, "Y");

            var changeLog = new ChangeLogManager(globals);

            //Act
            var exp = changeLog.ChangeLogCriteriaPresent;


            //Assert
            Assert.IsTrue(exp);
        }


        [TestMethod]
        public void ChangeLogManager_PassCencelCodeNChangeLogCriteriaPresent_ReturnTrue()
        {
            //Arrange
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.CANCELCODE, "N");

            var changeLog = new ChangeLogManager(globals);

            //Act
            var exp = changeLog.ChangeLogCriteriaPresent;


            //Assert
            Assert.IsTrue(exp);
        }


        [TestMethod]
        public void ChangeLogManager_PassInchangeCodeCancelCodeNChangeCodes_ResultDoNotMatch()
        {
            //Arrange
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.INCHANGECODE, "1");
            globals.SetParmValue(WhereCriteria.CANCELCODE, "N");

            var act = "1,101";

            var changeLog = new ChangeLogManager(globals);
            changeLog.SetSqlProperties(false, "");

            //Act
            var exp = changeLog.ChangeCodes;


            //Assert
            Assert.AreNotEqual(exp, act);
        }

        [TestMethod]
        public void ChangeLogManager_PassInchangeCodeCancelCodeYChangeCode_ResultMatch()
        {
            //Arrange
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.INCHANGECODE, "1");
            globals.SetParmValue(WhereCriteria.CANCELCODE, "Y");

            var act = "1,101";

            var changeLog = new ChangeLogManager(globals);
            changeLog.SetSqlProperties(false, "");

            //Act
            var exp = changeLog.ChangeCodes;


            //Assert
            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void ChangeLogManager_PassChangeChangeStampCancelCodeY_WhereClauseResultMatch()
        {
            //Arrange
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.CHANGESTAMP, "DT:2016,9,1");
            globals.SetParmValue(WhereCriteria.CANCELCODE, "Y");
            
            var act = "T1.reckey = TCL.reckey and [] and TCL.changecode in (101) and TCL.changstamp >= '9/1/2016 12:00:00 AM'";

            var changeLog = new ChangeLogManager(globals);
            changeLog.SetSqlProperties(false, "[]");

            //Act
            var exp = changeLog.WhereClause;


            //Assert
            Assert.AreEqual(exp, act);
        }

    }
}
