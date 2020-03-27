using System;
using System.Data;
using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

using iBank.Services.Implementation.Classes.Interfaces;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
namespace iBank.Services.Implementation.ReportPrograms
{
    public class SavedRptList : IReportRunner
    {
        public BuildWhere BuildWhere { get; set; }
        public string ReportProgramName { get; set; }

        public bool RunReport()
        {
            var rptFilePath = StringHelper.AddBS(BuildWhere.ReportGlobals.CrystalDirectory) + "ibSavedRptList.rpt";
            var genFilePath = StringHelper.AddBS(BuildWhere.ReportGlobals.ResultsDirectory) + "ibSavedRptList.{0}";

             var reportSource = new ReportDocument();
            try
            {
                //Load the report file
                reportSource.Load(rptFilePath);

                //each report needs a dataset that matches it's schema
                reportSource.SetDataSource(BuildDataSet());


                //Setting parameters when necessary
                reportSource.SetParameterValue("rptTitle", "Saved Report List");

                //export to various formats
                reportSource.ExportToDisk(ExportFormatType.PortableDocFormat, string.Format(genFilePath, "pdf"));
                //ReportSource.ExportToDisk(ExportFormatType.RichText, string.Format(expDir, "ibSavedRptList.rtf"));
                //ReportSource.ExportToDisk(ExportFormatType.Excel, string.Format(expDir, "ibSavedRptList.xls"));
                //ReportSource.ExportToDisk(ExportFormatType.ExcelWorkbook, string.Format(expDir, "ibSavedRptList.xlsx"));
                //ReportSource.ExportToDisk(ExportFormatType.WordForWindows, string.Format(expDir, "ibSavedRptList.docx"));
                //ReportSource.ExportToDisk(ExportFormatType.CharacterSeparatedValues, string.Format(expDir, "ibSavedRptList.csv"));
            }
            catch (Exception)
            {
                return false;
                
            }

            return true;
        }

        public DataSet BuildDataSet()
        {
            var ds = new DataSet();
            var dt = new DataTable { TableName = "ibsavedrptlist" };
            dt.Columns.Add(new DataColumn { ColumnName = "processkey", DataType = typeof(int) });
            dt.Columns.Add(new DataColumn { ColumnName = "userrptnam", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn { ColumnName = "firstname", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn { ColumnName = "lastname", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn { ColumnName = "caption", DataType = typeof(string) });
            dt.Columns.Add(new DataColumn { ColumnName = "lastused", DataType = typeof(DateTime) });

            for (int i = 0; i < 10; i++)
            {
                var row = dt.NewRow();
                row[0] = i;
                row[1] = "Report" + i;
                row[2] = "bob";
                row[3] = "bobson";
                row[4] = "Report" + i;
                row[5] = DateTime.Now.AddDays(i);

                dt.Rows.Add(row);
            }
            ds.Tables.Add(dt);
            //This line will generate the XML for the report. in the report designer, update the data source to the xml file, and re-save the report. 
            //ds.WriteXml(@"C:\Users\Geoff Callag\OneDrive\EPS\CIS Wired\ReportTables\" + dt.TableName + ".xml", XmlWriteMode.WriteSchema);

            return ds;
        }

        public int GetFinalRecordsCount() => 0;

    }
}
