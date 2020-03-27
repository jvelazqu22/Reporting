namespace Domain.Orm.Classes
{
    public class LanguageVariableInfo
    {
        public string VariableName { get; set; } = string.Empty;
        public string Translation { get; set; } = string.Empty;
        public int NumberOfLines { get; set; }

        public string TagType { get; set; } = string.Empty;
    }
}
