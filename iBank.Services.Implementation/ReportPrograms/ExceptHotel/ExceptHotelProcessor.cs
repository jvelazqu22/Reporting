using System;

using CODE.Framework.Core.Utilities;
using Domain.Helper;

using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;

using System.Collections.Generic;
using System.Linq;

using Domain.Models.ReportPrograms.ExceptHotelReport;
using Domain.Orm.Classes;

using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using iBank.Services.Implementation.Shared.Client;
using iBankDomain.RepositoryInterfaces;

namespace iBank.Services.Implementation.ReportPrograms.ExceptHotel
{
    public class ExceptHotelProcessor
    {
        public IList<FinalData> MapRawToFinalData(IList<RawData> rawData, bool accountBreak, UserBreaks userBreaks, ClientFunctions clientFunctions,
            IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, ReportGlobals globals, IClientDataStore clientStore, IMasterDataStore masterStore)
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
                Reascodh = s.Reascodh,
                Reasdesc = clientFunctions.LookupReason(getAllMasterAccountsQuery, s.Reascodh, s.Acct, clientStore, globals, masterStore.MastersQueryDb),
                Datein = s.Datein ?? DateTime.MinValue,
                Hotelnam = s.Hotelnam,
                Hotcity = s.Hotcity,
                Hotstate = s.Hotstate,
                Nights = s.Nights,
                Rooms = s.Rooms,
                Roomtype = s.Roomtype,
                Typedesc = LookupFunctions.LookupRoomType(s.Roomtype, globals.UserLanguage, new MasterDataStore()),
                Hplusmin = s.Hplusmin,
                Bookrate = s.Bookrate.ToDecimalSafe(),
                Confirmno = s.Confirmno,
                Hexcprat = (s.Hexcprat.ToDecimalSafe() > 0 && s.Bookrate.ToDecimalSafe() < 0) ? (0 - s.Hexcprat.ToDecimalSafe())
                                                              : (s.Hexcprat.ToDecimalSafe() == 0) ? s.Bookrate.ToDecimalSafe()
                                                                                 : s.Hexcprat.ToDecimalSafe()
            }).OrderBy(x => x.Acctdesc)
              .ThenBy(x => x.Break1)
              .ThenBy(x => x.Break2)
              .ThenBy(x => x.Break3)
              .ThenBy(x => x.Passlast)
              .ThenBy(x => x.Passfrst)
              .ThenBy(x => x.Reasdesc)
              .ThenBy(x => x.Datein)
              .ToList();
        }
    }
}