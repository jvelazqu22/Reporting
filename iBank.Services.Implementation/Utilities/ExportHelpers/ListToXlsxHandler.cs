using Domain.Helper;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Domain.Exceptions;
using com.ciswired.libraries.CISLogger;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.MacroHelpers;
using Domain;

namespace iBank.Services.Implementation.Utilities.ExportHelpers
{
    public static class ListToXlsxHandler
    {
        private static readonly com.ciswired.libraries.CISLogger.ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const string Quote = "\"";
        private const string Comma = ",";
        //private const string Crlf = "\r\n";
        private const string Exportable = "Exportable";
        private const string Sheet1 = "Sheet1";

        /// <summary>
        /// Exports all rows of a class to Excel. Class must be marked Exportable. 
        /// http://epplus.codeplex.com/
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="globals"></param>
        /// <param name="includeHeaders"></param>
        public static void ListToXlsx<T>(IReadOnlyList<T> collection, ReportGlobals globals, bool includeHeaders = true)
        {
            if (collection == null || collection.Count <= 0) return;
            var filename = globals.GetFileName();
            var newFile = new FileInfo(filename);
            if (newFile.Exists)
            {
                newFile.Delete();  // ensures we create a new workbook
                newFile = new FileInfo(filename);
            }
            if (Features.ExcelDateTimeFormatUsePropertyInfo.IsEnabled())
            {
                BuildWorksheet(collection, globals, includeHeaders, newFile, filename);
            }
            else
            {
                using (var package = new ExcelPackage(newFile))
                {
                    // add a new worksheet to the empty workbook
                    var worksheet = package.Workbook.Worksheets.Add(Sheet1);

                    var currentRow = 1;
                    var currentCol = 1;

                    var tType = collection[0].GetType();
                    var tProperties = new List<PropertyInfo>(tType.GetProperties());

                    var propertiesToExport = (from propertyInfo in tProperties select propertyInfo.Name).ToList();

                    if (!propertiesToExport.Any())
                    {
                        throw new ExportPropertiesException("Class not marked as exportable, so no properties found to add to worksheet.");
                    }

                    if (includeHeaders)
                    {
                        foreach (var propertyInfo in tProperties)
                        {
                            if (propertiesToExport.Contains(propertyInfo.Name))
                            {
                                worksheet.Cells[currentRow, currentCol].Value = propertyInfo.Name.NormalizeColumnHeader();
                                currentCol = currentCol + 1;
                            }
                        }
                        currentRow = currentRow + 1;
                    }

                    foreach (var item in collection)
                    {
                        currentCol = 1;
                        foreach (var propertyInfo in tProperties)
                        {
                            if (propertiesToExport.Contains(propertyInfo.Name))
                            {
                                var itemValue = propertyInfo.GetValue(item, null);
                                if (itemValue is string)
                                {
                                    worksheet.Cells[currentRow, currentCol].Value = itemValue.ToString().Trim();
                                }
                                else
                                {
                                    //if it's a datetime
                                    DateTime dateTime;
                                    if (null != itemValue && DateTime.TryParse(itemValue.ToString(), out dateTime))
                                    {
                                        worksheet.Cells[currentRow, currentCol].Value = dateTime.ToString("M/d/yyyy h:mm");
                                    }
                                    else
                                    {
                                        worksheet.Cells[currentRow, currentCol].Value = itemValue;
                                    }
                                }
                                currentCol = currentCol + 1;
                            }
                        }
                        currentRow = currentRow + 1;
                    }
                    worksheet.Cells[1, 1, currentRow, currentCol].Style.Font.Size = 10;
                    worksheet.Cells[1, 1, currentRow, currentCol].Style.Font.Name = GetFontType(globals.UserFontType);
                    worksheet.Cells.AutoFitColumns(0);  //Autofit columns for all cells

                    // save our new workbook and we are done!
                    package.Save();
                }
            }

            PowerMacroHandling.RunMacros(globals, filename);

            if (globals.IsEffectsDelivery) CrystalFunctions.ApplyEffectsDelivery(globals, filename);
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
            if (collection == null || collection.Count <= 0) return;
            var filename = globals.GetFileName();
            var newFile = new FileInfo(filename);
            if (newFile.Exists)
            {
                newFile.Delete();  // ensures we create a new workbook
                newFile = new FileInfo(filename);
            }
            using (var package = new ExcelPackage(newFile))
            {
                // add a new worksheet to the empty workbook
                var worksheet = package.Workbook.Worksheets.Add(Sheet1);

                var currentRow = 1;
                var currentCol = 1;

                var tType = collection[0].GetType();
                var tProperties = new List<PropertyInfo>(tType.GetProperties());

                if (includeHeaders)
                {
                    foreach (var field in fieldList)
                    {
                        var bAddHeader = true;
                        var fieldName = field;
                        var asIndex = fieldName.IndexOf(" AS ", StringComparison.OrdinalIgnoreCase);
                        if (asIndex >= 0)
                        {
                            fieldName = field.Substring(asIndex + 4);
                        }
                        if (fieldName == "")
                        {
                            fieldName = field.Substring(0, asIndex);
                        }
                        if (fieldName.ToLower() == "rptbreaks")
                        {
                            if (globals.IsParmValueOn(WhereCriteria.CBSUPPRPTBRKS))
                            {
                                bAddHeader = false;
                            }
                        }
                        if (bAddHeader)
                        {
                            worksheet.Cells[currentRow, currentCol].Value = fieldName.NormalizeColumnHeader();
                            currentCol = currentCol + 1;
                        }
                    }
                    currentRow = currentRow + 1;
                }

                foreach (var item in collection)
                {
                    currentCol = 1;
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
                            if (globals.IsParmValueOn(WhereCriteria.CBSUPPRPTBRKS))
                            {
                                bAddItem = false;
                            }
                        }
                        if (bAddItem)
                        {
                            var propertyInfo = tProperties.FirstOrDefault(s => s.Name.EqualsIgnoreCase(fieldName));
                            if (propertyInfo != null)
                            {
                                var itemValue = propertyInfo.GetValue(item, null);
                                if (itemValue is DateTime)
                                {
                                    if(new ExceptionRules().ApplyExceptionRules<T>(tProperties, fieldName, item))
                                    {
                                        worksheet.Cells[currentRow, currentCol].Value = string.Empty;
                                    }
                                    else
                                    {
                                        if (dateTimeFormat == "")
                                        {
                                            worksheet.Cells[currentRow, currentCol].Value = ((DateTime)itemValue).ToShortDateString();
                                            // ReSharper disable once PossibleNullReferenceException
                                            worksheet.Cells[currentRow, currentCol].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                                        }
                                        else
                                        {
                                            worksheet.Cells[currentRow, currentCol].Value = ((DateTime)itemValue).ToString(dateTimeFormat);
                                            // ReSharper disable once PossibleNullReferenceException
                                            worksheet.Cells[currentRow, currentCol].Style.Numberformat.Format = dateTimeFormat;
                                        }
                                    }
                                }
                                else if (itemValue is string)
                                {
                                    worksheet.Cells[currentRow, currentCol].Value = itemValue.ToString().Trim();
                                }
                                else
                                {
                                    worksheet.Cells[currentRow, currentCol].Value = itemValue;
                                }
                            }
                            else
                            {
                                worksheet.Cells[currentRow, currentCol].Value = string.Empty;
                            }
                            currentCol = currentCol + 1;
                        }
                    }
                    currentRow = currentRow + 1;
                }

                worksheet.Cells[1, 1, currentRow, currentCol].Style.Font.Size = 10;
                worksheet.Cells[1, 1, currentRow, currentCol].Style.Font.Name = GetFontType(globals.UserFontType);
                worksheet.Cells.AutoFitColumns(0);  //Autofit columns for all cells

                // save our new workbook and we are done!
                package.Save();
            }

            PowerMacroHandling.RunMacros(globals, filename);

            if (globals.IsEffectsDelivery) CrystalFunctions.ApplyEffectsDelivery(globals, filename);
        }


        private static void BuildWorksheet<T>(IReadOnlyList<T> collection, ReportGlobals globals, bool includeHeaders, FileInfo newFile, string filename)
        {
            using (var package = new ExcelPackage(newFile))
            {
                // add a new worksheet to the empty workbook
                var worksheet = package.Workbook.Worksheets.Add(Sheet1);
                var currentRow = 1;
                var currentCol = 1;

                var tType = collection[0].GetType();
                var tProperties = new List<PropertyInfo>(tType.GetProperties());
                var propertiesToExport = GetPropertiesToExport(tProperties);

                if (includeHeaders) BuildHeader(tProperties, worksheet, propertiesToExport, ref currentRow);

                //Keep in mind that we may need datetimeFormat in the future, we just need to pass globals.DateDisplayFormat in last parameter
                BuildRows(collection, tProperties, worksheet, propertiesToExport, ref currentRow, string.Empty);

                SetWorksheetFontAndAutoFitColumns(worksheet, currentCol, currentRow, globals);

                // save our new workbook and we are done!
                package.Save();
            }
        }

        private static List<string> GetPropertiesToExport(List<PropertyInfo> tProperties)
        {
            var propertiesToExport = (from propertyInfo in tProperties select propertyInfo.Name).ToList();
            if (!propertiesToExport.Any())
            {
                throw new ExportPropertiesException("Class not marked as exportable, so no properties found to add to worksheet.");
            }

            return propertiesToExport;
        }

        private static void BuildHeader(List<PropertyInfo> tProperties, ExcelWorksheet worksheet, List<string> propertiesToExport, ref int currentRow)
        {
            var currentCol = 1;
            foreach (var propertyInfo in tProperties)
            {
                if (propertiesToExport.Contains(propertyInfo.Name))
                {
                    worksheet.Cells[currentRow, currentCol].Value = propertyInfo.Name.NormalizeColumnHeader();
                    currentCol = currentCol + 1;
                }
            }
            currentRow = currentRow + 1;
        }

        private static void BuildRows<T>(IReadOnlyList<T> collection, List<PropertyInfo> tProperties, ExcelWorksheet worksheet, List<string> propertiesToExport, ref int currentRow, string datetimeFormat)
        {
            int currentCol;
            foreach (var item in collection)
            {
                currentCol = 1;
                foreach (var propertyInfo in tProperties)
                {
                    if (propertiesToExport.Contains(propertyInfo.Name))
                    {
                        var itemValue = propertyInfo.GetValue(item, null);
                        switch (propertyInfo.PropertyType.Name)
                        {
                            case "DateTime":
                                DateTime dateTime;
                                if (null != itemValue && DateTime.TryParse(itemValue.ToString(), out dateTime))
                                {
                                    if (datetimeFormat.Equals(string.Empty))
                                    {
                                        worksheet.Cells[currentRow, currentCol].Value = dateTime.ToString("M/d/yyyy HH:mm");
                                    }
                                    else
                                    {
                                        worksheet.Cells[currentRow, currentCol].Value = dateTime.ToString(datetimeFormat);
                                    }
                                }
                                break;
                            case "String":
                                itemValue = itemValue ?? "";
                                worksheet.Cells[currentRow, currentCol].Value = itemValue.ToString().Trim();
                                break;
                            default:
                                worksheet.Cells[currentRow, currentCol].Value = itemValue;
                                break;
                        }
                        currentCol = currentCol + 1;
                    }
                }
                currentRow = currentRow + 1;
            }
        }

        private static void SetWorksheetFontAndAutoFitColumns(ExcelWorksheet worksheet, int currentCol, int currentRow, ReportGlobals globals)
        {
            worksheet.Cells[1, 1, currentRow, currentCol].Style.Font.Size = 10;
            worksheet.Cells[1, 1, currentRow, currentCol].Style.Font.Name = GetFontType(globals.UserFontType);
            worksheet.Cells.AutoFitColumns(0);  //Autofit columns for all cells
        }

        private static string GetFontType(string fontType)
        {
            //Note: .NET raw XLSX will use the user's font, if something other than the default. 
            //This is changed behavior from FoxPro reports, so output will not match in this regard.
            if (fontType == Constants.UserDefinedReport || fontType.IsNullOrWhiteSpace() || fontType == Constants.DefaultFontPlaceholder)
                return "Arial";

            return fontType;
        }


    }

}
