using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using Domain.Exceptions;
using Domain.Helper;

using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.Interfaces;
using iBank.Server.Utilities.Logging;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Utilities.CurrencyConversion;

namespace iBank.Services.Implementation.Utilities
{
    public class ReportRunConditionals
    {
        private string _serverNumber = ConfigurationManager.AppSettings["ServerNumber"];
        public bool IsBeginDateSupplied(ReportGlobals globals)
        {
            if (globals.BeginDate == null)
            {
                globals.ReportInformation.ReturnCode = 2;
                globals.ReportInformation.ErrorMessage = globals.ReportMessages.RptMsg_DateRange;
                return false;
            }

            return true;
        }

        public bool IsUdidNumberSuppliedWithUdidText(ReportGlobals globals)
        {
            var udidText = globals.GetParmValue(WhereCriteria.UDIDTEXT);
            var udidNumber = globals.GetParmValue(WhereCriteria.UDIDNBR);
            if (string.IsNullOrEmpty(udidNumber) && !string.IsNullOrEmpty(udidText))
            {
                globals.ReportInformation.ReturnCode = 2;
                globals.ReportInformation.ErrorMessage = globals.ReportMessages.RptMsg_UDIDNbrReqd;
                return false;
            }

            return true;
        }

        public bool IsUdidNumberSupplied(ReportGlobals globals)
        {
            var udidNumber = globals.GetParmValue(WhereCriteria.UDIDNBR);
            if (!string.IsNullOrEmpty(udidNumber)) return true;
            globals.ReportInformation.ReturnCode = 2;
            globals.ReportInformation.ErrorMessage = globals.ReportMessages.RptMsg_UDIDNbrMust;
            return false;
        }

        public bool IsDateRangeValid(ReportGlobals globals)
        {
            if(!globals.BeginDate.HasValue || !globals.EndDate.HasValue || !globals.BeginDate.Value.IsPriorToOrSameDay(globals.EndDate.Value))
            {
                globals.ReportInformation.ReturnCode = 2;
                globals.ReportInformation.ErrorMessage = globals.ReportMessages.RptMsg_DateRange;
                return false;
            }

            return true;
        }

        public bool IsStartParseDateValid(ReportGlobals globals)
        {
            var start = globals.GetParmValue(WhereCriteria.TXTPARSEDTSTART);
            DateTime startDate;
            if (!DateTime.TryParse(start, out startDate))
            {
                globals.ReportInformation.ReturnCode = 2;
                globals.ReportInformation.ErrorMessage = "The Starting Parsed Date/Time is not valid.";
                return false;
            }
            return true;
        }


        public bool IsEndParseDateValid(ReportGlobals globals)
        {
            var end = globals.GetParmValue(WhereCriteria.TXTPARSEDTEND);
            DateTime endDate;
            if (!DateTime.TryParse(end, out endDate))
            {
                globals.ReportInformation.ReturnCode = 2;
                globals.ReportInformation.ErrorMessage = "The Ending Parsed Date/Time is not valid.";
                return false;
            }
            return true;
        }

        public bool IsDateRangeUnderThreeMonths(ReportGlobals globals, IReportDelayer pusher)
        {
            if (globals.BeginDate == null) return IsBeginDateSupplied(globals);

            if (globals.EndDate > globals.BeginDate.Value.AddMonths(3))
            {
                pusher.PushReportOffline(_serverNumber);
                return false;
            }

            return true;
        }

        public bool IsGoodDateRangeReportTypeCombo(ReportGlobals globals)
        {
            if (globals.ParmValueEquals(WhereCriteria.PREPOST, "2") && globals.ParmValueEquals(WhereCriteria.DATERANGE, "3"))
            {
                globals.ReportInformation.ReturnCode = 2;
                globals.ReportInformation.ErrorMessage = globals.ReportMessages.ErrorBadCombo;
                return false;
            }

            return true;
        }

        public bool IsOnlineReport(ReportGlobals globals, IReportDelayer pusher)
        {
            var runOffline = globals.GetParmValue(WhereCriteria.RUNOFFLINE) == Constants.Yes;
            if (runOffline && !globals.IsOfflineServer)
            {
                pusher.PushReportOffline(_serverNumber);
                return false;
            }

            return true;
        }

        public bool HasAccount(ReportGlobals globals, IReportDelayer pusher)
        {
            if (!globals.ParmHasValue(WhereCriteria.ACCT) && !globals.ParmHasValue(WhereCriteria.INACCT) && !globals.IsOfflineServer)
            {
                //if the account exists as advanced parameters
                foreach(var parms in globals.AdvancedParameters.Parameters)
                {
                    if (parms.FieldName == "ACCT") return true;
                }
                pusher.PushReportOffline(_serverNumber);
                return false;
            }

            return true;
        }

        public bool DataExists<T>(IList<T> data, ReportGlobals globals)
        {
            if (!data.Any())
            {
                globals.ReportInformation.ReturnCode = 2;
                globals.ReportInformation.ErrorMessage = globals.ReportMessages.RptMsg_NoData;

                var logger = new ReportLogLogger();
                logger.UpdateLog(globals.ReportLogKey, ReportLogLogger.ReportStatus.NODATA);
                return false;
            }

            return true;
        }

        public bool IsUnderOfflineThreshold(int recordCount, ReportGlobals globals, IReportDelayer delayer)
        {
            if (!globals.IsOfflineServer && globals.RecordLimit > 0 && globals.RecordLimit < recordCount)
            {
                delayer.PushReportOffline(ConfigurationManager.AppSettings["serverNumber"]);
                return false;
            }

            return true;
        }

        public bool IsCurrencyConversionRequired<T>(List<T> dataToConvert, string moneyType)
        {
            if (!dataToConvert.Any()) return false;

            var retriever = new CurrencyPropertyRetriever();

            var currencyTypeProperties = retriever.GetCurrencyTypeProperties(dataToConvert[0]);

            if (currencyTypeProperties == null || !currencyTypeProperties.Any())
            {
                throw new CurrencyConversionException("Currency conversion attempted on a class with no Source Currency attribute");
            }

            if (currencyTypeProperties.Any(prop => prop.PropertyType != typeof(string)))
            {
                throw new CurrencyConversionException("Source Currency property not of type string.");
            }

            return retriever.IsConversionRequired(dataToConvert, currencyTypeProperties, moneyType);
        }
    }
}

