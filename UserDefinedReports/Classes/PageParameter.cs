using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserDefinedReports.Classes
{
    public class PageParameter
    {
        public double FontSize { get; set; }
        public double FontConverter { get; set; }
        public double CellHeight { get; set; }
        public double PageWidth { get; set; }
        public double PageHeight { get; set; }
        public double MaxContentWidth { get; set; }

        public PageParameter(double fontSize, double fontConverter, double cellHeight, double pageWidth, double pageHeight, double maxContentWidth)
        {
            FontSize = fontSize;
            FontConverter = fontConverter;
            CellHeight = cellHeight;
            PageWidth = pageWidth;
            PageHeight = pageHeight;
            MaxContentWidth = maxContentWidth;
        }
    }
}
