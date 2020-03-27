using System;
using System.Diagnostics;

using com.ciswired.libraries.CISLogger;

namespace Domain.Helper
{
    public enum SubjectOfTiming
    {
        OnlineReport,
        CurrencyConversion
    }

    /// <summary>
    /// The CustomTimer, when wrapped in a using statement, will start and stop a watch and log the total time taken
    /// </summary>
    public class CustomTimer : IDisposable
    {
        private readonly ILogger _logger;

        private readonly int _processKey;

        private readonly int _userNumber;

        private readonly string _agency;

        private readonly int _reportLogKey;

        private readonly Stopwatch _watch;

        private readonly string _subject;

        public CustomTimer(int processKey, int userNumber, string agency, int reportLogKey, ILogger logger, SubjectOfTiming subject)
        {
            _logger = logger;

            _processKey = processKey;
            _userNumber = userNumber;
            _agency = agency;
            _reportLogKey = reportLogKey;
            _subject = subject.ToString();

            var msg = $"Running report. User: [{userNumber}] | Agency: [{agency}] | Process Key: [{processKey}] | Report Log Key: [{reportLogKey}]";
            logger.Debug(msg);

            _watch = new Stopwatch();
            _watch.Start();
        }

        public CustomTimer(int processKey, int userNumber, string agency, int reportLogKey, ILogger logger, string subject)
        {
            _logger = logger;

            _processKey = processKey;
            _userNumber = userNumber;
            _agency = agency;
            _reportLogKey = reportLogKey;
            _subject = subject;

            var msg = $"{_subject}. User: [{userNumber}] | Agency: [{agency}] | Process Key: [{processKey}] | Report Log Key: [{reportLogKey}]";
            logger.Debug(msg);

            _watch = new Stopwatch();
            _watch.Start();
        }

        public CustomTimer(int processKey, int userNumber, string agency, int reportLogKey, ILogger logger, string subject, int udrKey)
        {
            _logger = logger;

            _processKey = processKey;
            _userNumber = userNumber;
            _agency = agency;
            _reportLogKey = reportLogKey;
            _subject = subject;

            var msg = $"{_subject}. User: [{userNumber}] | Agency: [{agency}] | Process Key: [{processKey}] | UDR Key: [{udrKey}] | Report Log Key: [{reportLogKey}]";
            logger.Debug(msg);

            _watch = new Stopwatch();
            _watch.Start();
        }

        public void Dispose()
        {
            _watch.Stop();
            _logger.Debug($"Time to complete {_subject} (ss) [{_watch.Elapsed.TotalSeconds}] | User: [{_userNumber}] Agency: [{_agency}] Process Key: [{_processKey}] | Report Log Key: [{_reportLogKey}]");
        }
    }
}
