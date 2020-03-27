using System;

namespace Domain.Interfaces
{
    public interface IXmlTripData
    {
        string Agency { get; set; }
        int RecKey { get; set; }
        string Recloc { get; set; }
        string Branch { get; set; }
        string Acct { get; set; }
        DateTime? Depdate { get; set; }
        DateTime? Bookdate { get; set; }
        string Sourceabbr { get; set; }
        string Moneytype { get; set; }
        string Gds { get; set; }
    }
}
