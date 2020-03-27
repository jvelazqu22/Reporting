using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class ReportOutputLocationException : Exception
    {
        public ReportOutputLocationException() : base()
        {
        }

        public ReportOutputLocationException(string message) : base(message)
        {
        }

        public ReportOutputLocationException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        public ReportOutputLocationException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        public ReportOutputLocationException(string format, Exception innerException, params object[] args) :
            base(string.Format(format, args), innerException)
        {
        }
    }
}
