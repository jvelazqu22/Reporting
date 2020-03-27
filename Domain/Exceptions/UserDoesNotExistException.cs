using System;

namespace Domain.Exceptions
{
    public class UserDoesNotExistException : Exception
    {
        public UserDoesNotExistException() : base()
        {
        }

        public UserDoesNotExistException(string message) : base(message)
        {
        }

        public UserDoesNotExistException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        public UserDoesNotExistException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        public UserDoesNotExistException(string format, Exception innerException, params object[] args) :
            base(string.Format(format, args), innerException)
        {
        }
    }
}
