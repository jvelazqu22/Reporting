using System;

using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.InvoiceReport
{
    public class KeysAndLocators : IRouteWhere
    {
        public string DitCode { get; set; }
        public string Origin { get; set; }
        public string Destinat { get; set; }
        public string Mode { get; set; }
        public string Airline { get; set; }
        public int RecKey { get; set; }
        public int SeqNo { get; set; }
        public DateTime? RDepDate { get; set; }
        public DateTime? RArrDate { get; set; }
        public string RecLoc { get; set; }

        public override bool Equals(object obj)
        {
            var item = obj as KeysAndLocators;

            if (item == null) return false;

            return this.RecKey == item.RecKey && this.RecLoc.Trim() == item.RecLoc.Trim();
        }

        public override int GetHashCode()
        {
            return this.ToString().ToLower().GetHashCode();
        }
    }
}
