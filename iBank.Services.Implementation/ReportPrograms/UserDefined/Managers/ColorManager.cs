using iBank.Server.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers
{
    public class ColorManager
    {
        public string GetColor(UserReportColumnInformation column, string cellValue, string theme)
        {
            string color = "";
            string fieldType = column.GoodFieldType; //BadFieldType should be the same
            if ((column.GoodHilite != "N" && !string.IsNullOrEmpty(column.GoodHilite)) ||
                (column.BadHilite != "N" && !string.IsNullOrEmpty(column.BadHilite)))
            {
                cellValue = cellValue.Trim();
                if (column.GoodHilite != "N")
                {
                    var goodValue = column.GoodValue;
                    if (goodValue != "" || (goodValue == "" && fieldType == "NUMERIC"))
                    {
                        var goodOperator = column.GoodOperator.ToOperator();
                        bool isGood = TwoValuesCompareManager.Compare(cellValue, goodValue, goodOperator, fieldType);
                        if (isGood)
                            color = TranslateColorCode(column.GoodHilite, theme);
                    }
                }
                if (column.BadHilite != "N" && color == "")
                {
                    var badValue = column.BadValue;
                    if (badValue != "" || (badValue == "" && fieldType == "NUMERIC"))
                    {
                        var badOperator = column.BadOperator.ToOperator();
                        bool isBad = TwoValuesCompareManager.Compare(cellValue, badValue, badOperator, fieldType);
                        if (isBad)
                            color = TranslateColorCode(column.BadHilite, theme);
                    }
                }
            }
            return color;
        }

        public string TranslateColorCode(string code, string theme)
        {
            switch (theme)
            {
                case "GRAYSCALE":
                    if (code == "G") return "B"; //Black Bold Italic
                    if (code == "B") return "R"; //Red Bold
                    break;
                case "FRESH":
                    if (code == "G") return "I"; //Red Bold Italic
                    if (code == "B") return "G"; //Green Bold
                    break;
                case "BOLD":
                case "CLASSIC":
                    if (code == "G") return "G"; //Green Bold 
                    if (code == "B") return "R"; //Red Bold
                    break;
            }
            return "";
        }
    }
}
