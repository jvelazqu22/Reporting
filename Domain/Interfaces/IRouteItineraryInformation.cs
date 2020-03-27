namespace Domain.Interfaces
{
    public interface IRouteItineraryInformation : IRecKey
    {
        string Origin { get; set; }
        string Destinat { get; set; }
        string Connect { get; set; }
        string Mode { get; set; }
        string OrigOrigin { get; set; }
        string OrigDest { get; set; }

    }
}
