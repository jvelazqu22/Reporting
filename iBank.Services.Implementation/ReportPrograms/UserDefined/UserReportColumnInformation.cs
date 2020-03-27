using iBank.Server.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined
{
    public class UserReportColumnInformation
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public int Sort { get; set; }
        public int GroupBreak { get; set; }
        public bool SubTotal { get; set; }
        public bool PageBreak { get; set; }
        public string Header1 { get; set; }
        public string Header2 { get; set; }
        public int Width { get; set; }
        public int UdidType { get; set; }
        public string HorizontalAlignment { get; set; }
        public string GoodField { get; set; }
        public string GoodValue { get; set; }
        public string GoodOperator { get; set; }
        public string GoodHilite { get; set; }
        public string GoodFieldType { get; set; }
        public string BadField { get; set; }
        public string BadValue { get; set; }
        public string BadOperator { get; set; }
        public string BadHilite { get; set; }
        public string BadFieldType { get; set; }
        public bool TlsOnly { get; set; }
        public string TableName { get; set; }

        public string Usage { get; set; }
        public string ColumnType { get; set; } 
        public string LookupTable { get; set; }
        public bool TotalThisField { get; set; }
        public int DecimalPlaces { get { return ColumnType.EqualsIgnoreCase("CURRENCY") ? 2 : 0; } }
        public bool SuppressDuplicates { get; set; }

    }
}
