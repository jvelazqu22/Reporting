using System.Collections.Generic;
using CODE.Framework.Core.Utilities.Extensions;
using iBank.Server.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ReportLokkup
{
    public static class FareTypeHandler
    {
        public static string LookupFareType(string prdfarebas)
        {
            //Order	Fare Type's Contents	Predominant Fare Basis Code Criteria
            // 1. 		First 				begins with F 
            // 2. 		Business 			begins with B 
            // 3.		DG					the letters DG as the 2nd and 3rd character 
            // 4. 		CPP Business		the letters CB as the 2nd and 3rd character 
            // 5. 		YCA					these have values of YCA 
            // 5. 		Dash CA				the letters CA as the 2nd and 3rd character 
            // 7. 		Other				everything else 

            if (prdfarebas.Left(1) == "F") return "First";
            if (prdfarebas.Left(1) == "B") return "Business";
            if (prdfarebas?.SubStr(2, 2) == "DG") return "DG";
            if (prdfarebas?.SubStr(2, 2) == "CB") return "CPP Business";
            if (prdfarebas != null && prdfarebas.Contains("YCA")) return "YCA";
            if (prdfarebas?.SubStr(2, 2) == "CA") return "Dash CA";
            return "Other";
        }

        public static string LookupGsaFareType(string prdfarebas)
        {
            if (prdfarebas == null) return "Other";
            //--First: Predominant fare basis code beginning with R, F, P, or A(this one changes)
            //--Business: Predominant fare basis code beginning with C, D, or J(this one changes)
            //--DG: Predominant fare basis code that has DG as the 2nd and 3rd characters(no change)
            //--CPP Business: Predominant fare basis code that has CB as the 2nd and 3rd characters(no change)
            //--Dash CA: Predominant fare basis code that has CA as the 2nd and 3rd characters(no change)

            //--YCA: Predominant fare basis code that has values of YCA as the first 3 letters(this one changes)
            //--Category Z: Predominant fare basis code that has MZ as the 2nd and 3rd characters(this one is brand new)
            //--Other: Everything else (no change)

            var firstLetterOptions = new List<string>() { "R", "F", "P", "A" };
            var firstLetter = prdfarebas.Left(1);
            if (firstLetterOptions.Contains(firstLetter)) return "First";

            if (prdfarebas?.SubStr(2, 2) == "DG") return "DG"; // No change
            if (prdfarebas?.SubStr(2, 2) == "CB") return "CPP Business";  // No change
            if (prdfarebas?.SubStr(1, 3) == "YCA") return "YCA";
            if (prdfarebas?.SubStr(2, 2) == "CA") return "Dash CA";
            if (prdfarebas?.SubStr(2, 2) == "MZ") return "Category Z"; // No change

            var businessLetterOptions = new List<string>() { "C", "D", "J" };
            if (businessLetterOptions.Contains(firstLetter)) return "Business";

            return "Other";
        }
    }
}
