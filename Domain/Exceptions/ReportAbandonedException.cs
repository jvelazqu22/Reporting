using System;

namespace Domain.Exceptions
{
    public class ReportAbandonedException : Exception
    {
        public ReportAbandonedException() : base()
        {
        }

        public ReportAbandonedException(string message) : base(message)
        {
        }

        public ReportAbandonedException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        public ReportAbandonedException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        public ReportAbandonedException(string format, Exception innerException, params object[] args) :
            base(string.Format(format, args), innerException)
        {
        }
    }
}
