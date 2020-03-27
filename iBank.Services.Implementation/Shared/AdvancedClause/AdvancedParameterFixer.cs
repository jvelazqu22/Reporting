using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Helper;

using iBank.Server.Utilities;

namespace iBank.Services.Implementation.Shared.AdvancedClause
{
    public class AdvancedParameterFixer
    {
        public string CreateCalculation(string arg, string txtToFind, string txtToAdd)
        {
            if (arg.Contains(txtToFind))
            {
                var temp = "";

                var index = arg.IndexOf(txtToFind, StringComparison.OrdinalIgnoreCase);

                if (index > 0)
                {
                    temp = arg.Left(index);
                }

                temp = temp + txtToAdd;

                if ((index + txtToFind.Length) < arg.Length)
                {
                    temp = temp + arg.Substring(index + txtToFind.Length);
                }

                return temp;
            }
            else
            {
                return arg;
            }
        }

        public string FixParameterAdvancedFieldName(AdvancedParameter parm)
        {
            var arg = parm.FieldName.ToUpper();

            if (arg.Contains("CARCOST")) return CreateCalculation(arg, "CARCOST", "ABOOKRAT*DAYS");

            if (arg.Contains("HOTELCOST")) return CreateCalculation(arg, "HOTELCOST", "BOOKRATE*NIGHTS*ROOMS");

            if (arg.Contains("DAYCOST")) return CreateCalculation(arg, "DAYCOST", "ABOOKRAT");

            if (arg.Contains("NITECOST")) return CreateCalculation(arg, "NITECOST", "BOOKRATE");

            if (arg.Contains("HOTCOST")) return CreateCalculation(arg, "HOTCOST", "BOOKRATE*NIGHTS*ROOMS");

            if (arg.Contains("SAVEDAIR")) return CreateCalculation(arg, "SAVEDAIR", "(STNDCHG-AIRCHG)");

            if (arg.Contains("SAVEDMKT")) return CreateCalculation(arg, "SAVEDMKT", "(MKTFARE-AIRCHG)");

            if (arg.Contains("LOSTAIR")) return CreateCalculation(arg, "LOSTAIR", "(AIRCHG-OFFRDCHG)");

            if (arg.Contains("CPM") && arg != "GROSAVGCPM")
            {
                var txtToAdd = "CASE MILES WHEN 0 THEN 0.00 ELSE ROUND((ACTFARE+MISCAMT)/MILES,2)) END";
                return CreateCalculation(arg, "CPM", txtToAdd);
            }

            if (arg.Contains("CARLOST")) return CreateCalculation(arg, "CARLOST", "(ABOOKRAT-AEXCPRAT)*DAYS");

            if (arg.Contains("HOTLOST")) return CreateCalculation(arg, "HOTLOST", "(BOOKRATE-HEXCPRAT)*NIGHTS*ROOMS");

            if (arg.Contains("RENTDTDOW")) return "datepart(dw,rentdate)";

            if (arg.Contains("RETDATEDOW")) return "datepart(dw,dateback)";

            if (arg.Contains("DATEINDOW")) return "datepart(dw,datein)";

            if (arg.Contains("CHKOUTDOW")) return "datepart(dw,dateout)";

            if (arg.Contains("DEPDTDOW")) return "datepart(dw,depdate)";

            if (arg.Contains("ARRDTDOW")) return "datepart(dw,arrdate)";

            if (arg.Contains("INVDTDOW")) return "datepart(dw,invdate)";

            if (arg.Contains("BOOKDTDOW")) return "datepart(dw,bookdate)";

            if (arg.Contains("ARRDATEDOW")) return "datepart(dw,tripend)";

            if (arg.Contains("ADVBOOK")) return "datediff(day,bookdate,depdate)";

            if (arg.Contains("ADVPURCH")) return "datediff(day,invdate,depdate)";

            if (arg.Contains("AXTADVDAYS")) return "datediff(day,trandate,depdate)";

            if (arg.Contains("TRIPDAYS")) return "(datediff(day,T1.depdate,T1.arrdate) + 1) * T1.plusmin";

            return parm.FieldName;
        }

        private readonly List<string> _sColumns = new List<string> { "SACCT", "SACCOUNT", "SINVOICE", "SRECLOC", "SPASSLAST", "SPASSFRST", "SPAXNAME" };
        public IList<AdvancedParameter> RemoveAdvancedFieldNameAndFieldNamePrefix(IList<AdvancedParameter> advancedParams)
        {
            foreach (var param in advancedParams.Where(param => _sColumns.Contains(param.AdvancedFieldName.ToUpper()) || _sColumns.Contains(param.FieldName.ToUpper())))
            {
                param.AdvancedFieldName = param.AdvancedFieldName.RemoveFirstChar();
                param.FieldName = param.FieldName.RemoveFirstChar();
            }

            return advancedParams;
        }

        public string TranslateValue1DayOfWeekNumericToCharacter(AdvancedParameter parm)
        {
            int day;
            if (int.TryParse(parm.Value1, out day))
            {
                switch (day)
                {
                    case 1:
                        return "SUN";
                    case 2:
                        return "MON";
                    case 3:
                        return "TUE";
                    case 4:
                        return "WED";
                    case 5:
                        return "THU";
                    case 6:
                        return "FRI";
                    case 7:
                        return "SAT";
                }
            }

            return parm.Value1;
        }

        public string TranslateValue1DayOfWeekCharacterToNumeric(AdvancedParameter parm)
        {
            switch (parm.Value1.ToUpper())
            {
                case "SUN":
                case "SUNDAY":
                    return "1";
                case "MON":
                case "MONDAY":
                    return "2";
                case "TUE":
                case "TUES":
                case "TUESDAY":
                    return "3";
                case "WED":
                case "WEDNESDAY":
                    return "4";
                case "THU":
                case "THURS":
                case "THURSDAY":
                    return "5";
                case "FRI":
                case "FRIDAY":
                    return "6";
                case "SAT":
                case "SATURDAY":
                    return "7";
            }

            return parm.Value1;
        }

        private readonly List<string> _prefaceWithT1 = new List<string> { "RECLOC", "ACCT", "PASSLAST", "PASSFRST", "INVOICE", "SVCFEE", "CARDNUM", "TRANTYPE", "SOURCEABBR", "GDS", "MONEYTYPE" };
        public string HandleAmbiguousFields(string originalField, string tableName)
        {
            originalField = originalField.Trim();
            //some fields need to be prefaced with a T1
            if (_prefaceWithT1.Contains(originalField))
                return "T1." + originalField;

            //special cases
            if (originalField.EqualsIgnoreCase("IATANBR") && tableName.EqualsIgnoreCase("TRIPS"))
                return "T1." + originalField;

            if (originalField.EqualsIgnoreCase("AMONEYTYPE"))
                return "T4.MONEYTYPE";

            if (originalField.EqualsIgnoreCase("HMONEYTYPE"))
                return "T5.MONEYTYPE";

            return originalField.Trim();
        }
    }
}
