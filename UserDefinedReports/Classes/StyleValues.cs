
namespace UserDefinedReports.Classes
{
    public class StyleValues
    {
        public string Hidden { get; set; }
        public string Color { get; set; }
        public string FontColor { get; set; }
        public string FontFamily { get; set; }

        private StyleValues(string hidden, string color, string fontColor, string fontFamily)
        {
            Hidden = hidden;
            Color = color;
            FontColor = fontColor;
            FontFamily = fontFamily;
        }

        public static StyleValues Build(string styleName)
        {
            switch (styleName.ToUpper())
            {
                case "FRESH":
                    return new StyleValues("false", "#F1F2F2", "#65C298", "Open Sans Semibold");
                case "BOLD":
                    return new StyleValues("false", "Black", "White", "Times New Roman");
                case "GRAYSCALE":
                    return new StyleValues("true", "Black", "Black","Arial");
                default:
                    return new StyleValues("true", "Navy", "Navy","Bookman Old Style");
            }

        }
    }
}
