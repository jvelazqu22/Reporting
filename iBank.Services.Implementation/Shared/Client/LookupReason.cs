using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using System.Collections.Generic;
using System.Linq;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;

namespace iBank.Services.Implementation.Shared.Client
{
    public class LookupReason
    {
        public string Lookup(string code, string accountId, IClientDataStore clientStore, ReportGlobals globals, IMastersQueryable masterDb, 
            List<MasterAccountInformation> accounts, IList<ReasonCode> reasonCodes, IList<ReasonSetInfo> reasonSets, bool useLongDescription = false)
        {
            code = code.Trim(' ', ',');
            if (code == string.Empty) return string.Empty;
            string[] reasonCodeList = code.Split(',');
            var lookup = string.Empty;
            var setDefLang = globals.UserLanguage;
            var acct = accounts.FirstOrDefault(s => s.AccountId == accountId.Trim());

            int setNbr;
            setNbr = acct?.ReasSetNbr ?? 0;

            foreach (var item in reasonCodeList)
            {
                if (item == string.Empty) continue;

                //** 1ST SEARCH - Use the user's language, and the "real" reason set number. **
                var reasonCode = reasonCodes.FirstOrDefault(s => s.ReasCode == item && s.LangCode == globals.UserLanguage && s.ReasSetNbr == setNbr);
                var lookup2 = string.Empty;
                if (reasonCode != null) lookup2 = useLongDescription || reasonCode.LongDesc.IsNullOrWhiteSpace() ? reasonCode.ReasDesc : reasonCode.LongDesc;

                //** 2nd SEARCH - Use the default language for the reason set, **
                //** and the "real" reason set number. **
                var reasonSet = reasonSets.FirstOrDefault(s => s.ReasSetNbr == setNbr);
                if (lookup2.IsNullOrWhiteSpace() || globals.MultiLingual)
                {
                    //**LOOK UP THE DEFAULT LANGUAGE FOR THE REASON SET. **
                    if (reasonSet != null && !reasonSet.DefLang.IsNullOrWhiteSpace()) setDefLang = reasonSet.DefLang;
                    if (setDefLang != globals.DefaultLanguage)
                    {
                        reasonCode = reasonCodes.FirstOrDefault(s => s.ReasCode == item && s.LangCode == setDefLang && s.ReasSetNbr == setNbr);
                        if (reasonCode != null) lookup2 = useLongDescription || reasonCode.LongDesc.IsNullOrWhiteSpace() ? reasonCode.ReasDesc : reasonCode.LongDesc;
                    }
                }
                //** (3rd) search for the English version for the reason set. **
                //** WE HAVE ALREADY ESTABLISHED THE VALUE OF lcSetDefLang,   **
                //** IN THE 2ND SEARCH ATTEMPT.                               **
                if (lookup2.IsNullOrWhiteSpace() && globals.MultiLingual && setDefLang != "EN" &&
                    globals.UserLanguage != "EN")
                {
                    reasonCode = reasonCodes.FirstOrDefault( s => s.ReasCode == item && s.LangCode == "EN" && s.ReasSetNbr == setNbr);
                    if (reasonCode != null) lookup2 = useLongDescription || reasonCode.LongDesc.IsNullOrWhiteSpace() ? reasonCode.ReasDesc : reasonCode.LongDesc;
                }

                //**(4th) search: Use the user's language and the default reason **
                //* *set number of 0(zero).                                      **
                if (lookup2.IsNullOrWhiteSpace() && setNbr != 0)
                {
                    reasonCode = reasonCodes.FirstOrDefault(s => s.ReasCode == item && s.LangCode == "EN" && s.ReasSetNbr == 0);
                    if (reasonCode != null) lookup2 = useLongDescription || reasonCode.LongDesc.IsNullOrWhiteSpace() ? reasonCode.ReasDesc : reasonCode.LongDesc;
                }

                //**(5th) search: Use the site default language and the **
                //**default reason set number of 0(zero).              * *
                var siteDefLang = "EN";
                //What is the site's default language?
                var extras = masterDb.ClientExtras.FirstOrDefault(s => s.ClientCode == globals.Agency && s.FieldFunction == "SITEDEFAULTLANGUAGE");
                if (extras != null) siteDefLang = extras.FieldData;

                //Note: the FP code checks lnSetNbr !=0, but seeks on an expression with 0 as the SetNbr. 
                //Is this contradictory?
                if (lookup2.IsNullOrWhiteSpace() && setNbr != 0 && siteDefLang != globals.UserLanguage)
                {
                    reasonCode = reasonCodes.FirstOrDefault(s => s.ReasCode == item && s.LangCode == siteDefLang && s.ReasSetNbr == 0);
                    if (reasonCode != null) lookup2 = useLongDescription || reasonCode.LongDesc.IsNullOrWhiteSpace() ? reasonCode.ReasDesc : reasonCode.LongDesc;
                }
                //**6th search: Use the English version and the default **
                //** reason set number of 0(zero).                      * *
                //Note: the FP code checks lnSetNbr !=0, but seeks on an expression with 0 as the SetNbr. 
                if (lookup2.IsNullOrWhiteSpace() && setNbr != 0 && siteDefLang != "EN")
                {
                    reasonCode = reasonCodes.FirstOrDefault(s => s.ReasCode == item && s.LangCode == "EN" && s.ReasSetNbr == 0);
                    if (reasonCode != null) lookup2 = useLongDescription || reasonCode.LongDesc.IsNullOrWhiteSpace() ? reasonCode.ReasDesc : reasonCode.LongDesc;
                }

                if (lookup2.IsNullOrWhiteSpace()) lookup2 = item + " " + globals.ReportMessages.NotFound;

                if (lookup.Contains(lookup2)) continue;
                lookup += "; " + lookup2;
            }

            masterDb.Dispose();

            return lookup.Trim(';').Trim();
        }

    }
}
