using Domain.Helper;
using Domain.Models.ReportPrograms.ServiceFeeDetailByTransaction;
using Domain.Orm.Classes;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBankDomain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using iBank.Services.Implementation.Shared.Client;

namespace iBank.Services.Implementation.ReportPrograms.ServiceFeeDetailByTransaction
{
    public class SvcFeeDetailByTransactionFinalData
    {
        public List<FinalData> GetFinalDataList(List<RawData> RawDataList, ReportGlobals Globals, bool AccountBreak, ClientFunctions clientFunctions,
            IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, Dictionary<int, string> routeItineraries)
        {
            List<FinalData> FinalDataList =
            RawDataList.Select(s => new FinalData
            {
                Acct = AccountBreak ? s.Acct : Constants.NotApplicable,
                Acctdesc = AccountBreak ? clientFunctions.LookupCname(getAllMasterAccountsQuery, s.Acct, Globals) : Constants.NotApplicable,
                Descript = s.SvcDesc,
                Passlast = s.Passlast,
                Passfrst = s.Passfrst,
                Trandate = s.Trandate ?? DateTime.MinValue,
                Recloc = !string.IsNullOrEmpty(s.Recloc) ? s.Recloc : new string(' ', 8),
                Svcfee = s.SvcAmt ?? 0,
                SvcFeeCurrType = s.FeeCurrTyp,
                Depdate = s.Depdate ?? DateTime.MinValue,
                Invoice = !string.IsNullOrEmpty(s.Invoice) ? s.Invoice : new string(' ', 10),
                Ticket = !routeItineraries.ContainsKey(s.RecKey) ? "NODATA" : s.Ticket,
                Itinerary = routeItineraries.ContainsKey(s.RecKey) ? routeItineraries[s.RecKey] : string.Empty,
                Brkfield = Globals.ParmValueEquals(WhereCriteria.GROUPBY, "2")
                    ? string.Format("{0}/{1}", s.Passlast.Trim(), s.Passfrst.Trim()).PadRight(35)
                    : Globals.ParmValueEquals(WhereCriteria.GROUPBY, "3")
                        ? s.Trandate.HasValue ? s.Trandate.Value.ToShortDateString() : ""
                        : s.SvcDesc
            }).ToList();

            if (Globals.ParmValueEquals(WhereCriteria.GROUPBY, "2"))
            {
                FinalDataList = FinalDataList.OrderBy(s => s.Acct)
                    .ThenBy(s => s.Passlast)
                    .ThenBy(s => s.Passfrst)
                    .ThenBy(s => s.Trandate)
                    .ThenBy(s => s.Descript).ToList();
            }
            else if (Globals.ParmValueEquals(WhereCriteria.GROUPBY, "3"))
            {
                FinalDataList = FinalDataList.OrderBy(s => s.Acct)
                    .ThenBy(s => s.Trandate)
                    .ThenBy(s => s.Passlast)
                    .ThenBy(s => s.Passfrst)
                    .ThenBy(s => s.Descript).ToList();
            }
            else
            {
                FinalDataList = FinalDataList.OrderBy(s => s.Acct)
                    .ThenBy(s => s.Descript)
                    .ThenBy(s => s.Passlast)
                    .ThenBy(s => s.Passfrst)
                    .ThenBy(s => s.Trandate)
                    .ToList();
            }

            return FinalDataList;
        }
    }
}
