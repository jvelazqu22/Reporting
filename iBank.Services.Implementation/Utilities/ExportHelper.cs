using iBank.Server.Utilities.Classes;
using System.Collections.Generic;
using System.Reflection;
using com.ciswired.libraries.CISLogger;
using iBank.Services.Implementation.Utilities.ExportHelpers;

namespace iBank.Services.Implementation
{
    public static class ExportHelper
    {
        private static readonly com.ciswired.libraries.CISLogger.ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const string Quote = "\"";
        private const string Comma = ",";
        //private const string Crlf = "\r\n";
        private const string Exportable = "Exportable";
        private const string Sheet1 = "Sheet1";

        public static void ListToXlsx<T>(IReadOnlyList<T> collection, ReportGlobals globals, bool includeHeaders = true)
        {
            ListToXlsxHandler.ListToXlsx(collection, globals, includeHeaders);
        }

        /// <summary>
        /// Exports all rows of a class to Excel. Populate fieldList with the fields you want to export, in the order you want them to appear. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="fieldList"></param>
        /// <param name="globals"></param>
        /// <param name="includeHeaders"></param>
        public static void ListToXlsx<T>(IReadOnlyList<T> collection, List<string> fieldList, ReportGlobals globals, bool includeHeaders = true, string dateTimeFormat = "")
        {
            ListToXlsxHandler.ListToXlsx(collection, fieldList, globals, includeHeaders, dateTimeFormat);
        }

        public static void ConvertToCsv<T>(IReadOnlyList<T> collection, ReportGlobals globals, string separator = "", bool includeHeaders = true)
        {
            CsvConverterHandler.ConvertToCsv(collection, globals, separator, includeHeaders);
        }

        public static void ConvertToCsv<T>(IReadOnlyList<T> collection, List<string> fieldList, ReportGlobals globals, string separator = "", bool includeHeaders = true, string dateFormat = "")
        {
            CsvConverterHandler.ConvertToCsv(collection, fieldList, globals, separator, includeHeaders, dateFormat);
        }
    }

}
