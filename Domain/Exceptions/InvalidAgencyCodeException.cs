using System;

namespace Domain.Exceptions
{
    public class InvalidAgencyCodeException : Exception
    {
        public InvalidAgencyCodeException() : base()
        {
        }

        public InvalidAgencyCodeException(string message) : base(message)
        {
        }

        public InvalidAgencyCodeException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        public InvalidAgencyCodeException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        public InvalidAgencyCodeException(string format, Exception innerException, params object[] args) :
            base(string.Format(format, args), innerException)
        {
        }
    }
}
