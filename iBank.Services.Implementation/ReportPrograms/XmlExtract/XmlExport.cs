namespace iBank.Services.Implementation.ReportPrograms.XmlExtract
{
    
    public class XmlExport
    {
        public XmlExport()
        {
            Name = string.Empty;
            Type = string.Empty;
            Title = string.Empty;
        }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
    }
}
