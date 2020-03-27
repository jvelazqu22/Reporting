using CODE.Framework.Core.Utilities.Extensions;
using Domain.Helper;
using Domain.Models.ReportPrograms.TravAuthKpi;
using Domain.Orm.iBankAdminQueries;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Orm.iBankClientQueries;
using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Shared.Client;

namespace iBank.Services.Implementation.ReportPrograms.TravAuthKpi
{
    public static class TravAuthKpiHelpers
    {
        public static WhereClauses GetWhereClauses(ReportGlobals globals)
        {
            var monthName = globals.GetParmValue(WhereCriteria.STARTMONTH);
            var month = SharedProcedures.GetMonthNum(monthName);
            var year = globals.GetParmValue(WhereCriteria.STARTYEAR).TryIntParse(-1);

            var begMthCy = new DateTime(year,month,1);
            var begYrCy = new DateTime(year,1,1);
            var endMthCy = begMthCy.AddMonths(1).AddSeconds(-1);
       
            var begMthPy = new DateTime(year-1, month, 1);
            var begYrPy = new DateTime(year-1, 1, 1);
            var endMthPy = begMthPy.AddMonths(1).AddSeconds(-1);

            var clauses = new WhereClauses();

            switch (globals.GetParmValue(WhereCriteria.DATERANGE))
            {
                case "3":
                    clauses.WhereCyMth = "bookedgmt between '" + begMthCy.ToShortDateString() + "' and '" + endMthCy.ToShortDateString() + " 11:59:59 PM'";
                    clauses.WhereCyYtd = "bookedgmt between '" + begYrCy.ToShortDateString() + "' and '" + endMthCy.ToShortDateString() + " 11:59:59 PM'";
                    clauses.WherePyMth = "bookedgmt between '" + begMthPy.ToShortDateString() + "' and '" + endMthPy.ToShortDateString() + " 11:59:59 PM'";
                    clauses.WherePyYtd = "bookedgmt between '" + begYrPy.ToShortDateString() + "' and '" + endMthPy.ToShortDateString() + " 11:59:59 PM'";
                    break;
                case "12":
                    clauses.WhereCyMth = "T7.statustime between '" + begMthCy.ToShortDateString() + "' and '" + endMthCy.ToShortDateString() + " 11:59:59 PM'";
                    clauses.WhereCyYtd = "T7.statustime between '" + begYrCy.ToShortDateString() + "' and '" + endMthCy.ToShortDateString() + " 11:59:59 PM'";
                    clauses.WherePyMth = "T7.statustime between '" + begMthPy.ToShortDateString() + "' and '" + endMthPy.ToShortDateString() + " 11:59:59 PM'";
                    clauses.WherePyYtd = "T7.statustime between '" + begYrPy.ToShortDateString() + "' and '" + endMthPy.ToShortDateString() + " 11:59:59 PM'";
                    break;
                default:
                    clauses.WhereCyMth = "T1.depdate between '" + begMthCy.ToShortDateString() + "' and '" + endMthCy.ToShortDateString() + " 11:59:59 PM'";
                    clauses.WhereCyYtd = "T1.depdate between '" + begYrCy.ToShortDateString() + "' and '" + endMthCy.ToShortDateString() + " 11:59:59 PM'";
                    clauses.WherePyMth = "T1.depdate between '" + begMthPy.ToShortDateString() + "' and '" + endMthPy.ToShortDateString() + " 11:59:59 PM'";
                    clauses.WherePyYtd = "T1.depdate between '" + begYrPy.ToShortDateString() + "' and '" + endMthPy.ToShortDateString() + " 11:59:59 PM'";
                    break;
            }

            return clauses;
        }

        public static List<ReasonCodeRawData> SplitCodes(List<ReasonCodeRawData> groupedData, ReportGlobals globals)
        {
            var groupedDataWithMultipleCodes = groupedData.Where(s => s.OutPolCods.Contains(",")).ToList();
            groupedData.RemoveAll(s => groupedDataWithMultipleCodes.Contains(s));

            var oopCrit = globals.GetParmValue(WhereCriteria.INOOPCODES);
            var notInOopList = globals.IsParmValueOn(WhereCriteria.NOTINOOPCODES);

            var oopCritList = string.IsNullOrEmpty(oopCrit)
                ? new List<string>()
                : oopCrit.Split(',').ToList();

            foreach (var row in groupedDataWithMultipleCodes)
            {
                foreach (var oopCode in row.OutPolCods.Split(','))
                {
                    var keep = true;
                    if (oopCritList.Any())
                    {
                        if (notInOopList)
                        {
                            if (oopCritList.Contains(oopCode)) keep = false;
                        }
                        else
                        {
                            if (!oopCritList.Contains(oopCode)) keep = false;
                        }
                    }
                    if (keep)
                    {
                        groupedData.Add(new ReasonCodeRawData
                        {
                            RecKey = row.RecKey,
                            Acct = row.Acct,
                            OutPolCods = oopCode,
                            NumTrips = row.NumTrips
                        });
       
                    }
                }
            }

            return groupedData;
        }

        public static List<ReasonCodeRawData> GroupReasons(List<ReasonCodeRawData> temp)
        {
            return temp.GroupBy(s => new {s.Acct, OutPolCodes = s.OutPolCods}, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new ReasonCodeRawData
                {
                    Acct = key.Acct,
                    OutPolCods = key.OutPolCodes.Trim(),
                    Cost = reclist.Sum(s => s.Cost),
                    NumTrips = reclist.Count
                };
            }).ToList();
        }

        public static List<ReasonCodeRawData> GroupReasonsAgain(List<ReasonCodeRawData> temp, ClientFunctions clientFunctions, GetAllMasterAccountsQuery getAllMasterAccountsQuery, ReportGlobals globals,
            IClientDataStore clientStore, IMasterDataStore masterStore)
        {
            return temp.GroupBy(s => new
            {
                s.Acct,
                OutPolCodes = s.OutPolCods
            }, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new ReasonCodeRawData
                {
                    Acct = key.Acct,
                    OutPolCods = clientFunctions.LookupReason(getAllMasterAccountsQuery, key.OutPolCodes,key.Acct, clientStore, globals, masterStore.MastersQueryDb),
                    Cost = reclist.Sum(s => s.Cost),
                    NumTrips = reclist.Sum(s => s.NumTrips)
                };
            })
            .OrderByDescending(s => s.NumTrips)
            .ToList();
        }

        public static List<ibuser> Users;
        public static string LookupAuthorizerName(int userNumber, string email, IClientQueryable clientQueryable)
        {
            if (Users == null || !Users.Any())
            {
                Users = new GetAllUsersQuery(clientQueryable).ExecuteQuery().ToList();
            }

            var user = Users.FirstOrDefault(s => s.UserNumber == userNumber);
            if (user == null)
            {
                user = Users.FirstOrDefault(s =>s.emailaddr != null && s.emailaddr.EqualsIgnoreCase(email));
                if (user == null) return "NOT FOUND".PadRight(30);
            }

            return (user.firstname + " " + user.lastname).PadRight(30);
        }

        public static string GetDefaultWhereClause(BuildWhere buildWhere)
        {
            return buildWhere.WhereClauseFull +
                (buildWhere.ReportGlobals.IsParmValueOn(WhereCriteria.CBINCLNOTIFONLY)
                ? " and T7.authStatus != 'C' "
                : " and T7.authStatus not in ('C','N')");
        }
    }
}
