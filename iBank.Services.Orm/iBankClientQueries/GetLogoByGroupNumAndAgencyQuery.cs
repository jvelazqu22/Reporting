using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankClientQueries
{
    public class GetLogoByGroupNumAndAgencyQuery : BaseiBankClientQueryable<StyleGroupExtra>
    {
        public int SGroupNumber { get; set; }
        public string Agency { get; set; }

        private readonly string _fieldFunction = "";

        public enum ReportType
        {
            StandardReport,
            BcstReport
        }
        
        public GetLogoByGroupNumAndAgencyQuery(IClientQueryable db, int sGroupNumber, string agency, ReportType reportType)
        {
            _db = db;
            SGroupNumber = sGroupNumber;
            Agency = agency;

            switch (reportType)
            {
                case ReportType.StandardReport:
                    _fieldFunction = "RPTLOGO_IMAGENBR";
                    break;
                case ReportType.BcstReport:
                    _fieldFunction = "BCSTLOGO_IMAGENBR";
                    break;
            }
        }

        public override StyleGroupExtra ExecuteQuery()
        {
            using (_db)
            {
                return _db.StyleGroupExtra.FirstOrDefault(x => x.SGroupNbr == SGroupNumber 
                                                            && x.ClientCode.Trim() == Agency.Trim() 
                                                            && x.FieldFunction.Trim() == _fieldFunction);
            }
        }
    }
}
