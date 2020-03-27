using System;

namespace Domain.Exceptions
{
    public class ImageNotFoundException : Exception
    {
        public ImageNotFoundException() : base()
        {
        }

        public ImageNotFoundException(string message) : base(message)
        {
        }

        public ImageNotFoundException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        public ImageNotFoundException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        public ImageNotFoundException(string format, Exception innerException, params object[] args) :
            base(string.Format(format, args), innerException)
        {
        }
    }
}
