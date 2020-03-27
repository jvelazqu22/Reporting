using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class ServerAddressNotFoundException : Exception
    {
        public ServerAddressNotFoundException() : base()
        {
        }

        public ServerAddressNotFoundException(string message) : base(message)
        {
        }

        public ServerAddressNotFoundException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        public ServerAddressNotFoundException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        public ServerAddressNotFoundException(string format, Exception innerException, params object[] args) :
            base(string.Format(format, args), innerException)
        {
        }
    }
}
