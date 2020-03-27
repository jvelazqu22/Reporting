using System;

namespace Domain.Exceptions
{
    public class FeatureFlagDoesNotExistException : Exception
    {
        public FeatureFlagDoesNotExistException() : base()
        {
        }

        public FeatureFlagDoesNotExistException(string message) : base(message)
        {
        }

        public FeatureFlagDoesNotExistException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        public FeatureFlagDoesNotExistException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        public FeatureFlagDoesNotExistException(string format, Exception innerException, params object[] args) :
            base(string.Format(format, args), innerException)
        {
        }
    }
}
