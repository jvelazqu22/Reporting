using Domain.Helper;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iBank.Services.Implementation.ReportPrograms.UserDefined;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers;
using iBank.Services.Implementation.Shared.MacroHelpers;

namespace iBank.Services.Implementation.Utilities.ExportHelpers
{
    public static class StringListToXlsxHandler
    {
        private const string Sheet1 = "Sheet1";

        private static List<string> GetFieldList(List<UserReportColumnInformation> colList)
        {
            var fieldList = new List<string>();
            for (var i = 0; i < colList.Count; i++)
            {
                var header1 = colList[i].Header1.NormalizeColumnHeader();
                var header2 = colList[i].Header2.NormalizeColumnHeader();
                var header = header1 == "" ?
                    header2 :
                    header2 != "" ?
                        (header1 + "_" + header2) :
                        header1;
                fieldList.Add(header);
            }
            return fieldList;
        }

        //used in user defined report
        public static void StringListToXlsx(string filename, IReadOnlyList<List<string>> collection, List<UserReportColumnInformation> colList, ReportGlobals globals, bool includeHeaders = true)
        {
            var fieldList = GetFieldList(colList);
            var typeList = colList.Select(s => s.ColumnType).ToList();
            var tableList = colList.Select(s => s.TableName).ToList();

            if (collection == null || collection.Count <= 0) return;

            var suppressHandler = new TripFieldsSuppressHandler(globals);

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

                if (includeHeaders)
                {
                    foreach (var field in fieldList)
                    {
                        worksheet.Cells[currentRow, currentCol].Value = field;
                        currentCol = currentCol + 1;
                    }
                    currentRow = currentRow + 1;
                }

                foreach (var row in collection)
                {
                    currentCol = 0;

                    foreach (var item in row)
                    {
                        var val = suppressHandler.GetDataAccordingToTheSuppressType(item.Trim(), fieldList[currentCol], typeList[currentCol], tableList[currentCol], row[row.Count - 1] == "1");
                        var isCurrency = suppressHandler.IsCurrencyField(typeList[currentCol]);
                        currentCol = currentCol + 1;

                        if (isCurrency && !string.IsNullOrWhiteSpace(val))
                        {
                            worksheet.Cells[currentRow, currentCol].Value = Convert.ToDecimal(val);
                        }
                        else if (IsNumeric(val))
                        {
                            worksheet.Cells[currentRow, currentCol].Value = Convert.ToInt64(val);
                        }
                        else
                        {
                            worksheet.Cells[currentRow, currentCol].Value = val;
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
        }

        private static bool IsNumeric(string val)
        {
            if (val.Length > 1)
            {
                //not long string only evaluate to 1 or 0
                if (val.IsNumeric())
                {
                    if (Convert.ToInt64(val) == 1 || Convert.ToInt64(val) == 0)
                    {
                        return true;
                    }
                }
                //we want to retain leading 0, eg: 0004617871
                if (val.Left(1) == "0") return false;
                else return val.IsNumeric();
            }

            return val.IsNumeric();
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
