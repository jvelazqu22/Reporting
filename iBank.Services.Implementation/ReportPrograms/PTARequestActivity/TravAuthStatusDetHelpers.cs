using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Helper;
using Domain.Models.ReportPrograms.PTARequestActivityReport;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;

namespace iBank.Services.Implementation.ReportPrograms.PTARequestActivity
{
    public static class TravAuthStatusDetHelpers
    {
        public static List<FinalData> ProcessNotifyOnly(List<FinalData> finalDataList, bool notifyOnly)
        {
            if (notifyOnly)
            {
                foreach (var row in finalDataList.Where(s => s.Authstatus.Trim().Equals("N", StringComparison.OrdinalIgnoreCase)))
                {
                    row.Statusdesc = "Notified";
                    row.Detstatdes = "Notified";
                    row.Apvreason = "[Notification Only]";
                }
            }
            else
            {
                finalDataList.RemoveAll(s => s.Authstatus.Equals("N"));
            }

            return finalDataList;
        }

        public static List<FinalData> ProcessNotRequired(List<FinalData> finalDataList)
        {
            var notRequired = new List<string> { "A", "D", "C" };
            foreach (var row in finalDataList.Where(s => notRequired.Contains(s.Authstatus) && !notRequired.Contains(s.Detlstatus)))
            {
                row.Detstatdes = "Not Req'd";
            }

            return finalDataList;
        }

        public static List<FinalData> ProcessOutOfPolicy(List<FinalData> finalDataList, ReportGlobals globals)
        {
            
            var rowsToKeep = new List<FinalData>();
            var oopCrit = globals.GetParmValue(WhereCriteria.INOOPCODES);
            if (string.IsNullOrEmpty(oopCrit)) return finalDataList;

            var oopCritList = oopCrit.Split(',');

            var notIn = globals.IsParmValueOn(WhereCriteria.NOTINOOPCODES);
            foreach (var row in finalDataList)
            {
                if (!string.IsNullOrEmpty(row.Outpolcods))
                {
                    var codes = row.Outpolcods.Split(',');
                    foreach (var code in codes)
                    {
                        if (notIn)
                        {
                            if (!oopCritList.Contains(code)) rowsToKeep.Add(row);
                        }
                        else
                        {
                            if (oopCritList.Contains(code)) rowsToKeep.Add(row);
                        }
                    }
                }
            }

            return rowsToKeep;
        }
        
        public static string BuildTimeZoneCaption(string timeZone, ReportGlobals globals)
        {
            var lv = globals.LanguageVariables.FirstOrDefault(s => s.VariableName.Equals("XDATETIMETIMEZONE"));
            var translation = lv == null ? "Booked Date/Time and Status Date/Time are in " : lv.Translation;

            return translation + timeZone;
        }

        public static List<FinalData> SortData(List<FinalData> finalDataList, string sortBy)
        {
            switch (sortBy)
            {
                case "2":
                    return finalDataList.OrderBy(s => s.Acctdesc)
                        .ThenBy(s => s.Acct)
                        .ThenBy(s => s.Break1)
                        .ThenBy(s => s.Break2)
                        .ThenBy(s => s.Break3)
                        .ThenBy(s => s.Bookdate)
                        .ThenBy(s => s.Passlast)
                        .ThenBy(s => s.Passfrst)
                        .ThenBy(s => s.Reckey)
                        .ThenBy(s => s.Travauthno)
                        .ThenBy(s => s.Apsequence)
                        .ToList();
                case "3":
                    return finalDataList.OrderBy(s => s.Acctdesc)
                        .ThenBy(s => s.Acct)
                        .ThenBy(s => s.Break1)
                        .ThenBy(s => s.Break2)
                        .ThenBy(s => s.Break3)
                        .ThenBy(s => s.Statustime)
                        .ThenBy(s => s.Passlast)
                        .ThenBy(s => s.Passfrst)
                        .ThenBy(s => s.Reckey)
                        .ThenBy(s => s.Travauthno)
                        .ThenBy(s => s.Apsequence)
                        .ToList();
                case "4":
                    return finalDataList.OrderBy(s => s.Acctdesc)
                        .ThenBy(s => s.Acct)
                        .ThenBy(s => s.Break1)
                        .ThenBy(s => s.Break2)
                        .ThenBy(s => s.Break3)
                        .ThenBy(s => s.Passlast)
                        .ThenBy(s => s.Passfrst)
                        .ThenBy(s => s.Reckey)
                        .ThenBy(s => s.Travauthno)
                        .ThenBy(s => s.Apsequence)
                        .ToList();
                case "5":
                    return finalDataList.OrderBy(s => s.Acctdesc)
                        .ThenBy(s => s.Acct)
                        .ThenBy(s => s.Break1)
                        .ThenBy(s => s.Break2)
                        .ThenBy(s => s.Break3)
                        .ThenBy(s => s.Authstatus)
                        .ThenBy(s => s.Passlast)
                        .ThenBy(s => s.Passfrst)
                        .ThenBy(s => s.Reckey)
                        .ThenBy(s => s.Travauthno)
                        .ThenBy(s => s.Apsequence)
                        .ToList();
                case "6":
                    return finalDataList.OrderBy(s => s.Acctdesc)
                        .ThenBy(s => s.Acct)
                        .ThenBy(s => s.Break1)
                        .ThenBy(s => s.Break2)
                        .ThenBy(s => s.Break3)
                        .ThenBy(s => s.Oopreasdes)
                        .ThenBy(s => s.Passlast)
                        .ThenBy(s => s.Passfrst)
                        .ThenBy(s => s.Reckey)
                        .ThenBy(s => s.Travauthno)
                        .ThenBy(s => s.Apsequence)
                        .ToList();
                default:
                    var list = finalDataList.OrderBy(s => s.Acctdesc)
                        .ThenBy(s => s.Acct)
                        .ThenBy(s => s.Break1)
                        .ThenBy(s => s.Break2)
                        .ThenBy(s => s.Break3)
                        .ThenBy(s => s.DepartureDate)
                        .ThenBy(s => s.Passlast)
                        .ThenBy(s => s.Passfrst)
                        .ThenBy(s => s.Reckey)
                        .ThenBy(s => s.Travauthno)
                        .ThenBy(s => s.Apsequence)
                        .ToList();

                    return list;
            }
        }

        public static List<string> GetExportFields(bool includeFareCols, UserBreaks userBreaks, ReportGlobals globals)
        {
            var fields = new List<string>();

            if (globals.Agency.EqualsIgnoreCase("AXI"))
            {
                fields.Add("Acct as Account_Number");
                fields.Add("AcctDesc as Account_Name");
                if (userBreaks.UserBreak1)
                    fields.Add("break1 as " + globals.User.Break1Name);
                if (userBreaks.UserBreak2)
                    fields.Add("break2 as " + globals.User.Break2Name);
                if (userBreaks.UserBreak3)
                    fields.Add("break3 as " + globals.User.Break3Name);

                fields.Add("Depdate as Departure_Date");
                fields.Add("DepTime as Departure_Time");
                fields.Add("BookDate as Booked_Date");
                fields.Add("StatusTime as Auth_Status_Time");
                fields.Add("Recloc as Record_Locator");
                fields.Add("DaysAdvanc as Days_Advanced_Booked");
                fields.Add("PassLast as Passenger_Last_Name");
                fields.Add("PassFrst as Passenger_First_Name");
                fields.Add("RTvlCode as Reason_for_Travel_Code");
                fields.Add("TvlReasDes as Reason_for_Travel_Description");
                fields.Add("OutPolCods as Out_of_Policy_Code");
                fields.Add("OOPReasDes as Out_of_Policy_Description");
                fields.Add("AuthStatus as Notification_or_Authorization_Status_Code");
                fields.Add("StatusDesc as Notification_or_Authorization_Status_Description");
                fields.Add("CliAuthNbr as Client_Input_Auth_Number");
                fields.Add("AuthrzrNbr as Authorizer_Number");
                fields.Add("ApSequence as Approval_Sequence_Number");
                fields.Add("Auth1Email as Approval_Email_Recipient_1"); 
                fields.Add("Authorizer as Authorizer_Who_Actioned");
                fields.Add("DetlStatus as Detailed_Status");
                fields.Add("DetStatDes as Detailed_Status_Description");
                fields.Add("DetStatTim as Detailed_Status_Time");
                fields.Add("ApvReason as Approval_Reason");
                fields.Add("TravAuthNo as Trip_Reference_Number");
                if (includeFareCols)
                {
                    fields.Add("AirChg as Air_Charge");
                    fields.Add("OffrdChg as Lowest_Offered_Fare");
                    fields.Add("LostSvngs as Lost_Savings");
                }

                return fields;
            }
           

            fields.Add("Acct");
            fields.Add("AcctDesc");
            if (userBreaks.UserBreak1)
                fields.Add("break1 as " + globals.User.Break1Name);
            if (userBreaks.UserBreak2)
                fields.Add("break2 as " + globals.User.Break2Name);
            if (userBreaks.UserBreak3)
                fields.Add("break3 as " + globals.User.Break3Name);

            fields.Add("Depdate");
            fields.Add("DepTime");
            fields.Add("BookDate");
            fields.Add("StatusTime");
            fields.Add("Recloc");
            fields.Add("DaysAdvanc");
            fields.Add("PassLast");
            fields.Add("PassFrst");
            fields.Add("RTvlCode");
            fields.Add("TvlReasDes");
            fields.Add("OutPolCods");
            fields.Add("OOPReasDes");
            fields.Add("AuthStatus");
            fields.Add("StatusDesc");
            fields.Add("CliAuthNbr");
            fields.Add("AuthrzrNbr");
            fields.Add("ApSequence");
            fields.Add("Auth1Email"); //Limit to 100 chars
            fields.Add("Authorizer");
            fields.Add("DetlStatus");
            fields.Add("DetStatDes");
            fields.Add("DetStatTim");
            fields.Add("ApvReason");//Limit to 100 chars
            fields.Add("TravAuthNo");

            if (includeFareCols)
            {
                fields.Add("AirChg");
                fields.Add("OffrdChg");
                fields.Add("LostSvngs");
            }

            return fields;
        }
    }
}
