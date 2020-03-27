using System;

namespace Domain.Exceptions
{
    public class UnknownWildcardException : Exception
    {
        public UnknownWildcardException() : base()
        {
        }

        public UnknownWildcardException(string message) : base(message)
        {
        }

        public UnknownWildcardException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        public UnknownWildcardException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        public UnknownWildcardException(string format, Exception innerException, params object[] args) :
            base(string.Format(format, args), innerException)
        {
        }
    }
}
