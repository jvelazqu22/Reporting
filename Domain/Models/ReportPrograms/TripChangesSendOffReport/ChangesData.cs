using System;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.TripChangesSendOffReport
{
    public class ChangesData: IRecKey
    {
        public int RecKey { get; set; } = 0;
        public int SegNum { get; set; } = 0;
        public int ChangeCode { get; set; } = 0;
        public DateTime? ChangStamp { get; set; } = DateTime.MinValue;
        public string ChangeDesc { get; set; } = string.Empty;
        public string ChangeFrom { get; set; } = string.Empty;
        public string ChangeTo { get; set; } = string.Empty;
        public string PriorItin { get; set; } = string.Empty;
        public DateTime BookDate { get; set; } = DateTime.MinValue;
    }
}
