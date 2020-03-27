using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class HaulNotFoundException : Exception
    {
        public HaulNotFoundException() : base()
        {
        }

        public HaulNotFoundException(string message) : base(message)
        {
        }

        public HaulNotFoundException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        public HaulNotFoundException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        public HaulNotFoundException(string format, Exception innerException, params object[] args) :
            base(string.Format(format, args), innerException)
        {
        }
    }
}
