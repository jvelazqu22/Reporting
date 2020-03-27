using System;

namespace Domain.Exceptions
{
    public class UserDefinedColumnNotFoundException : Exception
    {
        public UserDefinedColumnNotFoundException() : base()
        {
        }

        public UserDefinedColumnNotFoundException(string message) : base(message)
        {
        }

        public UserDefinedColumnNotFoundException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        public UserDefinedColumnNotFoundException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        public UserDefinedColumnNotFoundException(string format, Exception innerException, params object[] args) :
            base(string.Format(format, args), innerException)
        {
        }
    }
}
