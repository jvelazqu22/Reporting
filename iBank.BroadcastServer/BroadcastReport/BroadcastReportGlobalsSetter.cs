using Domain.Constants;
using Domain.Helper;
using Domain.Orm.iBankClientQueries;
using iBank.BroadcastServer.Utilities;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.ReportCriteriaHandlers;
using iBank.Services.Implementation.Utilities;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

namespace iBank.BroadcastServer.BroadcastReport
{
    public class BroadcastReportGlobalsSetter
    {
        private readonly IList<int> _xlsUnsupported = new List<int> { 34, 36, 23, 27, 25, 127, 136 };
        private readonly List<int> _alwaysHistory = new List<int> { 2, 6, 7, 31, 32, 35, 36 };
        private readonly List<int> _alwaysPreview = new List<int> { 82, 72, 8 };
        private readonly List<int> _neitherHistoryNorPreview = new List<int> { 245, 142, 144, 146, 504, 651, 7000, 7199 };

        public ReportGlobals Globals { get; set; }

        private IClientQueryable _clientQueryDb;

        private IClientQueryable ClientQueryDb
        {
            get
            {
                return _clientQueryDb.Clone() as IClientQueryable;
            }
            set
            {
                _clientQueryDb = value;
            }
        }

        private IMastersQueryable _masterQueryDb;

        private IMastersQueryable MasterQueryDb
        {
            get
            {
                return _masterQueryDb.Clone() as IMastersQueryable;
            }
            set
            {
                _masterQueryDb = value;
            }
        }

        public DateTime ReportPeriodStartDate { get; set; }
        public DateTime ReportPeriodEndDate { get; set; }

        public BroadcastReportGlobalsSetter(ReportGlobals globals, IMastersQueryable masterQueryDb, IClientQueryable clientQueryDb)
        {
            Globals = globals;
            MasterQueryDb = masterQueryDb;
            ClientQueryDb = clientQueryDb;
        }

        public void RemoveSingleQuotesFromAdvanceDateParameters(ReportGlobals globals)
        {
            foreach (var parameter in globals.AdvancedParameters.Parameters)
            {
                if (parameter.Type.Equals("DATE"))
                {
                    if (!string.IsNullOrWhiteSpace(parameter.Value1))
                    {
                        parameter.Value1 = string.Join(" ", parameter.Value1.Split(' ').Select(x => x.Trim('\'')));
                    }
                    if (!string.IsNullOrWhiteSpace(parameter.Value2))
                    {
                        parameter.Value2 = string.Join(" ", parameter.Value2.Split(' ').Select(x => x.Trim('\'')));
                    }
                }
            }
        }

        public void SetReportGlobalsForSavedReport(bcstque4 batch, BroadcastReportInformation report)
        {
            var savedRptRetriever = new SavedReportRetriever();
            var standardCritHandler = new StandardReportCritieraRetriever();
            var advancedCritHandler = new AdvancedParameterRetriever();
            var multiUdidHandler = new MultiUdidParameterRetriever();

            Globals.OutputDestination = string.Empty;
            Globals.OutputType = string.Empty;
            Globals.EProfileNumber = 0;
            Globals.SavedReportKey = report.SavedReportNumber;

            var whereCriteriaQuery = new GetAllWhereCriteriaQuery(MasterQueryDb);
            var whereCriteria = whereCriteriaQuery.ExecuteQuery();

            var savedRpt1 = savedRptRetriever.GetSavedReport1(ClientQueryDb, report.SavedReportNumber, batch.batchnum);

            Globals.ProcessKey = savedRpt1.processkey ?? 0;

            var reportCriteria = standardCritHandler.GetReportCriteriaFromSaved2(new GetAllSavedReport2ByRecordLinkQuery(ClientQueryDb, savedRpt1.recordnum));
            Globals.ReportParameters = standardCritHandler.GenerateStandardCriteria(whereCriteria, reportCriteria);

            var savedRpt3 = savedRptRetriever.GetSavedReport3(ClientQueryDb, savedRpt1.recordnum);

            Globals.AdvancedParameters.Parameters = advancedCritHandler.GetAdvancedParametersFromSavedReport3(savedRpt3, new GetActiveColumnsQuery(MasterQueryDb)).ToList();
            RemoveSingleQuotesFromAdvanceDateParameters(Globals);
            Globals.AdvancedParameters.AndOr = advancedCritHandler.GetAdvancedParametersAndOr(savedRpt3);

            var multiUdids = multiUdidHandler.GetMultiUdidParametersFromSavedReport3(savedRpt3);
            Globals.MultiUdidParameters.Parameters.AddRange(multiUdids);
            Globals.MultiUdidParameters.AndOr = multiUdidHandler.GetMultiUdidAndOr(savedRpt3);

            //* 11/11/2014 - when the bcst is run with the option "data updated since last time bcst ran" we do set taBatchPrg[6] with that of the saved report.
            //*  The value of taBatchPrg[6] in this situation is 91.  
            if (report.DateType != 91)
            {
                var dateType = Globals.GetParmValue(WhereCriteria.DATERANGE);
                int temp;
                if (int.TryParse(dateType, out temp))
                {
                    report.DateType = temp;
                }
            }
            else
            {
                if (report.ProcessKey == (int)ReportTitles.iXMLUserDefinedExport)
                {
                    Globals.SetParmValue(WhereCriteria.DATERANGE, "91");
                }
            }
        }

        public void SetReportGlobalsForStandardReport(bcstque4 batch, BroadcastReportInformation report)
        {
            Globals.SetParmValue(WhereCriteria.PROCESSID, report.ProcessKey.ToString(CultureInfo.InvariantCulture));

            Globals.OutputType = batch.outputtype;
            Globals.OutputDestination = batch.outputdest;
            Globals.SetParmValue(WhereCriteria.OUTPUTDEST, batch.outputdest);
            if ((Globals.OutputType.Equals("2") || Globals.OutputType.Equals("2")) && _xlsUnsupported.Contains(report.ProcessKey))
            {
                Globals.OutputType = "3";//default to PDF
            }
            if (report.ProcessKey == (int)ReportTitles.iXMLUserDefinedExport)
            {
                Globals.OutputType = "9";//xml
            }

            Globals.SetParmValue(WhereCriteria.OUTPUTTYPE, Globals.OutputType);
            if (_alwaysHistory.Contains(report.PrevHist))
            {
                Globals.SetParmValue(WhereCriteria.PREPOST, "2");
                if (report.ProcessKey == 4 || report.ProcessKey == 36)
                {
                    Globals.SetParmValue(WhereCriteria.PREPOSTAIR, "2");
                }
            }
            else
            {
                Globals.SetParmValue(WhereCriteria.PREPOST, "1");
            }

            if (_alwaysPreview.Contains(report.ProcessKey))
            {
                Globals.SetParmValue(WhereCriteria.PREPOST, "1");
            }

            if (!report.Usage.EqualsIgnoreCase("BOTH"))
            {
                Globals.SetParmValue(WhereCriteria.PREPOST, report.Usage.EqualsIgnoreCase("HISTORY") ? "2" : "1");
            }

            if (_neitherHistoryNorPreview.Contains(report.ProcessKey))
            {
                Globals.SetParmValue(WhereCriteria.PREPOST, string.Empty);
            }

            if (report.ProcessKey == (int)ReportTitles.TicketTrackerLogReport)
            {
                Globals.SetParmValue(WhereCriteria.DDGENERIC1, "5");
            }

            if (report.ProcessKey == (int)ReportTitles.OneScoreTravelScorecard)
            {
                Globals.SetParmValue(WhereCriteria.STARTMONTH, report.ReportEnd.Month.ToString());
                Globals.SetParmValue(WhereCriteria.STARTYEAR, report.ReportEnd.Year.ToString());
            }

            switch (report.ProcessKey)
            {
                case (int)ReportTitles.PassengersOnAPlane:
                case (int)ReportTitles.SameCity:
                    Globals.SetParmValue(WhereCriteria.HOWMANY, "5");
                    Globals.SetParmValue(WhereCriteria.NBRPASSENGERS, "5");
                    break;
                case (int)ReportTitles.CombinedUserDefinedReports:
                case (int)ReportTitles.TicketTrackerDetailReport:
                case (int)ReportTitles.TravelOptixReport:
                    Globals.SetParmValue(WhereCriteria.HOWMANY, string.Empty);
                    break;
                case (int)ReportTitles.AdvanceBookingAir:
                    Globals.SetParmValue(WhereCriteria.HOWMANY, string.Empty);
                    Globals.SetParmValue(WhereCriteria.NBRDAYSINADVANCE, string.Empty);
                    break;
                case (int)ReportTitles.TicketTrackerUnusedTickets:
                case (int)ReportTitles.TicketTrackerCustomerNotifications:
                case (int)ReportTitles.TicketTrackerLostValue:
                case (int)ReportTitles.TicketTrackerLogReport:
                case (int)ReportTitles.TicketTrackerEmailLog:
                    Globals.SetParmValue(WhereCriteria.HOWMANY, string.Empty);
                    break;
                case (int)ReportTitles.MissedHotelOpportunities:
                    Globals.SetParmValue(WhereCriteria.HOWMANY, "1");
                    Globals.SetParmValue(WhereCriteria.NBRDAYSDURATION, "1");
                    break;
                case (int)ReportTitles.TripDuration:
                    Globals.SetParmValue(WhereCriteria.NBRDAYSTRIPDUR, "1");
                    break;
                default:
                    Globals.SetParmValue(WhereCriteria.HOWMANY, "10");
                    Globals.SetParmValue(WhereCriteria.NBRDAYSINADVANCE, "10");
                    break;
            }

            Globals.SetParmValue(WhereCriteria.GROUPBY, "1");
            Globals.SetParmValue(WhereCriteria.SORTBY, "1");
            if (report.ProcessKey == (int)ReportTitles.ExecutiveSummaryWithGraphs)
            {
                Globals.SetParmValue(WhereCriteria.RBONGRAPHSSHOW, "1");
            }

            Globals.SetParmValue(WhereCriteria.RBGENERIC1, "1");
            Globals.SetParmValue(WhereCriteria.RBCITYPAIRCOMBINEORNOT, "1");
            Globals.SetParmValue(WhereCriteria.RBAPPLYTOLEGORSEG, "1");
            Globals.SetParmValue(WhereCriteria.RBSORTDESCASC, "1");
            Globals.SetParmValue(WhereCriteria.DDCONCURRENTSEGS, "1");

            if (report.ProcessKey == (int)ReportTitles.AdvanceBookingAir)
            {
                Globals.SetParmValue(WhereCriteria.RBINADVANCERECORDS, "1");
            }

            if (report.ProcessKey == (int)ReportTitles.MarketShare || report.ProcessKey == (int)ReportTitles.CarrierConcentration)
            {
                Globals.SetParmValue(WhereCriteria.RBONEWAYBOTHWAYS, "1");
            }

            if (report.ProcessKey == (int)ReportTitles.iXMLUserDefinedExport)
            {
                Globals.SetParmValue(WhereCriteria.DDPASSENGERXMLRECORD, "1");
            }

            if (report.ProcessKey == (int)ReportTitles.SpecialProductivityAir)
            {
                Globals.SetParmValue(WhereCriteria.DDSHOWCOUNTSORFARE, "1");
            }

            if (report.UdrKey != 0)
            {
                Globals.SetParmValue(WhereCriteria.UDRKEY, report.UdrKey.ToString());
            }

            //Date range type
            if (report.ProcessKey == (int)ReportTitles.TicketTrackerUnusedTickets
                || report.ProcessKey == (int)ReportTitles.TicketTrackerCustomerNotifications
                || report.ProcessKey == (int)ReportTitles.TicketTrackerDetailReport)
            {
                if (report.DateType == 2)
                {
                    Globals.SetParmValue(WhereCriteria.NBRTTEXPIREDAYS, "90");
                }
                else
                {
                    if (report.ProcessKey == (int)ReportTitles.TicketTrackerDetailReport)
                    {
                        Globals.SetParmValue(WhereCriteria.CBTTTKTSWITHOPENSEG, "ON");
                        Globals.SetParmValue(WhereCriteria.NBRTTDAYSBEFORETODAY, "1");
                        Globals.SetParmValue(WhereCriteria.DATERANGE, "0");
                    }
                    else
                    {
                        Globals.SetParmValue(WhereCriteria.DDGENERIC1, "A");
                    }
                }
            }
            else
            {
                if (report.DateType > 0)
                {
                    Globals.SetParmValue(WhereCriteria.DATERANGE, report.DateType.ToString());
                }
                else
                {
                    Globals.SetParmValue(WhereCriteria.DATERANGE, report.ProcessKey == 144 ? "8" : "1");
                }
            }

            if (report.ProcessKey == (int)ReportTitles.TopBottomCityPair)
            {
                Globals.SetParmValue(WhereCriteria.RBONEWAYBOTHWAYS, "1");
            }

            if (report.ProcessKey == (int)ReportTitles.ItineraryDetailCombined)
            {
                Globals.SetParmValue(WhereCriteria.DDAIRRAILCARHOTELOPTIONS, "ALL RECORDS");
            }

            if (report.ProcessKey == (int)ReportTitles.TicketTrackerUnusedTickets
                || report.ProcessKey == (int)ReportTitles.TicketTrackerCustomerNotifications)
            {
                Globals.SetParmValue(WhereCriteria.BEGDATE, string.Empty);
                Globals.SetParmValue(WhereCriteria.ENDDATE, string.Empty);
                Globals.SetParmValue(WhereCriteria.DATERANGE, "0");
            }

            Globals.SetParmValue(WhereCriteria.RPTTITLE2, batch.titleacct);
        }

        public void SetAccountValues(bcstque4 batch)
        {
            if (!string.IsNullOrEmpty(batch.acctlist.Trim()))
            {
                var notPrefix = batch.acctlist.Left(5);
                if (notPrefix.EqualsIgnoreCase("[NOT]"))
                {
                    Globals.SetParmValue(WhereCriteria.NOTINACCT, "ON");
                    batch.acctlist = batch.acctlist.Replace(notPrefix, "");
                }
                else
                {
                    Globals.SetParmValue(WhereCriteria.ACCT, string.Empty);
                }

                Globals.SetParmValue(WhereCriteria.INACCT, batch.acctlist.Trim());
            }
        }

        public void SetSpecialCasesForSpecificReports(bcstque4 batch, BroadcastReportInformation report)
        {
            ReportPeriodStartDate = report.ReportStart;
            ReportPeriodEndDate = report.ReportEnd;

            if (report.ProcessKey == (int)ReportTitles.ServiceFeeDetailByTransaction)
            {
                Globals.SetParmValue(WhereCriteria.DATERANGE, "8");
            }

            if (report.ProcessKey == (int)ReportTitles.PTAKeyPerformanceIndicators && !report.IsOfflineReport)
            {
                //TODO: this is not in the FP, but it seems to be needed as the global startmonth and startyear won't be set otherwise
                Globals.SetParmValue(WhereCriteria.STARTMONTH, report.ReportEnd.Month.MonthNameFromNumber());
                Globals.SetParmValue(WhereCriteria.STARTYEAR, report.ReportEnd.Year.ToString());
            }

            //Exec Sum YTY
            if (report.ProcessKey == (int)ReportTitles.ExecutiveSummaryYearToYear && !report.IsOfflineReport)
            {
                //FP comments state that Start Month should be the month of the End Date in this case. 
                Globals.SetParmValue(WhereCriteria.STARTMONTH, report.ReportEnd.Month.MonthNameFromNumber());
                Globals.SetParmValue(WhereCriteria.STARTYEAR, report.ReportStart.Year.ToString());
                if (Globals.ParmValueEquals(WhereCriteria.DDRPTOPTION, "2"))
                {
                    Globals.SetParmValue(WhereCriteria.STARTMONTH, report.ReportStart.Month.MonthNameFromNumber());
                }

                int FiscalYearStartMonth = string.IsNullOrEmpty(Globals.FyStartMonth) ? -1 : Helpers.GetMonthNum(Globals.FyStartMonth);
                if (FiscalYearStartMonth > 0 && FiscalYearStartMonth < 13)
                {
                    Globals.SetParmValue(WhereCriteria.TXTFYSTARTMTH, Helpers.GetMonthName(FiscalYearStartMonth));
                }
            }

            if (report.ProcessKey == (int)ReportTitles.TravelAuditReasonsByMonth && !report.IsOfflineReport)
            {
                var year = report.ReportStart.Year;
                Globals.SetParmValue(WhereCriteria.STARTYEAR, year.ToString(CultureInfo.InvariantCulture));
                if (year > 1900 && year < 2099)
                {
                    ReportPeriodStartDate = new DateTime(year, 01, 01);
                    ReportPeriodEndDate = new DateTime(year, 12, 31);
                }
            }

            if (report.ProcessKey == (int)ReportTitles.AccountSummaryAir12MonthTrend && !report.IsOfflineReport)
            {
                if (!Globals.ParmHasValue(WhereCriteria.STARTMONTH)) Globals.SetParmValue(WhereCriteria.STARTMONTH, "January");
                Globals.SetParmValue(WhereCriteria.STARTYEAR, report.ReportStart.Year.ToString(CultureInfo.InvariantCulture));
            }

            if (report.ProcessKey == (int)ReportTitles.TravelManagementSummary && !report.IsOfflineReport)
            {
                if (!Globals.ParmHasValue(WhereCriteria.TXTFYSTARTMTH)) Globals.SetParmValue(WhereCriteria.TXTFYSTARTMTH, "January");
                Globals.SetParmValue(WhereCriteria.STARTYEAR, report.ReportStart.Year.ToString(CultureInfo.InvariantCulture));

                int year;
                var yearTemp = Globals.GetParmValue(WhereCriteria.STARTYEAR);
                if (!int.TryParse(yearTemp, out year))
                {
                    throw new Exception("Invalid value for Start Year!");
                }
                //* 07/18/2011 - IF THE FISCAL MONTH IS GREATER THAN THE MONTH OF THE START DATE,
                //* THEN THE YEAR THAT WE PASS TO THE TRAVELMGMT.PRG IS ONE YEAR LESS.
                var fiscMonth = Helpers.GetMonthNum(Globals.GetParmValue(WhereCriteria.TXTFYSTARTMTH));

                if (fiscMonth > report.ReportStart.Month)
                {
                    year--;
                    Globals.SetParmValue(WhereCriteria.STARTYEAR, year.ToString(CultureInfo.InvariantCulture));
                }

                //Set the dates that will be used by the calling program
                ReportPeriodStartDate = new DateTime(year, fiscMonth, 1);
                var tempEndDate1 = ReportPeriodStartDate.AddYears(1).AddDays(-1);
                var tempEndDate2 = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
                ReportPeriodEndDate = tempEndDate1 < tempEndDate2 ? tempEndDate1 : tempEndDate2;
            }

            if (report.ProcessKey == (int)ReportTitles.TransactionSummary && !report.IsOfflineReport)
            {
                Globals.SetParmValue(WhereCriteria.STARTYEAR, report.ReportStart.Year.ToString(CultureInfo.InvariantCulture));
                Globals.SetParmValue(WhereCriteria.STARTMONTH, report.ReportStart.Month.MonthNameFromNumber());
                Globals.SetParmValue(WhereCriteria.ENDYEAR, report.ReportEnd.Year.ToString(CultureInfo.InvariantCulture));
                Globals.SetParmValue(WhereCriteria.ENDMONTH, report.ReportEnd.Month.MonthNameFromNumber());
            }

            Globals.BeginDate = ReportPeriodStartDate;
            Globals.EndDate = ReportPeriodEndDate;
        }

        public void SetMoneyType(int userNumber, bcstque4 batch)
        {
            var user = new GetUserByUserNumberQuery(ClientQueryDb, userNumber).ExecuteQuery();

            if (user != null)
            {
                var userMoneyType = !string.IsNullOrEmpty(user.ConvCurrTo.Trim())
                                            ? user.ConvCurrTo.Trim()
                                            : "";
                if (user.UserNumber != batch.rptusernum && batch.rptusernum > 0)
                {
                    //if running on behalf of another user always use that user's currency settings
                    Globals.SetParmValue(WhereCriteria.MONEYTYPE, userMoneyType);
                }
                else
                {
                    //if running as the broadcast owner
                    if (!Globals.ParmHasValue(WhereCriteria.MONEYTYPE))
                    {
                        //if the money type has not yet been set try to set it to the user's money type (the report is not a saved report)
                        Globals.SetParmValue(WhereCriteria.MONEYTYPE, userMoneyType);
                    }
                }
            }
        }

        public void SetOutputValues()
        {
            if (!Globals.ParmHasValue(WhereCriteria.OUTPUTTYPE)) Globals.SetParmValue(WhereCriteria.OUTPUTTYPE, OutputType.PORTABLE_DOC_FORMAT);

            if (!Globals.ParmHasValue(WhereCriteria.OUTPUTDEST)) Globals.SetParmValue(WhereCriteria.OUTPUTDEST, OutputType.CRYSTAL_REPORT);
        }
    }
}
