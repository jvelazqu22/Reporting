using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class MissingFieldParamsHandlerException : Exception
    {
        public MissingFieldParamsHandlerException() : base()
        {
        }

        public MissingFieldParamsHandlerException(string message) : base(message)
        {
        }

        public MissingFieldParamsHandlerException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        public MissingFieldParamsHandlerException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        public MissingFieldParamsHandlerException(string format, Exception innerException, params object[] args) :
            base(string.Format(format, args), innerException)
        {
        }

    }
}
