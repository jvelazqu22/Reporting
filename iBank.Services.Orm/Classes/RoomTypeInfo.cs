namespace iBank.Services.Orm.Classes
{
    public class RoomTypeInfo
    {
        public RoomTypeInfo()
        {
            RoomType = string.Empty;
            LanguageCode = string.Empty;
            Description = string.Empty;

        }
        public string RoomType { get; set; }
        public string LanguageCode { get; set; }
        public string Description { get; set; }

    }
}
