using Domain.Helper;
using Domain.Models.ReportPrograms.ConcurrentSegmentsBookedReport;
using Domain.Orm.iBankClientQueries;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using iBank.Services.Implementation.Shared.Client;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;

namespace iBank.Services.Implementation.ReportPrograms.ConcurrentSegmentsBooked
{
    public class OverlapFinalDataProcessor
    {
        private List<GroupedRecsByRecKeyWhereSegsIsLessThanThree> GetGroupedRecsByRecKeyWhereSegsIsLessThanThree(List<RawData> rawDataList)
        {
            return rawDataList.GroupBy(s => s.RecKey, (key, recs) =>
            {
                var recList = recs.ToList();
                return new GroupedRecsByRecKeyWhereSegsIsLessThanThree()
                {
                    RecKey = key,
                    FirstDate = recList.Min(s => s.RDepDate),
                    LastDate = recList.Max(s => s.RDepDate),
                    Segs = recList.Count
                };
            }).Where(s => s.Segs < 3).ToList();
        }

        private List<RecsFromRawDataListWhereSegsIsLessThanThree> GetRecsFromRawDataListWhereSegsIsLessThanThree(List<RawData> rawDataList, List<GroupedRecsByRecKeyWhereSegsIsLessThanThree> groupedRecsByRecKeyWhereSegsIsLessThanThree)
        {
            return rawDataList.Join(groupedRecsByRecKeyWhereSegsIsLessThanThree, r => r.RecKey, o => o.RecKey, (r, o) =>
            new RecsFromRawDataListWhereSegsIsLessThanThree()
            {
                RecKey = r.RecKey,
                RecLoc = r.RecLoc,
                Ticket = r.Ticket,
                Acct = r.Acct,
                Passlast = r.Passlast,
                Passfrst = r.Passfrst,
                Bktool = r.Bktool,
                Bookdate = r.Bookdate,
                Gds = r.Gds,
                Airchg = r.Airchg,
                FirstDate = o.FirstDate,
                LastDate = o.LastDate,
                Airline = r.Airline,
                RDepDate = r.RDepDate,
                RArrDate = r.RArrDate,
                Origin = r.Origin,
                Destinat = r.Destinat,
                SeqNo = r.SeqNo,
                ClassCode = r.ClassCode,
                Segs = o.Segs
            }).OrderBy(s => s.RecKey).ToList();
        }

        private List<TempData> GetTempRecsFromRecsFromRawDataListWhereSegsIsLessThanThree(List<RecsFromRawDataListWhereSegsIsLessThanThree> recsFromRawDataListWhereSegsIsLessThanThree)
        {
            return recsFromRawDataListWhereSegsIsLessThanThree.GroupBy(s => s.RecKey, (reckey, recs) =>
            {
                var reclist = recs.ToList();
                var firstRec = reclist.First();
                var lastRec = reclist.Last();
                return new TempData
                {
                    RecKey = reckey,
                    RecLoc = lastRec.RecLoc,
                    Ticket = lastRec.Ticket,
                    Acct = lastRec.Acct,
                    Passlast = lastRec.Passlast,
                    Passfrst = lastRec.Passfrst,
                    BkTool = lastRec.Bktool,
                    Bookdate = lastRec.Bookdate,
                    Gds = lastRec.Gds,
                    Airchg = lastRec.Airchg,
                    Origin = firstRec.Origin,
                    Destinat = firstRec.Destinat,
                    DepartDate = firstRec.RDepDate,
                    MidDate = lastRec.RDepDate,
                    ReturnDate = lastRec.RArrDate,
                    Airline = lastRec.Airline,
                    Class = lastRec.ClassCode
                };
            }).ToList();
        }

        private void SearchOverlap()
        {

        }

        public List<FinalData> GetFinalData(List<RawData> rawDataList, ReportGlobals globals, ClientFunctions clientFunctions, IMasterDataStore masterStore)
        {
            List<FinalData> FinalDataList = new List<FinalData>();
            var groupedRecsByRecKeyWhereSegsIsLessThanThree = GetGroupedRecsByRecKeyWhereSegsIsLessThanThree(rawDataList);

            var recsFromRawDataListWhereSegsIsLessThanThree = GetRecsFromRawDataListWhereSegsIsLessThanThree(rawDataList, groupedRecsByRecKeyWhereSegsIsLessThanThree);

            var tempList = GetTempRecsFromRecsFromRawDataListWhereSegsIsLessThanThree(recsFromRawDataListWhereSegsIsLessThanThree);

            var tempCopyList = tempList.ToList();
            //search for overlap
            var anyConcurrentSegs = globals.ParmValueEquals(WhereCriteria.DDCONCURRENTSEGS, "2");
            var matchCount = 1;
            foreach (var row in tempList)
            {
                var matches = anyConcurrentSegs
                    ? tempCopyList.Where( //ANY CONCURRENT SEGMENTS.
                        s => s.Passlast.EqualsIgnoreCase(row.Passlast) && s.Passfrst.EqualsIgnoreCase(row.Passfrst)
                             && s.Airline.EqualsIgnoreCase(row.Airline) && s.Acct.EqualsIgnoreCase(row.Acct)
                             && s.MatchNo == 0
                             && ((s.DepartDate >= row.DepartDate && s.DepartDate <= row.ReturnDate) || (s.ReturnDate >= row.DepartDate && s.ReturnDate <= row.ReturnDate))
                             ).ToList()
                        : tempCopyList.Where(//CONCURRENT / SAME CITY PAIR ONLY.
                        s => s.Passlast.EqualsIgnoreCase(row.Passlast) && s.Passfrst.EqualsIgnoreCase(row.Passfrst)
                             && s.Airline.EqualsIgnoreCase(row.Airline) && s.Acct.EqualsIgnoreCase(row.Acct)
                             && s.MatchNo == 0
                             && ((s.DepartDate >= row.DepartDate && s.DepartDate <= row.ReturnDate) || (s.ReturnDate >= row.DepartDate && s.ReturnDate <= row.ReturnDate))
                             && ((s.Origin.EqualsIgnoreCase(row.Origin) && s.Destinat.EqualsIgnoreCase(row.Destinat)) || (s.Origin.EqualsIgnoreCase(row.Destinat) && s.Destinat.EqualsIgnoreCase(row.Origin)))
                             ).ToList();

                if (matches.Count > 1)
                {
                    row.MatchNo = matchCount;
                    foreach (var match in matches)
                    {
                        match.MatchNo = matchCount;
                        if (match.DepartDate == row.DepartDate && match.ReturnDate == row.ReturnDate
                            && match.Origin == row.Origin && match.Destinat == row.Destinat && match.Class == row.Class)
                        {
                            //DUPLICATE BOOKING
                            match.MatchType = "D";
                        }
                        else if (match.DepartDate == row.DepartDate && match.ReturnDate == row.ReturnDate
                            && match.Origin == row.Origin && match.Destinat == row.Destinat && match.Class != row.Class)
                        {
                            //DUPLICATE BOOKING WITH DIFFERENT CLASS OF SERVICE.
                            match.MatchType = "C";
                        }
                        else if ((match.Origin == row.Origin && match.Destinat == row.Destinat) || (match.Origin == row.Destinat && match.Destinat == row.Origin))
                        {
                            //VERLAPPING TRIPS WITH SAME CITY PAIR.
                            match.MatchType = "A";
                        }
                        else
                        {
                            //OVERLAPPING TRIPS WITH DIFFERENT CITY PAIRS.
                            match.MatchType = "B";
                        }
                    }
                    matchCount++;
                }
            }

            //**IF THERE ARE MORE THAN 2 TRIPS WITH THE SAME MATCHNO, IT IS POSSIBLE**
            //** THAT THEY COULD HAVE DIFFERENT MATCHTYPES.WE NEED TO SET THEM ALL TO**
            //** THE "WORST" SCENARIO FOR EACH GROUP OF RECORDS WITH THE SAME MATCHNO.  **
            var reconcile = tempCopyList.Where(s => s.MatchNo != 0)
                .GroupBy(s => s.MatchNo, (key, recs) => new
                {
                    MatchNo = key,
                    MatchType = recs.Min(s => s.MatchType)
                }
            );

            foreach (var row in reconcile)
            {
                var matchNo = row.MatchNo;
                foreach (var item in tempCopyList.Where(s => s.MatchNo == matchNo))
                {
                    item.MatchType = row.MatchType;
                }
            }

            var server = globals.AgencyInformation.ServerName;
            var db = globals.AgencyInformation.DatabaseName;

            FinalDataList = SortFinalData(tempCopyList, globals, clientFunctions, masterStore);

            return FinalDataList;
        }

        private List<FinalData> SortFinalData(List<TempData> tempCopyList, ReportGlobals globals, ClientFunctions clientFunctions, IMasterDataStore masterStore)
        {
            var server = globals.AgencyInformation.ServerName;
            var db = globals.AgencyInformation.DatabaseName;

            return tempCopyList.Where(s => s.MatchNo != 0)
                .Select(s => new FinalData
                {
                    Acct = globals.User.AccountBreak ? s.Acct : "^na^",
                    Acctdesc = globals.User.AccountBreak ? clientFunctions.LookupCname(new GetAllMasterAccountsQuery(new iBankClientQueryable(server, db), globals.Agency),
                    s.Acct, globals) : "^na^",
                    Matchtype = s.MatchType,
                    Matchdesc = GetMatchDescription(s.MatchType),
                    Matchno = s.MatchNo,
                    Airline = s.Airline,
                    Passlast = s.Passlast,
                    Passfrst = s.Passfrst,
                    Departdate = s.DepartDate ?? DateTime.MinValue,
                    Middate = s.MidDate ?? DateTime.MinValue,
                    Orgdesc = AportLookup.LookupAport(masterStore, s.Origin, "A", globals.Agency),
                    Destdesc = AportLookup.LookupAport(masterStore, s.Destinat, "A", globals.Agency),
                    Class = s.Class,
                    RecLoc = s.RecLoc,
                    Gdsdesc = GetGds(s.Gds),
                    Ticket = s.Ticket,
                    Bookdate = s.Bookdate ?? DateTime.MinValue,
                    Airchg = s.Airchg
                })
                .OrderBy(s => s.Acctdesc)
                .ThenBy(s => s.Matchtype)
                .ThenBy(s => s.Passlast)
                .ThenBy(s => s.Passfrst)
                .ThenBy(s => s.Airline)
                .ThenBy(s => s.Matchno)
                .ThenBy(s => s.Departdate)
                .ToList();
        }

        private string GetMatchDescription(string matchType)
        {
            switch (matchType)
            {
                case "":
                    return new string(' ', 42);
                case "A":
                    return "Concurrent Segments - Same City Pair".PadRight(42);
                case "B":
                    return "Concurrent Segments - Different City Pair".PadRight(42);
                case "C":
                    return "Dupe Booking w/ change in Class of Svc".PadRight(42);
                case "D":
                    return "Duplicate Booking".PadRight(42);
                default:
                    return "Unknown".PadRight(42);
            }
        }

        private string GetGds(string gds)
        {
            switch (gds.Trim().ToUpper())
            {
                case "":
                    return new string(' ', 10);
                case "SA":
                    return "SABRE".PadRight(10);
                case "WS":
                    return "WORLDSPAN".PadRight(10);
                case "AP":
                    return "APOLLO".PadRight(10);
                case "AM":
                    return "AMADEUS".PadRight(10);
                case "GA":
                    return "GALILEO".PadRight(10);
                default:
                    return gds.PadRight(10);
            }
        }

    }
}
