using System;
using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.AirFareSavingsReport
{
    public class UdidData : IRecKey
    {
        public UdidData()
        {
            RecKey = 0;
            UdidText = string.Empty;
            UdidNo = 0;
            UdidLabel = "";
        }

        public int RecKey { get; set; }
        public Int16 UdidNo { get; set; }
        public string UdidText { get; set; }
        public string UdidLabel { get; set; }
    }
}
