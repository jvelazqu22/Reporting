using Domain.Models.ReportPrograms.WeeklyTravelerActivity;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Utilities.ClientData;

namespace iBank.Services.Implementation.ReportPrograms.WeeklyTravelerActivity
{
    public class UdidCalculator
    {
        public void AddReportSettingsUdidItem(int udidOnRpt, string udidLbl, List<UdidData> ReportSettingsUdidInfoList, CommaDelimitedStringCollection ReportSettingsUdidsString)
        {
            if (udidOnRpt == 0) return;

            ReportSettingsUdidInfoList.Add(new UdidData { UdidLabel = udidLbl, UdidNo = (short)udidOnRpt });

            var s = string.IsNullOrEmpty(udidLbl) ? "UdidTxt" + udidOnRpt : udidLbl.Trim();
            ReportSettingsUdidsString.Add(udidOnRpt.ToString());
        }

        public List<UdidData> GetReportSettingsUdidData(CommaDelimitedStringCollection ReportSettingsUdidsString, bool IsPreview, ReportGlobals Globals, BuildWhere BuildWhere, IClientQueryable ClientQueryableDb)
        {
            if (ReportSettingsUdidsString.Count == 0) return new List<UdidData>();

            var fromClause = IsPreview ? "ibtrips T1, ibudids T3" : "hibtrips T1, hibudids T3";
            var whereClause =
                @"T1.reckey = T3.reckey and T1.agency = T3.agency and valcarr not in ('ZZ','$$') and " + BuildWhere.WhereClauseFull +
                " and cast(udidno as int) in (" + ReportSettingsUdidsString + ") and udidtext is not null ";
            var fieldList = @"T1.reckey, udidno, udidtext, count(*) as count ";
            var orderClause = "";
            whereClause += " Group By t1.reckey, udidno, udidtext ";
            var fullSql = SqlProcessor.ProcessSql(fieldList, false, fromClause, whereClause, orderClause, Globals);
            
            return ClientDataRetrieval.GetUdidFilteredOpenQueryData<UdidData>(fullSql, Globals, BuildWhere.Parameters, IsPreview).ToList();
        }

        public UdidData GetUdidsForTrip(TripLevelData tripRow, int cntr, bool outputIsXlsOrCsv, List<UdidData> reportSettingsUdidDataList, CommaDelimitedStringCollection ReportSettingsUdidsString, List<UdidData> ReportSettingsUdidInfoList)
        {
            var udid = new UdidData();

            var tripUdids = new List<UdidData>();
            if (reportSettingsUdidDataList.Any())
            {
                tripUdids = reportSettingsUdidDataList.Where(s => s.RecKey == tripRow.RecKey).ToList();
            }

            if (!tripUdids.Any()) return udid;

            var numUdids = Math.Min(ReportSettingsUdidsString.Count, tripUdids.Count);
            if (numUdids > 0)
            {
                if (cntr < numUdids)
                {
                    udid.UdidText = tripUdids[cntr].UdidText;
                    udid.UdidNo = tripUdids[cntr].UdidNo;
                    udid.UdidLabel = "Udid # " + udid.UdidNo + " text:";

                    // Find the udidNo in the UdidsCollection...
                    var udidInfo = ReportSettingsUdidInfoList.FirstOrDefault(s => s.UdidNo == udid.UdidNo);
                    if (udidInfo != null && !udidInfo.UdidLabel.IsNullOrWhiteSpace())
                    {
                        var label = udidInfo.UdidLabel;
                        udid.UdidLabel = label + (outputIsXlsOrCsv || label.Right(1).EqualsIgnoreCase(":") ? "" : ":");
                    }
                }
            }

            return udid;
        }

    }
}
