using System.Collections.Generic;
using System.Linq;
using Domain.Helper;
using Domain.Models.ReportPrograms.TopAccountAir;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Shared.Client;
using iBankDomain.RepositoryInterfaces;

namespace iBank.Services.Implementation.ReportPrograms.TopAccountAir
{
    public class TopAccountAirDataProcessor
    {
        private ReportGlobals _globals;
        private ClientFunctions _clientFunctions;
        private IQuery<IList<MasterAccountInformation>> _getAllParentAccountsQuery;
        private IQuery<IList<MasterAccountInformation>> _getAllMasterAccountsQuery;

        public TopAccountAirDataProcessor(ReportGlobals globals, ClientFunctions clientFunctions, IQuery<IList<MasterAccountInformation>> getAllParentAccountsQuery, IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery)
        {
            // Set up processing variables
            _globals = globals;
            _clientFunctions = clientFunctions;
            _getAllParentAccountsQuery = getAllParentAccountsQuery;
            _getAllMasterAccountsQuery = getAllMasterAccountsQuery;
        }


        public List<FinalData> ConvertRawDatatoFinalData(List<RawData> rawDataList, bool isReservation, IMasterDataStore store)
        {
            // Initial grouping of data
            var bySourceAndAcct = rawDataList.GroupBy(s => new { s.SourceAbbr, s.Acct }, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new FinalData
                {
                    SourceAbbr = key.SourceAbbr.Trim(),
                    Account = key.Acct.Trim(),
                    Amt = reclist.Sum(s => s.AirChg),
                    Acommisn = reclist.Sum(s => s.ACommisn),
                    OffrdChg = reclist.Sum(s => s.OffrdChg > 0 && s.AirChg < 0
                                   ? 0 - s.OffrdChg
                                   : s.OffrdChg == 0 ? s.AirChg : s.OffrdChg),
                    Svcfee = reclist.Sum(s => s.SvcFee),
                    Trips = reclist.Sum(s => s.Plusmin)
                };
            }).ToList();

            // Group data
            var finalDataList = GroupData(bySourceAndAcct, _getAllMasterAccountsQuery, _getAllParentAccountsQuery, store);

            var outputType = _globals.GetParmValue(WhereCriteria.OUTPUTTYPE);

            // Format account names
            if (_globals.ParmValueEquals(WhereCriteria.DDGRPFIELD, "2") && !outputType.Equals("2") && !outputType.Equals("5"))
            {
                //GROUP BY ACCOUNT NAME OR DATA SOURCE DESCRIPTION.
                foreach (var row in finalDataList)
                {
                    row.Account = row.AcctName;
                }
            }
            else
            {
                var temp = new List<string> { "4", "6", "RG", "XG", "2", "5", "2X" };
                if (!temp.Contains(outputType))
                {
                    if (!_globals.ParmValueEquals(WhereCriteria.GROUPBY, "3"))
                    {
                        foreach (var row in finalDataList)
                        {
                            row.Account = (row.AcctName + "(" + row.Account + ")").PadRight(44);
                        }
                    }
                }
            }
            return finalDataList;
        }

        // Groups data according to report parameters
        private List<FinalData> GroupData(List<FinalData> bySourceAndAcct, IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, 
            IQuery<IList<MasterAccountInformation>> getAllParentAccountsQuery, IMasterDataStore store)
        {
            //we have to group by different values, but those values must be retrieved first. 
            switch (_globals.GetParmValue(WhereCriteria.GROUPBY))
            {
                case "2":
                    // Group by parent account
                    var acctsToLookUp = bySourceAndAcct.Select(s => s.Account).Distinct().ToList();
                    foreach (var acct in acctsToLookUp)
                    {
                        var parentAcct = _clientFunctions.LookupParent(getAllMasterAccountsQuery, acct, getAllParentAccountsQuery);
                        foreach (var row in bySourceAndAcct.Where(s => s.Account.EqualsIgnoreCase(acct)))
                        {
                            row.Account = parentAcct.AccountId.Trim();
                            row.AcctName = parentAcct.AccountDescription.Trim();
                        }
                    }
                    break;
                case "3":
                    // Group by data source
                    var abbrsToLookUp = bySourceAndAcct.Select(s => s.SourceAbbr).Distinct();
                    foreach (var abbr in abbrsToLookUp)
                    {
                        var sourceDesc = LookupFunctions.LookupSourceDescription(store, abbr,
                            _globals.Agency);
                        foreach (var row in bySourceAndAcct.Where(s => s.SourceAbbr.EqualsIgnoreCase(abbr)))
                        {
                            row.Account = abbr.Trim();
                            row.AcctName = sourceDesc.Trim();
                        }
                    }
                    break;
                default:
                    // Group by accounts
                    acctsToLookUp = bySourceAndAcct.Select(s => s.Account).Distinct().ToList();
                    foreach (var acct in acctsToLookUp)
                    {
                        var acctName = _clientFunctions.LookupCname(getAllMasterAccountsQuery, acct, _globals);
                        foreach (var row in bySourceAndAcct.Where(s => s.Account.EqualsIgnoreCase(acct)))
                        {
                            row.AcctName = acctName.Trim();
                        }
                    }
                    break;
            }

            return bySourceAndAcct.GroupBy(s => new { s.Account, s.AcctName }, (key, recs) =>
            {
                var reclist = recs.ToList();
                return new FinalData
                {
                    Account = key.Account,
                    AcctName = key.AcctName,
                    Amt = reclist.Sum(s => s.Amt),
                    Trips = reclist.Sum(s => s.Trips),
                    Svcfee = reclist.Sum(s => s.Svcfee),
                    Acommisn = reclist.Sum(s => s.Acommisn),
                    LowFare = reclist.Sum(s => s.OffrdChg),
                    LostAmt = reclist.Sum(s => s.Amt - s.OffrdChg)

                };
            }).ToList();
        }

        // Adds service fees from hibServices to final data
        public static List<RawData> AddSvcFees(List<SvcFeeData> svcFeeData, List<RawData> rawDataList)
        {
            foreach (var svc in svcFeeData)
            {
                var rawDataRecord = rawDataList.FirstOrDefault(
                    s => (svc.Acct.EqualsIgnoreCase(s.Acct) 
                    && svc.SourceAbbr.EqualsIgnoreCase(s.SourceAbbr)));
                if (rawDataRecord != null)
                {
                    rawDataRecord.SvcFee += svc.SvcAmt;
                }
            }
            return rawDataList;
        }

    }
}
