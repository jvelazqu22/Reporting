namespace iBank.Services.Orm.Classes
{
    public class CarTypeInfo
    {
        public CarTypeInfo()
        {
            CarType = string.Empty;
            LanguageCode = string.Empty;
            Description = string.Empty;

        }
        public string CarType { get; set; }
        public string LanguageCode { get; set; }
        public string Description { get; set; }

    }
}
