using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.TravelAuditReasonsbyMonthReport
{
    public interface IFinalData
    {
        decimal Mth1Cnt { get; set; }
        decimal Mth1Pcnt { get; set; }
        decimal Mth2Cnt { get; set; }
        decimal Mth2Pcnt { get; set; }
        decimal Mth3Cnt { get; set; }
        decimal Mth3Pcnt { get; set; }
        decimal Mth4Cnt { get; set; }
        decimal Mth4Pcnt { get; set; }
        decimal Mth5Cnt { get; set; }
        decimal Mth5Pcnt { get; set; }
        decimal Mth6Cnt { get; set; }
        decimal Mth6Pcnt { get; set; }
        decimal Mth7Cnt { get; set; }
        decimal Mth7Pcnt { get; set; }
        decimal Mth8Cnt { get; set; }
        decimal Mth8Pcnt { get; set; }
        decimal Mth9Cnt { get; set; }
        decimal Mth9Pcnt { get; set; }
        decimal Mth10Cnt { get; set; }
        decimal Mth10Pcnt { get; set; }
        decimal Mth11Cnt { get; set; }
        decimal Mth11Pcnt { get; set; }
        decimal Mth12Cnt { get; set; }
        decimal Mth12Pcnt { get; set; }
        decimal YtdCnt { get; set; }
        decimal YtdPcnt { get; set; }
    }
}
