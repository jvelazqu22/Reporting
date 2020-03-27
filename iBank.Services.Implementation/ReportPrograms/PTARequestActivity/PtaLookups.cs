using System.Collections.Generic;
using System.Linq;
using Domain.Orm.iBankClientQueries;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;

namespace iBank.Services.Implementation.ReportPrograms.PTARequestActivity
{
    public static class PtaLookups
    {
        public static string LookupAuthName(IClientQueryable clientQueryable, int authNo, int apSeq, string authStatus, string auth1Email)
        {
            if (authNo == 0)
            {
                return (!string.IsNullOrEmpty(auth1Email))
                    ? auth1Email.PadRight(34)
                    : authStatus.Equals("N")
                        ? "[Notification Only]"
                        : "Authorizer " + apSeq;
            }

            var user = new GetUserByUserNumberQuery(clientQueryable, authNo).ExecuteQuery();

            return user == null
                ? auth1Email.PadRight(34)
                : user.lastname + ", " + user.firstname;

        }

        public static string LookupAuthStatus(List<KeyValue> list, string key)
        {
            var authStat = list.FirstOrDefault(s => s.Key.Equals(key.Trim()));
            return authStat == null ? "Unknown" : authStat.Value;
        }

        public static string LookupAuthPageLink(string urlPath, string clientURL, int sGroupNbr, int travAuthNo, string authStatus)
        {
            if (authStatus.Trim().Equals("I") || authStatus.Trim().Equals("P"))
            {
                return $"{urlPath}kslogin.cfm&agclient={clientURL}&frpage=svcsOpenAuth.cfm" +
                    $"&frstyle={sGroupNbr}&frappkey={travAuthNo}";
            }
            return string.Empty;
        }
    }
}
