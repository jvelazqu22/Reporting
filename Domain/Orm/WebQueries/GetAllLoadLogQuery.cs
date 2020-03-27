using System.Collections.Generic;
using System.Linq;

using Domain.Models.ViewModels;

using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.WebQueries
{
    public class GetAllLoadLogQuery : IQuery<IList<LoadLogViewModelDisplayData>>
    {
        private IMastersQueryable _db;
        private SearchLoadLogViewModel _searchParameters;
        public GetAllLoadLogQuery(IMastersQueryable db, SearchLoadLogViewModel searchParameters)
        {
            _db = db;
            _searchParameters = searchParameters;
        }
        ~GetAllLoadLogQuery()
        {
            _db.Dispose();
        }

        public IList<LoadLogViewModelDisplayData> ExecuteQuery()
        {
            var select = "select t2.recordno as llrecno, t2.loaddate, t2.loadtype, t2.sourceabbr, t2.sourcever, t2.gds_bo, t2.loadmsg, t2.triprecs, t3.startdate, t3.enddate ";
            var from = "from MstrAgcySources T1 ";
            var innerJoin1 = "inner join LoadLog T2 on t1.sourceabbr = t2.sourceabbr and T1.agency = T2.agency ";
            var innerJoin2 = "inner join LoadLogExtras T3 on t2.recordno = t3.llrecno ";
            var where = GetWhereClause();
            var sql = select + from + innerJoin1 + innerJoin2 + where;
            return _db.Database.SqlQuery<LoadLogViewModelDisplayData>(sql).ToList();
        }

        public string GetWhereClause()
        {
            var where = "where T1.agency = 'demo' and T1.SourceAbbr is not null and T2.LoadType is not null and len(ltrim(T2.LoadType)) > 1 ";
            if (_searchParameters == null) return where;

            where += "AND t2.loaddate BETWEEN '" + _searchParameters.FromLoadDateSelected + "' AND '" + _searchParameters.ToLoadDateSelected + "' ";

            if ( !string.IsNullOrWhiteSpace(_searchParameters.LoadTypeSelected) &&  !_searchParameters.LoadTypeSelected.Equals("All"))
                where += "AND t2.loadtype = '" + _searchParameters.LoadTypeSelected + "' ";

            if (!string.IsNullOrWhiteSpace(_searchParameters.GdsBo) && !_searchParameters.GdsBo.Equals("All"))
                where += "AND t2.gds_bo = '" + _searchParameters.GdsBo + "' ";

            if (!string.IsNullOrWhiteSpace(_searchParameters.DataSource) && !_searchParameters.DataSource.Equals("All"))
                where += "AND t2.sourceabbr = '" + _searchParameters.DataSource + "' ";

            return where;
        }
    }
}
