using Domain.Helper;
using Domain.Models.ReportPrograms.TripChangesSendOffReport;
using Domain.Orm.Classes;

using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;

namespace iBank.Services.Implementation.ReportPrograms.TripChanges.TripChangesSendOff
{
    public class SendOffDataProcessor
    {
        public List<FinalData> MapRawToFinalData(List<RawData> rawData, bool useAirportCodes, IMasterDataStore store, ReportGlobals globals)
        {
            return rawData.Select(s => new FinalData
            {
                Reckey = s.RecKey,
                Mtggrpnbr = s.Mtggrpnbr,
                Acct = s.Acct,
                Passlast = s.Passlast,
                Passfrst = s.Passfrst,
                Origin = s.Origin,
                Destinat = s.Destinat,
                Emailaddr = s.Emailaddr,
                Destdesc = useAirportCodes ? s.Destinat : AportLookup.LookupAport(store, s.Destinat, string.Empty, globals.Agency),
                Origdesc = useAirportCodes ? s.Origin : AportLookup.LookupAport(store, s.Origin, string.Empty, globals.Agency),
                Fstdestdes = useAirportCodes ? s.FirstDest : AportLookup.LookupAport(store, s.FirstDest, string.Empty, globals.Agency),
                RDepDate = s.RDepDate ?? DateTime.MinValue,
                Alinedesc = LookupFunctions.LookupAline(store, s.Airline),
                Fltno = s.fltno,
                Arrtime = SharedProcedures.ConvertTime(s.Arrtime),
                Sorttime = s.DepTime,
                Deptime = SharedProcedures.ConvertTime(s.DepTime),
                Bookdate = s.Bookdate ?? DateTime.MinValue,
                Recloc = s.Recloc,
                Ticket = s.Ticket,
                Airline = s.Airline,
                SegNum = s.SegNum
            }).ToList();
        }

        public List<FinalData> MapRawCancelledDataToFinalCancelledData(List<RawData> rawData, bool useAirportCodes, bool suppressChangeDetails, IMasterDataStore store, ReportGlobals globals)
        {
            return rawData.Select(s => new FinalData
            {
                Reckey = s.RecKey,
                SegNum = s.SegNum,
                Mtggrpnbr = s.Mtggrpnbr,
                Acct = s.Acct,
                Passlast = s.Passlast,
                Passfrst = s.Passfrst,
                Origin = s.Origin,
                Destinat = s.Destinat,
                Emailaddr = s.Emailaddr,
                Destdesc = useAirportCodes ? s.Destinat : AportLookup.LookupAport(store, s.Destinat, string.Empty, globals.Agency),
                Origdesc = useAirportCodes ? s.Origin : AportLookup.LookupAport(store, s.Origin, string.Empty, globals.Agency),
                Fstdestdes = useAirportCodes ? s.FirstDest : AportLookup.LookupAport(store, s.FirstDest, string.Empty, globals.Agency),
                RDepDate = s.RDepDate ?? DateTime.MinValue,
                Alinedesc = LookupFunctions.LookupAline(store, s.Airline),
                Fltno = s.fltno,
                Arrtime = SharedProcedures.ConvertTime(s.Arrtime),
                Sorttime = s.DepTime,
                Deptime = SharedProcedures.ConvertTime(s.DepTime),
                Changedesc = suppressChangeDetails ? "Cancelled" : "CANCELLED",
                Bookdate = s.Bookdate ?? DateTime.MinValue,
                Recloc = s.Recloc,
                Changstamp = s.ChangStamp ?? DateTime.MinValue,
                Ticketed = string.IsNullOrEmpty(s.Ticket) ? "N" : "Y",
                Airline = s.Airline
            }).ToList();
        }

        public List<ChangesData> CombineChangesAndChangeCodes(List<ChangesData> changes, IEnumerable<TripChangeCodeInformation> changeCodes, bool suppressChangeDetails, ReportGlobals globals)
        {
            if (suppressChangeDetails)
            {
                return changes.Join(changeCodes, o => o.ChangeCode, i => i.ChangeCode, (o, i) => new { o, i })
                    .Where(s => s.i.Priority == 1
                                && (s.i.ChangeGroup.EqualsIgnoreCase("T") || s.i.ChangeGroup.EqualsIgnoreCase("S")))
                    .Select(s => new { s.o.RecKey, s.o.SegNum, s.o.ChangStamp })
                    .GroupBy(t => new { t.RecKey, t.SegNum }, (k, g) => new ChangesData { RecKey = k.RecKey, SegNum = k.SegNum, ChangStamp = g.Max(m => m.ChangStamp), ChangeDesc = "Changed   " }).ToList();
            }
            else
            {
                return changes.Join(changeCodes, changeData => changeData.ChangeCode, changeCodeInfo => changeCodeInfo.ChangeCode, (changeData, changeCode) =>
                new { changeData, changeCode })
                    .Where(s => s.changeCode.Priority == 1
                                && (s.changeCode.ChangeGroup.EqualsIgnoreCase("T") || s.changeCode.ChangeGroup.EqualsIgnoreCase("S")))
                    .Select(s => new ChangesData
                    {
                        RecKey = s.changeData.RecKey,
                        SegNum = s.changeData.SegNum,
                        ChangStamp = s.changeData.ChangStamp,
                        ChangeFrom = s.changeData.ChangeFrom,
                        ChangeTo = s.changeData.ChangeTo,
                        ChangeDesc = LookupFunctions.LookupChangeDescirption(s.changeData.ChangeCode, s.changeCode.CodeDescription, s.changeData.ChangeFrom, s.changeData.ChangeTo, s.changeData.PriorItin, globals, new MasterDataStore())
                    }).OrderBy(s => s.RecKey).ToList();
            }
        }

        public List<FinalData> GetTripsWithChanges(List<FinalData> finalData, List<ChangesData> changes)
        {
            return (from f in finalData
                    join c in changes on f.Reckey equals c.RecKey
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
                        Origin = f.Origin,
                        Origdesc = f.Origdesc,
                        Fstdestdes = f.Fstdestdes,
                        RDepDate = f.RDepDate,
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
        }

        public List<FinalData> GetTripsWithoutChanges(List<FinalData> finalData, List<FinalData> tripsWithChanges, bool datesPresent, DateTime? beginDateTwo, bool suppressChangeDetails)
        {
            var blankString = new string(' ', 8);
            return finalData.Where(s => tripsWithChanges.All(t => t.Reckey != s.Reckey)).Select(f => new FinalData
            {
                Reckey = f.Reckey,
                Mtggrpnbr = f.Mtggrpnbr,
                Passlast = f.Passlast,
                Passfrst = f.Passfrst,
                Destinat = f.Destinat,
                Destdesc = f.Destdesc,
                Emailaddr = f.Emailaddr,
                Origin = f.Origin,
                Origdesc = f.Origdesc,
                Fstdestdes = f.Fstdestdes,
                RDepDate = f.RDepDate,
                Alinedesc = f.Alinedesc,
                Fltno = f.Fltno,
                Arrtime = f.Arrtime,
                Sorttime = f.Sorttime,
                Deptime = f.Deptime,
                Recloc = f.Recloc,
                Ticketed = string.IsNullOrEmpty(f.Ticket?.Trim()) ? "N" : "Y",
                Changedesc = datesPresent
                                        ? f.Bookdate >= beginDateTwo.Value
                                            ? suppressChangeDetails ? "New" : "NEW"
                                            : blankString
                                        : blankString,
                Airline = f.Airline,
                Bookdate = f.Bookdate
            }).ToList();
        }

        public List<FinalData> SuppressChangeDetails(List<FinalData> finalData)
        {
            return finalData.GroupBy(
                        s =>
                            new
                            {
                                s.Reckey,
                                s.Mtggrpnbr,
                                s.Passlast,
                                s.Passfrst,
                                s.Emailaddr,
                                s.Destinat,
                                s.Destdesc,
                                s.Origdesc,
                                s.Origin,
                                s.Fstdestdes,
                                s.RDepDate,
                                s.Alinedesc,
                                s.Airline,
                                s.Fltno,
                                s.Arrtime,
                                s.Deptime,
                                s.Sorttime,
                                s.Recloc,
                                s.Ticketed,
                                s.Bookdate
                            },
                        (k, g) => new FinalData
                        {
                            Reckey = k.Reckey,
                            Mtggrpnbr = k.Mtggrpnbr,
                            Passlast = k.Passlast,
                            Passfrst = k.Passfrst,
                            Emailaddr = k.Emailaddr,
                            Destinat = k.Destinat,
                            Destdesc = k.Destdesc,
                            Origdesc = k.Origdesc,
                            Origin = k.Origin,
                            RDepDate = k.RDepDate,
                            Fstdestdes = k.Fstdestdes,
                            Airline = k.Airline,
                            Alinedesc = k.Alinedesc,
                            Fltno = k.Fltno,
                            Arrtime = k.Arrtime,
                            Deptime = k.Deptime,
                            Sorttime = k.Sorttime,
                            Recloc = k.Recloc,
                            Ticketed = k.Ticketed,
                            Bookdate = k.Bookdate,
                            Changedesc = g.Min(t => t.Changedesc),
                            Changstamp = g.Max(t => t.Changstamp)
                        }).ToList();
        }

        public List<FinalData> SortFinalData(List<FinalData> finalData, ReportGlobals globals)
        {
            var sortBy = globals.GetParmValue(WhereCriteria.SORTBY);
            switch (sortBy)
            {
                case "2":
                    // Departure Time
                    return finalData.OrderBy(s => s.Origdesc)
                        .ThenBy(s => s.RDepDate)
                        .ThenBy(s => s.Sorttime)
                        .ThenBy(s => s.Airline)
                        .ThenBy(s => s.Alinedesc)
                        .ThenBy(s => s.Fltno)
                        .ThenBy(s => s.Passlast)
                        .ThenBy(s => s.Passfrst)
                        .ThenBy(s => s.Changstamp).ToList();
                case "3":
                    // Passenger Name
                    return finalData.OrderBy(s => s.Origdesc)
                        .ThenBy(s => s.RDepDate)
                        .ThenBy(s => s.Passlast)
                        .ThenBy(s => s.Passfrst)
                        .ThenBy(s => s.Sorttime)
                        .ThenBy(s => s.Airline)
                        .ThenBy(s => s.Alinedesc)
                        .ThenBy(s => s.Fltno)
                        .ThenBy(s => s.Changstamp).ToList();
                default:
                    // Airline
                    return finalData.OrderBy(s => s.Origdesc)
                        .ThenBy(s => s.RDepDate)
                        .ThenBy(s => s.Airline)
                        .ThenBy(s => s.Alinedesc)
                        .ThenBy(s => s.Arrtime)
                        .ThenBy(s => s.Fltno)
                        .ThenBy(s => s.Passlast)
                        .ThenBy(s => s.Passfrst)
                        .ThenBy(s => s.Changstamp).ToList();
            }
        }

        public List<FinalData> SetUdids(List<FinalData> finalData, List<UdidData> udidNumberOneData, List<UdidData> udidNumberTwoData, int udidNumberOne,
                                        int udidNumberTwo)
        {
            var calc = new SendOffCalculations();
            finalData.ForEach(s =>
            {
                s.Udidnbr1 = udidNumberOne;
                s.Udidtext1 = calc.GetUdidDescription(s.Reckey, udidNumberOneData, udidNumberOne);
                s.Udidnbr2 = udidNumberTwo;
                s.Udidtext2 = calc.GetUdidDescription(s.Reckey, udidNumberTwoData, udidNumberTwo);
            });

            return finalData;
        }

        public List<RawData> GetDataWithArrivalBetweenDates(List<RawData> rawData, DateTime beginDate, DateTime endDate)
        {
            return rawData.Where(s => s.RArrDate >= beginDate && s.RArrDate <= endDate).ToList();
        }

        public List<FinalDataExport> MapFinalDataToFinalDataExportWithoutConsolidatingChanges(List<FinalData> finalData)
        {
            return finalData.Select(s => new FinalDataExport
            {
                Reckey = s.Reckey,
                Mtggrpnbr = s.Mtggrpnbr,
                Passlast = s.Passlast,
                Passfrst = s.Passfrst,
                Emailaddr = s.Emailaddr,
                Destinat = s.Destinat,
                Destdesc = s.Destdesc,
                Origdesc = s.Origdesc,
                Origin = s.Origin,
                Fstdestdes = s.Fstdestdes,
                RDepDate = s.RDepDate,
                Airline = s.Airline,
                Alinedesc = s.Alinedesc,
                Fltno = s.Fltno,
                Arrtime = s.Arrtime,
                Deptime = s.Deptime,
                Sorttime = s.Sorttime,
                Recloc = s.Recloc,
                Ticketed = s.Ticketed,
                Bookdate = s.Bookdate,
                Changedesc = s.Changedesc,
                Changstamp = s.Changstamp.ToString(CultureInfo.InvariantCulture),
                Udidnbr1 = s.Udidnbr1,
                Udidtext1 = s.Udidtext1,
                Udidnbr2 = s.Udidnbr2,
                Udidtext2 = s.Udidtext2,
            }).ToList();
        }

        public List<FinalDataExport> MapFinalDataToFinalDataExportConsolidateChanges(List<FinalData> finalData)
        {
            return finalData.Select(
                    s =>
                        new
                        {
                            s.Reckey,
                            s.Mtggrpnbr,
                            s.Passlast,
                            s.Passfrst,
                            s.Emailaddr,
                            s.Destinat,
                            s.Origdesc,
                            s.Origin,
                            s.RDepDate,
                            s.Fstdestdes,
                            s.Destdesc,
                            s.Airline,
                            s.Alinedesc,
                            s.Fltno,
                            s.Arrtime,
                            s.Deptime,
                            s.Sorttime,
                            s.Recloc,
                            s.Ticketed,
                            s.Bookdate,
                            s.Udidnbr1,
                            s.Udidtext1,
                            s.Udidnbr2,
                            s.Udidtext2
                        }).Distinct()
                    .Select(t => new FinalDataExport
                    {
                        Reckey = t.Reckey,
                        Mtggrpnbr = t.Mtggrpnbr,
                        Passlast = t.Passlast,
                        Passfrst = t.Passfrst,
                        Emailaddr = t.Emailaddr,
                        Destinat = t.Destinat,
                        Origdesc = t.Origdesc,
                        Origin = t.Origin,
                        RDepDate = t.RDepDate,
                        Fstdestdes = t.Fstdestdes,
                        Destdesc = t.Destdesc,
                        Airline = t.Airline,
                        Alinedesc = t.Alinedesc,
                        Fltno = t.Fltno,
                        Arrtime = t.Arrtime,
                        Deptime = t.Deptime,
                        Sorttime = t.Sorttime,
                        Recloc = t.Recloc,
                        Ticketed = t.Ticketed,
                        Bookdate = t.Bookdate,
                        Udidnbr1 = t.Udidnbr1,
                        Udidnbr2 = t.Udidnbr2,
                        Udidtext1 = t.Udidtext1,
                        Udidtext2 = t.Udidtext2
                    })
                    .OrderBy(s => s.Reckey)
                    .ThenBy(s => s.RDepDate)
                    .ThenBy(s => s.Airline)
                    .ThenBy(s => s.Arrtime)
                    .ThenBy(s => s.Fltno)
                    .ThenBy(s => s.Changstamp).ToList();
        }

        public List<FinalData> GetMatchingFinalDataRecords(FinalDataExport exportData, List<FinalData> finalData)
        {
            return finalData.Where(s =>
                                    s.Reckey == exportData.Reckey && s.Passlast == exportData.Passlast && s.Destdesc == exportData.Destdesc &&
                                    s.Origin == exportData.Origin && s.Airline == exportData.Airline && s.Fltno == exportData.Fltno).ToList();
        }
    }
}
