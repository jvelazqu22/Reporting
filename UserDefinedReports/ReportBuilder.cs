using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Configuration;
using CODE.Framework.Core.Utilities.Extensions;

using UserDefinedReports.Classes;
using com.ciswired.libraries.CISLogger;
using Microsoft.Reporting.WinForms;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Domain.Helper;

namespace UserDefinedReports
{
    public class ReportBuilder
    {
        private static readonly ILogger Log = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static readonly XNamespace Xmlns = @"http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition";
        private static readonly XNamespace Xmlnsrd = @"http://schemas.microsoft.com/SQLServer/reporting/reportdesigner";

        public List<ReportParameter> Parameters { get; set; }
        public List<CustomColumnInformation> Columns { get; set; }
        public List<List<string>> Rows { get; set; }
        public PageParameter PageParameter { get; set; }
        public int RowCount { get; set; }//due to how credits are handled, this is not simply "Rows.Count". 
        public string Style { get; set; }
        public bool SuppressDetail { get; set; }
        public string FileName { get; set; }
        public string ErrorMessage { get; set; }
        public byte[] LogoBytes { get; set; }
        public string ReportTotalText { get; set; }

        public string OutputExtension { get; set; }


        private XDocument _xDocument;
        private readonly string _templateName;
        private readonly StyleManager _styleManager;

        //Report constants
        private const double ColumnGap = .1;
        private const double LimitedWidth = 3;
        private const double LogoOffset = 3.5;
        private const double CopyRight2Offset = 2;
        private const double TotalCellHeightFactor = 1.2;
        private const double SpanningCellHeightFactor = 2;

        public ReportBuilder()
        {
            Parameters = new List<ReportParameter>();
            Columns = new List<CustomColumnInformation>();
            Rows = new List<List<string>>();
            FileName = string.Empty;
            
            _templateName = "StandardPortrait.rdlc";
            Log.Info($"Default Template Name:[{_templateName}]");
            _styleManager = new StyleManager();
        }

        public void AddParameter(string name, string value)
        {
            Parameters.Add(new ReportParameter(name, value));
        }

        public bool BuildReport(DestinationSwitch outputFormat)
        {
            Log.Info("BuildReport - Start");
            var sw = Stopwatch.StartNew();

            Log.Debug("VerifyUserReportValid - Start");
            if (!VerifyUserReportValid()) return false;

            Log.Info("OrderEachColumn - Start");
            OrderEachColumn();

            Log.Info("SetReportDirectory - Start");
            SetReportDirectory();

            Log.Info("SetPageParameter - Start");
            SetPageParameter();

            Log.Info("SetPageProperties - Start");
            SetPageProperties();

            Log.Info("SetStyle - Start");
            SetStyle();

            Log.Info("CreateTablix - Start");
            if (!CreateTablix()) return false;

            Log.Info("SetReportLogo - Start");
            SetReportLogo();

            Log.Info("BuildReport - Start");
            if (!SaveReportTemplateFile()) return false;

            sw.Stop();
            Log.Info($"BuildReport - End | Elasped:[{sw.Elapsed}]");
            return RenderReport(outputFormat);
        }

        private bool VerifyUserReportValid()
        {
            if (!Columns.Any())
            {
                ErrorMessage = "No columns defined!";
                return false;
            }
            if (string.IsNullOrWhiteSpace(FileName))
            {
                ErrorMessage = "Please set a filename, complete with path!";
                return false;
            }
            var path = Path.GetDirectoryName(FileName);
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            {
                ErrorMessage = "Path not found!";
                return false;
            }
            return true;
        }

        public void OrderEachColumn()
        {
            //Make sure that the cells are ordered, and uniquely numbered. 
            Columns = Columns.OrderBy(s => s.Order).ToList();
            for (var i = 1; i <= Columns.Count; i++)
            {
                Columns[i - 1].Order = i;
                //also verify that no column is shorter than 4
                if (Columns[i - 1].Width < 4)
                    Columns[i - 1].Width = 4;                
            }
            Log.Debug($"OrderEachColumn is done");
        }

        private void SetReportDirectory()
        {
            var rptDirectory = ConfigurationManager.AppSettings["CustomReportsDirectory"];
            Log.Info($"Custom Reports Directory is [{rptDirectory}] ");
            _xDocument = XDocument.Load(rptDirectory.AddBS() + _templateName);
        }

        private void SetReportLogo()
        {
            if (LogoBytes.Length > 0)
            {
                var image = _xDocument.Descendants().FirstOrDefault(s => s.Name.LocalName.Equals("ImageData"));
                image?.ReplaceWith(new XElement(Xmlns + "ImageData", Convert.ToBase64String(LogoBytes)));
            }
            Log.Debug($"SetReportLogo is done");
        }

        private string GetTemplateName()
        {
            //return "C:\\temp\\temp.rdlc";
            return FileName.Substring(0, FileName.LastIndexOf(".", StringComparison.Ordinal)) + ".rdlc";
        }


        private bool SaveReportTemplateFile()
        {
            try
            {
                var tempfile = GetTemplateName();
                _xDocument.Save(tempfile);
                Log.Info($"SaveReportTemplateFile - Saved:[{tempfile}]");
                return true;
            }
            catch (Exception e)
            {
                ErrorMessage = "Unable to save temp file:" + e.Message;
                Log.Error($"SaveReportTemplateFile - Error:[{ErrorMessage}]");
                return false;
            }
        }
        /// <summary>
        /// Reports can be one of four styles: Fresh, Bold, Grayscale or Classic. This function sets the headers and footers accordingly. 
        /// </summary>
        private void SetStyle()
        {
            Style = Style.ToUpper().Trim();
            var fontColor = "Navy";
            Log.Info($"SetStyle - Default font color:[{fontColor}]"); 

            var recs = _xDocument.Descendants().Where(s => s.Name.LocalName.Equals("Rectangle")).ToList();
            if (!recs.Any())
            {
                Log.Warn($"SetStyle - [{GetTemplateName()}] No Rectangle elements");
                return;
            }

            //Style = "";
            var styleValues = StyleValues.Build(Style);
            try
            {
                Parallel.ForEach(recs, new ParallelOptions { MaxDegreeOfParallelism = 10 }, rec =>
                {
                    var hidden = rec.Descendants().FirstOrDefault(s => s.Name.LocalName.Equals("Hidden"));
                    if (hidden == null)
                    {
                        hidden = new XElement(Xmlns + "Hidden", "true");
                        //var vis = new XElement(Xmlns + "Visibility", hidden);
                        //   rec.Add(vis);
                    }
                    var color = rec.Descendants().FirstOrDefault(s => s.Name.LocalName.Equals("BackgroundColor"));
                    if (color == null)
                    {
                        color = new XElement(Xmlns + "BackgroundColor", "Gainsboro");
                        var border = new XElement(Xmlns + "Border", new XElement(Xmlns + "Style", "None"));
                        var style = new XElement(Xmlns + "Style", border, color);
                        rec.Add(style);
                    }

                    hidden.Value = styleValues.Hidden;
                    color.Value = styleValues.Color;
                    fontColor = styleValues.FontColor;

                });
            }
            catch (Exception e)
            {
                if (e.InnerException != null) throw e.InnerException;
                throw;
            }
            Log.Debug($"SetStyle - All Rectangles Size/Color/Style are set");

            var pageHeader = _xDocument.Descendants().FirstOrDefault(s => s.Name.LocalName.Equals("PageHeader"));
            if (pageHeader == null)
            {
                Log.Warn($"SetStyle - [{GetTemplateName()}] No PageHeader");
                return;
            };
            var textBoxes = pageHeader.Descendants().Where(s => s.Name.LocalName.Equals("Textbox"));
            foreach (var textBox in textBoxes)
            {
                //In Fresh style, the parameters stay black
                var xAttribute = textBox.Attribute("Name");
                if (xAttribute != null && (xAttribute.Value.Equals("ReportParameters") && Style.ToUpper().Equals("FRESH")))
                    continue;

                var headerTextRuns = textBox.Descendants().Where(s => s.Name.LocalName.Equals("TextRun")).ToList();
                if (!headerTextRuns.Any()) continue;

                foreach (var textRun in headerTextRuns)
                {
                    var fontColorNode = textRun.Descendants().FirstOrDefault(s => s.Name.LocalName.Equals("Color"));
                    if (fontColorNode != null)
                        fontColorNode.Value = fontColor;
                    else
                    {
                        fontColorNode = new XElement(Xmlns + "Color", fontColor);
                        var styleNode = textRun.Descendants().FirstOrDefault(s => s.Name.LocalName.Equals("Style"));
                        if (styleNode != null)
                        {
                            styleNode.Add(fontColorNode);
                        }
                        else
                        {
                            styleNode = new XElement(Xmlns + "Style", fontColorNode);
                            textRun.Add(styleNode);
                        }
                    }
                }
            }
            Log.Debug($"SetStyle - Page Header TextRuns Size/Color/Style are set");

            var pageFooter = _xDocument.Descendants().FirstOrDefault(s => s.Name.LocalName.Equals("PageFooter"));
            if (pageFooter == null)
            {
                Log.Warn($"SetStyle - [{GetTemplateName()}] No PageFooter");
                return;
            }

            var textRuns = pageFooter.Descendants().Where(s => s.Name.LocalName.Equals("TextRun"));
            try
            {
                Parallel.ForEach(textRuns, new ParallelOptions { MaxDegreeOfParallelism = 10 }, textRun =>
                {
                    var fontSizeNode = textRun.Descendants().FirstOrDefault(s => s.Name.LocalName.Equals("FontSize"));
                    if (fontSizeNode != null)
                    {
                        fontSizeNode.Value = PageParameter.FontSize + "pt";
                    }

                    var fontColorNode = textRun.Descendants().FirstOrDefault(s => s.Name.LocalName.Equals("Color"));
                    if (fontColorNode != null)
                        fontColorNode.Value = fontColor;
                    else
                    {
                        fontColorNode = new XElement(Xmlns + "Color", Style == "FRESH" ? "Black" : fontColor);
                        var styleNode = textRun.Descendants().FirstOrDefault(s => s.Name.LocalName.Equals("Style"));
                        if (styleNode != null)
                        {
                            styleNode.Add(fontColorNode);
                        }
                        else
                        {
                            styleNode = new XElement(Xmlns + "Style", fontColorNode);
                            textRun.Add(styleNode);
                        }
                    }
                });
            }
            catch (Exception e)
            {
                if (e.InnerException != null) throw e.InnerException;
                throw;
            }
            Log.Debug($"SetStyle - Page Footer TextRuns Size/Color/Style are set");

            //Change all font families to the chosen style
            var fontFamilies = _xDocument.Descendants().Where(s => s.Name.LocalName.Equals("FontFamily"));
            foreach (var fontFamily in fontFamilies)
            {
                fontFamily.Value = styleValues.FontFamily;
            }
            Log.Debug($"SetStyle - Font Families are set");
        }
        
        /// <summary>
        /// Depending on how much data we are trying to fit on a line, this function will determine a font size and page width. 
        /// </summary>
        public void SetPageParameter()
        {
            Log.Debug($"SetPageParameter");
            var fontConversions = new List<PageParameter>
            {
                new PageParameter(8.75, 12.5, 0.15, 8.5, 11, 7.0),
                new PageParameter(8.10, 14, 0.12, 11, 8.5, 10),
                new PageParameter(5.4, 18.0, 0.10, 14, 8.5, 13 )
            };

            var success = false;
            var idx = 1;
            foreach (var fontConversion in fontConversions)
            {
                var allColumnWidths = Columns.Sum(s => (s.Width / fontConversion.FontConverter) + ColumnGap);
                if (allColumnWidths < fontConversion.MaxContentWidth || idx == fontConversions.Count)
                {
                    PageParameter = fontConversion;
                    success = true;
                }
                if (success) break;
                idx++;
            }
            Log.Info($"SetPageParameter - PageParameter/FontConversion is set");
        }
        
        private void SetPageProperties()
        {
            Log.Debug($"SetPageProperties");

            var page = _xDocument.Descendants().FirstOrDefault(s => s.Name.LocalName.Equals("Page"));
            if (page == null)
            {
                Log.Error($"SetPageProperties - [{GetTemplateName()}] Page is missing");
                throw new Exception("Invalid RDLC file!");
            }
            var pageHeight = page.Descendants().FirstOrDefault(s => s.Name.LocalName.Equals("PageHeight"));

            if (pageHeight != null)
            {
                pageHeight.Value = PageParameter.PageHeight + "in";
            }
            else
            {
                page.Add(new XElement(Xmlns + "PageHeight", PageParameter.PageHeight + "in"));
            }
            Log.Debug($"SetPageProperties - PageHeight:[{PageParameter.PageHeight} in]");

            var pageWidth = page.Descendants().FirstOrDefault(s => s.Name.LocalName.Equals("PageWidth"));

            if (pageWidth != null)
            {
                pageWidth.Value = PageParameter.PageWidth + "in";
            }
            else
            {
                page.Add(new XElement(Xmlns + "PageWidth", PageParameter.PageWidth + "in"));
            }
            Log.Debug($"SetPageProperties - PageWidth:[{PageParameter.PageWidth} in]");

            var rects = _xDocument.Descendants().Where(s => s.Name.LocalName.Equals("Rectangle"));

            try
            {
                Parallel.ForEach(rects, new ParallelOptions { MaxDegreeOfParallelism = 10 }, rect =>
                {
                    if (rect.FirstAttribute.Value == "FooterRectangle" || rect.FirstAttribute.Value == "HeaderRectangle")
                    {
                        var elem = rect.Descendants().FirstOrDefault(s => s.Name.LocalName.Equals("Width"));
                        if (elem != null) elem.Value = PageParameter.PageWidth + "in";
                    }

                });
            }
            catch (Exception e)
            {
                if (e.InnerException != null) throw e.InnerException;
                throw;
            }

            Log.Info($"SetPageProperties - Adjusted Footer and Header Width to {PageParameter.PageWidth} in");

            double rptttWidth = GetPropertyValue("Textbox", "ReportTitle", "Width");
            var rptttLeft = (PageParameter.PageWidth - rptttWidth) / 2;
            AdjustItemPosition("Textbox", "ReportTitle", "Left", rptttLeft + "in");
            Log.Info($"SetPageProperties - Adjusted ReportTitle Left Position to {rptttLeft} in");

            rptttWidth = GetPropertyValue("Textbox", "ReportTitle2", "Width");
            rptttLeft = (PageParameter.PageWidth - rptttWidth) / 2;
            AdjustItemPosition("Textbox", "ReportTitle2", "Left", rptttLeft + "in");
            Log.Info($"SetPageProperties - Adjusted ReportTitle2 Left Position to {rptttLeft} in");

            AdjustItemPosition("Textbox", "Copyright2", "Left", PageParameter.PageWidth - CopyRight2Offset + "in");
            Log.Info($"SetPageProperties - Adjusted Copyright2 Left Position to {PageParameter.PageWidth - CopyRight2Offset} in");

            AdjustItemPosition("Image", "Image3", "Left", PageParameter.PageWidth - LogoOffset + "in");
            Log.Info($"SetPageProperties - Adjusted Image Left Position to {PageParameter.PageWidth - LogoOffset } in");
        }
        
        private void AdjustItemPosition(string objType, string objName, string itemName, string itemValue)
        {
            var objs = _xDocument.Descendants().Where(s => s.Name.LocalName.Equals(objType));
            foreach (var obj in objs)
            {
                if (obj.FirstAttribute.Value == objName)
                {
                    var leftElem = obj.Descendants().FirstOrDefault(s => s.Name.LocalName.Equals(itemName));
                    if (leftElem != null) leftElem.Value = itemValue;
                    return;
                }
            }
        }

        ///get an item value
        private double GetPropertyValue(string objType, string objName, string itemName)
        {
            var objs = _xDocument.Descendants().Where(s => s.Name.LocalName.Equals(objType));
            foreach (var obj in objs)
            {
                if (obj.FirstAttribute.Value == objName)
                {
                    var leftElem = obj.Descendants().FirstOrDefault(s => s.Name.LocalName.Equals(itemName));
                    if (leftElem != null) return Convert.ToDouble(leftElem.Value.Replace("in", ""));
                }
            }
            return 1.5;
        }

        /// <summary>
        /// the Tablix contains the grid with all data and headers. 
        /// </summary>
        /// <returns></returns>
        private bool CreateTablix()
        {
            Log.Info($"CreateTablix - Start");
            var sw = Stopwatch.StartNew();

            var cols = _xDocument.Descendants().FirstOrDefault(s => s.Name.LocalName.Equals("TablixColumns"));

            var dataSet = _xDocument.Descendants().FirstOrDefault(s => s.Name.LocalName.Equals("DataSet"));

            if (cols == null || dataSet == null)
            {
                Log.Error($"CreateTablix - [{GetTemplateName()}] TablixColumns or DataSet is missing");
                throw new Exception("Invalid RDLC file!");
            }

            cols.ReplaceNodes(BuildColumns());
            Log.Debug($"CreateTablix - Columns built");

            var rowContainer = _xDocument.Descendants().FirstOrDefault(s => s.Name.LocalName.Equals("TablixRows"));
            if (rowContainer == null)
            {
                Log.Error($"CreateTablix - [{GetTemplateName()}] TablixRows is missing");
                throw new Exception("Invalid RDLC file!");
            }
            var rows = rowContainer.Descendants().Where(s => s.Name.LocalName.Equals("TablixRow")).ToList();

            rows[0].ReplaceNodes(BuildHeaderRow());
            Log.Debug($"CreateTablix - Header Row Built");

            rows[1].ReplaceNodes(BuildDetailRow());
            Log.Debug($"CreateTablix - Detail Row Built");

            AddGroups(rowContainer);
            Log.Debug($"CreateTablix - Groups Added");

            rowContainer.Add(BuildTotalRow());
            Log.Debug($"CreateTablix - Total Row Added");

            rowContainer.Add(BuildSpanningRow("ReportTotal", ReportTotalText));
            Log.Debug($"CreateTablix - Report Total Added");

            //The Note may not always be "Credits count as negative", but leave it as it for future review if need it.
            rowContainer.Add(BuildSpanningRow("RowCount", $"Count:{RowCount}  (Note: Credits count as negative)"));
            Log.Debug($"CreateTablix - Row Count and Credit notes added");

            rowContainer.Add(BuildSpanningRow("FooterRow", "= Parameters!Footer.Value"));
            Log.Debug($"CreateTablix - Footer Added");

            var colHeirarchy = _xDocument.Descendants().FirstOrDefault(s => s.Name.LocalName.Equals("TablixColumnHierarchy"));
            if (colHeirarchy == null)
            {
                Log.Error($"CreateTablix - [{GetTemplateName()}] TablixColumnHierarchy is missing");
                throw new Exception("Invalid RDLC file!");
            }
            colHeirarchy.ReplaceNodes(BuildColumnHeirarchy());
            Log.Debug($"CreateTablix - Heirarchy Added");

            var fields = dataSet.Descendants().FirstOrDefault(s => s.Name.LocalName.Equals("Fields"));

            if (fields == null)
            {
                Log.Error($"CreateTablix - [{GetTemplateName()}] Fields is missing");
                throw new Exception("Invalid RDLC file!");
            }
            fields.ReplaceNodes(BuildDataFields());
            Log.Debug($"CreateTablix - Data Fields Added");

            sw.Stop();
            Log.Info($"CreateTablix - End | Elapsed:[{sw.Elapsed}]");
            return true;
        }
        
        /// <summary>
        /// Adds the data and parameters to the report and generates it. 
        /// </summary>
        /// <returns></returns>
        private bool RenderReport(DestinationSwitch outputFormat)
        {
            Log.Info("RenderReport - Start");
            var sw = Stopwatch.StartNew(); 

            var localReport = new LocalReport
            {
                EnableExternalImages = true,
                ReportPath = GetTemplateName() 
            };

            localReport.SetParameters(Parameters);
            Log.Debug($"RenderReport - SetParameters is done");

            var tempDataTable = new DataTable("Dataset1");
            for (var i = 1; i <= Columns.Count; i++)
            {
                tempDataTable.Columns.Add(new DataColumn("Field" + i));
            }
            tempDataTable.Columns.Add(new DataColumn("HideTripFields", typeof(bool)));
            
            try
            {
                foreach (var row in Rows)
                {
                    var newRow = tempDataTable.NewRow();
                    var cellCount = 1;
                    foreach (var cell in row)
                    {
                        if (cellCount == row.Count)
                            newRow["HideTripFields"] = cell.Trim().ToUpper().Equals("TRUE");
                        else
                            newRow["Field" + cellCount] = cell;
                        cellCount++;
                    }
                    tempDataTable.Rows.Add(newRow);
                }
            }
            catch (Exception ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                throw;//this throw doesn't actually get executed, but the compiler complains if it isn't there
            }                

            localReport.DataSources.Clear(); 
            localReport.DataSources.Add(new ReportDataSource("DataSet1", tempDataTable));
            Log.Debug($"RenderReport - DataSet1 Added");
            
            try
            {
                var reportRenderer = new ReportRenderer(localReport, FileName);
                reportRenderer.Render(outputFormat);
                FileName = reportRenderer.FileName;
            }
            catch (Exception e)
            {
                ErrorMessage = "Unable to save report file:" + e.Message;
                return false;
            }
            sw.Stop();
            Log.Info($"RenderReport - End | Elapsed:[{sw.Elapsed}]");
            return true;
        }


        #region Build rows 
        /// <summary>
        /// Builds a row that contains the field headers
        /// </summary>
        /// <returns></returns>
        private List<XElement> BuildHeaderRow()
        {
            Log.Debug("BuildHeaderRow");
            var elements = new List<XElement>{
                new XElement(Xmlns+"Height",  PageParameter.CellHeight + "in")
            };

            var cellsContainer = new XElement(Xmlns + "TablixCells");

            var cells = new List<XElement>();

            foreach (var column in Columns)
            {
                cells.Add(GetHeaderCell(column));
                Log.Debug($"BuildHeaderRow - Column:[{column.Order}]");
            }

            cellsContainer.ReplaceNodes(cells);
            Log.Debug("BuildHeaderRow - Cells Added");
            elements.Add(cellsContainer);
            Log.Debug("BuildHeaderRow - Cell Container Added");
            return elements;
        }

        /// <summary>
        /// Builds the tablix row that contains all the data. 
        /// </summary>
        /// <returns></returns>
        private List<XElement> BuildDetailRow()
        {
            Log.Info("BuildDetailRow");
            var elements = new List<XElement>{
                new XElement(Xmlns+"Height",  PageParameter.CellHeight + "in")
            };

            var cellsContainer = new XElement(Xmlns + "TablixCells");
            var cells = new List<XElement>();

            foreach (var column in Columns)
            {
                var boxName = "CustomTextbox" + column.Order;
                // [B], [G] [R] and [I] are cell text highlight indicator
                var field = string.Format("Replace(Replace(Replace(Replace(Fields!Field{0}.Value, \"-[B]\", \"\"), \"-[G]\", \"\" ), \"-[I]\", \"\" ), \"-[R]\", \"\" )", column.Order);
                var fieldExp = string.Format("={0}", field);
                if (column.IsDecimal)
                {
                    fieldExp = string.Format("=iif(IsNumeric({0}),CDec(iif(IsNumeric({0}),{0},0)),\"\")", field);
                }
                if (column.IsInteger)
                {
                    fieldExp = string.Format("=iif(IsNumeric({0}),CInt(iif(IsNumeric({0}),{0},0)),\"\")", field);
                }

                cells.Add(GetCell(boxName, fieldExp, column, true));
                Log.Debug($"BuildDetailRow - Column:[{boxName}] | Expression:[{fieldExp}]");
            }

            cellsContainer.ReplaceNodes(cells);
            Log.Info("BuildDetailRow - Cells Added");
            elements.Add(cellsContainer);
            Log.Info("BuildDetailRow - Cells Container Added");
            return elements;
        }

        /// <summary>
        /// Builds a subtotal row
        /// </summary>
        /// <param name="breaklevel"></param>
        /// <param name="columnOrder"></param>
        /// <returns></returns>
        private XElement BuildSubtotalRow(int breaklevel, int columnOrder)
        {
            Log.Debug("BuildSubtotalRow");
            var elements = new List<XElement> { new XElement(Xmlns + "Height", PageParameter.CellHeight * TotalCellHeightFactor + "in") };

            var cellsContainer = new XElement(Xmlns + "TablixCells");
            var cells = new List<XElement>();

            foreach (var column in Columns)
            {
                var boxName = "SubtotalTextbox_" + breaklevel + "_" + column.Order;
                var field =
                    $"Replace(Replace(Replace(Replace(Fields!Field{column.Order}.Value, \"-[B]\", \"\"), \"-[G]\", \"\" ), \"-[I]\", \"\" ), \"-[R]\", \"\" )";
                string fieldExp;
                if (column.IsSubtotal)
                {
                    fieldExp = column.Order == columnOrder
                        ? $"={field}"
                        : string.Empty;
                }
                else
                {
                    var totalFormula = column.IsTripField
                        ? string.Format("=Sum(CDec(iif(IsNumeric({0}) And Not Fields!HideTripFields.Value,{0},0)))", field)
                        : string.Format("=Sum(CDec(iif(IsNumeric({0}),{0},0)))", field);
                    fieldExp = column.TotalThisField ? string.Format(totalFormula, column.Order) : string.Empty;
                }
                cells.Add(GetCell(boxName, fieldExp, column, false, column.Order == columnOrder));
            }

            cellsContainer.ReplaceNodes(cells);
            Log.Debug("BuildSubtotalRow - Cells Added");
            elements.Add(cellsContainer);
            Log.Debug("BuildSubtotalRow - Cells Container Added");
            return new XElement(Xmlns + "TablixRow", elements);
        }

        /// <summary>
        /// Builds the row that shows the grand total for all decimal fields. 
        /// </summary>
        /// <returns></returns>
        private XElement BuildTotalRow()
        {
            Log.Debug("BuildTotalRow");
            var elements = new List<XElement>{
                new XElement(Xmlns+"Height", PageParameter.CellHeight * TotalCellHeightFactor + "in")
            };

            var cellsContainer = new XElement(Xmlns + "TablixCells");

            var cells = new List<XElement>();

            for (int i = 0; i < Columns.Count; i++)
            {
                //because the style and color is appended after the value, need to remove it.
                var fieldExp = "Replace(Replace(Replace(Replace(Fields!Field{0}.Value, \"-[B]\", \"\"), \"-[G]\", \"\" ), \"-[I]\", \"\" ), \"-[R]\", \"\" )";
                var totalFormula = Columns[i].IsTripField
                    ? string.Format("=Sum(CDec(iif(IsNumeric({0}) And Not Fields!HideTripFields.Value,{0},0)))", fieldExp)
                    : string.Format("=Sum(CDec(iif(IsNumeric({0}),{0},0)))", fieldExp);

                var cellValue = Columns[i].TotalThisField ? string.Format(totalFormula, i + 1) : string.Empty;

                cells.Add(GetCell("TotalCell" + i, cellValue, Columns[i], true));
            }

            cellsContainer.ReplaceNodes(cells);
            Log.Debug("BuildTotalRow - Cells Added");
            elements.Add(cellsContainer);
            Log.Debug("BuildTotalRow - Cells Container Added");

            //adding a row means we need to add a tablixmember to the rowheirarchy
            var rh = _xDocument.Descendants().FirstOrDefault(s => s.Name.LocalName.Equals("TablixRowHierarchy"));
            if (rh == null)
            {
                Log.Error($"BuildTotalRow - [{GetTemplateName()}] TablixRowHierarchy is missing");
                throw new Exception("Invalid RDLC file!");
            }
            var members = rh.Descendants().FirstOrDefault(s => s.Name.LocalName.Equals("TablixMembers"));
            if (members == null)
            {
                Log.Error($"BuildTotalRow - [{GetTemplateName()}] TablixMembers is missing");
                throw new Exception("Invalid RDLC file!");
            }
            members.Add(new XElement(Xmlns + "TablixMember"));

            return new XElement(Xmlns + "TablixRow", elements);
        }
        
        /// <summary>
        /// Builds a row that spans all columns
        /// </summary>
        /// <param name="name">The name of the textbox to add (all textboxes in the report must have unique names)</param>
        /// <param name="value">The value that will be displayed in the cell</param>
        /// <returns></returns>
        private XElement BuildSpanningRow(string name, string value)
        {
            Log.Debug("BuildSpanninglRow");
            var elements = new List<XElement>{
                new XElement(Xmlns+"Height", PageParameter.CellHeight * SpanningCellHeightFactor + "in")
            };

            var cellsContainer = new XElement(Xmlns + "TablixCells");

            var cells = new List<XElement> { GetCellWithColSpan(name, value, Columns.Count) };
            Log.Debug("BuildSpanningRow - GetCellWithColSpan Called");

            for (int i = 1; i < Columns.Count; i++)
            {
                cells.Add(new XElement(Xmlns + "TablixCell"));
            }

            cellsContainer.ReplaceNodes(cells);
            Log.Debug("BuildSpanningRow - Cells Added");
            elements.Add(cellsContainer);
            Log.Debug("BuildSpanningRow - Cells Container Added");

            //adding a row means we need to add a tablixmember to the rowheirarchy
            var rh = _xDocument.Descendants().FirstOrDefault(s => s.Name.LocalName.Equals("TablixRowHierarchy"));
            if (rh == null)
            {
                Log.Error($"BuildSpanningRow - [{GetTemplateName()}] TablixRowHierarchy is missing");
                throw new Exception("Invalid RDLC file!");
            }
            var members = rh.Descendants().FirstOrDefault(s => s.Name.LocalName.Equals("TablixMembers"));
            if (members == null)
            {
                Log.Error($"BuildSpanningRow - [{GetTemplateName()}] TablixMembers is missing");
                throw new Exception("Invalid RDLC file!");
            }
            members.Add(new XElement(Xmlns + "TablixMember"));

            return new XElement(Xmlns + "TablixRow", elements);
        }
        #endregion

        #region Build XML snippets
             
        private XElement GetCell(string name, string value, CustomColumnInformation column, bool isDetail, bool summaryColumn = false)
        {
            var cellHighLightManager = new CellHighLightManager();
            var cellStyle = cellHighLightManager.GetCellStyle(column, PageParameter.FontSize, Style, name.Contains("Subtotal") || name.Contains("Total"));

            var useAlternating = !Style.Equals("CLASSIC") && isDetail;
            var detailBorderStyle = _styleManager.DetailCellBorderStyle(useAlternating, Style);

            var subTotalBorderStyle = _styleManager.SubtotalCellBorderStyle(summaryColumn || column.IsDecimal || column.IsInteger);

            var totalBorderStyle = _styleManager.TotalCellBorderStyle(column.TotalThisField);

            var borderStyle = name.IndexOf("Subtotal", StringComparison.Ordinal) ==0
                         ? subTotalBorderStyle
                         : name.IndexOf("Total", StringComparison.Ordinal) ==0
                            ? totalBorderStyle
                            : detailBorderStyle;
            var alignment = GetAlignment(column);

            return new XElement(Xmlns + "TablixCell",
                 new XElement(Xmlns + "CellContents",
                     new XElement(Xmlns + "Textbox", new XAttribute("Name", name),
                         new XElement(Xmlns + "CanGrow", "true"),
                         new XElement(Xmlns + "KeepTogether", "true"),
                         new XElement(Xmlns + "Paragraphs",
                             new XElement(Xmlns + "Paragraph",
                                 new XElement(Xmlns + "TextRuns",
                                     new XElement(Xmlns + "TextRun",
                                         new XElement(Xmlns + "Value", value),
                                         cellStyle)
                                     ),
                                 alignment)),
                         new XElement(Xmlns + "Visibility"
                             //This formula will suppress trip fields when multiple rows exist for one trip
                             , new XElement(Xmlns + "Hidden", column.IsTripField ? "=Fields!HideTripFields.Value" : "false"))
                         , borderStyle)
                     )
                 );

        }


        /// <summary>
        /// Builds a cell that spans multiple columns. The cell will contain a textbox.
        /// </summary>
        /// <param name="name">The name of the textbox to add (all textboxes in the report must have unique names)</param>
        /// <param name="value">The value to display in the textbox</param>
        /// <param name="colSpan">How many columns the cell will span</param>
        /// <returns></returns>
        private XElement GetCellWithColSpan(string name, string value, int colSpan)
        {
            return new XElement(Xmlns + "TablixCell",
                new XElement(Xmlns + "CellContents",
                    new XElement(Xmlns + "Textbox", new XAttribute("Name", name),
                        new XElement(Xmlns + "CanGrow", "false"),
                        new XElement(Xmlns + "KeepTogether", "true"),
                        new XElement(Xmlns + "Paragraphs",
                            new XElement(Xmlns + "Paragraph",
                                new XElement(Xmlns + "TextRuns",
                                    new XElement(Xmlns + "TextRun",
                                        new XElement(Xmlns + "Value", value),
                                        new XElement(Xmlns + "Style", new XElement(Xmlns + "FontSize", PageParameter.FontSize + "pt")))
                                    ))),
                        new XElement(Xmlns + "Style",
                            new XElement(Xmlns + "Border",
                                new XElement(Xmlns + "Style", "None")),
                            new XElement(Xmlns + "PaddingLeft", "1pt"),
                            new XElement(Xmlns + "PaddingRight", "1pt"),
                            new XElement(Xmlns + "PaddingBottom", "1pt"),
                            new XElement(Xmlns + "PaddingTop", "1pt")
                            )),
                    new XElement(Xmlns + "ColSpan", colSpan)
                    )
                );
        }

        private XElement GetHeaderCell(CustomColumnInformation column)
        {
            return new XElement(Xmlns + "TablixCell",
                new XElement(Xmlns + "CellContents",
                    new XElement(Xmlns + "Textbox", new XAttribute("Name", "Header" + column.Order),
                        new XElement(Xmlns + "CanGrow", "true"),
                        new XElement(Xmlns + "KeepTogether", "true"),
                        new XElement(Xmlns + "Paragraphs",
                            new XElement(Xmlns + "Paragraph",
                                new XElement(Xmlns + "TextRuns",
                                    new XElement(Xmlns + "TextRun",
                                        new XElement(Xmlns + "Value", column.Header),
                                        new XElement(Xmlns + "Style",
                                            new XElement(Xmlns + "FontWeight", "Bold"),
                                            new XElement(Xmlns + "FontSize", PageParameter.FontSize + "pt")
                                            ))))),
                        _styleManager.HeaderCellBorderStyle())));
        }

        /// <summary>
        /// Returns an XElement containing the details node. All reports will have this node. 
        /// </summary>
        /// <returns></returns>
        private XElement GetDetailsNode()
        {
            Log.Debug("GetDetailsNode");
            Log.Debug($"GetDetailsNode - SuppressDetail:[{SuppressDetail}]");

            if (SuppressDetail)
            {
                return new XElement(Xmlns + "TablixMember",
                new XElement(Xmlns + "Group", new XAttribute("Name", "Details")),
                new XElement(Xmlns + "Visibility",
                new XElement(Xmlns + "Hidden", "true")));
            }

            return new XElement(Xmlns + "TablixMember",
                new XElement(Xmlns + "Group", new XAttribute("Name", "Details")));
        }
        
        private XElement GetSortExpression(string fieldOne)
        {
            return new XElement(Xmlns + "SortExpressions",
                new XElement(Xmlns + "SortExpression", new XElement(Xmlns + "Value", "=Fields!" + fieldOne + ".Value")));
        }

        private XElement GetGroupExpression(CustomColumnInformation col)
        {
            var fieldOne = "Field" + col.Order;
            if (col.IsPageBreak)
            {
                return new XElement(Xmlns + "Group", new XAttribute("Name", fieldOne),
                 new XElement(Xmlns + "GroupExpressions",
                 new XElement(Xmlns + "GroupExpression", "=Fields!" + fieldOne + ".Value")),
                      new XElement(Xmlns + "PageBreak",
                         new XElement(Xmlns + "BreakLocation", "Between"))
                 );
            }

            return new XElement(Xmlns + "Group", new XAttribute("Name", fieldOne),
                new XElement(Xmlns + "GroupExpressions",
                    new XElement(Xmlns + "GroupExpression", "=Fields!" + fieldOne + ".Value")));
        }

        #endregion

        #region Report Setup

        /// <summary>
        /// Adds up to three groups to the report
        /// </summary>
        /// <param name="rowContainer"></param>
        private void AddGroups(XElement rowContainer)
        {
            Log.Info($"AddGroups");
            var rh = _xDocument.Descendants().FirstOrDefault(s => s.Name.LocalName.Equals("TablixRowHierarchy"));
            if (rh == null)
            {
                Log.Error($"AddGroups - [{GetTemplateName()}] TablixRowHierarchy is missing");
                throw new Exception("Invalid RDLC file!");
            }
            var group = rh.Descendants().FirstOrDefault(s => s.Name.LocalName.Equals("Group"));
            if (group == null)
            {
                Log.Error($"AddGroups - [{GetTemplateName()}] Group is missing");
                throw new Exception("Invalid RDLC file!");
            }

            var tablixMember = group.Parent; //here's where we add all the groups, if necessary.
            if (tablixMember == null)
            {
                Log.Error($"AddGroups - [{GetTemplateName()}] parent of Group [TablixMember] is missing");
                throw new Exception("Invalid RDLC file!");
            }
            tablixMember.ReplaceWith(BuildGroupings(rowContainer));
            Log.Info($"AddGroups - BuildGroupings is done");
        }

        /// <summary>
        /// Builds the elements that contain the nested groupings
        /// </summary>
        /// <param name="rowContainer"></param>
        /// <returns></returns>
        private XElement BuildGroupings(XElement rowContainer)
        {
            Log.Debug($"BuildGroupings");

            var groupCols = Columns.Where(s => s.GroupBreak > 0 && s.GroupBreak <= 3).OrderByDescending(s => s.GroupBreak).Take(3).ToList();

            var tm = GetDetailsNode();
            Log.Debug($"BuildGroupings - GetDetailsNode is called");

            if (!groupCols.Any()) return tm;

            var groupCounter = 3;
            //"recursively" add groups
            foreach (var col in groupCols)
            {
                Log.Info($"BuildGroupings - Group Count:[{groupCounter}] | Col:[{col.Header}] | Subtotal:[{col.IsSubtotal}]");
                if (col.IsSubtotal)
                {
                    rowContainer.Add(BuildSubtotalRow(groupCounter, col.Order));
                    tm = new XElement(Xmlns + "TablixMember",
                    GetGroupExpression(col),
                    GetSortExpression("Field" + col.Order),
                    new XElement(Xmlns + "TablixMembers", tm,
                        new XElement(Xmlns + "TablixMember", new XElement(Xmlns + "KeepWithGroup", "Before")))
                    );
                }
                else
                {
                    tm = new XElement(Xmlns + "TablixMember",
                    GetGroupExpression(col),
                    GetSortExpression("Field" + col.Order),
                    new XElement(Xmlns + "TablixMembers", tm)
                    );
                }
                groupCounter--;

            }

            return tm;
        }

        /// <summary>
        /// Adds a column to the tablix for each column in the report
        /// </summary>
        /// <returns></returns>
        private XElement BuildColumnHeirarchy()
        {
            Log.Debug($"BuildColumnHeirarchy");
            var element = new XElement(Xmlns + "TablixMembers");

            var cols = new List<XElement>();

            for (int i = 0; i < Columns.Count; i++)
            {
                cols.Add(new XElement(Xmlns + "TablixMember"));
            }
            element.ReplaceNodes(cols);
            return element;
        }

        /// <summary>
        /// Adds Fields to the dataset node
        /// </summary>
        /// <returns></returns>
        private List<XElement> BuildDataFields()
        {
            Log.Debug($"BuildColumnHeirarchy");
            var cols = new List<XElement>();

            foreach (var column in Columns)
            {
                var fieldName = "Field" + column.Order;

                cols.Add(new XElement(Xmlns + "Field", new XAttribute("Name", fieldName), new XElement(Xmlns + "DataField", fieldName), new XElement(Xmlnsrd + "TypeName", "System.String")));
            }
            cols.Add(new XElement(Xmlns + "Field", new XAttribute("Name", "HideTripFields"), new XElement(Xmlns + "DataField", "HideTripFields"), new XElement(Xmlnsrd + "TypeName", "System.Boolean")));
            return cols;
        }

        /// <summary>
        /// Adds a column to the tablix for each field on the report. 
        /// </summary>
        /// <returns></returns>
        private List<XElement> BuildColumns()
        {
            Log.Debug($"BuildColumns");
            var cols = new List<XElement>();
            double totalWidth = 0;
            foreach (var column in Columns)
            {

                var colwidth = (column.Width / PageParameter.FontConverter) + ColumnGap;
                if (column.Order == Columns.Count && totalWidth + colwidth < LimitedWidth)
                {
                    //make sure it is wide enought to print the report total, expand last column if it's not.
                    colwidth = LimitedWidth - totalWidth;
                }
                totalWidth = totalWidth + colwidth;
                cols.Add(new XElement(Xmlns + "TablixColumn",
                    new XElement(Xmlns + "Width", colwidth + "in")
                    ));
                Log.Debug($"BuildColumns - Column:[{column.Order}] | Width:[{colwidth} in]");
            }

            return cols;
        }

        #endregion

        private static XElement GetAlignment(CustomColumnInformation column)
        {
            var align = new XElement(Xmlns + "Style");
            switch (column.TextAlignment)
            {
                case HorizontalAlignment.Left:
                    align = new XElement(Xmlns + "Style",
                               new XElement(Xmlns + "TextAlign", "Left"));
                    break;
                case HorizontalAlignment.Center:
                    align = new XElement(Xmlns + "Style",
                               new XElement(Xmlns + "TextAlign", "Center"));
                    break;
                case HorizontalAlignment.Right:
                    align = new XElement(Xmlns + "Style",
                               new XElement(Xmlns + "TextAlign", "Right"));
                    break;
            }
            return align;
        }

        /// <summary>
        /// Used by calling programs to convert a property of an object to a string. The property can be of string, decimal or int type. 
        /// </summary>
        /// <param name="obj">The object </param>
        /// <param name="propName">The name of the property to convert.</param>
        /// <returns>The property as a string. If the property does not exist, or is not of an allowed type, an empty string will be returned.</returns>
        public static string GetValueAsString(object obj, string propName)
        {
            var tType = obj.GetType();
            var tProperties = new List<PropertyInfo>(tType.GetProperties()).OrderBy(s => s.Name).ToList();
            var prop = tProperties.FirstOrDefault(s => s.Name.Equals(propName.Trim(), StringComparison.OrdinalIgnoreCase));
            if (prop == null) return "?";

            switch (prop.PropertyType.Name.ToUpper())
            {
                case "STRING":
                    return (string)prop.GetValue(obj) ?? string.Empty;
                case "DECIMAL":
                    var decObj = (decimal)prop.GetValue(obj);
                    return decObj.ToString("0.00");
                case "INT32":
                    var intObj = (int)prop.GetValue(obj);
                    return intObj.ToString(CultureInfo.InvariantCulture);
                case "DATETIME":
                    var dateObj = (DateTime)prop.GetValue(obj);
                    return dateObj.ToString(CultureInfo.InvariantCulture);
                default:
                    if (prop.GetValue(obj) != null)
                    {
                        if (prop.PropertyType.FullName.IndexOf("Int32", StringComparison.InvariantCulture) > -1)
                        {
                            intObj = (int) prop.GetValue(obj);
                            return intObj.ToString(CultureInfo.InvariantCulture);
                        }
                        else if (prop.PropertyType.FullName.IndexOf("DateTime", StringComparison.InvariantCulture) > -1)
                        {
                            dateObj = (DateTime) prop.GetValue(obj);
                            return dateObj.ToString(CultureInfo.InvariantCulture);
                        }
                        else if (prop.PropertyType.FullName.IndexOf("Decimal", StringComparison.InvariantCulture) > -1)
                        {
                            decObj = (decimal) prop.GetValue(obj);
                            return decObj.ToString("0.00");
                        }
                        else
                        {
                            return prop.GetValue(obj).ToStringSafe();
                        }
                    }
                    else
                    {
                        return prop.GetValue(obj).ToStringSafe();
                    }
            }
        }
    }
}
