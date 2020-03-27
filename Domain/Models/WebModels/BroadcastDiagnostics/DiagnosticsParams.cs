using System.Collections.Generic;
using AutoMapper;
using iBank.Entities.MasterEntities;

namespace Domain.Models.WebModels.BroadcastDiagnostics
{
    public class DiagnosticsParams
    {
        public MainDiagnostics MainDiagnostics { get; set; } = new MainDiagnostics();
        public BroadcastDiagnosticsRow1 BroadcastDiagnosticsRow1 { get; set; } = new BroadcastDiagnosticsRow1();
        public BroadcastDiagnosticsRow2 BroadcastDiagnosticsRow2 { get; set; } = new BroadcastDiagnosticsRow2();
        public BroadcastDiagnosticsRow3 BroadcastDiagnosticsRow3 { get; set; } = new BroadcastDiagnosticsRow3();
        public List<reporting_error_log> ReportingErrorLogs { get; set; } = new List<reporting_error_log>();


        public void MapDataModels()
        {
            BroadcastDiagnosticsRow1 = Mapper.Map<BroadcastDiagnosticsRow1>(BroadcastDiagnosticsRow1);
            BroadcastDiagnosticsRow2 = Mapper.Map<BroadcastDiagnosticsRow2>(BroadcastDiagnosticsRow2);
            BroadcastDiagnosticsRow3 = Mapper.Map<BroadcastDiagnosticsRow3>(BroadcastDiagnosticsRow3);
        }
    }
}
