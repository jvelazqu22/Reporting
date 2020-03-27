using System;

namespace Domain.Exceptions
{
    public class TimeBucketException : Exception
    {
        public TimeBucketException() : base()
        {
        }

        public TimeBucketException(string message) : base(message)
        {
        }

        public TimeBucketException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        public TimeBucketException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        public TimeBucketException(string format, Exception innerException, params object[] args) :
            base(string.Format(format, args), innerException)
        {
        }
    }
}
