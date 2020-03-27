namespace iBank.Services.Implementation.Utilities.eFFECTS
{
    public class XmlReport
    {
        public XmlReport(string crName, string exportType)
        {
            CrName = crName;
            ExportType = exportType;
        }

        public string CrName { get; }
        public string ExportType { get; }
    }
}