using System;

namespace Domain.Exceptions
{
    public class CarbonRateNotFoundException : Exception
    {
        public CarbonRateNotFoundException() : base()
        {
        }

        public CarbonRateNotFoundException(string message) : base(message)
        {
        }

        public CarbonRateNotFoundException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        public CarbonRateNotFoundException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        public CarbonRateNotFoundException(string format, Exception innerException, params object[] args) :
            base(string.Format(format, args), innerException)
        {
        }
    }
}
