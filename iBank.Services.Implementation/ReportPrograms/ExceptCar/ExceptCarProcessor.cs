using System;
using System.Collections.Generic;
using System.Linq;

using CODE.Framework.Core.Utilities;

using Domain.Helper;
using Domain.Models.ReportPrograms.ExceptCarReport;
using Domain.Orm.Classes;

using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.Client;
using iBankDomain.RepositoryInterfaces;

namespace iBank.Services.Implementation.ReportPrograms.ExceptCar
{
    public class ExceptCarProcessor
    {
        public IList<FinalData> MapRawToFinalData(IList<RawData> rawData, bool accountBreak, ClientFunctions clientFunctions, IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, ReportGlobals globals,
            UserBreaks userBreaks, IClientDataStore clientStore, IMasterDataStore masterStore)
        {
            return rawData.Select(s => new FinalData
            {
                Recloc = s.Recloc,
                Reckey = s.RecKey,
                Acct = !accountBreak ? Constants.NotApplicable : s.Acct,
                Acctdesc = !accountBreak ? Constants.NotApplicable : clientFunctions.LookupCname(getAllMasterAccountsQuery, s.Acct, globals),
                Break1 = !userBreaks.UserBreak1 ? Constants.NotApplicable
                                                : string.IsNullOrEmpty(s.Break1.Trim()) ? "NONE".PadRight(30) : s.Break1.Trim(),
                Break2 = !userBreaks.UserBreak2 ? Constants.NotApplicable
                                                : string.IsNullOrEmpty(s.Break2.Trim()) ? "NONE".PadRight(30) : s.Break2.Trim(),
                Break3 = !userBreaks.UserBreak3 ? Constants.NotApplicable
                                                : string.IsNullOrEmpty(s.Break3.Trim()) ? "NONE".PadRight(16) : s.Break3.Trim(),
                Passlast = s.Passlast.Trim(),
                Passfrst = s.Passfrst.Trim(),
                Reascoda = s.Reascoda,
                Rentdate = s.Rentdate ?? DateTime.MinValue,
                Company = s.Company,
                Autocity = s.Autocity,
                Autostat = s.Autostat,
                Days = s.Days,
                Cartype = s.Cartype,                             
                Reasdesc = string.Format($"{s.Reascoda.Trim()} {clientFunctions.LookupReason(getAllMasterAccountsQuery, s.Reascoda, s.Acct, clientStore, globals, masterStore.MastersQueryDb)}"),
                Ctypedesc = LookupFunctions.LookupCarType(s.Cartype, globals.UserLanguage, new MasterDataStore()),
                Cplusmin = s.Cplusmin,
                Abookrat = s.Abookrat.ToDecimalSafe(),
                Aexcprat = (s.Aexcprat.ToDecimalSafe() > 0 && s.Abookrat.ToDecimalSafe() < 0) ? (0 - s.Aexcprat.ToDecimalSafe())
                                                              : (s.Aexcprat.ToDecimalSafe() == 0) ? s.Abookrat.ToDecimalSafe()
                                                                                 : s.Aexcprat.ToDecimalSafe()

            }).OrderBy(x => x.Acctdesc)
              .ThenBy(x => x.Break1)
              .ThenBy(x => x.Break2)
              .ThenBy(x => x.Break3)
              .ThenBy(x => x.Passlast)
              .ThenBy(x => x.Passfrst)
              .ThenBy(x => x.Reasdesc)
              .ThenBy(x => x.Rentdate)
              .ToList();
        }
    }
}
