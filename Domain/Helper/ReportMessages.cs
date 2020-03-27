
using System.Collections.Generic;

namespace Domain.Helper
{
    public class ReportMessages
    {

        public ReportMessages()
        {

            RptMsg_DateRange = "You need to specify a valid date range.";
            RptMsg_UDIDNbrReqd ="You cannot submit a report request with UDID text criteria unless you also specify a UDID #.";
            RptMsg_UDIDNbrMust = "You must specify a UDID # for this report.";
            RptMsg_Offline = "This report will be queued for offline processing.";
            RptMsg_NoData = "There is no data for your selection criteria.";
            RptMsg_BigVolume = "Due to the large volume of data selected for this report, it must be run offline.";
            RptMsg_InvalidChangeCodes = "Your list of Change Codes must all be numeric.";
            NotFound = "Not Found";
            ErrHandlMsg1 = "An unexpected error has occurred.";
            ErrHandlMsg2 = "Error No";
            Currency = "Currency";
            ErrorBadCombo = "Not a valid combination -- You cannot compare date range to Booked Date for History.";
            RptMsg_Timeout = "The report timed out and did not return from the server.";

            IncludingVoids = "Including Voids:";
            InvoicesOnly = "Invoices Only";
            InvoicesAndVoids = "Invoices/Voids";
            VoidsOnly = "Voids Only";
            CreditsOnly = "Credits Only";
            CreditsAndVoids = "Credits/Voids";
            StandardAccountsOnly = "Standard Accounts Only";
            CustomAccountsOnly = "Custom Accounts Only";
            CarbCalcProvBy = "Carbon calculations provided by";
            BadCompareDateRange = "Your comparison date range doesn't make sense.";
            BadCompareTripChanges = "Your date range for trip changes doesn't make sense";
            FromDateNotValid = "The \"From\" date you entered is not valid.";
            ToDateNotValid = "The \"To\" date you entered is not valid.";
            SpecifyMonthYear = "You need to specify a month and year.";

            GenericErrorMessage = "An error occurred processing the report.";

            RptMsg_eProfileProcessKeyAuthorization = "This report is not authorized for delivery by the eProfile.";

            RptMsg_ReportNotSupported = "The selected report is not supported by this version";
            QueuedForEffectsDelivery = "Report file {0} queued for eFFECTS delivery as specified by the eProfile {1}";

        }

        public string RptMsg_eProfileProcessKeyAuthorization { get; }
        public string RptMsg_Timeout { get; set; }
        public string RptMsg_DateRange { get; set; }
        public string RptMsg_UDIDNbrReqd { get; set; }
        public string RptMsg_UDIDNbrMust { get; set; }
        public string RptMsg_Offline { get; set; }
        public string RptMsg_NoData { get; set; }
        public string RptMsg_BigVolume { get; set; }
        public string RptMsg_InvalidChangeCodes { get; set; }
        public string RptMsg_ReportNotSupported { get; set; }
        public string NotFound { get; set; }
        public string ErrHandlMsg1 { get; set; }
        public string ErrHandlMsg2 { get; set; }
        public string ErrorBadCombo { get; set; }
        public string CarbCalcProvBy { get; set; }
        public string BadCompareDateRange { get; set; }
        public string BadCompareTripChanges { get; set; }
        public string FromDateNotValid { get; set; }
        public string ToDateNotValid { get; set; }
        public string SpecifyMonthYear { get; set; }

        public string IncludingVoids { get; set; }
        public string InvoicesOnly { get; set; }
        public string InvoicesAndVoids { get; set; }
        public string VoidsOnly { get; set; }
        public string CreditsOnly { get; set; }
        public string CreditsAndVoids { get; set; }
        public string StandardAccountsOnly { get; set; }
        public string CustomAccountsOnly { get; set; }
        public string Currency { get; set; }
        public string GenericErrorMessage { get; set; }
        /// <summary>
        /// Report file {0} queued for eFFECTS delivery as specified by the eProfile {1}
        /// </summary>
        public string QueuedForEffectsDelivery { get; set; }

    }
}
