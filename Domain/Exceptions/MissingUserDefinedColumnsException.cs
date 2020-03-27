using System;

namespace Domain.Exceptions
{
    public class MissingUserDefinedColumnsException : Exception
    {
        public MissingUserDefinedColumnsException() : base()
        {
        }

        public MissingUserDefinedColumnsException(string message) : base(message)
        {
        }

        public MissingUserDefinedColumnsException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        public MissingUserDefinedColumnsException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        public MissingUserDefinedColumnsException(string format, Exception innerException, params object[] args) :
            base(string.Format(format, args), innerException)
        {
        }
    }
}
