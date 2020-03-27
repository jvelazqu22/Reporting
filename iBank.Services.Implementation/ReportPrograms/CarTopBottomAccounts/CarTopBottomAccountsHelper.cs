using Domain.Models.ReportPrograms.CarTopBottomAccountsReport;

namespace iBank.Services.Implementation.ReportPrograms.CarTopBottomAccounts
{
    public static class CarTopBottomAccountsHelper
    {
        public static string GetColHead1(string groupBy)
        {
            switch (groupBy)
            {
                case "2":
                    return "Parent Account";
                case "3":
                    return "Data Source";
                default:
                    return "Account";

            }
        }

        public static decimal GetGraphData1(FinalData rec, string sortBy)
        {
            switch (sortBy)
            {
                case "2":
                    return rec.Rentals;
                case "3":
                    return rec.Days;
                case "4":
                    return rec.avgbook;
                default:
                    return rec.Carcost;
            }
        }

    }
}
