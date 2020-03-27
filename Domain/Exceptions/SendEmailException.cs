using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class SendEmailException : Exception
    {
        public SendEmailException() : base()
        {
        }

        public SendEmailException(string message) : base(message)
        {
        }

        public SendEmailException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        public SendEmailException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        public SendEmailException(string format, Exception innerException, params object[] args) :
            base(string.Format(format, args), innerException)
        {
        }
    }
}
