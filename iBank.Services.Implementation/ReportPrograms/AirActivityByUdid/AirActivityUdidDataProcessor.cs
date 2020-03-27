using System.Collections.Generic;
using System.Linq;

using CODE.Framework.Core.Utilities;

using Domain.Helper;
using Domain.Models.ReportPrograms.AirActivityByUdidReport;
using Domain.Orm.Classes;

using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.Client;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;
using iBankDomain.RepositoryInterfaces;

namespace iBank.Services.Implementation.ReportPrograms.AirActivityByUdid
{
    public class AirActivityUdidDataProcessor
    {
        public IList<FinalData> MapRawToFinalData(IList<RawData> rawData, bool accountBreak, ClientFunctions clientFunctions, UserBreaks userBreaks,
            IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, ReportGlobals globals)
        {
            return rawData.Select(s => new FinalData
            {
                Acct = !accountBreak ? Constants.NotApplicable : s.Acct,
                AcctDesc = !accountBreak ? Constants.NotApplicable : clientFunctions.LookupCname(getAllMasterAccountsQuery, s.Acct, globals),
                Break1 = !userBreaks.UserBreak1 ? Constants.NotApplicable : (string.IsNullOrEmpty(s.Break1?.Trim()) ? "NONE".PadRight(30) : s.Break1?.Trim()),
                Break2 = !userBreaks.UserBreak2 ? Constants.NotApplicable : (string.IsNullOrEmpty(s.Break2?.Trim()) ? "NONE".PadRight(30) : s.Break2?.Trim()),
                Break3 = !userBreaks.UserBreak3 ? Constants.NotApplicable : (string.IsNullOrEmpty(s.Break3?.Trim()) ? "NONE".PadRight(16) : s.Break3?.Trim()),
                Reckey = s.RecKey,
                SeqNo = s.SeqNo,
                Udidtext = s.Udidtext,
                Passlast = s.Passlast,
                Passfrst = s.Passfrst,
                Origin = s.Origin,
                Orgdesc = AportLookup.LookupAport(new MasterDataStore(), s.Origin, s.Mode, globals.Agency),
                Destinat = s.Destinat,
                Rdepdate = s.Rdepdate.ToDateTimeSafe(),
                Destdesc = AportLookup.LookupAport(new MasterDataStore(), s.Destinat, s.Mode, globals.Agency),
                Connect = s.Connect,
                Depdate = s.Depdate.ToDateTimeSafe(),
                Ticket = s.Ticket,
                Airline = string.IsNullOrEmpty(s.OrigCarr?.Trim()) ? s.OrigCarr?.Trim() : s.Airline,
                Fltno = s.Fltno,
                Class = s.ClassCode.Trim(),
                Airchg = s.Airchg ?? 0,
                Trantype = s.Trantype
            })
            .OrderBy(s => s.Acct)
            .ThenBy(s => s.Break1)
            .ThenBy(s => s.Break2)
            .ThenBy(s => s.Break3)
            .ThenBy(s => s.Udidtext)
            .ThenBy(s => s.Passlast)
            .ThenBy(s => s.Passfrst)
            .ThenBy(s => s.Reckey)
            .ThenBy(s => s.SeqNo)
            .ToList();
        }
    }
}
