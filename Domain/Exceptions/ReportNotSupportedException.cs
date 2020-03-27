using System;

namespace Domain.Exceptions
{
    public class ReportNotSupportedException : Exception
    {
        public ReportNotSupportedException() : base()
        {
        }

        public ReportNotSupportedException(string message) : base(message)
        {
        }

        public ReportNotSupportedException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        public ReportNotSupportedException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        public ReportNotSupportedException(string format, Exception innerException, params object[] args) :
            base(string.Format(format, args), innerException)
        {
        }
    }
}
