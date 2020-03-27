using iBank.Server.Utilities.Classes;
using System;
using System.Collections.Generic;

namespace iBank.Services.Implementation.ReportPrograms.DocumentDeliveryLog
{
    public class TravAuthStatusLogHelpers
    {
        public static string GmtConvert(DateTime? bookedGmt, double offset, string gds)
        {
            if ((gds.Equals("AM") || gds.Equals("AP")) && bookedGmt.GetValueOrDefault().Hour ==0 && bookedGmt.GetValueOrDefault().Minute == 0 && bookedGmt.GetValueOrDefault().Second == 0)
            {
                return bookedGmt.GetValueOrDefault().ToString($"MM/dd/yyyy hh:mm:ss tt");
            }

            return bookedGmt.GetValueOrDefault().AddHours(offset).ToString($"MM/dd/yyyy hh:mm:ss tt");
        }

        public static List<string> GetExportFields()
        {
            var fields = new List<string>();

            fields.Add("acct");
            fields.Add("acctdesc");
            fields.Add("depdate");
            fields.Add("recloc");
            fields.Add("passlast");
            fields.Add("passfrst");
            fields.Add("timezone"); 
            fields.Add("bookedtim");
            fields.Add("rtvlcode");
            fields.Add("travauthno");
            fields.Add("sgroupnbr");
            fields.Add("AuthStatus");
            fields.Add("StatusDesc");
            fields.Add("statustime");
            fields.Add("authLogNbr");
            fields.Add("statusNbr");
            fields.Add("docStattim");
            fields.Add("docSuccess");
            fields.Add("docType");
            //fields.Add("docRecips");
            //fields.Add("docSubject");
            //fields.Add("docText");
            //fields.Add("docHtml");
            //fields.Add("dlvRespons");

            return fields;
        } 
    }
}
