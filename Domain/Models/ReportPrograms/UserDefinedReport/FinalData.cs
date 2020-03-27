using Domain.Interfaces;

namespace Domain.Models.ReportPrograms.UserDefinedReport
{
    public class FinalData : IRecKey
    {
        public FinalData()
        {
            RecKey = 0;
        }
        public int RecKey { get; set; }
    }
}
