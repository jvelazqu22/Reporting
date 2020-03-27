namespace iBank.Services.Orm.Classes
{
    public class LanguageVariableInfo
    {
        public LanguageVariableInfo()
        {
            VariableName = string.Empty;
            Translation = string.Empty;
            TagType = string.Empty;
        }
        public string VariableName { get; set; }
        public string Translation { get; set; }
        public int NumberOfLines { get; set; }

        public string TagType { get; set; }
    }
}
