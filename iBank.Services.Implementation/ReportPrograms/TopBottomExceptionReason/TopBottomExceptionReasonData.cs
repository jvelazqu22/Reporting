using System.Collections.Generic;

namespace iBank.Services.Implementation.ReportPrograms.TopBottomExceptionReason
{
    public class TopBottomExceptionReasonData
    {
        public List<string> GetExportFields()
        {
            var fieldList = new List<string>();

            fieldList.Add("category");
            fieldList.Add("reascode");
            fieldList.Add("numoccurs");
            fieldList.Add("lostamt");

            return fieldList;
        }
    }
}
