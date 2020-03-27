using Domain.iBank.Services.Orm.Classes;
using Domain.Interfaces;
using iBank.Services.Orm.Databases.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetPendingReportsQuery : IQuery<IList<PendingReportInformation>>
    {
        private IMastersQueryable _db;
        private IList<int> DemoUsers { get; set; }
        private int ServerNumber { get; set; }

        public GetPendingReportsQuery(IMastersQueryable db, IList<int> demoUsers, int serverNumber)
        {
            _db = db;
            DemoUsers = demoUsers;
            ServerNumber = serverNumber;
        }

        public IList<PendingReportInformation> ExecuteQuery()
        {
            using (_db)
            {
                //get all the reports for the demo users 
                var demoUsers = new List<PendingReportInformation>();
                if (DemoUsers.Any())
                {
                    demoUsers = _db.ReportHandoff.Where(x => x.svrnumber == 0
                                                    && x.parmname.ToUpper().Trim() == "REPORTSTATUS"
                                                    && x.parmvalue.ToUpper().Trim() == "PENDING"
                                                    && DemoUsers.Contains(x.usernumber))
                                        .Select(x => new PendingReportInformation
                                        {
                                            ReportId = x.reportid,
                                            Agency = x.agency,
                                            ColdFusionBox = x.cfbox,
                                            UserNumber = x.usernumber,
                                            ServerNumber = ServerNumber,
                                            IsDemoUser = true
                                        }).ToList();
                }

                var demoIds = demoUsers.Select(x => x.ReportId.Trim());

                //get all the other reports
                var allReports =  _db.ReportHandoff.Where(x => x.svrnumber == 0
                                                    && x.parmname.ToUpper().Trim() == "REPORTSTATUS"
                                                    && x.parmvalue.ToUpper().Trim() == "PENDING"
                                                    && !demoIds.Contains(x.reportid.Trim()))
                                        .Select(x => new PendingReportInformation
                                         {
                                            ReportId = x.reportid,
                                            Agency = x.agency,
                                            ColdFusionBox = x.cfbox,
                                            UserNumber = x.usernumber,
                                            ServerNumber = ServerNumber,
                                            IsDemoUser = false
                                        }).ToList();

                allReports.AddRange(demoUsers);

                return allReports;
            }

        }
    }
}
