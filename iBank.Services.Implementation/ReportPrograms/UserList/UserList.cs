using System;
using System.Collections.Generic;
using System.Linq;
using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using Domain.Models;
using Domain.Models.ReportPrograms.UserList;
using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Utilities.ClientData;

namespace iBank.Services.Implementation.ReportPrograms.UserList
{
    public class UserList : ReportRunner<RawData, FinalData>
    {
        private List<AcctData> _acctDataList;
        private List<AltAuthData> _altAuthDataList;
        private List<CriteriaData> _extrasDataList;
        private bool _includeAuths;

        public UserList()
        {
            CrystalReportName = "ibUserList";
        }

        public override bool InitialChecks()
        {
            if (!IsOnlineReport()) return false;

            return true;
        }

        public override bool GetRawData()
        {

           var sql = ProcessSql(SqlBuilder.GetSql(Globals));
            RawDataList = ClientDataRetrieval.GetOpenQueryData<RawData>(sql, Globals, BuildWhere.Parameters).ToList();

            if (!DataExists(RawDataList)) return false;

            sql = ProcessSql(SqlBuilder.GetAcctSql(Globals));
            _acctDataList = ClientDataRetrieval.GetOpenQueryData<AcctData>(sql, Globals, BuildWhere.Parameters).ToList();

            _includeAuths = Globals.IsParmValueOn(WhereCriteria.CBINCLALTAUTHS) &&
                               (Globals.OutputFormat == DestinationSwitch.Csv ||
                                Globals.OutputFormat == DestinationSwitch.Xls);
            if (_includeAuths)
            {
                sql = ProcessSql(SqlBuilder.GetAltsSql(Globals));
                _altAuthDataList = ClientDataRetrieval.GetOpenQueryData<AltAuthData>(sql, Globals, BuildWhere.Parameters).ToList();

                _altAuthDataList = _altAuthDataList.Join(RawDataList, 
                    a => a.FieldData.TryIntParse(-1), 
                    r => r.UserNumber, (a,r) =>
                    new AltAuthData
                    {
                        UserNumber = a.UserNumber,
                        FirstName = r.FirstName,
                        LastName = r.LastName,
                        FieldData = a.FieldData
                    }).ToList();
            }

            sql = ProcessSql(SqlBuilder.GetNewCriteria(Globals));
            _extrasDataList = ClientDataRetrieval.GetOpenQueryData<CriteriaData>(sql, Globals, BuildWhere.Parameters).ToList();

            return true;
        }

        public override bool ProcessData()
        {
            var sql = "select convert(int,adminlvl) as adminlvl from ibuser where usernumber = " + Globals.UserNumber;
            var userAdminLevelList = ClientDataRetrieval.GetOpenQueryData<int>(sql, Globals, BuildWhere.Parameters).ToList();
            var userAdminLevel = userAdminLevelList.Any()
                ? userAdminLevelList.FirstOrDefault()
                : 0;

            var suppressPasswords = Globals.IsParmValueOn(WhereCriteria.CBSUPPPASSWORDS);

            var analytics = Globals.GetParmValue(WhereCriteria.DDANALYTICS);
            var travets = Globals.GetParmValue(WhereCriteria.DDTRAVETSYESNO);
            var emailFilter = Globals.GetParmValue(WhereCriteria.DDEMAILFILTERCHECKED);

            foreach (var row in RawDataList)
            {
                var newRow = new FinalData
                {
                    Userid = row.Userid,
                    Password =
                        suppressPasswords || row.Pwencrypt || (row.AdminLvl == 1 && userAdminLevel != 1)
                            ? "xxxxxxxx"
                            : row.Password,
                    Firstname = row.FirstName,
                    Lastname = row.LastName,
                    Emailaddr = row.Emailaddr,
                    Orgname = row.Orgname,
                    Lastlogin = row.Lastlogin ?? DateTime.MinValue,
                    Purgemsg = " -N/A- ",
                    Purgetemps = row.Purgetemps,
                    Purgeinact = row.Purgeinact,
                    Adminlvl = row.AdminLvl,
                    Reports = row.Reports
                };

                if (row.Purgeinact && row.Inactdays > 0)
                {
                    newRow.Days = !row.Lastlogin.HasValue 
                        ? 999 
                        : (row.Lastlogin.Value.AddDays(row.Inactdays) - DateTime.Now).Days;
                    newRow.Purgemsg = newRow.Days.ToString();
                }
                newRow.Accts = row.Allaccts == 3 ? "All except " : string.Empty;
                if (row.Allaccts == 1)
                {
                    newRow.Accts = "All Accounts";
                }
                else
                {
                    var accts = _acctDataList.Where(s => s.UserNumber == row.UserNumber).Select(s => s.Acct.Trim());
                    newRow.Accts += string.Join(",", accts);
                }
                if (_includeAuths)
                {
                    var altAuths = _altAuthDataList.Where(s => s.UserNumber == row.UserNumber).Select(s => s.FirstName.Trim() + " " + s.LastName.Trim());
                    newRow.AltAuths += string.Join(",", altAuths);
                }

                newRow.Analytics = _extrasDataList.Any(s => s.UserNumber == row.UserNumber && s.FieldFunction.EqualsIgnoreCase("ANALYTICS") && s.FieldData.EqualsIgnoreCase("YES"))
                    ?"YES"
                    :"NO";

                newRow.Travets = _extrasDataList.Any(s => s.UserNumber == row.UserNumber && s.FieldFunction.EqualsIgnoreCase("ALLOW_DASHBOARD") && s.FieldData.EqualsIgnoreCase("YES"))
                   ? "YES"
                   : "NO";

                newRow.Emailfiltr = _extrasDataList.Any(s => s.UserNumber == row.UserNumber && s.FieldFunction.EqualsIgnoreCase("TAEMAILFILTER") && s.FieldData.EqualsIgnoreCase("YES"))
                  ? "NOTCHECKED"
                  : "CHECKED";


                var skip = (analytics.Equals("1") && newRow.Analytics.EqualsIgnoreCase("NO")) || analytics.Equals("2") && newRow.Analytics.EqualsIgnoreCase("YES");


                if (!skip)
                {
                    skip = (travets.Equals("1") && newRow.Travets.EqualsIgnoreCase("NO")) || travets.Equals("2") && newRow.Travets.EqualsIgnoreCase("YES");
                }
                
                if (!skip)
                {
                    skip = (emailFilter.Equals("1") && newRow.Emailfiltr.EqualsIgnoreCase("NOTCHECKED")) || emailFilter.Equals("2") && newRow.Emailfiltr.EqualsIgnoreCase("CHECKED");
                }

                newRow.Stylegroup = row.SGroupName.Trim() + " (" + row.SGroupNbr + ")";
                if (!skip)
                {
                    FinalDataList.Add(newRow);
                }
            }
         
            if (!DataExists(FinalDataList)) return false;

            return true;
        }

        public override bool GenerateReport()
        {
            switch (Globals.OutputFormat)
            {
                case DestinationSwitch.Xls:
                case DestinationSwitch.Csv:
                    if (Globals.OutputFormat == DestinationSwitch.Csv)
                    {
                        ExportHelper.ConvertToCsv(FinalDataList, Globals);
                    }
                    else
                    {
                        ExportHelper.ListToXlsx(FinalDataList, Globals);
                    }
                    break;
                default:

                  
                    var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." + Constants.CrystalReportExt;

                    ReportSource = new ReportDocument();
                    ReportSource.Load(rptFilePath);

                    ReportSource.SetDataSource(FinalDataList);

                    CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
                    ReportSource.SetParameterValue("cAgencyName",Globals.AgencyInformation.AgencyName);

                    CrystalFunctions.CreatePdf(ReportSource, Globals);
                    break;
            }
            return true;
        }

        private static string ProcessSql(SqlScript script)
        {
            return string.Format("select {0} from {1} where {2} {3}", script.FieldList, script.FromClause, script.WhereClause, script.OrderBy);
        }
    }
}
