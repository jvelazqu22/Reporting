using iBank.Services.Implementation.Shared.BuildWhereHelpers;

namespace iBank.Services.Implementation.Classes.Interfaces
{
    public interface IReportRunner
    {
        BuildWhere BuildWhere { get; set; }
        
        bool RunReport();

        int GetFinalRecordsCount();
    }
}
