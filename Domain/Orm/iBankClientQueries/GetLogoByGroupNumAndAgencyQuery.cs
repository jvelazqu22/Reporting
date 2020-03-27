using System.Linq;

using Domain.Helper;

using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries
{
    public class GetLogoByGroupNumAndAgencyQuery : IQuery<StyleGroupExtras>
    {
        public int SGroupNumber { get; set; }
        public string Agency { get; set; }

        private readonly string _fieldFunction = "";

        private readonly IClientQueryable _db;
        
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

        public StyleGroupExtras ExecuteQuery()
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
