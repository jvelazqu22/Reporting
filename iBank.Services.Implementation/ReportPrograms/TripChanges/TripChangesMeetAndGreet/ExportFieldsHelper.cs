using AutoMapper;
using Domain.Models.ReportPrograms.TripChangesMeetAndGreetReport;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.TripChanges.TripChangesMeetAndGreet
{

    public class ExportFieldsHelper
    {
        public ExportFieldsHelper()
        {
            if (!Features.AutoMapperInitializer.IsEnabled())
            {
                Mapper.Initialize(cfg => cfg.CreateMap<FinalData, FinalDataExport>());
            }
        }

        public List<FinalDataExport> SetupFinalReportAndSetExportFields(List<FinalData> FinalDataList, string UdidLabel1, bool consolidateChanges, 
            string UdidLabel2, bool UseAirportCodes, ref List<string> ExportFields, bool IncludeEmailAddress)
        {
            var finalDataExportList = Mapper.Map<List<FinalDataExport>>(FinalDataList);

            var udid1ColumnName = "UdidText1";
            var udid2ColumnName = "UdidText2";

            if (!string.IsNullOrEmpty(UdidLabel1))
                udid1ColumnName = SharedProcedures.FixDbfColumnName(UdidLabel1);

            if (!string.IsNullOrEmpty(UdidLabel2))
                udid2ColumnName = SharedProcedures.FixDbfColumnName(UdidLabel2);

            if (udid1ColumnName == udid2ColumnName)
            {
                if (udid2ColumnName.Length == 10)
                    udid2ColumnName = udid2ColumnName.EndsWith("2") ? udid2ColumnName.Left(9) + "X" : udid2ColumnName.Trim() + "2";
                else
                    udid2ColumnName = udid2ColumnName + "2";
            }

            var displayFields = 1;

            if (consolidateChanges)
            {
                finalDataExportList = GetDistinctAndSortedList(FinalDataList);
                ConsolidateChanges(FinalDataList, ref finalDataExportList, ref displayFields);
                finalDataExportList = finalDataExportList.OrderBy(s => s.Destdesc).ThenBy(s => s.Destinat).ToList();
            }

            if (UseAirportCodes)
            {
                finalDataExportList.ForEach(s =>
                {
                    s.Origin = s.Origdesc;
                    s.LastOrigin = s.Lastorgdes;
                });
            }

            SetExportFields(ref ExportFields, UseAirportCodes, IncludeEmailAddress, displayFields, udid1ColumnName, udid2ColumnName);

            return finalDataExportList;
        }

        private void ConsolidateChanges(List<FinalData> FinalDataList, ref List<FinalDataExport> finalDataExportList, ref int displayFields)
        {
            foreach (var finalData in finalDataExportList)
            {
                var useChangeFields = 1;
                string changeDesc1 = "", changeDesc2 = "", changeDesc3 = "";
                string changeStamp1 = "", changeStamp2 = "", changeStamp3 = "";

                var data = finalData;

                var records =
                    FinalDataList.Where( s =>
                            s.Reckey == data.Reckey 
                                && s.Passlast == data.Passlast 
                                    && s.Destinat == data.Destinat 
                                        && s.Origdesc == data.Origdesc 
                                            && s.Airline == data.Airline && s.Fltno == data.Fltno);

                foreach (var record in records)
                {
                    var originalChangeDescription = ShortenChangeDescription(record.Changedesc);
                    if (!string.IsNullOrEmpty(originalChangeDescription))
                    {
                        switch (useChangeFields)
                        {
                            case 1:
                                if (changeDesc1.Length > 50 && (changeDesc1.Length + originalChangeDescription.Trim().Length > 250))
                                    useChangeFields = 2;
                                break;
                            case 2:
                                if (changeDesc2.Length > 50 && (changeDesc2.Length + originalChangeDescription.Trim().Length > 250))
                                    useChangeFields = 3;
                                break;
                        }

                        if ((string.IsNullOrEmpty(changeDesc1.Trim()) || (originalChangeDescription.Trim().IndexOf(changeDesc1, StringComparison.Ordinal) == -1)) 
                            && (string.IsNullOrEmpty(changeDesc2.Trim()) || (originalChangeDescription.Trim().IndexOf(changeDesc2, StringComparison.Ordinal) == -1))
                                && (string.IsNullOrEmpty(changeDesc3.Trim()) || (originalChangeDescription.Trim().IndexOf(changeDesc3, StringComparison.Ordinal) == -1)))
                        {
                            switch (useChangeFields)
                            {
                                case 1:
                                    changeDesc1 = changeDesc1 + "|" + originalChangeDescription.Trim();
                                    changeStamp1 = changeStamp1 + "|" + record.Changstamp;
                                    break;
                                case 2:
                                    changeDesc2 = changeDesc2 + "|" + originalChangeDescription.Trim();
                                    changeStamp2 = changeStamp2 + "|" + record.Changstamp;
                                    break;
                                case 3:
                                    changeDesc3 = changeDesc3 + "|" + originalChangeDescription.Trim();
                                    changeStamp3 = changeStamp3 + "|" + record.Changstamp;
                                    break;
                            }
                        }

                    }
                }

                changeDesc1 = CleanField(changeDesc1);
                changeDesc2 = CleanField(changeDesc2);
                changeDesc3 = CleanField(changeDesc3);
                changeStamp1 = CleanField(changeStamp1);
                changeStamp2 = CleanField(changeStamp2);
                changeStamp3 = CleanField(changeStamp3);

                if (displayFields < useChangeFields) displayFields =  useChangeFields;

                finalData.Changedesc = changeDesc1;
                finalData.Changedesc2 = changeDesc2;
                finalData.Changedesc3 = changeDesc3;

                finalData.Changstamp = changeStamp1;
                finalData.Changstamp2 = changeStamp2;
                finalData.Changstamp3 = changeStamp3;
            }
        }

        private List<FinalDataExport> GetDistinctAndSortedList(List<FinalData> FinalDataList)
        {
            var results = FinalDataList.Select(
                s =>
                    new
                    {
                        s.Reckey,
                        s.Mtggrpnbr,
                        s.Passlast,
                        s.Passfrst,
                        s.Emailaddr,
                        s.Destinat,
                        s.Origdesc,
                        s.Lastorgdes,
                        s.Rarrdate,
                        s.Airline,
                        s.Alinedesc,
                        s.Fltno,
                        s.Arrtime,
                        s.Deptime,
                        s.Sorttime,
                        s.Recloc,
                        s.Ticketed,
                        s.Bookdate,
                        s.Udidnbr1,
                        s.Udidtext1,
                        s.Udidnbr2,
                        s.Udidtext2
                    }).Distinct();

            var results2 = results
                .Select(t => new FinalDataExport
                {
                    Reckey = t.Reckey,
                    Mtggrpnbr = t.Mtggrpnbr,
                    Passlast = t.Passlast,
                    Passfrst = t.Passfrst,
                    Emailaddr = t.Emailaddr,
                    Destinat = t.Destinat,
                    Origdesc = t.Origdesc,
                    Lastorgdes = t.Lastorgdes,
                    Rarrdate = t.Rarrdate,
                    Airline = t.Airline,
                    Alinedesc = t.Alinedesc,
                    Fltno = t.Fltno,
                    Arrtime = t.Arrtime,
                    Deptime = t.Deptime,
                    Sorttime = t.Sorttime,
                    Recloc = t.Recloc,
                    Ticketed = t.Ticketed,
                    Bookdate = t.Bookdate,
                    Udidnbr1 = t.Udidnbr1,
                    Udidnbr2 = t.Udidnbr2,
                    Udidtext1 = t.Udidtext1,
                    Udidtext2 = t.Udidtext2
                });

            return results2
                .OrderBy(s => s.Reckey)
                .ThenBy(s => s.Rarrdate)
                .ThenBy(s => s.Airline)
                .ThenBy(s => s.Arrtime)
                .ThenBy(s => s.Fltno)
                .ThenBy(s => s.Changstamp).ToList();
        }

        private void SetExportFields(ref List<string> ExportFields, bool UseAirportCodes, bool IncludeEmailAddress, 
            int displayFields, string udid1ColumnName, string udid2ColumnName)
        {
            //Setup the export fields
            ExportFields.Add("Reckey");
            ExportFields.Add("mtggrpnbr");
            ExportFields.Add("passlast");
            ExportFields.Add("passfrst");
            if (IncludeEmailAddress)
                ExportFields.Add("emailaddr");

            ExportFields.Add("destinat");

            if (UseAirportCodes)
            {
                ExportFields.Add("origin");
                ExportFields.Add("LastOrigin");
            }
            else
            {
                ExportFields.Add("destdesc");
                ExportFields.Add("origdesc");
                ExportFields.Add("lastorgdes");
            }

            ExportFields.Add("rarrdate");
            ExportFields.Add("airline");
            ExportFields.Add("alinedesc");
            ExportFields.Add("fltno");
            ExportFields.Add("arrtime");
            ExportFields.Add("deptime");
            ExportFields.Add("sorttime");
            ExportFields.Add("recloc");
            ExportFields.Add("ticketed");
            ExportFields.Add("bookdate");

            //The FoxPro code was splitting the changedesc into differnet fields if it exceeded 250 characters but they were never part of the export. 
            //I think this is a bug in FoxPro, so C# version will actually include the split columns in the export
            switch (displayFields)
            {
                case 1:
                    ExportFields.Add("changedesc");
                    ExportFields.Add("changstamp");
                    break;
                case 2:
                    ExportFields.Add("changedesc");
                    ExportFields.Add("changstamp");
                    //ExportFields.Add("changedesc2");
                    //ExportFields.Add("changstamp2");
                    break;
                case 3:
                    ExportFields.Add("changedesc");
                    ExportFields.Add("changstamp");
                    ExportFields.Add("changedesc2");
                    ExportFields.Add("changstamp2");
                    ExportFields.Add("changedesc3");
                    ExportFields.Add("changstamp3");
                    break;
                default:
                    ExportFields.Add("changedesc");
                    ExportFields.Add("changstamp");
                    break;
            }

            ExportFields.Add("udidnbr1");
            var columnName = "udidtext1 AS " + udid1ColumnName;
            ExportFields.Add(columnName);
            ExportFields.Add("udidnbr2");
            columnName = "udidtext2 AS " + udid2ColumnName;
            ExportFields.Add(columnName);
        }

        private string ShortenChangeDescription(string changeDescription)
        {
            return changeDescription.Replace("Changed", "Chngd")
                .Replace("Change", "Chng")
                .Replace("Destination", "Dest")
                .Replace("Origin", "Org")
                .Replace("Validating Carrier", "Val Carr")
                .Replace("Departure Date", "Dep Date")
                .Replace("Arrival Date", "Arr Date")
                .Replace("ITINERARY", "Itin")
                .Replace("Seat Assignment", "Seat")
                .Replace("Flight #", "Flt #")
                .Replace("Departure Time", "Dep Time")
                .Replace("Arrival Time", "Arr Time")
                .Replace("Hotel Property", "Hotel");
        }

        private string CleanField(string fieldValue)
        {
            var returnValue = fieldValue.Trim();

            if (fieldValue.EndsWith("|"))
                returnValue = fieldValue.Left(fieldValue.Length - 1);

            if (fieldValue.StartsWith("|"))
                returnValue = fieldValue.Length > 1 ? fieldValue.Substring(1) : "";

            return returnValue;
        }
    }
}
