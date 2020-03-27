using Domain.Models.ReportPrograms.TripChangesMeetAndGreetReport;
using Domain.Orm.Classes;
using Domain.Orm.iBankMastersQueries;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.TripChanges.TripChangesMeetAndGreet
{
    public class MeetGreetFinalDataProcessor
    {
        public List<FinalData> GetFinalData(List<RawData> RawDataList, bool UseAirportCodes, IMasterDataStore MasterStore, ReportGlobals globals)
        {
            var FinalDataList = RawDataList.Select(s => new FinalData
            {
                Reckey = s.RecKey,
                Mtggrpnbr = s.Mtggrpnbr,
                Acct = s.Acct,
                Passlast = s.Passlast,
                Passfrst = s.Passfrst,
                Destinat = s.Destinat,
                Emailaddr = s.Emailaddr,
                Destdesc = UseAirportCodes ? s.Destinat : AportLookup.LookupAport(MasterStore, s.Destinat, string.Empty, globals.Agency),
                Origdesc = UseAirportCodes ? s.Origin : AportLookup.LookupAport(MasterStore, s.Origin, string.Empty, globals.Agency),
                Lastorgdes = UseAirportCodes ? s.LastOrigin : AportLookup.LookupAport(MasterStore, s.LastOrigin, string.Empty, globals.Agency),
                Rarrdate = s.RArrDate ?? DateTime.MinValue,
                Alinedesc = LookupFunctions.LookupAline(MasterStore, s.Airline),
                Fltno = s.fltno,
                Arrtime = SharedProcedures.ConvertTime(s.Arrtime),
                Sorttime = s.Arrtime,
                Deptime = SharedProcedures.ConvertTime(s.DepTime),
                Bookdate = s.Bookdate ?? DateTime.MinValue,
                Recloc = s.Recloc,
                Ticket = s.Ticket,
                Airline = s.Airline,
                SegNum = s.SegNum
            }).ToList();

            return FinalDataList;
        }

        public List<FinalData> GetCancelledFinalData(List<RawData> CancelledRawDataList, bool UseAirportCodes, IMasterDataStore MasterStore, bool SuppressChangeDetails, ReportGlobals globals)
        {
            var CancelledFinalDataList = CancelledRawDataList.Select(s => new FinalData
            {
                Reckey = s.RecKey,
                SegNum = s.SegNum,
                Mtggrpnbr = s.Mtggrpnbr,
                Acct = s.Acct,
                Passlast = s.Passlast,
                Passfrst = s.Passfrst,
                Destinat = s.Destinat,
                Emailaddr = s.Emailaddr,
                Destdesc = UseAirportCodes ? s.Destinat : AportLookup.LookupAport(MasterStore, s.Destinat, string.Empty, globals.Agency),
                Origdesc = UseAirportCodes ? s.Origin : AportLookup.LookupAport(MasterStore, s.Origin, string.Empty, globals.Agency),
                Lastorgdes = UseAirportCodes ? s.LastOrigin : AportLookup.LookupAport(MasterStore, s.LastOrigin, string.Empty, globals.Agency),
                Rarrdate = s.RArrDate ?? DateTime.MinValue,
                Alinedesc = LookupFunctions.LookupAline(MasterStore, s.Airline),
                Fltno = s.fltno,
                Arrtime = SharedProcedures.ConvertTime(s.Arrtime),
                Sorttime = s.Arrtime,
                Deptime = SharedProcedures.ConvertTime(s.DepTime),
                Changedesc = SuppressChangeDetails ? "Cancelled" : "CANCELLED",
                Bookdate = s.Bookdate ?? DateTime.MinValue,
                Recloc = s.Recloc,
                Changstamp = s.ChangStamp ?? DateTime.MinValue,
                Ticketed = string.IsNullOrEmpty(s.Ticket) ? "N" : "Y",
                Airline = s.Airline
            }).ToList();

            return CancelledFinalDataList;
        }

        public List<FinalData> GetTripsWithChanges(List<FinalData> FinalDataList, List<ChangesData> changes)
        {
            var tripsWithChanges = (from f in FinalDataList
                                    join c in changes on f.Reckey equals c.Reckey
                                    where (f.SegNum == c.SegNum || c.SegNum == 0)
                                    select new FinalData
                                    {
                                        Reckey = f.Reckey,
                                        Mtggrpnbr = f.Mtggrpnbr,
                                        Passlast = f.Passlast,
                                        Passfrst = f.Passfrst,
                                        Destinat = f.Destinat,
                                        Destdesc = f.Destdesc,
                                        Emailaddr = f.Emailaddr,
                                        Origdesc = f.Origdesc,
                                        Lastorgdes = f.Lastorgdes,
                                        Rarrdate = f.Rarrdate,
                                        Alinedesc = f.Alinedesc,
                                        Fltno = f.Fltno,
                                        Arrtime = f.Arrtime,
                                        Sorttime = f.Sorttime,
                                        Deptime = f.Deptime,
                                        Recloc = f.Recloc,
                                        Ticketed = string.IsNullOrEmpty(f.Ticket) || string.IsNullOrEmpty(f.Ticket.Trim()) ? "N" : "Y",
                                        Changedesc = c.ChangeDesc,
                                        Changstamp = c.ChangStamp ?? DateTime.MinValue,
                                        Airline = f.Airline,
                                        Bookdate = f.Bookdate
                                    }).ToList();

            return tripsWithChanges;
        }

        public List<FinalData> GetTripsWithNoChanges(List<FinalData> tripsWithChanges, List<FinalData> FinalDataList, bool datesPresent, 
            DateTime? BeginDate2, bool SuppressChangeDetails, string blankString)
        {
            var tripsWithNoChanges =
                FinalDataList.Where(s => tripsWithChanges.All(t => t.Reckey != s.Reckey)).Select(f => new FinalData
                {
                    Reckey = f.Reckey,
                    Mtggrpnbr = f.Mtggrpnbr,
                    Passlast = f.Passlast,
                    Passfrst = f.Passfrst,
                    Destinat = f.Destinat,
                    Destdesc = f.Destdesc,
                    Emailaddr = f.Emailaddr,
                    Origdesc = f.Origdesc,
                    Lastorgdes = f.Lastorgdes,
                    Rarrdate = f.Rarrdate,
                    Alinedesc = f.Alinedesc,
                    Fltno = f.Fltno,
                    Arrtime = f.Arrtime,
                    Sorttime = f.Sorttime,
                    Deptime = f.Deptime,
                    Recloc = f.Recloc,
                    Ticketed = string.IsNullOrEmpty(f.Ticket) || string.IsNullOrEmpty(f.Ticket.Trim()) ? "N" : "Y",
                    Changedesc = datesPresent
                                    ? f.Bookdate >= BeginDate2.Value
                                            ? SuppressChangeDetails
                                                ? "New"
                                                : "NEW"
                                            : blankString
                                    : blankString,
                    Airline = f.Airline,
                    Bookdate = f.Bookdate
                }).ToList();

            return tripsWithNoChanges;
        }

        public List<ChangesData> GetUpdatedChanges(bool SuppressChangeDetails, List<ChangesData> changes, List<TripChangeCodeInformation> changeCodes, ReportGlobals Globals, IMasterDataStore MasterStore)
        {
            if (SuppressChangeDetails)
            {
                changes = changes.Join(changeCodes, o => o.ChangeCode, i => i.ChangeCode, (o, i) => new { o, i })
                    .Where(
                        s =>
                            s.i.Priority == 1 &&
                            (s.i.ChangeGroup.EqualsIgnoreCase("T") || s.i.ChangeGroup.EqualsIgnoreCase("S")))
                    .Select(s => new { s.o.Reckey, s.o.SegNum, s.o.ChangStamp })
                    .GroupBy(t => new { t.Reckey, t.SegNum }, (k, g) => new ChangesData { Reckey = k.Reckey, SegNum = k.SegNum, ChangStamp = g.Max(m => m.ChangStamp), ChangeDesc = "Changed   " }).ToList();
            }
            else
            {
                changes = changes.Join(changeCodes, o => o.ChangeCode, i => i.ChangeCode, (o, i) => new { o, i })
                    .Where(
                        s =>
                            s.i.Priority == 1 &&
                            (s.i.ChangeGroup.EqualsIgnoreCase("T") || s.i.ChangeGroup.EqualsIgnoreCase("S")))
                    .Select(s => new ChangesData
                    {
                        Reckey = s.o.Reckey,
                        SegNum = s.o.SegNum,
                        ChangStamp = s.o.ChangStamp,
                        ChangeFrom = s.o.ChangeFrom,
                        ChangeTo = s.o.ChangeTo,
                        ChangeDesc = LookupFunctions.LookupChangeDescirption(s.o.ChangeCode, s.i.CodeDescription, s.o.ChangeFrom, s.o.ChangeTo, s.o.PriorItin, Globals, MasterStore)
                    }).OrderBy(s => s.Reckey).ToList();
            }
            return changes;
        }
    }
}
