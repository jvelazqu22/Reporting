using com.ciswired.libraries.CISLogger;

using Domain.Orm.Classes;

namespace iBank.Server.Utilities.Logging
{
    public interface IReportServerLogger
    {
        void DebugLogWithReportInfo(PendingReportInformation report, string msg);

        void InfoLogWithReportInfo(PendingReportInformation report, string msg);

        void WarnLogWithReportInfo(PendingReportInformation report, string msg);

        void ErrorLogWithReportInfo(PendingReportInformation report, string msg);

        string FormatMessageWithReportInfo(PendingReportInformation report, string msg);

        string FormatWithReportInfo(PendingReportInformation report);
    }

    public class ReportServerLogger : IReportServerLogger
    {
        private readonly ILogger _log;

        public ReportServerLogger(ILogger log)
        {
            _log = log;
        }

        public void DebugLogWithReportInfo(PendingReportInformation report, string msg)
        {
            var message = FormatMessageWithReportInfo(report, msg);
            _log.Debug(message);
        }

        public void InfoLogWithReportInfo(PendingReportInformation report, string msg)
        {
            var message = FormatMessageWithReportInfo(report, msg);
            _log.Info(message);
        }

        public void WarnLogWithReportInfo(PendingReportInformation report, string msg)
        {
            var message = FormatMessageWithReportInfo(report, msg);
            _log.Warn(message);
        }

        public void ErrorLogWithReportInfo(PendingReportInformation report, string msg)
        {
            var message = FormatMessageWithReportInfo(report, msg);
            _log.Error(message);
        }

        public string FormatMessageWithReportInfo(PendingReportInformation report, string msg)
        {
            return $"{FormatWithReportInfo(report)} | Message:[{msg ?? "null msg"}]";
        }

        public string FormatWithReportInfo(PendingReportInformation report)
        {
            if (report == null) return "null PendingReportInformation report";

            return $"Report ID:[{report.ReportId ?? "null ReportId"}] | Agency:[{report.Agency ?? "null Agency"}]";
        }
    }
}
