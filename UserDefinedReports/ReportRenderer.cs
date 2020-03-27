using System;
using System.IO;
using Domain.Helper;
using Microsoft.Reporting.WinForms;

namespace UserDefinedReports
{
    public class ReportRenderer
    {
        public string FileName { get; set; }

        private LocalReport Report { get; set; }

        public ReportRenderer(LocalReport report, string fileName)
        {
            Report = report;
            FileName = fileName;
        }

        public virtual void WriteAllBytes(string fileName, byte[] streamBytes) => File.WriteAllBytes(fileName, streamBytes);

        public virtual void DeleteFile(string fileName) => File.Delete(fileName);

        public virtual byte[] RenderToStream(string outputType) => Report.Render(outputType);

        public void Render(DestinationSwitch outputFormat)
        {
            var outputType = "pdf";
            if (outputFormat == DestinationSwitch.XlsxFormatted)
            {
                outputType = "excelopenxml";
                //change extension from .xls to .xlsx
                if (new FileInfo(FileName).Extension == ".xls") FileName += "x";
            }

            var streamBytes = RenderToStream(outputType);
            WriteAllBytes(FileName, streamBytes);
            var templateFile = FileName.Substring(0, FileName.LastIndexOf(".", StringComparison.Ordinal)) + ".rdlc";
            DeleteFile(templateFile);
        }
    }
}
