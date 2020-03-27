using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;

using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using Domain.Models.ReportPrograms.AirFareSavingsReport;

using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Utilities.ClientData;

namespace iBank.Services.Implementation.ReportPrograms.AirFareSavings
{
    public class Udid
    {
        public List<UdidData> ReportSettingsUdidInfoList = new List<UdidData>();
        public CommaDelimitedStringCollection ReportSettingsUdidsString = new CommaDelimitedStringCollection();

        public void AddReportSettingsUdidItem(int udidOnRpt, string udidLbl)
        {
            if (udidOnRpt == 0) return;

            var label = string.IsNullOrEmpty(udidLbl) ? "UdidTxt" + udidOnRpt : udidLbl.Trim();
            ReportSettingsUdidInfoList.Add(new UdidData { UdidLabel = label, UdidNo = (short)udidOnRpt });

            ReportSettingsUdidsString.Add(udidOnRpt.ToString());
        }

        public List<UdidData> GetReportSettingsUdidData(bool isPreview, IClientQueryable db, object[] parameters, string whereClauseFull, ReportGlobals globals)
        {
            
            if (ReportSettingsUdidsString.Count == 0) return new List<UdidData>();

            var fromClause = isPreview ? "ibtrips T1, ibudids T3" : "hibtrips T1, hibudids T3";
            var whereClause =
                @"T1.reckey = T3.reckey and T1.agency = T3.agency and valcarr not in ('ZZ','$$') and " + whereClauseFull +
                " and cast(udidno as int) in (" + ReportSettingsUdidsString + ") and udidtext is not null ";
            var fieldList = @"T1.reckey, udidno, udidtext, count(*) as count ";
            var orderClause = "";
            whereClause += " Group By t1.reckey, udidno, udidtext ";
            var fullSql = SqlProcessor.ProcessSql(fieldList, false, fromClause, whereClause, orderClause, globals);
            
            return ClientDataRetrieval.GetUdidFilteredOpenQueryData<UdidData>(fullSql, globals, parameters, isPreview).ToList();
        }

        public void SetUdids(List<UdidData> udidList, FinalData fData, bool outputIsXlsOrCsv, List<UdidData> reportSettingsUdidInfoList)
        {
            var counter = 1;
            foreach (var udid in udidList.Where(u => u.RecKey == fData.Reckey).OrderBy(o => o.UdidNo))
            {
                var udidInfo = reportSettingsUdidInfoList.FirstOrDefault(u => u.UdidNo == udid.UdidNo);
                if (udidInfo != null && !udidInfo.UdidLabel.IsNullOrWhiteSpace())
                {
                    var label = udidInfo.UdidLabel;
                    udid.UdidLabel = label + (outputIsXlsOrCsv || label.Right(1).EqualsIgnoreCase(":") ? "" : ":");

                    switch (counter)
                    {
                        case 1:
                            fData.Udidlbl1 = udid.UdidLabel;
                            fData.Udidtext1 = udid.UdidText;
                            break;
                        case 2:
                            fData.Udidlbl2 = udid.UdidLabel;
                            fData.Udidtext2 = udid.UdidText;
                            break;
                        case 3:
                            fData.Udidlbl3 = udid.UdidLabel;
                            fData.Udidtext3 = udid.UdidText;
                            break;
                        case 4:
                            fData.Udidlbl4 = udid.UdidLabel;
                            fData.Udidtext4 = udid.UdidText;
                            break;
                        case 5:
                            fData.Udidlbl5 = udid.UdidLabel;
                            fData.Udidtext5 = udid.UdidText;
                            break;
                        case 6:
                            fData.Udidlbl6 = udid.UdidLabel;
                            fData.Udidtext6 = udid.UdidText;
                            break;
                        case 7:
                            fData.Udidlbl7 = udid.UdidLabel;
                            fData.Udidtext7 = udid.UdidText;
                            break;
                        case 8:
                            fData.Udidlbl8 = udid.UdidLabel;
                            fData.Udidtext8 = udid.UdidText;
                            break;
                        case 9:
                            fData.Udidlbl9 = udid.UdidLabel;
                            fData.Udidtext9 = udid.UdidText;
                            break;
                        case 10:
                            fData.Udidlbl10 = udid.UdidLabel;
                            fData.Udidtext10 = udid.UdidText;
                            break;
                    }
                    counter++;
                }
            }
        }
    }
}
