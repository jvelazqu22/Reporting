using System;

namespace Domain.Exceptions
{
    public class ReportNotFoundException : Exception
    {
        public ReportNotFoundException() : base()
        {
        }

        public ReportNotFoundException(string message) : base(message)
        {
        }

        public ReportNotFoundException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        public ReportNotFoundException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        public ReportNotFoundException(string format, Exception innerException, params object[] args) :
            base(string.Format(format, args), innerException)
        {
        }
    }
}
