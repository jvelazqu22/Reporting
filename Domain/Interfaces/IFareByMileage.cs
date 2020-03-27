namespace Domain.Interfaces
{
    public interface IFareByMileage : IRecKey
    {
        decimal BaseFare { get; set; }
        decimal ActFare { get; set; }
        int Miles { get; set; }
    }
}
