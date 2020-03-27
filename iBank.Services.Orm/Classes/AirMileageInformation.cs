namespace iBank.Services.Orm.Classes
{
    public class AirMileageInformation
    {
        public AirMileageInformation()
        {
            Origin = string.Empty;
            Destination = string.Empty;
        }
        public int RecordNumber { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public int Mileage { get; set; }
    }
}
