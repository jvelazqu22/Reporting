using System.Linq;
using Domain.Exceptions;
using Domain.Helper;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetReportOutputLocationQuery : IQuery<ReportLocation>
    {
        private readonly IMastersQueryable _db;

        public GetReportOutputLocationQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public ReportLocation ExecuteQuery()
        {
            const string RPT_OUT_DIR = "rptoutdir";
            const string RPT_OUT_URL = "rptouturl";
            using (_db)
            {
                var recs = _db.MiscParams.Where(x => x.parmcode == RPT_OUT_DIR || x.parmcode == RPT_OUT_URL).ToList();
                if (!recs.Any()) throw new ReportOutputLocationException("No report output directory or url found.");

                var outputDirectory = recs.FirstOrDefault(x => x.parmcode.Trim() == RPT_OUT_DIR);
                if (outputDirectory == null) throw new ReportOutputLocationException("No report output directory found.");

                var outputUrl = recs.FirstOrDefault(x => x.parmcode.Trim() == RPT_OUT_URL);
                if (outputUrl == null) throw new ReportOutputLocationException("No report output url found.");

                return new ReportLocation { ReportOutputDirectory = outputDirectory.parmdesc.Trim(), ReportOutputUrl = outputUrl.parmdesc.Trim() };
            }
        }
    }
}
