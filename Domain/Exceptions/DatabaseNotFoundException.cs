using System;

namespace Domain.Exceptions
{
    public class DatabaseNotFoundException : Exception
    {
        public DatabaseNotFoundException() : base()
        {
        }

        public DatabaseNotFoundException(string message) : base(message)
        {
        }

        public DatabaseNotFoundException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        public DatabaseNotFoundException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        public DatabaseNotFoundException(string format, Exception innerException, params object[] args) :
            base(string.Format(format, args), innerException)
        {
        }
    }
}
