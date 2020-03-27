using System.Collections.Generic;
using iBank.Server.Utilities.Classes;

namespace iBank.Server.Utilities.ReportGlobalsSetters
{
    public class ReportParametersGlobalsSetter
    {
        public void SetValuesFromReportParameters(ReportGlobals globals)
        {
            //some variables MUST be uppercase. 
            var uppers = new List<string>
                {
                    "TITLEACCT2",
                    "UDIDLBL1",
                    "UDIDLBL2",
                    "RPTTITLE2",
                    "UDIDLBL3",
                    "UDIDLBL4",
                    "UDIDLBL5",
                    "UDIDLBL6",
                    "UDIDLBL7",
                    "UDIDLBL8",
                    "UDIDLBL9",
                    "UDIDLBL10"
                };

            foreach (var reportParam in globals.ReportParameters)
            {

                var parm = reportParam.Value;

                if (uppers.Contains(parm.VarValue))
                {
                    parm.VarValue = parm.VarValue.ToUpper();
                }

                parm.VarValue = parm.VarValue.Replace("[AMPER]", "&").Replace("[amper]", "&").Trim();

                int tempInt;

                switch (parm.VarName.ToUpper())
                {
                    case "REPORTLOGKEY":
                        if (int.TryParse(parm.VarValue, out tempInt))
                        {
                            globals.ReportLogKey = tempInt;
                        }
                        break;
                    case "EPROFILENO":
                        if (int.TryParse(parm.VarValue, out tempInt))
                        {
                            globals.EProfileNumber = tempInt;
                        }
                        break;
                    case "OUTPUTDEST":
                        globals.OutputDestination = parm.VarValue;
                        break;
                    case "OUTPUTLANGUAGE":
                        globals.OutputLanguage = parm.VarValue;
                        break;
                    case "AGENCY":
                        globals.Agency = parm.VarValue;
                        break;
                    case "USERLANGUAGE":
                        globals.UserLanguage = parm.VarValue;
                        break;
                    case "IMAGENBR":
                        if (int.TryParse(parm.VarValue, out tempInt))
                        {
                            globals.ImageNumber = tempInt;
                        }
                        break;
                    case "USERNBR":
                        if (int.TryParse(parm.VarValue, out tempInt))
                        {
                            globals.UserNumber = tempInt;
                        }
                        break;
                    case "OUTPUTTYPE":
                        //TODO: I removed code here that truncated the X from 2X
                        globals.OutputType = parm.VarValue;
                        break;
                    case "POWERMACRO":
                        if (int.TryParse(parm.VarValue, out tempInt))
                        {
                            globals.MacroKey = tempInt;
                        }
                        break;
                    case "TABBGCOLOR":
                        globals.TabBgColor = parm.VarValue;
                        break;
                    case "PICKRECNUM": //TODO: This case appears twice in FoxPro, once for gnSavedRptKey 
                        if (int.TryParse(parm.VarValue, out tempInt))
                        {
                            globals.SavedReportKey = tempInt;
                        }
                        break;
                    case "TITLEACCT2":
                        globals.TitleAcct = parm.VarValue.Replace("_AND_", " & ");
                        break;
                    case "RPTTITLE2":
                        globals.ReportTitle = parm.VarValue;
                        break;
                    case "PROCESSID":
                        if (int.TryParse(parm.VarValue, out tempInt))
                        {
                            globals.ProcessKey = tempInt;
                        }
                        break;
                    case "BEGDATE":
                        globals.BeginDate = parm.VarValue.ToDateFromiBankFormattedString();
                        break;
                    case "ENDDATE":
                        globals.EndDate = parm.VarValue.ToDateFromiBankFormattedString(true);
                        break;
                    case "REPORTFILENAME":
                        globals.SavedReportName = parm.VarValue;
                        break;
                    case "GLOBALDATEFMT":
                        globals.DateDisplayFormat = parm.VarValue;
                        break;
                    case "REPORTINGTRAVET":
                        globals.ReportingTravet = true; //FoxPro code sets this global to true regardless of the parameter value. 
                        break;
                    case "BASE_URL":
                        globals.BaseUrl = parm.VarValue;
                        break;
                    case "TXTFYSTARTMTH":
                            globals.FyStartMonth = parm.VarValue;
                        break;
                }
            }
        }
    }
}
