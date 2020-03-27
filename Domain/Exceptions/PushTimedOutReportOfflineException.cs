using System;

namespace Domain.Exceptions
{
    public class PushTimedOutReportOfflineException : Exception
    {
        public PushTimedOutReportOfflineException() : base()
        {
        }

        public PushTimedOutReportOfflineException(string message) : base(message)
        {
        }

        public PushTimedOutReportOfflineException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        public PushTimedOutReportOfflineException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        public PushTimedOutReportOfflineException(string format, Exception innerException, params object[] args) :
            base(string.Format(format, args), innerException)
        {
        }
    }
}
