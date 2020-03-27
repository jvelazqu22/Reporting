
namespace Domain.Interfaces
{
    public interface ICarbonCalculations : IMileage
    {
        string ClassCat { get; set; }
        string DitCode { get; set; }
        decimal AirCo2 { get; set; }
        decimal AltCarCo2 { get; set; }
        decimal AltRailCo2 { get; set; }
        string HaulType { get; set; }
    }

    public interface ICarbonCalculationsCar
    {
        string CarType { get; set; }
        int Days { get; set; }
        decimal CarCo2 { get; set; }
        int CPlusMin { get; set; }
    }

    public interface ICarbonCalculationsHotel
    {
        int Nights { get; set; }
        int Rooms { get; set; }
        decimal HotelCo2 { get; set; }
        int HPlusMin { get; set; }
    }
}
