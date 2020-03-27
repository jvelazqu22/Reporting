using System;

namespace Domain.Interfaces
{
    public interface IRouteWhere : IRecKey
    {
        string DitCode { get; set; }
        string Origin { get; set; }
        string Destinat { get; set; }
        string Mode { get; set; }
        string Airline { get; set; }
        int SeqNo { get; set; }
        DateTime? RDepDate { get; set; }
        DateTime? RArrDate { get; set; }
    }
}
