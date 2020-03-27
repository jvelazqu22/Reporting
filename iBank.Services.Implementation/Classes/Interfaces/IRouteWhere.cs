using System;

namespace iBank.Services.Implementation.Shared
{
    public interface IRouteWhere
    {
        string DitCode { get; set; }
        string Origin { get; set; }
        string Destinat { get; set; }
        string Mode { get; set; }
        string Airline { get; set; }
        int RecKey { get; set; }
        int SeqNo { get; set; }
        DateTime RDepDate { get; set; }
        DateTime RArrDate { get; set; }
    }
}
