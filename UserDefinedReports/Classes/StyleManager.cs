using System.Xml.Linq;

namespace UserDefinedReports.Classes
{
    public class StyleManager
    {
        public readonly XNamespace Xmlns = @"http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition";
        public readonly XNamespace Xmlnsrd = @"http://schemas.microsoft.com/SQLServer/reporting/reportdesigner";


        public XElement BuildCellBorderStyle(string name, string style, string color, string borderWidth="1pt")
        {
            return new XElement(Xmlns + name,
                        new XElement(Xmlns + "Color", color),
                        new XElement(Xmlns + "Style", style),
                        new XElement(Xmlns + "Width", borderWidth));
        }

        public XElement SubtotalCellBorderStyle(bool summaryColumn)
        {
            return new XElement(Xmlns + "Style",
                 new XElement(Xmlns + "BackgroundColor", "White"),
                 new XElement(Xmlns + "Border",
                     new XElement(Xmlns + "Style", "None")),
                 summaryColumn ? BuildCellBorderStyle("TopBorder", "Solid", "Black") : null,
                 BuildCellBorderStyle("LeftBorder", "Solid", "White"),
                 BuildCellBorderStyle("RightBorder", "Solid", "White"),
                 new XElement(Xmlns + "PaddingLeft", "1pt"),
                 new XElement(Xmlns + "PaddingRight", "1pt"),
                 new XElement(Xmlns + "PaddingBottom", "1pt"),
                 new XElement(Xmlns + "PaddingTop", "1pt")
            );
        }

        public XElement TotalCellBorderStyle(bool summaryColumn)
        {
            return new XElement(Xmlns + "Style",
                 new XElement(Xmlns + "BackgroundColor", "White"),
                 new XElement(Xmlns + "Border",
                     new XElement(Xmlns + "Style", "None")),
                 summaryColumn ? BuildCellBorderStyle("TopBorder", "Solid", "Black"):null,
                 summaryColumn ? BuildCellBorderStyle("BottomBorder", "Solid", "Black", "2pt") : null,
                 BuildCellBorderStyle("LeftBorder", "Solid", "White"),
                 BuildCellBorderStyle("RightBorder", "Solid", "White"),
                 new XElement(Xmlns + "PaddingLeft", "1pt"),
                 new XElement(Xmlns + "PaddingRight", "1pt"),
                 new XElement(Xmlns + "PaddingBottom", "1pt"),
                 new XElement(Xmlns + "PaddingTop", "1pt")
            );
        }

        public XElement HeaderCellBorderStyle()
        {
            return new XElement(Xmlns + "Style",
                 new XElement(Xmlns + "BackgroundColor", "White"),
                 new XElement(Xmlns + "Border",
                     new XElement(Xmlns + "Style", "None")),
                 BuildCellBorderStyle("LeftBorder", "Solid", "White"),
                 BuildCellBorderStyle("RightBorder", "Solid", "White"),
                 BuildCellBorderStyle("BottomBorder", "Solid", "Black"),
                 new XElement(Xmlns + "PaddingLeft", "1pt"),
                 new XElement(Xmlns + "PaddingRight", "1pt"),
                 new XElement(Xmlns + "PaddingBottom", "1pt"),
                 new XElement(Xmlns + "PaddingTop", "1pt")
            );
        }

        public XElement DetailCellBorderStyle(bool useAlternating, string style)
        {
            return new XElement(Xmlns + "Style",
                new XElement(Xmlns + "BackgroundColor",
                    useAlternating ? style == "BOLD" ? "=IIf(RowNumber(Nothing) Mod 2 = 1, \"#504D4C\", \"White\")" : "=IIf(RowNumber(Nothing) Mod 2 = 1, \"#F1F2F2\", \"White\")" : "White"),
                new XElement(Xmlns + "Border",
                     new XElement(Xmlns + "Style", "None")),
                 BuildCellBorderStyle("LeftBorder", "Solid", "White"),
                 BuildCellBorderStyle("RightBorder", "Solid", "White"),
                 BuildCellBorderStyle("BottomBorder", "Solid", "White"),
                 new XElement(Xmlns + "PaddingLeft", "1pt"),
                 new XElement(Xmlns + "PaddingRight", "1pt"),
                 new XElement(Xmlns + "PaddingBottom", "1pt"),
                 new XElement(Xmlns + "PaddingTop", "1pt")
            );
        }
        
    }
}
