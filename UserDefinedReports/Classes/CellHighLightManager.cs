using System.Xml.Linq;

namespace UserDefinedReports.Classes
{
    public class CellHighLightManager
    {
        private readonly XNamespace _xmlns = ReportBuilder.Xmlns;

        public XElement GetCellStyle(CustomColumnInformation column, double fontSize, string style, bool isTotal = true)
   {
            var cellStyle = column.IsDecimal
                ? new XElement(_xmlns + "Style", new XElement(_xmlns + "FontSize", fontSize + "pt"), new XElement(_xmlns + "Format", "#,0.00"))
                : new XElement(_xmlns + "Style", new XElement(_xmlns + "FontSize", fontSize + "pt"), new XElement(_xmlns + "Format", "#,0"));

            var cellStyleHighlight = GetCellHighlight(column);
            var styleBoldValue = StyleBoldValue(column);
            var styleItalicValue =  StyleItalicValue(column);

            if (cellStyleHighlight != null || isTotal)
            {
                if (!isTotal)
                {
                    cellStyle.AddFirst(cellStyleHighlight);
                    cellStyle.AddFirst(new XElement(_xmlns + "FontWeight", styleBoldValue));
                    if (style == "GRAYSCALE" || style =="FRESH")
                    {
                        cellStyle.AddFirst(new XElement(_xmlns + "FontStyle", styleItalicValue));
                    }
                }
                else
                {
                    cellStyle.AddFirst(new XElement(_xmlns + "FontWeight", "Bold"));
                }
            }
            
            return cellStyle;
        }

        public XElement GetCellHighlight(CustomColumnInformation column)
        {
            if (column.GoodHighlight == "N" && column.BadHighlight == "N")
                return null;
            var field = string.Format("Fields!Field{0}.Value", column.Order);
            var styleValue = string.Format("=IIF(Right({0}, Len({0}) - InStr({0}, \"-\")) = \"[G]\", \"green\", " +
                                            "IIF(Right({0}, Len({0}) - InStr({0}, \"-\")) = \"[R]\", \"red\", " +
                                            "IIF(Right({0}, Len({0}) - InStr({0}, \"-\")) = \"[I]\", \"red\", " +
                                            "\"black\")))", field);

            return new XElement(_xmlns + "Color", styleValue);
        }

        public string StyleBoldValue(CustomColumnInformation column)
        {
            if (column.GoodHighlight == "N" && column.BadHighlight == "N")
                return "Normal";
            var field = string.Format("Fields!Field{0}.Value", column.Order);
            return string.Format("=IIF(Right({0}, Len({0}) - InStr({0}, \"-\")) = \"[G]\", \"Bold\", " +
                                  "IIF(Right({0}, Len({0}) - InStr({0}, \"-\")) = \"[R]\", \"Bold\", " +
                                  "IIF(Right({0}, Len({0}) - InStr({0}, \"-\")) = \"[I]\", \"Bold\", " +
                                  "IIF(Right({0}, Len({0}) - InStr({0}, \"-\")) = \"[B]\", \"Bold\", " +
                                  "\"Normal\"))))", field);
        }

        public string StyleItalicValue(CustomColumnInformation column)
        {
            if (column.GoodHighlight == "N" && column.BadHighlight == "N")
                return "Normal";

            var field = string.Format("Fields!Field{0}.Value", column.Order);
            return string.Format("=IIF(Right({0}, Len({0}) - InStr({0}, \"-\")) = \"[I]\", \"Italic\", "+
                                  "IIF(Right({0}, Len({0}) - InStr({0}, \"-\")) = \"[B]\", \"Italic\", " +
                                  "\"Normal\"))", field);
        }
    }
}
