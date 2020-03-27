namespace iBank.Services.Orm.Classes
{
    public class IntKeyValue
    {
        public IntKeyValue()
        {
            Value = string.Empty;
        }
        public int Key { get; set; }
        public string Value { get; set; }
    }
}
