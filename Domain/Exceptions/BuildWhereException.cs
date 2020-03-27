using System;

namespace Domain.Exceptions
{
    public class BuildWhereException : Exception
    {
        public BuildWhereException() : base()
        {
        }

        public BuildWhereException(string message) : base(message)
        {
        }

        public BuildWhereException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        public BuildWhereException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        public BuildWhereException(string format, Exception innerException, params object[] args) :
            base(string.Format(format, args), innerException)
        {
        }
    }
}
