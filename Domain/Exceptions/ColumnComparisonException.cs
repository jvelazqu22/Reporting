using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class ColumnComparisonException : Exception
    {
        public ColumnComparisonException() : base()
        {
        }

        public ColumnComparisonException(string message) : base(message)
        {
        }

        public ColumnComparisonException(string format, params object[] args)
            : base(string.Format(format, args))
        {
        }

        public ColumnComparisonException(string message, Exception innerException) :
            base(message, innerException)
        {
        }

        public ColumnComparisonException(string format, Exception innerException, params object[] args) :
            base(string.Format(format, args), innerException)
        {
        }
    }
}
