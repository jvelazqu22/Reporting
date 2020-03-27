using System;

namespace Domain.Models.ReportPrograms.ExecutiveSummaryYearToYearReport
{
    public class FinalData
    {
        public int RowNum { get; set; } = 0;
        public string Grp { get; set; } = string.Empty;
        public int GrpSort { get; set; }
        public int SubGrp { get; set; }
        public string Descrip { get; set; } = string.Empty;
        public DisplayType DisplayType { get; set; }
        public decimal MonthValuePreviousYear { get; set; } = 0;
        public decimal YearToDatePreviousYear { get; set; } = 0;
        public decimal MonthValueCurrentYear { get; set; } = 0;
        public decimal YearToDateCurrentYear { get; set; } = 0;
        public string CurrencySymbol { get; set; } = "$";
        public string SymbolPosition { get; set; } = "L";

        //MonthValuePreviousYear
        public string ColVal1 => GetStringRepresentation(MonthValuePreviousYear);

        //YearToDatePreviousYear
        public string ColVal2 => GetStringRepresentation(YearToDatePreviousYear);

        //MonthValueCurrentYear
        public string ColVal3 => GetStringRepresentation(MonthValueCurrentYear);

        //YearToDateCurrentYear
        public string ColVal4 => GetStringRepresentation(YearToDateCurrentYear);
        
        //MonthValueYearToYearChange
        public string ColVal5 => GetCalculatedPercentage(MonthValueCurrentYear, MonthValuePreviousYear);

        //YearToDateYearToYearChange
        public string ColVal6 => GetCalculatedPercentage(YearToDateCurrentYear, YearToDatePreviousYear);

        private string AddCurrency(decimal val)
        {
            return SymbolPosition == "L"
            ? CurrencySymbol.Trim() + val.ToString("C").Trim().Replace("$", string.Empty)
            : val.ToString("C").Trim().Replace("$", string.Empty) + CurrencySymbol.Trim();
        }

        private string GetStringRepresentation(decimal val)
        {
            switch (DisplayType)
            {
                case DisplayType.Currency:
                    return AddCurrency(val);
                case DisplayType.Percentage:
                    return val.ToString("P");
                case DisplayType.OneDecimal:
                    return val.ToString("0.0");
                case DisplayType.Integer:
                default:
                    return val.ToString("N0");
            }
        }

        private string GetCalculatedPercentage(decimal valOne, decimal valTwo)
        {
            switch (DisplayType)
            {
                case DisplayType.Percentage:
                    valOne = Math.Round(valOne, 4);
                    valTwo = Math.Round(valTwo, 4);
                    break;
                case DisplayType.Currency:
                    valOne = Math.Round(valOne, 2);
                    valTwo = Math.Round(valTwo, 2);
                    break;
                case DisplayType.OneDecimal:
                    valOne = Math.Round(valOne, 1);
                    valTwo = Math.Round(valTwo, 1);
                    break;
                case DisplayType.Integer:
                default:
                    valOne = Math.Round(valOne, 0);
                    valTwo = Math.Round(valTwo, 0);
                    break;
            }

            var calculatedVal = valTwo == 0 ? 0 : (valOne - valTwo) / valTwo;
            return calculatedVal.ToString("P");
        }
    }

    public enum DisplayType
    {
        Integer,
        OneDecimal,
        Currency,
        Percentage
    }
}
