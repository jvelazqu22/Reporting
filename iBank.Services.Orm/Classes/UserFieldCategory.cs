namespace iBank.Services.Orm.Classes
{
    public class UserFieldCategory
    {
        public UserFieldCategory()
        {
            Key = 0;
            Description = string.Empty;
        }
        public int Key { get; set; }
        public string Description { get; set; }
    }
}
