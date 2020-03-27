using System.Collections.Generic;
using System.Linq;
using Domain.Exceptions;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankClientQueries
{
    public class GetAllAccountsByParentAcctQuery : IQuery<IList<string>>
    {
        public IList<string> ParentAcct { get; set; }

        private readonly IClientQueryable _db;

        private readonly bool _containsWildcard;

        public GetAllAccountsByParentAcctQuery(IClientQueryable db, string parentAcct, bool containsWildcard = false)
        {
            _db = db;
            ParentAcct = new List<string> { parentAcct };
            _containsWildcard = containsWildcard;
        }

        public GetAllAccountsByParentAcctQuery(IClientQueryable db, IList<string> parentAccts)
        {
            _db = db;
            ParentAcct = parentAccts;
            _containsWildcard = false;
        }

        public IList<string> ExecuteQuery()
        {
            using (_db)
            {
                if (_containsWildcard)
                {
                    var parentAcct = ParentAcct[0];
                    var parentAcctSansWildcard = parentAcct.Replace("%", "").Replace("*", "").Replace("_", "").Replace("?", "").Trim();

                    //ex: %100
                    if (parentAcct.StartsWith("%") || parentAcct.StartsWith("*"))
                    {
                        return _db.AcctMast.Where(x => x.parentacct.Trim().EndsWith(parentAcctSansWildcard)).Select(x => x.acct.Trim()).Distinct().ToList();
                    }

                    //ex: 100%
                    if (parentAcct.EndsWith("%") || parentAcct.EndsWith("*"))
                    {
                        return _db.AcctMast.Where(x => x.parentacct.Trim().StartsWith(parentAcctSansWildcard)).Select(x => x.acct.Trim()).Distinct().ToList();
                    }
                    
                    //ex: _100
                    if (parentAcct.StartsWith("_") || parentAcct.StartsWith("?"))
                    {
                        return _db.AcctMast.Where(x => x.parentacct.Trim().EndsWith(parentAcctSansWildcard)
                                                       && x.parentacct.Trim().Length == (parentAcctSansWildcard.Length + 1))
                                            .Select(x => x.acct.Trim()).Distinct ().ToList();
                    }

                    //ex: 100_
                    if (parentAcct.EndsWith("_") || parentAcct.EndsWith("?"))
                    {
                        return _db.AcctMast.Where(x => x.parentacct.Trim().StartsWith(parentAcctSansWildcard)
                                                        && x.parentacct.Trim().Length == (parentAcctSansWildcard.Length + 1))
                                            .Select(x => x.acct.Trim()).Distinct().ToList();
                    }

                    throw new UnknownWildcardException($"Wildcard present in string {parentAcct} not handled.");
                }
                else
                {
                    var parentAcct = ParentAcct[0];
                    return ParentAcct.Count == 1
                        ? _db.AcctMast.Where(x => x.parentacct.Trim().Equals(parentAcct)).Select(x => x.acct.Trim()).Distinct().ToList()
                        : _db.AcctMast.Where(x => ParentAcct.Contains(x.parentacct)).Select(x => x.acct.Trim()).Distinct().ToList();
                }
            }
        }
    }
}
