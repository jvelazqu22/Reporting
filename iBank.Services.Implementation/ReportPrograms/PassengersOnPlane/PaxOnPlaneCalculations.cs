using CODE.Framework.Core.Utilities.Extensions;
using Domain.Helper;
using Domain.Models.ReportPrograms.PassengersOnPlaneReport;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.PassengersOnPlane
{
    public class PaxOnPlaneCalculations
    {
        public string GetCrystalReportName(bool isPrintBreakInfoInBodyOn)
        {
            return isPrintBreakInfoInBodyOn ? "ibPaxOnPlane2" : "ibPaxOnPlane";
        }

        public string GetBreakCombo(RawData row, UserBreaks breaks, bool bBreakInfoInReport)
        {
            if (!breaks.UserBreak1 && !breaks.UserBreak2 && !breaks.UserBreak3 && !bBreakInfoInReport) return new string(' ', 10);

            var breakCombo = string.Empty;
            if (breaks.UserBreak1 || bBreakInfoInReport) breakCombo += row.Break1.Trim() + "/";

            if (breaks.UserBreak2 || bBreakInfoInReport) breakCombo += row.Break2.Trim() + "/";

            if (breaks.UserBreak3 || bBreakInfoInReport) breakCombo += row.Break3.Trim() + "/";

            return breakCombo.Substring(0, breakCombo.Length - 1);
        }

        public int GetNumberOfPassengers(ReportGlobals globals)
        {
            var numPaxString = globals.GetParmValue(WhereCriteria.NBRPASSENGERS);
            int numPax;
            var goodPaxValue = int.TryParse(numPaxString, out numPax);

            if (!goodPaxValue || numPax < 1) return 5;

            return numPax;
        }

        public int GetDomesticInternationalValue(ReportGlobals globals)
        {
            return globals.GetParmValue(WhereCriteria.DOMINTL).TryIntParse(0);
        }

        public string GetDomesticInternationalWhereText(int domesticInternationalValue)
        {
            var whereText = "";
            switch (domesticInternationalValue)
            {
                case 2:
                    whereText = "Domestic Only ;";
                    break;
                case 3:
                    whereText = "International Only ;";
                    break;
                case 4:
                    whereText = "Transborder Only ;";
                    break;
                case 5:
                    whereText = "Exclude Domestic ;";
                    break;
                case 6:
                    whereText = "Exclude International ;";
                    break;
                case 7:
                    whereText = "Exclude Transborder ;";
                    break;
            }

            return whereText;
        }

        public bool IsIgnoreBreakSettingsOn(ReportGlobals globals)
        {
            return globals.IsParmValueOn(WhereCriteria.CBIGNOREBRKSETTINGS);
        }

        public bool IsPrintBreakInfoInBodyOn(ReportGlobals globals)
        {
            return globals.IsParmValueOn(WhereCriteria.CBPRINTBRKINFOINBODY);
        }

        public string GetBreakColumnHeader(UserBreaks userBreaks, string break1Name, string break2Name, string break3Name)
        {
            var breakColumnHeader = "";

            if (userBreaks.UserBreak1) breakColumnHeader = break1Name;

            if (userBreaks.UserBreak2)
            {
                if (!string.IsNullOrEmpty(breakColumnHeader)) breakColumnHeader += "/";

                breakColumnHeader += break2Name;
            }

            if (userBreaks.UserBreak3)
            {
                if (!string.IsNullOrEmpty(breakColumnHeader)) breakColumnHeader += "/";

                breakColumnHeader += break3Name;
            }

            return breakColumnHeader == "" ? "Breaks" : breakColumnHeader;
        }

        public bool IsGantAgency(string agency)
        {
            return agency.EqualsIgnoreCase("GANT");
        }

        public string GetGantReportTitle()
        {
            return " (Special Gant version)";
        }

        public string GetUdid71(int reckey, IEnumerable<GantData> gantData)
        {
            var udid = gantData.FirstOrDefault(s => s.RecKey == reckey);
            return udid == null ? string.Empty : udid.UdidText;
        }

        public bool IsDateRange1(ReportGlobals globals)
        {
            //TODO: what date range is this
            return globals.ParmValueEquals(WhereCriteria.DATERANGE, "1");
        }

        public bool IsDateRange9(ReportGlobals globals)
        {
            //TODO: what date range is this
            return globals.ParmValueEquals(WhereCriteria.DATERANGE, "9");
        }

        public IList<string> GetExportFields(bool acctBrk, UserBreaks userBreaks, bool isGantAgency, bool isPrintBreakInfoInBodyOn)
        {
            var fieldList = new List<string>();

            if (isGantAgency) fieldList.Add("agentid");

            if (acctBrk)
            {
                fieldList.Add("acct");
                fieldList.Add("acctdesc");
            }

            if (userBreaks.UserBreak1) fieldList.Add("break1");

            if (userBreaks.UserBreak2) fieldList.Add("break2");

            if (userBreaks.UserBreak3) fieldList.Add("break3");

            fieldList.Add("rdepdate");
            fieldList.Add("pseudocity");
            fieldList.Add("recloc");
            fieldList.Add("airline");
            fieldList.Add("alinedesc");
            fieldList.Add("fltno");
            fieldList.Add("origin");
            fieldList.Add("destinat");
            fieldList.Add("orgdesc");
            fieldList.Add("destdesc");
            fieldList.Add("deptime");
            fieldList.Add("arrtime");
            fieldList.Add("passlast");
            fieldList.Add("passfrst");

            if (isPrintBreakInfoInBodyOn) fieldList.Add("BreakCombo");

            fieldList.Add("TranType");
            fieldList.Add("ticket");
            fieldList.Add("classcode");


            if (isGantAgency)
            {
                fieldList.Add("bookdate");
                fieldList.Add("udidtext");
            }

            return fieldList;
        }
    }
}
