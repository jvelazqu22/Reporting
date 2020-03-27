using System.Linq;
using System.Text.RegularExpressions;

using Domain.Helper;

using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.Shared.BuildWhereHelpers
{
    public class UdidWhere : AbstractWhere
    {
        public void GetUdidWhere(ReportGlobals globals, BuildWhere where, string inText)
        {
            var udid = globals.GetParmValue(WhereCriteria.UDIDNBR);
            int udidNbr;
            var validUdid = int.TryParse(udid, out udidNbr);
            if (validUdid && udidNbr != 0)
            {
                where.WhereClauseUdid = _whereClauseBuilder.AddToWhereClause(where.WhereClauseUdid, string.Format("udidno = {0}", udidNbr));
                var tempCaption = globals.GetLanguageTranslation("LT_UDIDNBR", "Udid #");
                
                globals.WhereText += string.Format(FIELD_EQUALS_VALUE, tempCaption, udid);

                var udidText = globals.GetParmValue(WhereCriteria.UDIDTEXT);
                if (!string.IsNullOrEmpty(udidText))
                {
                    //remove newlines, tabs, carriage returns
                    udidText = Regex.Replace(udidText, @"\t|\n|\r", "");
                    tempCaption = globals.GetLanguageTranslation("LT_UDIDTEXT", "Udid Text");
                    var udidTexts = udidText.Split('|').ToList();

                    where.WhereClauseUdid = _whereClauseBuilder.AddToWhereClause(where.WhereClauseUdid, udidTexts, "udidText", false);
                    if (udidTexts.Count == 1)
                    {
                        globals.WhereText += tempCaption + " = " + udidText.ToUpper();
                    }
                    else
                    {
                        globals.WhereText += tempCaption + inText + udidText.ToUpper();
                    }

                }
            }
        }
    }
}
