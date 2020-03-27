using System;

using Domain.Helper;

namespace Domain.Interfaces
{
    public interface ICollapsible : IAirMileage
    {
        int RecKey { get; set; }
        string DitCode { get; set; }
        string DomIntl { get; set; }
        decimal AltCarCo2 { get; set; }
        decimal AltRailCo2 { get; set; }
        decimal ActFare { get; set; }
        decimal MiscAmt { get; set; }
        string Connect { get; set; }
        int SeqNo { get; set; }
        DateTime? RDepDate { get; set; }
        string DepTime { get; set; }
        string Airline { get; set; }
        string fltno { get; set; }
        string ClassCode { get; set; }
        int Seg_Cntr { get; set; }
        decimal AirCo2 { get; set; }
    }
}
