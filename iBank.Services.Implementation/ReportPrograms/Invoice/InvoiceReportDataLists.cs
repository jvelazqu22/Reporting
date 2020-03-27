using Domain.Models.ReportPrograms.InvoiceReport;
using System.Collections.Generic;

namespace iBank.Services.Implementation.ReportPrograms.Invoice
{
    public class InvoiceReportDataLists
    {
        public List<FinalData> FinalData { get; set; }
        public List<SubReportData> SubReportData { get; set; }

        public InvoiceReportDataLists(List<FinalData> finalData, List<SubReportData> subReportData)
        {
            FinalData = finalData;
            SubReportData = subReportData;
        }
    }
}
