namespace iBank.Services.Orm.Classes
{
    public class ClassCategoryInformation
    {
        public ClassCategoryInformation()
        {
            Agency = string.Empty;
            Category = string.Empty;
            Description = string.Empty;
            Heirarchy = 0;
        }

        public string Agency { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public int Heirarchy { get; set; }
    }
}
