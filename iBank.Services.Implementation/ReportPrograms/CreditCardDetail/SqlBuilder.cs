using Domain.Helper;
using Domain.Models;
using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared.AdvancedClause;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;

namespace iBank.Services.Implementation.ReportPrograms.CreditCardDetail
{
    public static class SqlBuilder
    {
        public static SqlScript CreateScript(ReportGlobals globals, string whereClause, BuildWhere buildWhere, bool includeAllLegs)
        {
            var ccCompany = globals.GetParmValue(WhereCriteria.DDCREDCARDCOMP);
            var ccNumber = globals.GetParmValue(WhereCriteria.TXTCCNUM);
            var script = new SqlScript
            {
                WhereClause = whereClause.Replace("T1.acct", "T1.AcctNbr").Replace("useracctnbrs", "useraccts")
            };

            switch (ccCompany)
            {
                case "MC":
                    script.WhereClause += " and T1.CardType in ('MC','CA')";
                    break;
                case "VI":
                    script.WhereClause += " and T1.CardType in ('VI','BA')";
                    break;
                default:
                    script.WhereClause += " and T1.CardType = '" + ccCompany + "'";
                    break;
            }

            if (!string.IsNullOrEmpty(ccNumber))
            {
                script.WhereClause += " and right(ltrim(rtrim(T1.cardnum))," + ccNumber.Length + ") " + SharedProcedures.FixWildcard(ccNumber);
                globals.WhereText += "Card # = " + ccNumber;

            }
            script.FromClause = "ccTrans T1";
            script.FieldList = "CardType, CardNum, RefNbr, PostDate, TranDate, RecType, MerchName, MerchAddr1, MerchCity, MerchState, MerchSIC, TransAmt, TaxAmt ";
            script.OrderBy = "order by CardType, CardNum, PostDate, RefNbr";

            script.WhereClause = new WhereClauseWithAdvanceParamsHandler().GetWhereClauseWithAdvancedParametersAndUpdateSqlScriptIfNeeded(script, buildWhere, includeAllLegs);

            return script;
        }

        public static string GetSql(ReportGlobals globals, string whereClause)
        {
            var ccCompany = globals.GetParmValue(WhereCriteria.DDCREDCARDCOMP);
            var ccNumber = globals.GetParmValue(WhereCriteria.TXTCCNUM);
            var script = new SqlScript
            {
                WhereClause = whereClause.Replace("T1.acct", "T1.AcctNbr").Replace("useracctnbrs", "useraccts")
            };

            switch (ccCompany)
            {
                case "MC":
                    script.WhereClause += " and T1.CardType in ('MC','CA')";
                    break;
                case "VI":
                    script.WhereClause += " and T1.CardType in ('VI','BA')";
                    break;
                default:
                    script.WhereClause += " and T1.CardType = '" + ccCompany + "'";
                    break;
            }

            if (!string.IsNullOrEmpty(ccNumber))
            {
                script.WhereClause += " and right(ltrim(rtrim(T1.cardnum))," + ccNumber.Length +") " + SharedProcedures.FixWildcard(ccNumber);
                globals.WhereText += "Card # = " + ccNumber;
               
            }
            script.FromClause = "ccTrans T1";
            script.FieldList = "CardType, CardNum, RefNbr, PostDate, TranDate, RecType, MerchName, MerchAddr1, MerchCity, MerchState, MerchSIC, TransAmt, TaxAmt ";
            script.OrderBy = "order by CardType, CardNum, PostDate, RefNbr";

            return $"select {script.FieldList} from {script.FromClause} where {script.WhereClause} {script.OrderBy}";
        }
    }
}
