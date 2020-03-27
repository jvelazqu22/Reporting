using System;
using log4net;

namespace com.ciswired.libraries.CISLogger
{
    /// <summary>
    /// log4net implementation of ILogger
    /// </summary>
    public class Log4NetWrapper : ILogger
    {
        private readonly log4net.ILog _logger;

        public Log4NetWrapper(ILog logger)
        {
            _logger = logger;
        }

        public void Debug(object message)
        {
            _logger.Debug(message);
        }

        public void Info(object message)
        {
            _logger.Info(message);
        }

        public void Warn(object message)
        {
            _logger.Warn(message);
        }

        public void Error(object message)
        {
            _logger.Error(message);
        }

        public void Fatal(object message)
        {
            _logger.Fatal(message);
        }

        public void Debug(object message, Exception t)
        {
            _logger.Debug(message, t);
        }

        public void Info(object message, Exception t)
        {
            _logger.Info(message, t);
        }

        public void Warn(object message, Exception t)
        {
            _logger.Warn(message, t);
        }

        public void Error(object message, Exception t)
        {
            _logger.Error(message, t);
        }

        public void Fatal(object message, Exception t)
        {
            _logger.Fatal(message, t);
        }

        public void DebugFormat(string format, params object[] args)
        {
            _logger.DebugFormat(format, args);
        }

        public void InfoFormat(string format, params object[] args)
        {
            _logger.InfoFormat(format, args);
        }

        public void WarnFormat(string format, params object[] args)
        {
            _logger.WarnFormat(format, args);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void FatalFormat(string format, params object[] args)
        {
            _logger.FatalFormat(format, args);
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            _logger.DebugFormat(provider, format, args);
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            _logger.InfoFormat(provider, format, args);
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            _logger.WarnFormat(provider, format, args);
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            _logger.ErrorFormat(provider, format, args);
        }

        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            _logger.FatalFormat(provider, format, args);
        }

        public bool IsDebugEnabled
        {
            get { return _logger.IsDebugEnabled; }

            private set { }
        }
        
        public bool IsInfoEnabled
        {
            get { return _logger.IsInfoEnabled; }

            private set { }
        }

        public bool IsWarnEnabled
        {
            get { return _logger.IsWarnEnabled; }

            private set { }
        }

        public bool IsErrorEnabled
        {
            get { return _logger.IsErrorEnabled; }

            private set { }
        }

        public bool IsFatalEnabled
        {
            get { return _logger.IsFatalEnabled; }

            private set { }
        }

        public string EffectiveLevel
        {
            get 
            { 
                log4net.Repository.Hierarchy.Logger logger = (log4net.Repository.Hierarchy.Logger)_logger.Logger;

                return logger.EffectiveLevel.ToString();
            }

            private set { }
        }

        public bool IsOff
        {
            get
            {
                log4net.Repository.Hierarchy.Logger logger = (log4net.Repository.Hierarchy.Logger)_logger.Logger;

                return logger.EffectiveLevel == log4net.Core.Level.Off;
            }
        }
    }
}
