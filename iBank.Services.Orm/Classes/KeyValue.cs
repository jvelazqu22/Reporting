namespace iBank.Services.Orm.Classes
{
    public class KeyValue
    {
        public KeyValue()
        {
            Key = string.Empty;
            Value = string.Empty;
            Agency = string.Empty;
        }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Agency { get; set; }
    }
}
