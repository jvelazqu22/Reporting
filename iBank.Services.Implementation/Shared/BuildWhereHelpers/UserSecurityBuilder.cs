using System.Linq;

using Domain.Helper;
using Domain.Orm.iBankClientQueries;

using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Helpers;

namespace iBank.Services.Implementation.Shared.BuildWhereHelpers
{
    public class UserSecurityBuilder
    {
        private IClientQueryable _db;
        private IClientQueryable ClientDb
        {
            get
            {
                return _db.Clone() as IClientQueryable;
            }
            set
            {
                _db = value;
            }
        }

        private readonly int _userNumber;

        

        public UserSecurityBuilder(IClientQueryable db, int userNumber)
        {
            ClientDb = db;
            _userNumber = userNumber;
        }

        public string GetAllowedUserSource(ClientType clientType, string existingWhereClauseTrip, string agency, AllRecords userSource)
        {
            var whereClauseBuilder = new WhereClauseBuilder();
            if (clientType == ClientType.Sharer)
            {
                existingWhereClauseTrip = whereClauseBuilder.AddToWhereClause(existingWhereClauseTrip, string.Format("T1.corpacct = '{0}'", agency));

                if (userSource != AllRecords.All)
                {
                    var notIn = userSource == AllRecords.AllExceptSpecified;
                    var allowedAgencies = new GetSharerTypeAllowedAgenciesByUserNumberQuery(ClientDb, _userNumber).ExecuteQuery();

                    var agenciesToAdd = allowedAgencies.Any()
                                        ? SharedProcedures.OrList(allowedAgencies.ToList(), "T1.agency", notIn)
                                        : $"T1.agency = '{agency}'";
                    existingWhereClauseTrip = whereClauseBuilder.AddToWhereClause(existingWhereClauseTrip, agenciesToAdd);
                }
            }
            else //Agency
            {
                if (userSource != AllRecords.All)
                {
                    var notIn = userSource == AllRecords.AllExceptSpecified;
                    var allowedAgencies = new GetAgencyTypeAllowedAgenciesByUserNumberQuery(ClientDb, _userNumber).ExecuteQuery();

                    var agenciesToAdd = allowedAgencies.Any()
                                            ? SharedProcedures.OrList(allowedAgencies.ToList(), "T1.SourceAbbr", notIn)
                                            : $"T1.SourceAbbr = '{agency}'";
                    existingWhereClauseTrip = whereClauseBuilder.AddToWhereClause(existingWhereClauseTrip, agenciesToAdd);
                }

                existingWhereClauseTrip = whereClauseBuilder.AddToWhereClause(existingWhereClauseTrip, string.Format("T1.agency = '{0}'", agency));
            }

            return existingWhereClauseTrip;
        }

        public string GetAllowedUserAcct(string existingWhereClauseTrip, AllRecords userAccounts)
        {
            var whereClauseBuilder = new WhereClauseBuilder();
            var userAccts = new GetUserAcctsByUserNumberQuery(ClientDb, _userNumber).ExecuteQuery().ToList();
            if (userAccounts == AllRecords.Specified)
            {
                if (userAccts.Any())
                {
                    var accts = SharedProcedures.OrList(userAccts, "T1.acct");
                    existingWhereClauseTrip = whereClauseBuilder.AddToWhereClause(existingWhereClauseTrip, accts);
                }else
                {
                    //did not find any specified account, we need to include this query otherwise it returns all accounts
                    existingWhereClauseTrip = whereClauseBuilder.AddToWhereClause(existingWhereClauseTrip, $"T1.acct in (select acct from useraccts where UserNumber = {_userNumber})");
                }
            }

            if (userAccounts == AllRecords.AllExceptSpecified)
            {
                if (userAccts.Any())
                {
                    var accts = SharedProcedures.OrList(userAccts, "T1.acct", true);
                    existingWhereClauseTrip = whereClauseBuilder.AddToWhereClause(existingWhereClauseTrip, accts);
                }
            }

            return existingWhereClauseTrip;
        }

        public string GetUserBreaks(string existingWhereClauseTrip, bool allBreaksOne, bool allBreaksTwo)
        {
            //we need to add break1 and break2 clause when allBreaksOne and allBreaksTwo are not flaged, 
            //regardless if there are any items stored in userbrks1 and userbrks2 respectively
            var whereClauseBreaks = "";
            if (!allBreaksOne)
            {
               whereClauseBreaks = $"break1 in (select break1 from userbrks1 where UserNumber = {_userNumber})";
               existingWhereClauseTrip = string.IsNullOrEmpty(existingWhereClauseTrip) ? whereClauseBreaks : string.Format("{0} AND {1}", existingWhereClauseTrip, whereClauseBreaks);
            }

            if (!allBreaksTwo)
            {
                whereClauseBreaks = $"break2 in (select break2 from userbrks2 where UserNumber = {_userNumber})";
                existingWhereClauseTrip = string.IsNullOrEmpty(existingWhereClauseTrip) ? whereClauseBreaks : string.Format("{0} AND {1}", existingWhereClauseTrip, whereClauseBreaks);     
            }

            return existingWhereClauseTrip;
        }
    }
}
