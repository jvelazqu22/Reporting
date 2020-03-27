using Domain.Helper;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Interfaces;
using iBank.Services.Implementation.Shared;

namespace iBank.UnitTesting.iBank.Services.Implementation.Utilities
{
    [TestClass]
    public class ReportRunConditionalsTests
    {
        [TestMethod]
        public void IsBeginDateSupplied_BeginDateSupplied_ReturnTrueAndNoErrorMessage()
        {
            var conditionals = new ReportRunConditionals();
            var globals = new ReportGlobals
                              {
                                  BeginDate = new DateTime(2016, 1, 1),
                                  ReportInformation = { ReturnCode = -1,
                                                        ErrorMessage = ""
                                                      }
                              };

            var output = conditionals.IsBeginDateSupplied(globals);

            Assert.AreEqual(true, output);
            Assert.AreEqual(-1, globals.ReportInformation.ReturnCode);
            Assert.AreEqual("", globals.ReportInformation.ErrorMessage);
        }

        [TestMethod]
        public void IsBeginDateSupplied_BeginDateNull_ReturnFalseModifyErrorMessageAndReturnCode()
        {
            var conditionals = new ReportRunConditionals();
            var globals = new ReportGlobals
            {
                BeginDate = null,
                ReportInformation = { ReturnCode = -1,
                                      ErrorMessage = ""
                                    }
            };

            var output = conditionals.IsBeginDateSupplied(globals);

            Assert.AreEqual(false, output);
            Assert.AreEqual(2, globals.ReportInformation.ReturnCode);
            Assert.AreEqual(globals.ReportMessages.RptMsg_DateRange, globals.ReportInformation.ErrorMessage);
        }

        [TestMethod]
        public void IsUdidNumberSuppliedWithUdidText_BothAreSupplied_ReturnTrueNoErrorMessage()
        {
            var conditionals = new ReportRunConditionals();
            var globals = new ReportGlobals
                              {
                                  ReportInformation =
                                      {
                                          ReturnCode = -1,
                                          ErrorMessage = ""
                                      }
                              };
            globals.SetParmValue(WhereCriteria.UDIDTEXT, "foo");
            globals.SetParmValue(WhereCriteria.UDIDNBR, "1");

            var output = conditionals.IsUdidNumberSuppliedWithUdidText(globals);

            Assert.AreEqual(true, output);
            Assert.AreEqual(-1, globals.ReportInformation.ReturnCode);
            Assert.AreEqual("", globals.ReportInformation.ErrorMessage);
        }


        [TestMethod]
        public void IsUdidNumberSuppliedWithUdidText_NoUdidNumberWithUdidText_ReturnFalseModifyErrorMessageAndReturnCode()
        {
            var conditionals = new ReportRunConditionals();
            var globals = new ReportGlobals
            {
                ReportInformation =
                                      {
                                          ReturnCode = -1,
                                          ErrorMessage = ""
                                      }
            };
            globals.SetParmValue(WhereCriteria.UDIDTEXT, "foo");
            globals.SetParmValue(WhereCriteria.UDIDNBR, "");

            var output = conditionals.IsUdidNumberSuppliedWithUdidText(globals);

            Assert.AreEqual(false, output);
            Assert.AreEqual(2, globals.ReportInformation.ReturnCode);
            Assert.AreEqual(globals.ReportMessages.RptMsg_UDIDNbrReqd, globals.ReportInformation.ErrorMessage);
        }

        [TestMethod]
        public void IsUdidNumberSupplied_UdidSupplied_ReturnTrueNoErrorMessage()
        {
            var conditionals = new ReportRunConditionals();
            var globals = new ReportGlobals
            {
                ReportInformation =
                                      {
                                          ReturnCode = -1,
                                          ErrorMessage = ""
                                      }
            };
            globals.SetParmValue(WhereCriteria.UDIDNBR, "1");

            var output = conditionals.IsUdidNumberSupplied(globals);

            Assert.AreEqual(true, output);
            Assert.AreEqual(-1, globals.ReportInformation.ReturnCode);
            Assert.AreEqual("", globals.ReportInformation.ErrorMessage);
        }
        

        [TestMethod]
        public void IsDateRangeValid_IsValid_ReturnTrueNoErrorMessage()
        {
            var conditionals = new ReportRunConditionals();
            var globals = new ReportGlobals
            {
                BeginDate = new DateTime(2016, 1, 1),
                EndDate = new DateTime(2016, 2, 1),
                ReportInformation =
                                      {
                                          ReturnCode = -1,
                                          ErrorMessage = ""
                                      }
            };

            var output = conditionals.IsDateRangeValid(globals);

            Assert.AreEqual(true, output);
            Assert.AreEqual(-1, globals.ReportInformation.ReturnCode);
            Assert.AreEqual("", globals.ReportInformation.ErrorMessage);
        }

        [TestMethod]
        public void IsDateRangeValid_BothDatesSuppliedButEndDatePriorToBeginDate_ReturnFalseWithErrorMessageAndReturnCode()
        {
            var conditionals = new ReportRunConditionals();
            var globals = new ReportGlobals
            {
                BeginDate = new DateTime(2016, 2, 1),
                EndDate = new DateTime(2016, 1, 1),
                ReportInformation =
                                      {
                                          ReturnCode = -1,
                                          ErrorMessage = ""
                                      }
            };

            var output = conditionals.IsDateRangeValid(globals);

            Assert.AreEqual(false, output);
            Assert.AreEqual(2, globals.ReportInformation.ReturnCode);
            Assert.AreEqual(globals.ReportMessages.RptMsg_DateRange, globals.ReportInformation.ErrorMessage);
        }

        [TestMethod]
        public void IsDateRangeValid_BeginDateNotSupplied_ReturnFalseWithErrorMessageAndReturnCode()
        {
            var conditionals = new ReportRunConditionals();
            var globals = new ReportGlobals
            {
                BeginDate = null,
                EndDate = new DateTime(2016, 2, 1),
                ReportInformation =
                                      {
                                          ReturnCode = -1,
                                          ErrorMessage = ""
                                      }
            };

            var output = conditionals.IsDateRangeValid(globals);

            Assert.AreEqual(false, output);
            Assert.AreEqual(2, globals.ReportInformation.ReturnCode);
            Assert.AreEqual(globals.ReportMessages.RptMsg_DateRange, globals.ReportInformation.ErrorMessage);
        }

        [TestMethod]
        public void IsDateRangeValid_EndDateNotSupplied_ReturnFalseWithErrorMessageAndReturnCode()
        {
            var conditionals = new ReportRunConditionals();
            var globals = new ReportGlobals
            {
                BeginDate = new DateTime(2016, 2, 1),
                EndDate = null,
                ReportInformation =
                                      {
                                          ReturnCode = -1,
                                          ErrorMessage = ""
                                      }
            };

            var output = conditionals.IsDateRangeValid(globals);

            Assert.AreEqual(false, output);
            Assert.AreEqual(2, globals.ReportInformation.ReturnCode);
            Assert.AreEqual(globals.ReportMessages.RptMsg_DateRange, globals.ReportInformation.ErrorMessage);
        }

        [TestMethod]
        public void IsDateRangeUnderThreeMonths_IsUnderThreeMonths_ReturnTrueNoErrorMessage()
        {
            var conditionals = new ReportRunConditionals();
            var globals = new ReportGlobals
            {
                BeginDate = new DateTime(2016, 1, 1),
                EndDate = new DateTime(2016, 2, 1),
                ReportInformation =
                                      {
                                          ReturnCode = -1,
                                          ErrorMessage = ""
                                      }
            };

            var pusher = new Mock<IReportDelayer>();
            var output = conditionals.IsDateRangeUnderThreeMonths(globals, pusher.Object);

            pusher.Verify(x => x.PushReportOffline(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            Assert.AreEqual(true, output);
            Assert.AreEqual(-1, globals.ReportInformation.ReturnCode);
            Assert.AreEqual("", globals.ReportInformation.ErrorMessage);
        }

        [TestMethod]
        public void IsDateRangeUnderThreeMonths_IsOverThreeMonths_ReturnFalseWithErrorMessageAndReturnCodeAndPushesOffline()
        {
            var conditionals = new ReportRunConditionals();
            var globals = new ReportGlobals
            {
                BeginDate = new DateTime(2016, 1, 1),
                EndDate = new DateTime(2016, 8, 1),
                ReportInformation =
                                      {
                                          ReturnCode = -1,
                                          ErrorMessage = ""
                                      }
            };

            var pusher = new Mock<IReportDelayer>();
            var output = conditionals.IsDateRangeUnderThreeMonths(globals, pusher.Object);

            pusher.Verify(x => x.PushReportOffline(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void IsDateRangeUnderThreeMonths_BeginDateIsNull_ReturnFalseWithErrorMessageAndReturnCodeDoesNotPushOffline()
        {
            var conditionals = new ReportRunConditionals();
            var globals = new ReportGlobals
            {
                BeginDate = null,
                EndDate = new DateTime(2016, 8, 1),
                ReportInformation =
                                      {
                                          ReturnCode = -1,
                                          ErrorMessage = ""
                                      }
            };

            var pusher = new Mock<IReportDelayer>();
            var output = conditionals.IsDateRangeUnderThreeMonths(globals, pusher.Object);

            pusher.Verify(x => x.PushReportOffline(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            Assert.AreEqual(false, output);
            Assert.AreEqual(2, globals.ReportInformation.ReturnCode);
            Assert.AreEqual(globals.ReportMessages.RptMsg_DateRange, globals.ReportInformation.ErrorMessage);
        }

        [TestMethod]
        public void IsGoodDateRangeReportTypeCombo_ReservationReport_ReturnTrue()
        {
            var conditionals = new ReportRunConditionals();
            var globals = new ReportGlobals
            {
                ReportInformation =
                                      {
                                          ReturnCode = -1,
                                          ErrorMessage = ""
                                      }
            };
            globals.SetParmValue(WhereCriteria.PREPOST, "1");
            globals.SetParmValue(WhereCriteria.DATERANGE, "3");

            var output = conditionals.IsGoodDateRangeReportTypeCombo(globals);
            
            Assert.AreEqual(true, output);
            Assert.AreEqual(-1, globals.ReportInformation.ReturnCode);
            Assert.AreEqual("", globals.ReportInformation.ErrorMessage);
        }

        [TestMethod]
        public void IsGoodDateRangeReportTypeCombo_HistoryReportNotBookedDate_ReturnTrue()
        {
            var conditionals = new ReportRunConditionals();
            var globals = new ReportGlobals
            {
                ReportInformation =
                                      {
                                          ReturnCode = -1,
                                          ErrorMessage = ""
                                      }
            };
            globals.SetParmValue(WhereCriteria.PREPOST, "2");
            globals.SetParmValue(WhereCriteria.DATERANGE, "2");

            var output = conditionals.IsGoodDateRangeReportTypeCombo(globals);

            Assert.AreEqual(true, output);
            Assert.AreEqual(-1, globals.ReportInformation.ReturnCode);
            Assert.AreEqual("", globals.ReportInformation.ErrorMessage);
        }

        [TestMethod]
        public void IsGoodDateRangeReportTypeCombo_HistoryReportIsBookedDate_ReturnFalseWithNewErrorMessageAndReturnCode()
        {
            var conditionals = new ReportRunConditionals();
            var globals = new ReportGlobals
            {
                ReportInformation =
                                      {
                                          ReturnCode = -1,
                                          ErrorMessage = ""
                                      }
            };
            globals.SetParmValue(WhereCriteria.PREPOST, "2");
            globals.SetParmValue(WhereCriteria.DATERANGE, "3");

            var output = conditionals.IsGoodDateRangeReportTypeCombo(globals);

            Assert.AreEqual(false, output);
            Assert.AreEqual(2, globals.ReportInformation.ReturnCode);
            Assert.AreEqual(globals.ReportMessages.ErrorBadCombo, globals.ReportInformation.ErrorMessage);
        }

        [TestMethod]
        public void IsOnlineReport_IsOnlineReportAndOnlineServer_ReturnTrue()
        {
            var conditionals = new ReportRunConditionals();
            var globals = new ReportGlobals
            {
                ReportInformation =
                                      {
                                          ReturnCode = -1,
                                          ErrorMessage = ""
                                      }
            };
            globals.SetParmValue(WhereCriteria.RUNOFFLINE, "NO");
            globals.IsOfflineServer = false;
            
            var pusher = new Mock<IReportDelayer>();
            var output = conditionals.IsOnlineReport(globals, pusher.Object);

            pusher.Verify(x => x.PushReportOffline(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsOnlineReport_IsOfflineReportAndOnlineServer_ReturnFalseAndPushOffline()
        {
            var conditionals = new ReportRunConditionals();
            var globals = new ReportGlobals
            {
                ReportInformation =
                                      {
                                          ReturnCode = -1,
                                          ErrorMessage = ""
                                      }
            };
            globals.SetParmValue(WhereCriteria.RUNOFFLINE, "YES");
            globals.IsOfflineServer = false;

            var pusher = new Mock<IReportDelayer>();
            var output = conditionals.IsOnlineReport(globals, pusher.Object);

            pusher.Verify(x => x.PushReportOffline(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void IsOnlineReport_IsOfflineReportAndOfflineServer_ReturnTrue()
        {
            var conditionals = new ReportRunConditionals();
            var globals = new ReportGlobals
            {
                ReportInformation =
                                      {
                                          ReturnCode = -1,
                                          ErrorMessage = ""
                                      }
            };
            globals.SetParmValue(WhereCriteria.RUNOFFLINE, "YES");
            globals.IsOfflineServer = true;

            var pusher = new Mock<IReportDelayer>();
            var output = conditionals.IsOnlineReport(globals, pusher.Object);

            pusher.Verify(x => x.PushReportOffline(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void HasAccount_SingleAccountExists_ReturnTrue()
        {
            var conditionals = new ReportRunConditionals();
            var globals = new ReportGlobals
            {
                ReportInformation =
                                      {
                                          ReturnCode = -1,
                                          ErrorMessage = ""
                                      }
            };
            globals.SetParmValue(WhereCriteria.ACCT, "foo");
            globals.SetParmValue(WhereCriteria.INACCT, "");
            globals.IsOfflineServer = false;

            var pusher = new Mock<IReportDelayer>();
            var output = conditionals.HasAccount(globals, pusher.Object);

            pusher.Verify(x => x.PushReportOffline(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void HasAccount_AccountsInListExists_ReturnTrue()
        {
            var conditionals = new ReportRunConditionals();
            var globals = new ReportGlobals
            {
                ReportInformation =
                                      {
                                          ReturnCode = -1,
                                          ErrorMessage = ""
                                      }
            };
            globals.SetParmValue(WhereCriteria.ACCT, "");
            globals.SetParmValue(WhereCriteria.INACCT, "foo");
            globals.IsOfflineServer = false;

            var pusher = new Mock<IReportDelayer>();
            var output = conditionals.HasAccount(globals, pusher.Object);

            pusher.Verify(x => x.PushReportOffline(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void HasAccount_NoAccountsOfflineServer_ReturnTrue()
        {
            var conditionals = new ReportRunConditionals();
            var globals = new ReportGlobals
            {
                ReportInformation =
                                      {
                                          ReturnCode = -1,
                                          ErrorMessage = ""
                                      }
            };
            globals.SetParmValue(WhereCriteria.ACCT, "");
            globals.SetParmValue(WhereCriteria.INACCT, "foo");
            globals.IsOfflineServer = true;

            var pusher = new Mock<IReportDelayer>();
            var output = conditionals.HasAccount(globals, pusher.Object);

            pusher.Verify(x => x.PushReportOffline(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void HasAccount_NoAccountsOnlineServer_ReturnFalseAndPushOffline()
        {
            var conditionals = new ReportRunConditionals();
            var globals = new ReportGlobals
            {
                ReportInformation =
                                      {
                                          ReturnCode = -1,
                                          ErrorMessage = ""
                                      }
            };
            globals.SetParmValue(WhereCriteria.ACCT, "");
            globals.SetParmValue(WhereCriteria.INACCT, "");
            globals.IsOfflineServer = false;

            var pusher = new Mock<IReportDelayer>();
            var output = conditionals.HasAccount(globals, pusher.Object);

            pusher.Verify(x => x.PushReportOffline(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            Assert.AreEqual(false, output);
        }

        [TestMethod]
        public void IsUnderOfflineThreshold_OnlineServerAndRecordCountUnderLimit_ReturnTrue()
        {
            var conditionals = new ReportRunConditionals();
            var globals = new ReportGlobals
            {
                ReportInformation =
                                      {
                                          ReturnCode = -1,
                                          ErrorMessage = ""
                                      }
            };
            globals.RecordLimit = 1000;
            var recordCount = 5;
            globals.IsOfflineServer = false;

            var pusher = new Mock<IReportDelayer>();
            var output = conditionals.IsUnderOfflineThreshold(recordCount, globals, pusher.Object);

            pusher.Verify(x => x.PushReportOffline(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsUnderOfflineThreshold_OnlineServerNoRecordLimit_ReturnTrue()
        {
            var conditionals = new ReportRunConditionals();
            var globals = new ReportGlobals
            {
                ReportInformation =
                                      {
                                          ReturnCode = -1,
                                          ErrorMessage = ""
                                      }
            };
            globals.RecordLimit = -1;
            var recordCount = 5;
            globals.IsOfflineServer = false;

            var pusher = new Mock<IReportDelayer>();
            var output = conditionals.IsUnderOfflineThreshold(recordCount, globals, pusher.Object);

            pusher.Verify(x => x.PushReportOffline(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsUnderOfflineThreshold_OfflineServerOverRecordLimit_ReturnTrue()
        {
            var conditionals = new ReportRunConditionals();
            var globals = new ReportGlobals
            {
                ReportInformation =
                                      {
                                          ReturnCode = -1,
                                          ErrorMessage = ""
                                      }
            };
            globals.RecordLimit = 1000;
            var recordCount = 5000;
            globals.IsOfflineServer = true;

            var pusher = new Mock<IReportDelayer>();
            var output = conditionals.IsUnderOfflineThreshold(recordCount, globals, pusher.Object);

            pusher.Verify(x => x.PushReportOffline(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            Assert.AreEqual(true, output);
        }

        [TestMethod]
        public void IsUnderOfflineThreshold_OnlineServerOverRecordLimit_ReturnTrue()
        {
            var conditionals = new ReportRunConditionals();
            var globals = new ReportGlobals
            {
                ReportInformation =
                                      {
                                          ReturnCode = -1,
                                          ErrorMessage = ""
                                      }
            };
            globals.RecordLimit = 1000;
            var recordCount = 5000;
            globals.IsOfflineServer = false;

            var pusher = new Mock<IReportDelayer>();
            var output = conditionals.IsUnderOfflineThreshold(recordCount, globals, pusher.Object);

            pusher.Verify(x => x.PushReportOffline(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            Assert.AreEqual(false, output);
        }
    }
}
