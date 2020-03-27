using System;

namespace iBank.Services.Implementation.Classes.Interfaces
{
    public interface ICollapsible
    {
        int RecKey { get; set; }
        string DitCode { get; set; }
        string DomIntl { get; set; }
        int Miles { get; set; }
        //TODO: may need to implement these if collapsing for a report that uses CarbCalculators. 
        //decimal MiscAmt { get; set; }
        //decimal AirCO2 { get; set; }
        //decimal AltCarCO2 { get; set; }
        //decimal AltRailCO2 { get; set; }
        string Origin { get; set; }
        string Destinat { get; set; }
        decimal ActFare { get; set; }
        decimal MiscAmt { get; set; }
        string Connect { get; set; }
        int SeqNo { get; set; }
        DateTime RDepDate { get; set; }
        string DepTime { get; set; }
        string Airline { get; set; }
        string FltNo { get; set; }
        string ClassCode { get; set; }
        int Seg_Cntr { get; set; }
        

    }
}
