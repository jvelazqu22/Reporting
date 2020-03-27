using System;

using Domain.Helper;

using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Shared.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace iBank.UnitTesting.iBank.Services.Implementation.Shared.BuildWhereHelpers
{
    [TestClass]
    public class ChangesWhereTests
    {
        [TestMethod]
        public void AddBuildWhereChanges_AllPicklistItemsAreNotNumeric_DontModifyAnything_ReturnFalse()
        {
            var globals = new ReportGlobals();
            var where = new BuildWhere(new ClientFunctions());
            where.IncludeCancelled = false;
            where.WhereClauseChanges = "";
            globals.SetParmValue(WhereCriteria.CHANGECODE, "");
            globals.SetParmValue(WhereCriteria.INCHANGECODE, "1,2,foo");
            globals.SetParmValue(WhereCriteria.NOTINCHANGECODE, "OFF");
            globals.WhereText = "";
            var sut = new ChangesWhere();

            var output = sut.AddBuildWhereChanges(globals, where);

            Assert.AreEqual(false, output);
            Assert.AreEqual("", where.WhereClauseChanges);
            Assert.AreEqual(false, where.IncludeCancelled);
        }

        [TestMethod]
        public void AddBuildWhereChanges_NoItems_DontModifyAnything_ReturnTrue()
        {
            var globals = new ReportGlobals();
            var where = new BuildWhere(new ClientFunctions());
            globals.SetParmValue(WhereCriteria.CHANGECODE, "");
            globals.SetParmValue(WhereCriteria.INCHANGECODE, "");
            globals.SetParmValue(WhereCriteria.NOTINCHANGECODE, "OFF");
            where.IncludeCancelled = false;
            where.WhereClauseChanges = "";
            globals.WhereText = "";
            var sut = new ChangesWhere();

            var output = sut.AddBuildWhereChanges(globals, where);

            Assert.AreEqual(true, output);
            Assert.AreEqual("", where.WhereClauseChanges);
            Assert.AreEqual(false, where.IncludeCancelled);
            Assert.AreEqual("", globals.WhereText);
        }

        [TestMethod]
        public void AddBuildWhereChanges_OneItems_NoIsNotIn_IncludeCancelledEqualsFalse_ReturnTrue()
        {
            var globals = new ReportGlobals();
            var where = new BuildWhere(new ClientFunctions());
            globals.SetParmValue(WhereCriteria.CHANGECODE, "1");
            globals.SetParmValue(WhereCriteria.INCHANGECODE, "");
            globals.SetParmValue(WhereCriteria.NOTINCHANGECODE, "OFF");
            where.IncludeCancelled = false;
            where.WhereClauseChanges = "";
            globals.WhereText = "";
            var sut = new ChangesWhere();

            var output = sut.AddBuildWhereChanges(globals, where);

            Assert.AreEqual(true, output);
            Assert.AreEqual("changecode = '1'", where.WhereClauseChanges);
            Assert.AreEqual(false, where.IncludeCancelled);
            Assert.AreEqual("Change Code = 1; ", globals.WhereText);
        }

        [TestMethod]
        public void AddBuildWhereChanges_OneItems_WithIsNotIn_IncludeCancelledEqualsTrue_ReturnTrue()
        {
            var globals = new ReportGlobals();
            var where = new BuildWhere(new ClientFunctions());
            globals.SetParmValue(WhereCriteria.CHANGECODE, "1");
            globals.SetParmValue(WhereCriteria.INCHANGECODE, "");
            globals.SetParmValue(WhereCriteria.NOTINCHANGECODE, "ON");
            where.IncludeCancelled = false;
            where.WhereClauseChanges = "";
            globals.WhereText = "";
            var sut = new ChangesWhere();

            var output = sut.AddBuildWhereChanges(globals, where);

            Assert.AreEqual(true, output);
            Assert.AreEqual("changecode <> '1'", where.WhereClauseChanges);
            Assert.AreEqual(true, where.IncludeCancelled);
            Assert.AreEqual("Change Code <> 1; ", globals.WhereText);
        }

        [TestMethod]
        public void AddBuildWhereChanges_ListItems_NoIsNotIn_IncludeCancelledEqualsFalse_ReturnTrue()
        {
            var globals = new ReportGlobals();
            var where = new BuildWhere(new ClientFunctions());
            globals.SetParmValue(WhereCriteria.CHANGECODE, "");
            globals.SetParmValue(WhereCriteria.INCHANGECODE, "1,2");
            globals.SetParmValue(WhereCriteria.NOTINCHANGECODE, "OFF");
            where.IncludeCancelled = false;
            where.WhereClauseChanges = "";
            globals.WhereText = "";
            var sut = new ChangesWhere();

            var output = sut.AddBuildWhereChanges(globals, where);

            Assert.AreEqual(true, output);
            Assert.AreEqual("changecode In ('1','2')", where.WhereClauseChanges);
            Assert.AreEqual(false, where.IncludeCancelled);
            Assert.AreEqual("Change Code in  '1', '2'; ", globals.WhereText);
        }

        [TestMethod]
        public void AddBuildWhereChanges_ListItems_WithIsNotIn_IncludeCancelledEqualsTrue_ReturnTrue()
        {
            var globals = new ReportGlobals();
            var where = new BuildWhere(new ClientFunctions());
            globals.SetParmValue(WhereCriteria.CHANGECODE, "");
            globals.SetParmValue(WhereCriteria.INCHANGECODE, "1,2");
            globals.SetParmValue(WhereCriteria.NOTINCHANGECODE, "ON");
            where.IncludeCancelled = false;
            where.WhereClauseChanges = "";
            globals.WhereText = "";
            var sut = new ChangesWhere();

            var output = sut.AddBuildWhereChanges(globals, where);

            Assert.AreEqual(true, output);
            Assert.AreEqual("changecode Not In ('1','2')", where.WhereClauseChanges);
            Assert.AreEqual(true, where.IncludeCancelled);
            Assert.AreEqual("Change Code not in  '1', '2'; ", globals.WhereText);
        }

        [TestMethod]
        public void AddBuildWhereChanges_ListItems_NoIsNotIn_ListContains101_IncludeCancelledEqualsTrue_ReturnTrue()
        {
            var globals = new ReportGlobals();
            var where = new BuildWhere(new ClientFunctions());
            globals.SetParmValue(WhereCriteria.CHANGECODE, "");
            globals.SetParmValue(WhereCriteria.INCHANGECODE, "1,2,101");
            globals.SetParmValue(WhereCriteria.NOTINCHANGECODE, "OFF");
            where.IncludeCancelled = false;
            where.WhereClauseChanges = "";
            globals.WhereText = "";
            var sut = new ChangesWhere();

            var output = sut.AddBuildWhereChanges(globals, where);

            Assert.AreEqual(true, output);
            Assert.AreEqual("changecode In ('1','2','101')", where.WhereClauseChanges);
            Assert.AreEqual(true, where.IncludeCancelled);
            Assert.AreEqual("Change Code in  '1', '2', '101'; ", globals.WhereText);
        }

        [TestMethod]
        public void AddBuildWhereChanges_OneItem_NoIsNotIn_ItemEquals101_IncludeCancelledEqualsTrue_ReturnTrue()
        {
            var globals = new ReportGlobals();
            var where = new BuildWhere(new ClientFunctions());
            globals.SetParmValue(WhereCriteria.CHANGECODE, "101");
            globals.SetParmValue(WhereCriteria.INCHANGECODE, "");
            globals.SetParmValue(WhereCriteria.NOTINCHANGECODE, "OFF");
            where.IncludeCancelled = false;
            where.WhereClauseChanges = "";
            globals.WhereText = "";
            var sut = new ChangesWhere();

            var output = sut.AddBuildWhereChanges(globals, where);

            Assert.AreEqual(true, output);
            Assert.AreEqual("changecode = '101'", where.WhereClauseChanges);
            Assert.AreEqual(true, where.IncludeCancelled);
            Assert.AreEqual("Change Code = 101; ", globals.WhereText);
        }
    }
}
