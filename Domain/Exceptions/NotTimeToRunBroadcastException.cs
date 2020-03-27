using System;

namespace Domain.Exceptions
{
    public class NotTimeToRunBroadcastException : Exception
    {
        public NotTimeToRunBroadcastException() : base()
        {
        }

        public NotTimeToRunBroadcastException(string message) : base(message)
        {
        }

        public NotTimeToRunBroadcastException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        public NotTimeToRunBroadcastException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        public NotTimeToRunBroadcastException(string format, Exception innerException, params object[] args) :
            base(string.Format(format, args), innerException)
        {
        }
    }
}
