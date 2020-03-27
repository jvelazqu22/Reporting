using Domain.Models.ReportPrograms.InvoiceReport;
using System.Collections.Generic;

namespace iBank.Services.Implementation.ReportPrograms.Invoice
{
    public class Udid
    {
        public List<UdidRawData> UdidData { get; set; }
       
        public int UdidNumber { get; set; }

        public string UdidText { get; set; }
    }
}