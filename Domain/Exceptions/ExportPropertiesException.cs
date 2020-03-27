using System;

namespace Domain.Exceptions
{
    public class ExportPropertiesException : Exception
    {
        public ExportPropertiesException() : base()
        {
        }

        public ExportPropertiesException(string message) : base(message)
        {
        }

        public ExportPropertiesException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        public ExportPropertiesException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        public ExportPropertiesException(string format, Exception innerException, params object[] args) :
            base(string.Format(format, args), innerException)
        {
        }
    }
}
