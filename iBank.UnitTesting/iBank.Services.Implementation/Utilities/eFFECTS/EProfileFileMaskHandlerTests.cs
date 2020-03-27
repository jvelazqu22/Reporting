using Domain.Helper;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Utilities.eFFECTS;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Orm.iBankClientQueries;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.UnitTesting.TestingHelpers.MockHelpers;

namespace iBank.UnitTesting.iBank.Services.Implementation.Utilities.eFFECTS
{
    [TestClass]
    public class EProfileFileMaskHandlerTests
    {
        [TestMethod]
        public void ApplyMask_FIXEDVariable_ReturnTextAfterFIXED()
        {
            var handler = new EProfileFileMaskHandler();
            var eprofileInfo = new EffectsOutputInformation { Outbox = @"e:\outbox", FileNameMask = "FIXED:foo" };

            var output = handler.ApplyMask(null, eprofileInfo, null, 0, null, null);

            Assert.AreEqual(@"e:\outbox\foo", output);
        }

        [TestMethod]
        public void ApplyMask_DATYVariableReservationReport_ReplaceWithIb()
        {
            var handler = new EProfileFileMaskHandler();
            var eprofileInfo = new EffectsOutputInformation { Outbox = @"e:\outbox", FileNameMask = "foo$DATY" };
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.PREPOST, "1");

            var output = handler.ApplyMask(globals, eprofileInfo, null, 0, null, null);

            Assert.AreEqual(@"e:\outbox\fooib", output);
        }

        [TestMethod]
        public void ApplyMask_DATYVariableBackOfficeReport_ReplaceWithHib()
        {
            var handler = new EProfileFileMaskHandler();
            var eprofileInfo = new EffectsOutputInformation { Outbox = @"e:\outbox", FileNameMask = "foo$DATY" };
            var globals = new ReportGlobals();
            globals.SetParmValue(WhereCriteria.PREPOST, "2");

            var output = handler.ApplyMask(globals, eprofileInfo, null, 0, null, null);

            Assert.AreEqual(@"e:\outbox\foohib", output);
        }

        [TestMethod]
        public void ApplyMask_ENTYVariable_ReplaceWithAgency()
        {
            var handler = new EProfileFileMaskHandler();
            var eprofileInfo = new EffectsOutputInformation { Outbox = @"e:\outbox", FileNameMask = "foo$ENTY" };
            var globals = new ReportGlobals { Agency = "agency" };

            var output = handler.ApplyMask(globals, eprofileInfo, null, 0, null, null);

            Assert.AreEqual(@"e:\outbox\fooagency", output);
        }

        [TestMethod]
        public void ApplyMask_USERVariable_ReplaceWithUserId()
        {
            var handler = new EProfileFileMaskHandler();
            var eprofileInfo = new EffectsOutputInformation { Outbox = @"e:\outbox", FileNameMask = "foo$USER" };
            var globals = new ReportGlobals { User = new UserInformation { UserId = "userid" } };

            var output = handler.ApplyMask(globals, eprofileInfo, null, 0, null, null);

            Assert.AreEqual(@"e:\outbox\foouserid", output);
        }

        [TestMethod]
        public void ApplyMask_EPRFVariable_ReplaceWithProfileName()
        {
            var handler = new EProfileFileMaskHandler();
            var eprofileInfo = new EffectsOutputInformation { Outbox = @"e:\outbox", FileNameMask = "foo$EPRF", ProfileName = "bar" };
            var globals = new ReportGlobals();
            
            var output = handler.ApplyMask(globals, eprofileInfo, null, 0, null, null);

            Assert.AreEqual(@"e:\outbox\foobar", output);
        }

        [TestMethod]
        public void ApplyMask_TRDGVariable_ReplaceWithTradingPartnerName()
        {
            var handler = new EProfileFileMaskHandler();
            var eprofileInfo = new EffectsOutputInformation { Outbox = @"e:\outbox", FileNameMask = "foo$TRDG", TradingPartnerName = "bar" };
            var globals = new ReportGlobals();

            var output = handler.ApplyMask(globals, eprofileInfo, null, 0, null, null);

            Assert.AreEqual(@"e:\outbox\foobar", output);
        }

        [TestMethod]
        public void ApplyMask_TMPLVariableXmlReportNotNull_ReplaceWithCrName()
        {
            var handler = new EProfileFileMaskHandler();
            var eprofileInfo = new EffectsOutputInformation { Outbox = @"e:\outbox", FileNameMask = "foo$TMPL" };
            var globals = new ReportGlobals();
            var xml = new XmlReport("crName", "");

            var output = handler.ApplyMask(globals, eprofileInfo, xml, 0, null, null);

            Assert.AreEqual(@"e:\outbox\foocrName", output);
        }

        [TestMethod]
        public void ApplyMask_TMPLVariableXmlReportNull_ReplaceWithEmptyString()
        {
            var handler = new EProfileFileMaskHandler();
            var eprofileInfo = new EffectsOutputInformation { Outbox = @"e:\outbox", FileNameMask = "foo$TMPL" };
            var globals = new ReportGlobals();

            var output = handler.ApplyMask(globals, eprofileInfo, null, 0, null, null);

            Assert.AreEqual(@"e:\outbox\foo", output);
        }

        [TestMethod]
        public void ApplyMask_STYLEVariableStyleGroupNumberGreaterThanZeroStyleRecordExists_ReplaceWithStyleGroupName()
        {
            var handler = new EProfileFileMaskHandler();
            var eprofileInfo = new EffectsOutputInformation { Outbox = @"e:\outbox", FileNameMask = "foo$STYLE" };
            var data = new StyleGroup { SGroupNbr = 1, SGroupName = "bar", ClientCode = "agency" };
            var mockSet = new Mock<IQueryable<StyleGroup>>();
            mockSet.SetupIQueryable(new List<StyleGroup> { data }.AsQueryable());
            var mockDb = new Mock<IClientQueryable>();
            mockDb.Setup(x => x.StyleGroup).Returns(mockSet.Object);
            var query = new GetStyleGroupByNumberAndClientCodeQuery(mockDb.Object, 1, "agency");
            
            var output = handler.ApplyMask(null, eprofileInfo, null, 1, null, query);

            Assert.AreEqual(@"e:\outbox\foobar", output);
        }

        [TestMethod]
        public void ApplyMask_STYLEVariableStyleGroupNumberGreaterThanZeroStyleRecordDoesNotExists_ReplaceWithEmptyString()
        {
            var handler = new EProfileFileMaskHandler();
            var eprofileInfo = new EffectsOutputInformation { Outbox = @"e:\outbox", FileNameMask = "foo$STYLE" };
            var data = new StyleGroup { SGroupNbr = 1, SGroupName = "bar", ClientCode = "agency" };
            var mockSet = new Mock<IQueryable<StyleGroup>>();
            mockSet.SetupIQueryable(new List<StyleGroup> { data }.AsQueryable());
            var mockDb = new Mock<IClientQueryable>();
            mockDb.Setup(x => x.StyleGroup).Returns(mockSet.Object);
            var query = new GetStyleGroupByNumberAndClientCodeQuery(mockDb.Object, 5, "agency");

            var output = handler.ApplyMask(null, eprofileInfo, null, 5, null, query);

            Assert.AreEqual(@"e:\outbox\foo", output);
        }

        [TestMethod]
        public void ApplyMask_STYLEVariableStyleGroupNumberLessThanZero_ReplaceWithEmptyString()
        {
            var handler = new EProfileFileMaskHandler();
            var eprofileInfo = new EffectsOutputInformation { Outbox = @"e:\outbox", FileNameMask = "foo$STYLE" };
            var data = new StyleGroup { SGroupNbr = 1, SGroupName = "bar", ClientCode = "agency" };
            var mockSet = new Mock<IQueryable<StyleGroup>>();
            mockSet.SetupIQueryable(new List<StyleGroup> { data }.AsQueryable());
            var mockDb = new Mock<IClientQueryable>();
            mockDb.Setup(x => x.StyleGroup).Returns(mockSet.Object);
            var query = new GetStyleGroupByNumberAndClientCodeQuery(mockDb.Object, -1, "agency");

            var output = handler.ApplyMask(null, eprofileInfo, null, -1, null, query);

            Assert.AreEqual(@"e:\outbox\foo", output);
        }

        [TestMethod]
        public void ApplyMask_DATEVariable_ReplaceWithYearMonthDay()
        {
            var handler = new EProfileFileMaskHandler();
            var eprofileInfo = new EffectsOutputInformation { Outbox = @"e:\outbox", FileNameMask = "foo$DATE" };
            var globals = new ReportGlobals();
            var time = new TimeStrings(new DateTime(2016, 1, 2, 3, 4, 5));

            var output = handler.ApplyMask(globals, eprofileInfo, null, 0, time, null);

            Assert.AreEqual(@"e:\outbox\foo20160102", output);
        }

        [TestMethod]
        public void ApplyMask_TIMEVariable_ReplaceWithHourMinSecMs()
        {
            var handler = new EProfileFileMaskHandler();
            var eprofileInfo = new EffectsOutputInformation { Outbox = @"e:\outbox", FileNameMask = "foo$TIME" };
            var globals = new ReportGlobals();
            var time = new TimeStrings(new DateTime(2016, 1, 2, 3, 4, 5, 123));

            var output = handler.ApplyMask(globals, eprofileInfo, null, 0, time, null);

            Assert.AreEqual(@"e:\outbox\foo030405123", output);
        }

        [TestMethod]
        public void NeedToApplyMask_DATYVariable_ReturnTrue()
        {
            var handler = new EProfileFileMaskHandler();
            var fileNameMask = "foo$DATYbar";

            var output = handler.NeedToApplyMask(fileNameMask);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void NeedToApplyMask_ENTYVariable_ReturnTrue()
        {
            var handler = new EProfileFileMaskHandler();
            var fileNameMask = "foo$ENTYbar";

            var output = handler.NeedToApplyMask(fileNameMask);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void NeedToApplyMask_USERVariable_ReturnTrue()
        {
            var handler = new EProfileFileMaskHandler();
            var fileNameMask = "foo$USERbar";

            var output = handler.NeedToApplyMask(fileNameMask);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void NeedToApplyMask_EPRFVariable_ReturnTrue()
        {
            var handler = new EProfileFileMaskHandler();
            var fileNameMask = "foo$EPRFbar";

            var output = handler.NeedToApplyMask(fileNameMask);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void NeedToApplyMask_TRDGVariable_ReturnTrue()
        {
            var handler = new EProfileFileMaskHandler();
            var fileNameMask = "foo$TRDGbar";

            var output = handler.NeedToApplyMask(fileNameMask);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void NeedToApplyMask_TMPLVariable_ReturnTrue()
        {
            var handler = new EProfileFileMaskHandler();
            var fileNameMask = "foo$TMPLbar";

            var output = handler.NeedToApplyMask(fileNameMask);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void NeedToApplyMask_STYLEVariable_ReturnTrue()
        {
            var handler = new EProfileFileMaskHandler();
            var fileNameMask = "foo$STYLEbar";

            var output = handler.NeedToApplyMask(fileNameMask);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void NeedToApplyMask_DATEVariable_ReturnTrue()
        {
            var handler = new EProfileFileMaskHandler();
            var fileNameMask = "foo$DATEbar";

            var output = handler.NeedToApplyMask(fileNameMask);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void NeedToApplyMask_TIMEVariable_ReturnTrue()
        {
            var handler = new EProfileFileMaskHandler();
            var fileNameMask = "foo$TIMEbar";

            var output = handler.NeedToApplyMask(fileNameMask);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void NeedToApplyMask_FIXEDVariable_ReturnTrue()
        {
            var handler = new EProfileFileMaskHandler();
            var fileNameMask = "FIXED:barfoo";

            var output = handler.NeedToApplyMask(fileNameMask);

            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void NeedToApplyMask_FIXEDVariableInMiddleOfString_ReturnFalse()
        {
            var handler = new EProfileFileMaskHandler();
            var fileNameMask = "fooFIXED:barfoo";

            var output = handler.NeedToApplyMask(fileNameMask);

            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void NeedToApplyMask_NoVariableContained_ReturnFalse()
        {
            var handler = new EProfileFileMaskHandler();
            var fileNameMask = "fobar";

            var output = handler.NeedToApplyMask(fileNameMask);

            Assert.AreEqual(false, output);
        }
        
    }
}
