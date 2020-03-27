using System;
using System.Collections.Generic;

using Domain.Helper;
using Domain.Orm.Classes;

using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Shared.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared.BuildWhereHelpers
{
    [TestClass]
    public class UdidWhereTests
    {
        [TestMethod]
        public void GetUdidWhere_InvalidUdid_DontModifyAnything()
        {
            var globals = new ReportGlobals();
            var where = new BuildWhere(new ClientFunctions());
            globals.SetParmValue(WhereCriteria.UDIDNBR, "foo");
            globals.SetParmValue(WhereCriteria.UDIDTEXT, "");
            globals.WhereText = "";
            where.WhereClauseUdid = "";
            globals.LanguageVariables = new List<LanguageVariableInfo>();
            var sut = new UdidWhere();

            sut.GetUdidWhere(globals, where, " in ");

            Assert.AreEqual("", globals.WhereText);
            Assert.AreEqual("", where.WhereClauseUdid);
        }

        [TestMethod]
        public void GetUdidWhere_ValidUdid_OneUdidTextExists()
        {
            var globals = new ReportGlobals();
            var where = new BuildWhere(new ClientFunctions());
            globals.SetParmValue(WhereCriteria.UDIDNBR, "1");
            globals.SetParmValue(WhereCriteria.UDIDTEXT, "SAMPLE TEXT");
            globals.WhereText = "";
            where.WhereClauseUdid = "";
            var sut = new UdidWhere();

            sut.GetUdidWhere(globals, where, " in ");

            Assert.AreEqual("Udid # = 1; Udid Text = SAMPLE TEXT", globals.WhereText);
            Assert.AreEqual("udidno = 1 AND udidText = 'SAMPLE TEXT'", where.WhereClauseUdid);
        }

        [TestMethod]
        public void GetUdidWhere_ValidUdid_MultipleUdidTextExists()
        {
            var globals = new ReportGlobals();
            var where = new BuildWhere(new ClientFunctions());
            globals.SetParmValue(WhereCriteria.UDIDNBR, "1");
            globals.SetParmValue(WhereCriteria.UDIDTEXT, "SAMPLE TEXT|ANOTHER TEXT");
            globals.WhereText = "";
            where.WhereClauseUdid = "";
            var sut = new UdidWhere();

            sut.GetUdidWhere(globals, where, " in ");

            Assert.AreEqual("Udid # = 1; Udid Text in SAMPLE TEXT|ANOTHER TEXT", globals.WhereText);
            Assert.AreEqual("udidno = 1 AND udidText In ('SAMPLE TEXT','ANOTHER TEXT')", where.WhereClauseUdid);
        }

        [TestMethod]
        public void GetUdidWhere_ValidUdid_NoUdidTextExists()
        {
            var globals = new ReportGlobals();
            var where = new BuildWhere(new ClientFunctions());
            globals.SetParmValue(WhereCriteria.UDIDNBR, "1");
            globals.SetParmValue(WhereCriteria.UDIDTEXT, "");
            globals.WhereText = "";
            where.WhereClauseUdid = "";
            var sut = new UdidWhere();

            sut.GetUdidWhere(globals, where, " in ");

            Assert.AreEqual("Udid # = 1; ", globals.WhereText);
            Assert.AreEqual("udidno = 1", where.WhereClauseUdid);
        }
    }
}
