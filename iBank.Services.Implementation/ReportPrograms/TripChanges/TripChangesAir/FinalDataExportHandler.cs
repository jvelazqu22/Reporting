using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Domain.Models.ReportPrograms.TripChangesAir;
using iBank.Server.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.TripChanges.TripChangesAir
{
    public class FinalDataExportHandler
    {
        public List<FinalDataExport> SetupFinalReportAndSetExportFields(List<FinalData> FinalDataList, bool ConsolidateChanges, List<string> ExportFields)
        {
            var finalDataExportList = FinalDataList.Select(s => new FinalDataExport
            {
                Rectype = s.Rectype,
                Reckey = s.Reckey,
                Acct = s.Acct,
                Acctdesc = s.Acctdesc,
                Mtggrpnbr = s.Mtggrpnbr,
                Passlast = s.Passlast,
                Passfrst = s.Passfrst,
                Ticket = s.Ticket,
                Recloc = s.Recloc,
                Bookdate = s.Bookdate,
                Depdate = s.Depdate,
                Airchg = s.Airchg,
                Changstamp = s.Changstamp == DateTime.MinValue ? string.Empty : s.Changstamp.ToString(CultureInfo.InvariantCulture),
                Changedesc = s.Changedesc,
                Origin = s.Origin,
                Orgdesc = s.Orgdesc,
                Destinat = s.Destinat,
                Destdesc = s.Destdesc,
                Connect = s.Connect,
                Seqno = s.Seqno,
                Airline = s.Airline,
                Rdepdate = s.Rdepdate == DateTime.MinValue ? string.Empty : s.Rdepdate.ToShortDateString(),
                Fltno = s.Fltno,
                Deptime = s.Deptime,
                Arrtime = s.Arrtime,
                Segnum = s.Segnum
            }).OrderBy(s => s.Acct)
              .ThenBy(s => s.Passlast)
              .ThenBy(s => s.Passfrst)
              .ThenBy(s => s.Rectype)
              .ThenBy(s => s.Depdate)
              .ThenBy(s => s.Reckey)
              .ThenBy(s => s.Segnum)
              .ThenBy(s => s.Changstamp)
              .ThenBy(s => s.Seqno).ToList();

            var displayFields = 1;

            if (ConsolidateChanges)
            {
                finalDataExportList = FinalDataList.Where(s => s.Rectype == "A").Select(
                    s =>
                        new
                        {
                            s.Rectype,
                            s.Reckey,
                            s.Acct,
                            s.Acctdesc,
                            s.Mtggrpnbr,
                            s.Passlast,
                            s.Passfrst,
                            s.Ticket,
                            s.Recloc,
                            s.Bookdate,
                            s.Depdate,
                            s.Airchg,
                            s.Changstamp,
                            s.Changedesc,
                            s.Origin,
                            s.Orgdesc,
                            s.Destinat,
                            s.Destdesc,
                            s.Connect,
                            s.Seqno,
                            s.Airline,
                            s.Rdepdate,
                            s.Fltno,
                            s.Deptime,
                            s.Arrtime,
                            s.Segnum,
                        }).Distinct()
                    .Select(t => new FinalDataExport
                    {
                        Rectype = t.Rectype,
                        Reckey = t.Reckey,
                        Acct = t.Acct,
                        Acctdesc = t.Acctdesc,
                        Mtggrpnbr = t.Mtggrpnbr,
                        Passlast = t.Passlast,
                        Passfrst = t.Passfrst,
                        Ticket = t.Ticket,
                        Recloc = t.Recloc,
                        Bookdate = t.Bookdate,
                        Depdate = t.Depdate,
                        Airchg = t.Airchg,
                        Changstamp = new string(' ', 125),
                        Changedesc = new string(' ', 250),
                        Changstamp2 = new string(' ', 125),
                        Changedesc2 = new string(' ', 250),
                        Changstamp3 = new string(' ', 125),
                        Changedesc3 = new string(' ', 250),
                        Origin = t.Origin,
                        Orgdesc = t.Orgdesc,
                        Destinat = t.Destinat,
                        Destdesc = t.Destdesc,
                        Connect = t.Connect,
                        Seqno = t.Seqno,
                        Airline = t.Airline,
                        Rdepdate = t.Rdepdate.ToShortDateString(),
                        Fltno = t.Fltno,
                        Deptime = t.Deptime,
                        Arrtime = t.Arrtime,
                        Segnum = t.Segnum
                    })
                    .OrderBy(s => s.Reckey)
                    .ThenBy(s => s.Seqno).ToList();

                foreach (var finalData in finalDataExportList)
                {
                    var useChangeFields = 1;
                    string changeDesc1 = "", changeDesc2 = "", changeDesc3 = "";
                    string changeStamp1 = "", changeStamp2 = "", changeStamp3 = "";

                    var data = finalData;

                    var records =
                        FinalDataList.Where(
                            s =>
                                s.Reckey == data.Reckey && s.Segnum == data.Segnum);
                    foreach (var record in records)
                    {
                        var originalChangeDescription = ShortenChangeDescription(record.Changedesc);
                        if (!string.IsNullOrEmpty(originalChangeDescription))
                        {
                            switch (useChangeFields)
                            {
                                case 1:
                                    if (changeDesc1.Length > 50 &&
                                        (changeDesc1.Length + originalChangeDescription.Trim().Length > 250))
                                        useChangeFields = 2;
                                    break;
                                case 2:
                                    if (changeDesc2.Length > 50 &&
                                        (changeDesc2.Length + originalChangeDescription.Trim().Length > 250))
                                        useChangeFields = 3;
                                    break;
                            }

                            if ((string.IsNullOrEmpty(changeDesc1.Trim()) || (originalChangeDescription.Trim().IndexOf(changeDesc1, StringComparison.Ordinal) == -1)) &&
                                (string.IsNullOrEmpty(changeDesc2.Trim()) || (originalChangeDescription.Trim().IndexOf(changeDesc2, StringComparison.Ordinal) == -1))
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

                    displayFields = displayFields > useChangeFields ? displayFields : useChangeFields;
                    finalData.Changedesc = changeDesc1;
                    finalData.Changedesc2 = changeDesc2;
                    finalData.Changedesc3 = changeDesc3;

                    finalData.Changstamp = changeStamp1;
                    finalData.Changstamp2 = changeStamp2;
                    finalData.Changstamp3 = changeStamp3;

                }

                finalDataExportList = finalDataExportList
                            .OrderBy(s => s.Acct)
                            .ThenBy(s => s.Passlast)
                            .ThenBy(s => s.Passfrst)
                            .ThenBy(s => s.Rectype)
                            .ThenBy(s => s.Depdate)
                            .ThenBy(s => s.Reckey)
                            .ThenBy(s => s.Segnum)
                            .ThenBy(s => s.Changstamp)
                            .ThenBy(s => s.Seqno).ToList();

            }

            //Setup the export fields
            ExportFields.Add("rectype");
            ExportFields.Add("reckey");
            ExportFields.Add("acct");
            ExportFields.Add("acctdesc");
            ExportFields.Add("passlast");
            ExportFields.Add("passfrst");
            ExportFields.Add("mtggrpnbr");
            ExportFields.Add("ticket");
            ExportFields.Add("recloc");
            ExportFields.Add("bookdate");
            ExportFields.Add("DepdateDisplay AS depdate");
            ExportFields.Add("airchg");

            //The FoxPro code was splitting the changedesc into differnet fields if it exceeded 250 characters but they were never part of the export. 
            //I think this is a bug in FoxPro, so C# version will actually include the split columns in the export
            switch (displayFields)
            {
                case 1:
                    ExportFields.Add("changstamp");
                    ExportFields.Add("changedesc");
                    break;
                case 2:
                    ExportFields.Add("changstamp");
                    ExportFields.Add("changedesc");
                    ExportFields.Add("changstamp2");
                    ExportFields.Add("changedesc2");
                    break;
                case 3:
                    ExportFields.Add("changstamp");
                    ExportFields.Add("changedesc");
                    ExportFields.Add("changstamp2");
                    ExportFields.Add("changedesc2");
                    ExportFields.Add("changstamp3");
                    ExportFields.Add("changedesc3");
                    break;
                default:
                    ExportFields.Add("changstamp");
                    ExportFields.Add("changedesc");
                    break;
            }

            ExportFields.Add("origin");
            ExportFields.Add("orgdesc");
            ExportFields.Add("destinat");
            ExportFields.Add("destdesc");
            ExportFields.Add("connect");
            ExportFields.Add("seqno");
            ExportFields.Add("airline");
            ExportFields.Add("rdepdate");
            ExportFields.Add("fltno");
            ExportFields.Add("deptime");
            ExportFields.Add("arrtime");
            ExportFields.Add("segnum");

            return finalDataExportList;

        }

        /// <summary>
        /// Removes the trailing or starting pipe from the field (only the first instance)
        /// </summary>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        private string CleanField(string fieldValue)
        {
            var returnValue = fieldValue.Trim();

            if (fieldValue.EndsWith("|"))
                returnValue = fieldValue.Left(fieldValue.Length - 1);

            if (fieldValue.StartsWith("|"))
                returnValue = fieldValue.Length > 1 ? fieldValue.Substring(1) : "";

            return returnValue;
        }

        /// <summary>
        /// For CSV/Excel exports the change description needs to be shortened
        /// </summary>
        /// <param name="changeDescription"></param>
        /// <returns></returns>

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

    }
}
