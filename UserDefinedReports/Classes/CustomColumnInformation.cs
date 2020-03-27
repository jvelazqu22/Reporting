
namespace UserDefinedReports.Classes
{
    public class CustomColumnInformation
    {
        public CustomColumnInformation()
        {
            Header = string.Empty;
            Width = 2.09417;
            FontSize = 10;
            FontStyle = "";
            FontWeight = "";
            Order = 0;
            TextAlignment = HorizontalAlignment.Default;
        }
        public string Header { get; set; }
        public double Width { get; set; }
        public int FontSize { get; set; }
        public string FontSizeString => FontSize + "pt";
        public int Order { get; set; }
        public bool IsDecimal { get; set; }
        public bool IsInteger { get; set; }
        /// <summary>
        /// If true, field will be supressed when there are multiple rows for a given trip. 
        /// </summary>
        public bool IsTripField { get; set; }
        public HorizontalAlignment TextAlignment { get; set; }
        public int GroupBreak { get; set; }
        public bool IsSubtotal { get; set; }
        public bool TotalThisField { get; set; }
        public bool IsPageBreak { get; set; }
        public string GoodOperator { get; set; }
        public string GoodValue { get; set; }
        public string GoodHighlight { get; set; }
        public string BadOperator { get; set; }
        public string BadValue { get; set; }
        public string BadHighlight { get; set; }
        public string FontStyle { get; set; }
        public string FontWeight { get; set; }
        
    }

    public enum HorizontalAlignment
    {
        Default,
        Left,
        Center,
        Right
    }
}
