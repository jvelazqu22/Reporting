namespace Domain.Helper
{
    public class AdvancedParameter
    {
        public string FieldName { get; set; } = string.Empty;
        public string AdvancedFieldName { get; set; } = string.Empty;
        public string Value1 { get; set; } = string.Empty;
        public string Value2 { get; set; } = string.Empty;
        public Operator Operator { get; set; } = Operator.Equal;
        public string Type { get; set; } = "TEXT";
        public bool IsLookup { get; set; } = false;
        public bool IsMultiUdid { get; set; } = false;
        public string TableName { get; set; } = string.Empty;
    }
}
