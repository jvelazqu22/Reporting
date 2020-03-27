using System.Configuration;
using System.IO;

using log4net.Core;
using log4net.Layout.Pattern;

namespace CISLogger
{
    public class AppSettingConverter : PatternLayoutConverter
    {
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            var setting = ConfigurationManager.AppSettings[Option];

            writer.Write(!string.IsNullOrEmpty(setting) ? setting : "");
        }
    }
}
