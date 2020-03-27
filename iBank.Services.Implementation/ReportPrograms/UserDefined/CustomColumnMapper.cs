using System;
using iBank.Server.Utilities;
using UserDefinedReports.Classes;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined
{
    public class CustomColumnMapper
    {
        public CustomColumnInformation BuildCustomColumn(UserReportColumnInformation column, string theme)
        {
            var newCol = new CustomColumnInformation
            {
                Header = column.Header1 + Environment.NewLine + column.Header2,
                Order = column.Order,
                Width = column.Width,
                IsDecimal = column.ColumnType.EqualsIgnoreCase("CURRENCY"),
                IsInteger = column.ColumnType.EqualsIgnoreCase("NUMERIC"),
                GroupBreak = column.GroupBreak,
                IsSubtotal = column.SubTotal,
                TotalThisField = column.TotalThisField,
                IsPageBreak = column.PageBreak,
                IsTripField = column.TableName.Contains("TRIP"),
                GoodOperator = column.GoodOperator,
                GoodValue = column.GoodValue,
                GoodHighlight = theme == "GRAYSCALE" && column.GoodHilite == "G"
                                ? ""
                                : column.GoodHilite,
                BadOperator = column.BadOperator,
                BadValue = column.BadValue,
                BadHighlight = theme == "GRAYSCALE" && column.BadHilite == "G"
                                ? ""
                                : column.BadHilite
            };

            switch (column.HorizontalAlignment)
            {
                case "1":
                    newCol.TextAlignment = HorizontalAlignment.Left;
                    break;
                case "2":
                    newCol.TextAlignment = HorizontalAlignment.Center;
                    break;
                case "3":
                    newCol.TextAlignment = HorizontalAlignment.Right;
                    break;
                default:
                    newCol.TextAlignment = HorizontalAlignment.Default;
                    break;
            }

            return newCol;
        }

    }
}
