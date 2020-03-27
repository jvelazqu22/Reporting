using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Helper;
using Domain.Models.ReportPrograms.AirFareSavingsReport;

namespace iBank.Services.Implementation.ReportPrograms.AirFareSavings
{
    public class AirFareSavingsData
    {
        public List<FinalData> SuppressDuplicateFareInfo(List<FinalData> finalDataList)
        {
            var list = new List<FinalData>();
            for (var i=0; i<finalDataList.Count; i++)
            {
                var row = finalDataList[i];
                if (row.Seqno > 1)
                {
                    row.Stndchg = 0;
                    row.Offrdchg = 0;
                    row.Airchg = 0;
                    row.Savings = 0;
                    row.Lostamt = 0;
                }
                list.Add(row);
            }
            return list;
        }
        
        public List<string> GetExportFields(bool accountBreak, bool userBreak1, bool userBreak2, bool userBreak3, UserInformation user, List<FinalData> data)
        {
            var fieldList = new List<string>();

            if (accountBreak)
            {
                fieldList.Add("acct");
                fieldList.Add("acctdesc");
            }
            if (userBreak1)
            {
                var break1 = string.IsNullOrEmpty(user.Break1Name) ? "break_1" : user.Break1Name;
                fieldList.Add($"break1 as {break1}");
            }
            if (userBreak2)
            {
                var break2 = string.IsNullOrEmpty(user.Break2Name) ? "break_2" : user.Break2Name;
                fieldList.Add($"break2 as {break2}");
            }
            if (userBreak3)
            {
                var break3 = string.IsNullOrEmpty(user.Break3Name) ? "break_3" : user.Break3Name;
                fieldList.Add($"break3 as {break3}");
            }

            fieldList.Add("ticket");
            fieldList.Add("passlast");
            fieldList.Add("passfrst");
            fieldList.Add("reckey");
            fieldList.Add("invdate");
            fieldList.Add("rdepdate");
            fieldList.Add("origin");
            fieldList.Add("orgdesc");
            fieldList.Add("destinat");
            fieldList.Add("destdesc");
            fieldList.Add("seqno");
            fieldList.Add("airline");
            fieldList.Add("class");
            fieldList.Add("stndchg");
            fieldList.Add("offrdchg");
            fieldList.Add("airchg");
            fieldList.Add("savingcode");
            fieldList.Add("savings");
            fieldList.Add("negosvngs");
            fieldList.Add("reascode");
            fieldList.Add("lostamt");

            //the method that places values into excel columns is going to be looking for a property name that is the column name
            //so we set up the column name in the format "Udidlbl1 AS <value of FinalData.Udidlbl1>"
            //but then we need to transfer the value of the corresponding Udidtext over to the Udidlbl property
            //so when the excel mapping method gets the value of the Udidlbl1 property it is the actual value, not the label
            var udidHeaders = GetUdidColumnNames(data);
            if (udidHeaders.Any())
            {
                fieldList.AddRange(udidHeaders);
                TransferValueToUdidLabelField(data);
            }
            
            return fieldList;
        }

        private List<string> GetUdidColumnNames(List<FinalData> data)
        {
            var udidHeaders = new List<string>();
            var sample = data.FirstOrDefault();

            if (sample == null) throw new ArgumentNullException("We should have never gotten to this point if there is no data!");

            //set up the headers -- these are not unique values across records
            if (!string.IsNullOrEmpty(sample.Udidlbl1)) udidHeaders.Add($"Udidlbl1 AS {sample.Udidlbl1}");

            if (!string.IsNullOrEmpty(sample.Udidlbl2)) { udidHeaders.Add($"Udidlbl2 AS {sample.Udidlbl2}"); }

            if (!string.IsNullOrEmpty(sample.Udidlbl3)) udidHeaders.Add($"Udidlbl3 AS {sample.Udidlbl3}");

            if (!string.IsNullOrEmpty(sample.Udidlbl4)) udidHeaders.Add($"Udidlbl4 AS {sample.Udidlbl4}");

            if (!string.IsNullOrEmpty(sample.Udidlbl5)) udidHeaders.Add($"Udidlbl5 AS {sample.Udidlbl5}");

            if (!string.IsNullOrEmpty(sample.Udidlbl6)) udidHeaders.Add($"Udidlbl6 AS {sample.Udidlbl6}");

            if (!string.IsNullOrEmpty(sample.Udidlbl7)) udidHeaders.Add($"Udidlbl7 AS {sample.Udidlbl7}");

            if (!string.IsNullOrEmpty(sample.Udidlbl8)) udidHeaders.Add($"Udidlbl8 AS {sample.Udidlbl8}");

            if (!string.IsNullOrEmpty(sample.Udidlbl9)) udidHeaders.Add($"Udidlbl9 AS {sample.Udidlbl9}");

            if (!string.IsNullOrEmpty(sample.Udidlbl10)) udidHeaders.Add($"Udidlbl10 AS {sample.Udidlbl10}");

            return udidHeaders;
        }

        private void TransferValueToUdidLabelField(List<FinalData> data)
        {
            //iterate over all items, replacing the header value with the corresponding real value
            foreach (var val in data)
            {
                if (!string.IsNullOrEmpty(val.Udidlbl1)) val.Udidlbl1 = val.Udidtext1;

                if (!string.IsNullOrEmpty(val.Udidlbl2)) val.Udidlbl2 = val.Udidtext2;

                if (!string.IsNullOrEmpty(val.Udidlbl3)) val.Udidlbl3 = val.Udidtext3;

                if (!string.IsNullOrEmpty(val.Udidlbl4)) val.Udidlbl4 = val.Udidtext4;

                if (!string.IsNullOrEmpty(val.Udidlbl5)) val.Udidlbl5 = val.Udidtext5;

                if (!string.IsNullOrEmpty(val.Udidlbl6)) val.Udidlbl6 = val.Udidtext6;

                if (!string.IsNullOrEmpty(val.Udidlbl7)) val.Udidlbl7 = val.Udidtext7;

                if (!string.IsNullOrEmpty(val.Udidlbl8)) val.Udidlbl8 = val.Udidtext8;

                if (!string.IsNullOrEmpty(val.Udidlbl9)) val.Udidlbl9 = val.Udidtext9;

                if (!string.IsNullOrEmpty(val.Udidlbl10)) val.Udidlbl10 = val.Udidtext10;
            }
        }
    }
}
