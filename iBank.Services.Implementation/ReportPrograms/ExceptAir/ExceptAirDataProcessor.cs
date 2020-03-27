using System;
using System.Collections.Generic;
using System.Linq;

using CODE.Framework.Core.Utilities;

using Domain.Helper;
using Domain.Models.ReportPrograms.ExceptAirReport;

using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;

using Domain.Orm.Classes;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using iBank.Services.Implementation.Shared.Client;
using iBankDomain.RepositoryInterfaces;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;

namespace iBank.Services.Implementation.ReportPrograms.ExceptAir
{
    public class ExceptAirDataProcessor
    {
        public IList<RawData> ReplaceAirChargeWithBaseFare(IList<RawData> rawData)
        {
            foreach (var raw in rawData)
            {
                if (raw.BaseFare.HasValue && raw.BaseFare.Value != 0)
                {
                    raw.Airchg = raw.BaseFare.Value;
                }
            }

            return rawData;
        }

        public IList<FinalData> MapRawToFinalData(IList<RawData> rawData, bool accountBreak, ClientFunctions clientFunctions, ReportGlobals globals, 
            UserBreaks userBreaks, IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, 
            IMasterDataStore store, IClientDataStore clientStore)
        {
            return rawData.Select(s => new FinalData
            {
                Reckey = s.RecKey,
                Recloc = s.Recloc.Trim(),
                Invoice = s.Invoice.Trim(),
                Ticket = s.Ticket.Trim(),
                Acct = !accountBreak ? Constants.NotApplicable : s.Acct,
                Acctdesc = !accountBreak ? Constants.NotApplicable : clientFunctions.LookupCname(getAllMasterAccountsQuery, s.Acct, globals),
                Break1 = !userBreaks.UserBreak1
                                ? Constants.NotApplicable
                                : string.IsNullOrEmpty(s.Break1.Trim()) ? "NONE".PadRight(30) : s.Break1.Trim(),
                Break2 = !userBreaks.UserBreak2
                                ? Constants.NotApplicable
                                : string.IsNullOrEmpty(s.Break2.Trim()) ? "NONE".PadRight(30) : s.Break2.Trim(),
                Break3 = !userBreaks.UserBreak3
                                ? Constants.NotApplicable
                                : string.IsNullOrEmpty(s.Break3.Trim()) ? "NONE".PadRight(16) : s.Break3.Trim(),
                Passlast = s.Passlast,
                Passfrst = s.Passfrst,
                Reascode = s.Reascode,
                Reasdesc = clientFunctions.LookupReason(getAllMasterAccountsQuery, s.Reascode, s.Acct, clientStore, globals, store.MastersQueryDb),
                Airchg = s.Airchg.ToDecimalSafe(),
                Offrdchg = (s.Offrdchg > 0 && s.Airchg < 0)
                                ? 0 - s.Offrdchg.ToDecimalSafe()
                                : s.Offrdchg.ToDecimalSafe() == 0
                                        ? s.Airchg.ToDecimalSafe()
                                        : s.Offrdchg.ToDecimalSafe(),
                Depdate = s.Depdate.ToDateTimeSafe(),
                Origin = s.Origin,
                Orgdesc = AportLookup.LookupAport(store, s.Origin, s.Mode, globals.Agency),
                Destinat = s.Destinat,
                Destdesc = AportLookup.LookupAport(store, s.Destinat, s.Mode, globals.Agency),
                Connect = s.Connect,
                Rdepdate = s.Rdepdate.ToDateTimeSafe(),
                Airline = s.Airline,
                Fltno = s.Fltno,
                Carrdesc = LookupFunctions.LookupAline(store, s.Airline, s.Mode),
                Class = s.Class,
                Seqno = s.SeqNo,
                Lostamt = s.Airchg.ToDecimalSafe() - s.Offrdchg.ToDecimalSafe(),
            }).Where(x => Math.Abs(x.Airchg.ToDecimalSafe()) > Math.Abs(x.Offrdchg.ToDecimalSafe()))
              .OrderBy(x => x.Acctdesc)
              .ThenBy(x => x.Break1)
              .ThenBy(x => x.Break2)
              .ThenBy(x => x.Break3)
              .ThenBy(x => x.Passlast)
              .ThenBy(x => x.Passfrst)
              .ThenBy(x => x.Reasdesc)
              .ThenBy(x => x.Reckey)
              .ThenBy(x => x.Seqno)
              .ToList();
        }
    }
}
