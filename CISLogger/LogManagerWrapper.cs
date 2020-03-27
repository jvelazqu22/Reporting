using System;
using System.Linq;
using System.IO;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository.Hierarchy;
using System.Collections.Generic;
using System.Reflection;

namespace com.ciswired.libraries.CISLogger
{
    public class LogManagerWrapper : ILogManager
    {
        public static void SetLogProp(string propName, string propValue)
        {
            log4net.GlobalContext.Properties[propName] = propValue;
        }

        /// <summary>
        /// Constructor configures/watches config file named log4net.config.
        /// </summary>
        //public LogManagerWrapper()
        //{
        //    XmlConfigurator.ConfigureAndWatch("log4net.config".FindFilePath());
        //}

        /// <summary>
        /// Constructor configures/watches passed in config file.
        /// </summary>
        /// <param name="configFile"></param>
        public LogManagerWrapper(FileInfo configFile)
        {
            XmlConfigurator.ConfigureAndWatch(configFile);
        }

        public LogManagerWrapper()
        {
            var startpath = Path.Combine(Assembly.GetCallingAssembly().Location, "log4net.config");
            var file = new FileInfo(startpath);

            while (!file.Exists)
            {
                //search for the file
                if (file.Directory.Parent == null)
                {
                    break;
                }
                var parentDir = file.Directory.Parent;
                file = new FileInfo(Path.Combine(parentDir.FullName, file.Name));
            }

            XmlConfigurator.ConfigureAndWatch(file);
        }

        public ILogger GetLogger(Type type)
        {
            var logger = log4net.LogManager.GetLogger(type);
            return new Log4NetWrapper(logger);
        }

        public ILogger GetLogger(string name)
        {
            var logger = log4net.LogManager.GetLogger(name);
            return new Log4NetWrapper(logger);
        }

        /// <summary>
        /// Retrieves specified ADONetAppender and sets connection string to specified connection string.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="adoAppenderName"></param>
        /// <param name="type"></param>
        public static void InitializeConnectionString(string connectionString, string adoAppenderName, Type type)
        {
            Hierarchy hierarchy = LogManager.GetRepository() as Hierarchy;

            if (hierarchy != null)
            {
                //get the ADONetAppender for the declared type
                var adoAppender = hierarchy.GetLogger(type.ToString())
                                           .Repository
                                           .GetAppenders()
                                           .Where(app => app.GetType() == typeof(AdoNetAppender))
                                           .Cast<AdoNetAppender>()
                                           .FirstOrDefault(app => app.Name == adoAppenderName);

                if (adoAppender != null)
                {
                    //set the connection string
                    adoAppender.ConnectionString = connectionString;

                    //refresh the settings
                    adoAppender.ActivateOptions();
                }
            }
        }

        /// <summary>
        /// Retrieves specified ADONetAppender and sets connection string to specified connection string.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="adoAppenderName"></param>
        /// <param name="loggerName"></param>
        public static void InitializeConnectionString(string connectionString, string adoAppenderName, string loggerName)
        {
            Hierarchy hierarchy = LogManager.GetRepository() as Hierarchy;

            if (hierarchy != null)
            {
                //get the ADONetAppender for the declared type
                var adoAppender = hierarchy.GetLogger(loggerName)
                                           .Repository
                                           .GetAppenders()
                                           .Where(app => app.GetType() == typeof(AdoNetAppender))
                                           .Cast<AdoNetAppender>()
                                           .FirstOrDefault(app => app.Name == adoAppenderName);

                if (adoAppender != null)
                {
                    //set the connection string
                    adoAppender.ConnectionString = connectionString;

                    //refresh the settings
                    adoAppender.ActivateOptions();
                }
            }
        }

        public static void SetThreadContext(Dictionary<string, object> loggingObjProperties)
        {
            foreach (KeyValuePair<string, object> pair in loggingObjProperties)
            {
                log4net.ThreadContext.Properties[pair.Key] = pair.Value;
            }
        }

        public static void SetGlobalContext(string key, object value)
        {
            log4net.GlobalContext.Properties[key] = value;
        }
    }
}
