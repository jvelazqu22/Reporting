using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CODE.Framework.Core.Utilities;
using Domain.Helper;
using Domain.Models.ReportPrograms.AdvanceBookAir;
using Domain.Orm.iBankClientQueries;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.Client;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;

namespace iBank.Services.Implementation.ReportPrograms.AdvanceBookAir
{
    public class AdvanceBookAirDataProcessor
    {
        public IList<FinalData> MapRawToFinalData(IList<RawData> rawData, bool isReservationReport, bool accountBreak, ClientFunctions clientFunctions, UserBreaks userBreaks,
            string agency, ReportGlobals globals, IClientDataStore clientStore, IMasterDataStore masterStore)
        {
            return rawData.Select(s => new FinalData
            {
                RecKey = s.RecKey,
                Acct = s.Acct,
                Acctdesc = !accountBreak ? Constants.NotApplicable : clientFunctions.LookupCname(new GetAllMasterAccountsQuery(clientStore.ClientQueryDb, agency), s.Acct, globals),
                Break1 = !userBreaks.UserBreak1
                                ? Constants.NotApplicable
                                : string.IsNullOrEmpty(s.Break1.Trim()) ? "NONE".PadRight(30) : s.Break1,
                Break2 = !userBreaks.UserBreak2
                                ? Constants.NotApplicable
                                : string.IsNullOrEmpty(s.Break2.Trim()) ? "NONE".PadRight(30) : s.Break2,
                Break3 = !userBreaks.UserBreak3
                                ? Constants.NotApplicable
                                : string.IsNullOrEmpty(s.Break3.Trim()) ? "NONE".PadRight(30) : s.Break3,
                Seqno = s.SeqNo,
                Bookdate = isReservationReport
                                ? s.Bookdate.ToDateTimeSafe()
                                : s.Invdate.ToDateTimeSafe(),
                Depdate = s.DepDate.ToDateTimeSafe(),
                Passfrst = s.Passfrst,
                Passlast = s.Passlast,
                Origin = ReportHelper.CreateOriginDestCode(s.Origin, s.Mode, s.Airline),
                Orgdesc = AportLookup.LookupAport(new MasterDataStore(), s.Origin, s.Mode, s.Airline),
                Destinat = ReportHelper.CreateOriginDestCode(s.Destinat, s.Mode, s.Airline),
                Destdesc = AportLookup.LookupAport(new MasterDataStore(), s.Destinat, s.Mode, s.Airline),
                Airline = s.Airline,
                Airchg = s.Airchg ?? 0,
                Actfare = s.ActFare,
                Rdepdate = s.RDepDate.ToDateTimeSafe(),
                ClassCode = s.ClassCode,
                Fltno = s.fltno,
                Bookdays = isReservationReport
                                ? (s.DepDate.ToDateTimeSafe() - s.Bookdate.ToDateTimeSafe()).Days
                                : (s.DepDate.ToDateTimeSafe() - s.Invdate.ToDateTimeSafe()).Days,
                Bookcat = "A",
                Catdesc = "0-2     ",
                Alinedesc = LookupFunctions.LookupAline(masterStore, s.Airline, s.Mode)
            }).OrderBy(x => x.Acct)
                .ThenBy(x => x.Break1)
                .ThenBy(x => x.Break2)
                .ThenBy(x => x.Break3)
                .ThenBy(x => x.Bookdays)
                .ThenBy(x => x.Passlast)
                .ThenBy(x => x.Passfrst)
                .ThenBy(x => x.RecKey)
                .ThenBy(x => x.Seqno)
                .ToList();
        }

        public IList<FinalData> ZeroOutFields(IList<FinalData> finalData)
        {
            return ZeroOut<FinalData>.Process(finalData.ToList(), new List<string> { "airchg" });
        }

        public IList<FinalData> AddDataFromTimeBuckets(IList<FinalData> finalData, IList<TimeBucket> buckets)
        {
            for (int i = 0; i < 4; i++)
            {
                finalData.Where(s => s.Bookdays >= buckets[i].StartDay && s.Bookdays <= buckets[i].EndDay).ToList().ForEach(s =>
                {
                    s.Bookcat = "ABCD"[i].ToString(CultureInfo.InvariantCulture);
                    s.Catdesc = string.Format("{0}-{1}", buckets[i].StartDay, buckets[i].EndDay);
                });
            }

            finalData.Where(s => s.Bookdays > buckets[3].EndDay).ToList().ForEach(s =>
            {
                s.Bookcat = "E";
                s.Catdesc = string.Format("{0}+", buckets[3].EndDay);
            });

            return finalData;
        }
    }
}
