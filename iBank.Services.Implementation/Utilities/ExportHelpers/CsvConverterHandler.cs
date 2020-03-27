using Domain.Helper;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Domain.Exceptions;
using com.ciswired.libraries.CISLogger;
using iBank.Services.Implementation.Shared;

namespace iBank.Services.Implementation.Utilities.ExportHelpers
{
    public static class CsvConverterHandler
    {
        private static readonly com.ciswired.libraries.CISLogger.ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const string Quote = "\"";
        private const string Comma = ",";
        //private const string Crlf = "\r\n";
        private const string Exportable = "Exportable";
        private const string Sheet1 = "Sheet1";

        private static string ConvertToString(object val, string dateFormat)
        {
            if (val is DateTime) return dateFormat=="" ? ((DateTime)val).ToShortDateString() : ((DateTime)val).ToString(dateFormat);

            if (val is string) return val.ToString().Trim();

            if (val is int) return ((int)val).ToString(CultureInfo.InvariantCulture);

            //#.#### format meant that all the digits are optional, so for 0s it was returning empty string. Changed to 0.#### so that it returns a 0. For other values, it will be unchanged.
            if (val is decimal) return ((decimal)val).ToString("0.####", CultureInfo.InvariantCulture);

            if (val is double) return ((double)val).ToString("0.####", CultureInfo.InvariantCulture);

            if (val is bool) return val.ToString().ToUpper();

            return string.Empty;
        }

        public static void ConvertToCsv<T>(IReadOnlyList<T> collection, ReportGlobals globals, string separator = "", bool includeHeaders = true)
        {         
            if (collection == null || collection.Count <= 0) return;

            var tType = collection[0].GetType();
            var tProperties = new List<PropertyInfo>(tType.GetProperties());
            
            var propertiesToExport = (from propertyInfo in tProperties select propertyInfo.Name).ToList();

            if (!propertiesToExport.Any())
            {
                throw new ExportPropertiesException("Class not marked as exportable, so no properties found to add to worksheet.");
            }

            var fileName = globals.GetFileName();
            using (TextWriter writer = File.CreateText(fileName))
            {
                int pos;
                if (includeHeaders)
                {
                    var header = string.Empty;
                    foreach (var propertyInfo in tProperties.Where(propertyInfo => propertiesToExport.Contains(propertyInfo.Name)))
                    {
                        header += separator + propertyInfo.Name.NormalizeColumnHeader() + separator + Comma;
                    }
                    pos = header.Length - (separator.Length + 1);
                    header = header.Remove(pos + 1, separator.Length);
                    //header += Crlf;
                    writer.WriteLine(header);
                }

                foreach (var item in collection)
                {
                    var line = string.Empty;
                    foreach (var propertyInfo in tProperties.Where(propertyInfo => propertiesToExport.Contains(propertyInfo.Name)))
                    {
                        string val = string.Empty;
                        try
                        {
                            val = propertyInfo.GetValue(item, null).ToString().Trim();
                        }
                        catch (Exception ex)
                        {
                            LOG.Warn(ex);
                        }

                        if (val.Contains(Comma)) val = Quote + val + Quote;

                        line += separator + val + separator + Comma;
                    }
                    pos = line.Length - (separator.Length + 1);
                    line = line.Remove(pos + 1, separator.Length);
                    writer.WriteLine(line);
                }
            }

            if (globals.IsEffectsDelivery) CrystalFunctions.ApplyEffectsDelivery(globals, fileName);

        }

        public static void ConvertToCsv<T>(IReadOnlyList<T> collection, List<string> fieldList, ReportGlobals globals, string separator = "", bool includeHeaders = true, string dateFormat = "")
        {
            if (collection == null || collection.Count <= 0) return;

            var tType = collection[0].GetType();
            var tProperties = new List<PropertyInfo>(tType.GetProperties());

            var wrapper = new EncodingWrapper();
            var encoding = wrapper.GetEncoding(globals.OutputLanguage);

            var fileName = globals.GetFileName();
            using (var writer = new StreamWriter(fileName, false, encoding))
            {
                if (includeHeaders) WriteHeaders(fieldList, globals, writer, separator);

                WriteDetails(collection, fieldList, globals, writer, tProperties, separator, dateFormat);
            }

            if (globals.IsEffectsDelivery) CrystalFunctions.ApplyEffectsDelivery(globals, fileName);
        }

        private static void WriteHeaders(List<string> fieldList, ReportGlobals globals, StreamWriter writer, string separator)
        {
            var header = string.Empty;
            int pos;
            foreach (var field in fieldList)
            {
                var bAddHeader = true;
                var fieldName = field;
                var asIndex = fieldName.IndexOf(" AS ", StringComparison.OrdinalIgnoreCase);

                if (asIndex >= 0)
                {
                    fieldName = field.Substring(asIndex + 4);
                }

                if (fieldName == "") fieldName = field.Substring(0, asIndex);

                if (fieldName.ToLower() == "rptbreaks")
                {
                    if (globals.IsParmValueOn(WhereCriteria.CBSUPPRPTBRKS)) bAddHeader = false;
                }

                if (bAddHeader) header += separator + fieldName.NormalizeColumnHeader() + separator + Comma;
            }

            pos = header.Length - (separator.Length + 1);
            header = header.Remove(pos + 1, separator.Length);
            //Remove the blank line 
            //header += Crlf;
            writer.WriteLine(header);
        }

        private static void WriteDetails<T>(IReadOnlyList<T> collection, List<string> fieldList, ReportGlobals globals, StreamWriter writer, 
            List<PropertyInfo> tProperties, string separator, string dateFormat)
        {
            int pos;
            foreach (var item in collection)
            {
                var line = string.Empty;
                foreach (var field in fieldList)
                {
                    var fieldName = field;
                    var headerName = field;
                    var bAddItem = true;
                    var asIndex = fieldName.IndexOf(" AS ", StringComparison.OrdinalIgnoreCase);

                    if (asIndex >= 0)
                    {
                        fieldName = field.Left(asIndex);
                        headerName = field.Substring(asIndex + 4);
                    }

                    if (headerName.ToLower() == "rptbreaks")
                    {
                        if (globals.IsParmValueOn(WhereCriteria.CBSUPPRPTBRKS)) bAddItem = false;
                    }

                    if (bAddItem)
                    {
                        var propertyInfo = tProperties.FirstOrDefault(s => s.Name.EqualsIgnoreCase(fieldName));

                        if (new ExceptionRules().ApplyExceptionRules<T>(tProperties, fieldName, item))
                        {
                            propertyInfo = null;
                        }

                        if (propertyInfo != null)
                        {
                            var val = ConvertToString(propertyInfo.GetValue(item, null), dateFormat);

                            if (val.Contains(Comma)) val = Quote + val + Quote;

                            line += separator + val + separator + Comma;
                        }
                        else
                        {
                            line += separator + "" + separator + Comma;
                        }
                    }
                }

                pos = line.Length - (separator.Length + 1);
                line = line.Remove(pos + 1, separator.Length);
                writer.WriteLine(line);
            }
        }
    }
}
