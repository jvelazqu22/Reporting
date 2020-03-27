using System;

namespace Domain.Exceptions
{
    public class InvalidDatabaseConfigurationException : Exception
    {
        public InvalidDatabaseConfigurationException() : base() { }

        public InvalidDatabaseConfigurationException(string message) : base(message) { }

        public InvalidDatabaseConfigurationException(string format, params object[] args) : base(string.Format(format, args)) { }

        public InvalidDatabaseConfigurationException(string message, Exception innerException) : base(message, innerException) { }

        public InvalidDatabaseConfigurationException(string format, Exception innerException, params object[] args) : base(string.Format(format, args), innerException) { }
    }
}
