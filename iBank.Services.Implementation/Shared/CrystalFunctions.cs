using CODE.Framework.Core.Utilities;
using com.ciswired.libraries.CISLogger;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Domain.Helper;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Utilities;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using Domain;
using ILogger = com.ciswired.libraries.CISLogger.ILogger;
using Domain.Orm.Classes;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Repository.SQL.Repository;
using iBank.Services.Implementation.Utilities.eFFECTS;
using Workbook = Microsoft.Office.Interop.Excel.Workbook;

namespace iBank.Services.Implementation.Shared
{
    public static class CrystalFunctions
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Handles all functions that are required for all crystal reports, including setting parameters and preparing the logo. 
        /// </summary>
        /// <param name="reportSource"></param>
        /// <param name="globals"></param>
        /// <returns>A list of all parameters that still need to be set by the calling program. This can be useful in debugging.</returns>
        public static List<string> SetupCrystalReport(ReportDocument reportSource, ReportGlobals globals)
        {
            //set parameters
            var success = SetParameters(reportSource, globals);
            AddLogo(reportSource, globals);

            return success;
        }

        public static void CreatePdf(ReportDocument reportSource, ReportGlobals globals)
        {
            var fontHandler = new FontHandler();
            if (globals.OutputLanguage == null) globals.OutputLanguage = globals.UserLanguage;
            reportSource = fontHandler.DrawPdfWithUserFont(reportSource, globals.OutputLanguage, globals.UserFontType);
            //Fix page margin so it won't have wide margin on the left, that causes right side to be cutoff 
            try
            {
                reportSource.PrintOptions.ApplyPageMargins(new PageMargins(180, 240, 20, 240));
            }
            catch
            { 
                //just ignore it, use whatever page margin it has, mostly could be the landscape;
            }

            var fileName = globals.GetFileName();
            LoggingMediator.Log($"Creating file {fileName}");
            LOG.Info($"Creating file {fileName}");
            var outputType = globals.GetParmValue(WhereCriteria.OUTPUTTYPE);

            using (reportSource)
            {
                switch (outputType.ToUpper())
                {
                    case "XX":
                        CreateXlsFile(reportSource, fileName);
                        fileName = ConvertXlsToXlsxAndDeleteXlsFile(fileName, globals);
                        break;
                    case "XG":
                    case "X":
                        reportSource.ExportToDisk(ExportFormatType.Excel, fileName);
                        break;
                    case "RG":
                    case "R":
                        reportSource.ExportToDisk(ExportFormatType.WordForWindows, fileName);
                        break;
                    default:
                        reportSource.ExportToDisk(ExportFormatType.PortableDocFormat, fileName);
                        break;
                }

                reportSource.Close();
                reportSource.Dispose();
            }

            if (globals.IsEffectsDelivery) ApplyEffectsDelivery(globals, fileName);
        }

        private static void CreateXlsFile(ReportDocument reportSource, string fileName)
        {
            var crFormatTypeOptions = new ExcelFormatOptions
            {
                ExcelUseConstantColumnWidth = false,
                ExcelTabHasColumnHeadings = true
            };

            //CrFormatTypeOptions.ShowGridLines = true;
            //CrFormatTypeOptions.ConvertDateValuesToString = true;
            //CrFormatTypeOptions.ExcelAreaGroupNumber = 1;
            //CrFormatTypeOptions.ExcelAreaType = AreaSectionKind.Detail;
            //CrFormatTypeOptions.ExcelConstantColumnWidth = 9.5;
            //CrFormatTypeOptions.ExportPageBreaksForEachPage = true;
            //CrFormatTypeOptions.ExportPageHeadersAndFooters = ExportPageAreaKind.OnEachPage;
            //CrFormatTypeOptions.FirstPageNumber = 1;
            //CrFormatTypeOptions.LastPageNumber = 2;
            //CrFormatTypeOptions.UsePageRange = true;

            var crExportOptions = reportSource.ExportOptions;
            var crDiskFileDestinationOptions = new DiskFileDestinationOptions {DiskFileName = fileName}; //$"C:\\CScode\\Reports\\DEMO\\{Guid.NewGuid()}.xlsx";

            crExportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
            crExportOptions.ExportFormatType = ExportFormatType.Excel;
            crExportOptions.DestinationOptions = crDiskFileDestinationOptions;
            crExportOptions.FormatOptions = crFormatTypeOptions;
            reportSource.Export();
        }

        public static void ApplyEffectsDelivery(ReportGlobals globals, string fileName)
        {
            var handler = new EffectsOuputHandler(globals.EProfileNumber,
                new iBankClientQueryable(globals.AgencyInformation.ServerName, globals.AgencyInformation.DatabaseName), new iBankMastersQueryable());
            var returnInfo = handler.Process(globals.ProcessKey, globals, globals.User.SGroupNumber, fileName);

            globals.ReportInformation.ReturnCode = returnInfo.ReturnCode;
            globals.ReportInformation.ReturnText = returnInfo.ReturnMessage;
        }

        private static string ConvertXlsToXlsxAndDeleteXlsFile(string originalXlsFile, ReportGlobals globals)
        {
            try
            {
                if (File.Exists(originalXlsFile))
                {
                    var eWorkbook = GetOpenWorkbook(originalXlsFile);
                            
                    var saveFileName = Path.ChangeExtension(originalXlsFile, ".xlsx");
                            
                    SaveAndCloseWorkbook(eWorkbook, saveFileName);
                            
                    File.Delete(originalXlsFile);

                    globals.ReportInformation.Href = globals.ReportInformation.Href.Replace(".xls", ".xlsx");

                    return saveFileName;
                }
            }
            catch (Exception ex)
            {
                LOG.Warn(ex.Message.FormatMessageWithReportLogKey(globals.ReportLogKey), ex);
            }

            return originalXlsFile;
        }

        private static Workbook GetOpenWorkbook(string fileName)
        {
            Application excelApp = new Application { Visible = false };
            return excelApp.Workbooks.Open(fileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

        }

        private static void SaveAndCloseWorkbook(Workbook eWorkbook, string newFileName)
        {
            eWorkbook.SaveAs(newFileName, XlFileFormat.xlOpenXMLWorkbook, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                XlSaveAsAccessMode.xlNoChange, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            eWorkbook.Close(false, Type.Missing, Type.Missing);
        }

        private static List<string> SetParameters(ReportDocument reportSource, ReportGlobals globals)
        {
            var country = globals.GetParmValue(WhereCriteria.COUNTRY);
            var intlQuery = new GetSettingsByCountryAndLangCodeQuery(new iBankMastersQueryable(), country, globals.UserLanguage);
            var intl = intlQuery.ExecuteQuery();
            intl = UpdateCurrencySettings(intl, globals);

            //keep track of parameters we aren't able to set
            var missing = new List<string>();
            var list = reportSource.ParameterFields;

            foreach (var parameterField in list)
            {
                var parm = (ParameterField)parameterField;
                var parmName = parm.Name.Trim().ToUpper();

                //Find exact match (XTICKET,xTotalCost...); or VarName starts with "lc_..", "ll_..", "lt_.." etc, eg: xAccount matches lt_Account.          
                if (parmName.Left(1).EqualsIgnoreCase("X"))
                {
                    var labelTrans = globals.LanguageVariables.FirstOrDefault(s => s.VariableName.ToUpper() == parmName || s.VariableName.ToUpper().Substring(3) == parmName.Substring(1));
                    var translation = labelTrans == null
                        ? parmName + " NOT FOUND"
                        : labelTrans.Translation.Replace("<br>", Environment.NewLine);

                    try
                    {
                        reportSource.SetParameterValue(parmName, translation);
                    }
                    catch (Exception e)
                    {

                        throw new Exception($"Failed to set parameter {parmName} with value {translation}", e);
                    }
                }
                else
                {
                    object parmVal = string.Empty;
                    var parmFound = true;
                    //look for other parameters that are in all reports. 
                    switch (parmName)
                    {
                        case "CRPTTITLE":
                        case "RPTTITLE":
                            parmVal = globals.ReportTitle;
                            break;
                        case "GHSTPREPREF":
                            parmVal = globals.HstPrePref;
                            break;
                        case "CUSERID":
                            parmVal = globals.User.UserId;
                            break;
                        case "CURRENCYSYMBOL":
                            parmVal = intl.Symbol;
                            break;
                        case "CURRENCYSYMBOLPOSITION":
                            parmVal = intl.Position;
                            break;
                        case "DECIMALSEPARATOR":
                            parmVal = intl.Decimal;
                            break;
                        case "THOUSANDSSEPARATOR":
                            parmVal = intl.Thousand;
                            break;
                        case "CBRKNAME1":
                            parmVal = globals.User.Break1Name;
                            break;
                        case "CBRKNAME2":
                            parmVal = globals.User.Break2Name;
                            break;
                        case "CBRKNAME3":
                            parmVal = globals.User.Break3Name;
                            break;
                        case "GPROCKEY":
                            parmVal = globals.ProcessKey;
                            break;
                        case "NPAGEBRKLVL":
                            parmVal = SetPageBreakLevel(globals);
                            break;
                        case "LACCTPGBRK":
                            parmVal = SetAccountPageBreak(globals);
                            break;
                        case "CDATEDESC":
                            parmVal = globals.BuildDateDesc();
                            break;
                        case "CWHERETXT":
                            if (globals.IsParmValueOn(WhereCriteria.SUPPRESSCRIT))
                            {
                                parmVal = string.Empty;
                            }
                            else
                            {
                                if (globals.WhereText.Trim().Right(1).Equals(";"))
                                    globals.WhereText = globals.WhereText.Trim().RemoveLastChar();
                                parmVal = globals.WhereText;
                            }
                            break;
                        case "CACCTNAME":
                            if (globals.ReplaceCAcctNameAndAccountInParamsWithPickListName && !string.IsNullOrEmpty(globals.PickListName))
                            {
                                parmVal = globals.PickListName;
                                break;
                            }

                            var title2 = globals.GetParmValue(WhereCriteria.TITLEACCT2);
                            parmVal = string.IsNullOrEmpty(title2) ? globals.AccountName : title2;
                            break;
                        case "CCOPYRIGHT":
                            parmVal = globals.CopyRight;
                            break;
                        case "CSAVEDRNAME":
                            parmVal = string.Empty;//None of the FoxPro versions show anything here. 
                            break;
                        case "DATEDISPLAY":
                            parmVal = globals.DateDisplay;
                            break;
                        default:
                            if (!parmName.EqualsIgnoreCase("LOGOPATH") && !parmName.EqualsIgnoreCase("LOGOSUPPRESS"))
                            {
                                missing.Add(parmName);
                                parmFound = false;
                            }

                            break;
                    }

                    try
                    {
                        if (parmFound && !parmName.EqualsIgnoreCase("LOGOPATH") && !parmName.EqualsIgnoreCase("LOGOSUPPRESS"))
                        {
                            reportSource.SetParameterValue(parmName, parmVal);
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Failed to set parameter {parmName} with value {parmVal}", e);
                    }

                }

            }

            return missing;
        }

        private static int SetPageBreakLevel(ReportGlobals globals)
        {
            //if "Page Break on Rpt Break Level" is selected (db pagebrklvl value =1) then
            //ReportBreaks by   0-page break,
            //                  1 -break on break1
            //                  2 -break on break2
            //                  3 -break on break3 
            //else use the pagebreaklevel
            if (globals.User.PageBreakLevel == 1)
            {
                return globals.User.ReportBreaks;
            }
            return globals.User.PageBreakLevel;
        }
        private static bool SetAccountPageBreak(ReportGlobals globals)
        {
            //if "Page Break on Account" is selected, has page break on acct
            //if "No Page Break on Break Level" is selected (db pagebrklvl value = 0), no page break 
            return globals.User.AccountPageBreak
                ? true
                : globals.User.PageBreakLevel == 0
                    ? false
                    : globals.User.AccountBreak;
        }

        private static void AddLogo(ReportDocument reportSource, ReportGlobals globals)
        {
            LOG.Debug($"AddLogo - User Number:[{globals.UserNumber}] | HasLog:[{globals.HasLogo}] | Bytes:[{globals.LogoBytes.Length}] | Logo File Name:[{globals.LogoFileName}]");
            
            try
            {
                if (globals.HasLogo || globals.LogoBytes.Length > 0)
                {
                    var fileName = SharedProcedures.SaveTempLogo(globals.LogoFileName, globals.LogoBytes);

                    Bitmap resizedImage = null;
                    using (var image = Image.FromFile(fileName))
                    {
                        var desiredSize = new Size(192, 96);
                        resizedImage = ImageSizer.ResizeImage(image, desiredSize);
                    }

                    resizedImage.Save(fileName);

                    reportSource.SetParameterValue("LogoPath", fileName);
                    reportSource.SetParameterValue("LogoSuppress", false);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Failed to create Logo temp file. Please make sure report file has a parameter called 'LogoPath' or 'LogoSuppress' ", e);
            }
        }

        private static InternationalSettingsInformation UpdateCurrencySettings(InternationalSettingsInformation intl, ReportGlobals globals)
        {
            var moneyType = globals.GetParmValue(WhereCriteria.MONEYTYPE);
            if (moneyType == "") moneyType = "USD";
            if (!string.IsNullOrEmpty(moneyType))
            {
                var curSettings = new GetCurrencySettingsByMoneyTypeQuery(new iBankMastersQueryable(), moneyType).ExecuteQuery();
                if (curSettings != null)
                {
                    intl.Symbol = curSettings.csymbol.Trim();
                    intl.Position = curSettings.cleftright.Trim();
                    if (!string.IsNullOrEmpty(curSettings.cdecimal))
                    {
                        intl.Decimal = curSettings.cdecimal;
                    }
                    if (!string.IsNullOrEmpty(curSettings.cdecimal))
                    {
                        intl.Thousand = curSettings.cthousands;
                    }
                }
            }

            return intl;
        }

        public static void GenXmlDataset(Table table, string tableName, string fileName)
        {
            var xmlElements = new List<string>
            {
                "<?xml version=\"1.0\" standalone=\"yes\"?>",
                "<NewDataSet>",
                "<xs:schema id=\"NewDataSet\" xmlns=\"\" xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xmlns:msdata=\"urn:schemas-microsoft-com:xml-msdata\">",
                " <xs:element name=\"NewDataSet\" msdata:IsDataSet=\"true\" msdata:UseCurrentLocale=\"true\">",
                "<xs:complexType>",
                "<xs:choice minOccurs=\"0\" maxOccurs=\"unbounded\">",
                $"<xs:element name=\"{tableName}\">",
                "<xs:complexType>",
                "<xs:sequence>"
            };

            var rawDataClass = new List<string>();
            var ctor = new List<string> { "\tpublic FinalData()", "\t{" };

            var fields = table.Fields;


            foreach (var field in fields)
            {
                var element = "<xs:element name=\"{0}\" type=\"xs:{1}\" minOccurs=\"0\" />";
                var field2 = (DatabaseFieldDefinition)field;
                var name = Char.ToUpper(field2.Name[0]) + field2.Name.Substring(1);
                var type = field2.ValueType;

                //if (name.EqualsIgnoreCase("class"))
                //    name = "ClassCode";
                switch (type)
                {
                    //case FieldValueType.Int8sField:
                    //    break;
                    //case FieldValueType.Int8uField:
                    //    break;
                    //case FieldValueType.Int16sField:
                    //    break;
                    //case FieldValueType.Int16uField:
                    //    break;
                    case FieldValueType.Int8sField:
                    case FieldValueType.Int16sField:
                    case FieldValueType.Int32sField:
                        xmlElements.Add(string.Format(element, name, "int"));
                        rawDataClass.Add("\tpublic int " + name + "{ get; set; } = 0;");
                        ctor.Add("\t\t" + name + " = 0;");
                        break;
                    //case FieldValueType.Int32uField:
                    //    break;
                    case FieldValueType.NumberField:
                    case FieldValueType.CurrencyField:
                        xmlElements.Add(string.Format(element, name, "decimal"));
                        rawDataClass.Add("\tpublic decimal " + name + "{ get; set; } = 0m;");
                        ctor.Add("\t\t" + name + " = 0m;");
                        break;
                    //case FieldValueType.CurrencyField:
                    //    break;
                    case FieldValueType.BooleanField:
                        xmlElements.Add(string.Format(element, name, "boolean"));
                        rawDataClass.Add("\tpublic bool " + name + "{ get; set; } = false;");
                        ctor.Add("\t\t" + name + " = false;");
                        break;
                    case FieldValueType.DateField:
                        xmlElements.Add(string.Format(element, name, "dateTime"));
                        rawDataClass.Add("\tpublic DateTime " + name + "{ get; set; } = DateTime.MinValue;");
                        ctor.Add("\t\t" + name + " = DateTime.MinValue;");
                        break;
                    //case FieldValueType.TimeField:
                    //    break;
                    case FieldValueType.PersistentMemoField:
                    case FieldValueType.StringField:
                        xmlElements.Add(string.Format(element, name, "string"));
                        rawDataClass.Add("\tpublic string " + name + "{ get; set; } = string.Empty;");
                        ctor.Add("\t\t" + name + " = string.Empty;");
                        break;
                    //case FieldValueType.TransientMemoField:
                    //    break;
                    case FieldValueType.BlobField:
                        throw new Exception("Cannot convert a blob field; did you remove the Logo subreport?");
                    case FieldValueType.DateTimeField:
                        xmlElements.Add(string.Format(element, name, "dateTime"));
                        rawDataClass.Add("\tpublic DateTime " + name + "{ get; set; } = DateTime.MinValue;");
                        ctor.Add("\t\t" + name + " = DateTime.MinValue;");
                        break;
                    //case FieldValueType.BitmapField:
                    //    break;
                    //case FieldValueType.IconField:
                    //    break;
                    //case FieldValueType.PictureField:
                    //    break;
                    //case FieldValueType.OleField:
                    //    break;
                    //case FieldValueType.ChartField:
                    //    break;
                    //case FieldValueType.bookRateCounteAsInputField:
                    //    break;
                    //case FieldValueType.UnknownField:
                    //    break;
                    default:
                        throw new ArgumentOutOfRangeException(string.Format("Cannot identify property {0} of type {1}", name, type));
                }
            }
            xmlElements.Add("</xs:sequence>");
            xmlElements.Add("</xs:complexType>");
            xmlElements.Add("</xs:element>");
            xmlElements.Add("</xs:choice>");
            xmlElements.Add("</xs:complexType>");
            xmlElements.Add("</xs:element>");
            xmlElements.Add("</xs:schema>");
            xmlElements.Add("</NewDataSet>");

            ctor.Add("\t}");
            ctor.AddRange(rawDataClass);

            File.WriteAllLines(fileName + ".xml", xmlElements);
            File.WriteAllLines(fileName + ".cs", ctor);
        }

    }
}
