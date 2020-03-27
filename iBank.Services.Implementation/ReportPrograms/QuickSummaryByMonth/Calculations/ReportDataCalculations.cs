namespace iBank.Services.Implementation.ReportPrograms.QuickSummaryByMonth.Calculations
{
    public class ReportDataCalculations
    {
        public bool NeedToSumAirExcepts(decimal offeredCharge, decimal lostAmount)
        {
            return offeredCharge != 0 && lostAmount != 0;
        }
    }
}
