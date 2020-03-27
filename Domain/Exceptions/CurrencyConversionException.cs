using System;

namespace Domain.Exceptions
{
    public class CurrencyConversionException : Exception
    {
        public CurrencyConversionException() : base()
        {
        }

        public CurrencyConversionException(string message) : base(message)
        {
        }

        public CurrencyConversionException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        public CurrencyConversionException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        public CurrencyConversionException(string format, Exception innerException, params object[] args) :
            base(string.Format(format, args), innerException)
        {
        }
    }
}
