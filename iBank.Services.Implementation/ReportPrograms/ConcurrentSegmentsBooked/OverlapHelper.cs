using iBank.Server.Utilities.Classes;
using System.Collections.Generic;

namespace iBank.Services.Implementation.ReportPrograms.ConcurrentSegmentsBooked
{
    public class OverlapHelper
    {
        public List<string> GetExportFields(ReportGlobals globals)
        {
            var exportFields = new List<string>();
            if (globals.User.AccountBreak)
            {
                exportFields.Add("acct");
                exportFields.Add("acctdesc");
            }
            exportFields.Add("MatchDesc");
            exportFields.Add("airline");
            exportFields.Add("passlast");
            exportFields.Add("passfrst");
            exportFields.Add("DepartDate");
            exportFields.Add("MidDate");
            exportFields.Add("orgdesc");
            exportFields.Add("destdesc");
            exportFields.Add("recloc");
            exportFields.Add("GDSDesc");
            exportFields.Add("ticket");
            exportFields.Add("bookdate");
            exportFields.Add("airchg");

            return exportFields;
        }
    }
}
