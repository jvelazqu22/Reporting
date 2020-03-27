using iBank.Server.Utilities.Classes;
using Domain.Models.ReportPrograms.TripChangesAir;
using System.Collections.Generic;
using System.Linq;
using iBank.Services.Implementation.Shared;
using iBank.Repository.SQL.Interfaces;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using System;
using iBank.Services.Implementation.Utilities;
using Domain.Orm.iBankClientQueries;
using iBank.Repository.SQL.Repository;
using iBank.Services.Implementation.Shared.Client;
using iBank.Server.Utilities.Helpers;
using Domain.Helper;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;

namespace iBank.Services.Implementation.ReportPrograms.TripChanges.TripChangesAir
{
    public class FinalDataHandler
    {
        public List<FinalData> GetFinalDataLists(ReportGlobals globals, List<RawData> rawDataList, IMasterDataStore masterStore,
            RawDataParams rawDataParams, BuildWhere buildWhere, GlobalsCalculator globalCalc, ClientFunctions clientFunctions, bool consolidateChanges)
        {
            //TODO: The ApplyWhereRoute function is supposed to return all the legs for the trip that satisfies the routing condition.
            //However in this case it is not returning all the legs, so I had to search the routed raw data again. 

            //Get the filtered list after applying where route
            var routedFilteredList = buildWhere.ApplyWhereRoute(rawDataParams.RoutingRawDataList, globalCalc.IsAppliedToLegLevelData());

            //Get all the legs for the trip
            var routedList = rawDataParams.RoutingRawDataList.Where(t => routedFilteredList.Any(r => r.RecKey == t.RecKey))
            .Select(s => new FinalData
            {
                Reckey = s.RecKey,
                Acct = s.Acct,
                Passlast = s.Passlast,
                Passfrst = s.Passfrst,
                Mtggrpnbr = s.Mtggrpnbr,
                Ticket = s.Ticket,
                Recloc = s.Recloc,
                Bookdate = s.Bookdate ?? DateTime.MinValue,
                Depdate = s.Depdate ?? DateTime.MinValue,
                Airchg = s.Airchg,
                Origin = s.Origin,
                Orgdesc = AportLookup.LookupAport(masterStore, s.Origin, s.Mode, globals.Agency),
                Destinat = s.Destinat,
                Destdesc = AportLookup.LookupAport(masterStore, s.Destinat, s.Mode, globals.Agency),
                Connect = s.Connect,
                Seqno = s.SeqNo,
                Airline = s.Airline,
                Rdepdate = s.RDepDate ?? DateTime.MinValue,
                Fltno = s.Fltno,
                Deptime = s.Deptime,
                Arrtime = s.Arrtime,
                Segnum = s.Segnum,
                Rectype = "A"
            }).OrderBy(s => s.Reckey).ThenBy(s => s.Seqno).ToList();

            /** COMBINE TRIP/ROUTING DATA WITH TRIP/CHANGES DATA.  WE ONLY HAVE TO WORRY **
             ** ABOUT THE INTERSECTION.  TRIPS WITH OUT MATCHING ROUTING MEANS THAT THE  **
             ** TRIP DIDN'T MEET THE ROUTING CRITERIA;  TRIPS WITHOUT MATCHING CHANGES   **
             ** DON'T BELONG ON THE REPORT SINCE WE ARE REPORTING TRIPS WITH CHANGES.    **
             
             ** WE CAN ACCOMPLISH OUR GOAL, AND SHORT-CUT THE PROCESS, **
             ** BY GETTING THE  ROUTING RECORDS THAT INTERSECT.        **/
            var intersectingList = routedList.Where(s => rawDataList.Any(r => r.RecKey == s.Reckey)).ToList();

            /** NOW GET THE INTERSECTING ROWS FROM (cDbf1A), AND ADD THE **
             ** COLUMNS WE'LL NEED WHEN WE APPEND THE ROUTING DATA.      **/
            var formattedRawDataList = rawDataList.Where(s => intersectingList.Any(r => r.Reckey == s.RecKey)).Select(t => new FinalData
            {
                Reckey = t.RecKey,
                Acct = t.Acct,
                Passlast = t.Passlast,
                Passfrst = t.Passfrst,
                Mtggrpnbr = t.Mtggrpnbr,
                Ticket = t.Ticket,
                Recloc = t.Recloc,
                Bookdate = t.Bookdate ?? DateTime.MinValue,
                Depdate = t.Depdate ?? DateTime.MinValue,
                Airchg = t.Airchg,
                Changstamp = t.ChangStamp ?? DateTime.MinValue,
                Changedesc = t.ChangeDesc,
                Origin = new string(' ', 6),
                Orgdesc = new string(' ', 28),
                Destinat = new string(' ', 6),
                Destdesc = new string(' ', 28),
                Connect = new string(' ', 1),
                Seqno = 0m,
                Airline = new string(' ', 4),
                Fltno = new string(' ', 4),
                Deptime = new string(' ', 5),
                Arrtime = new string(' ', 5),
                Segnum = t.Segnum,
                Rectype = "B"
            }).OrderBy(t => t.Reckey).ThenBy(t => t.Changstamp).ToList();

            formattedRawDataList.AddRange(intersectingList);

            var server = globals.AgencyInformation.ServerName;
            var db = globals.AgencyInformation.DatabaseName;
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(new iBankClientQueryable(server, db), globals.Agency);

            /** 01/29/2014 - ONLY REPLACE THE SEGNUM WITH 999 IF NOT    **
             ** GOING TO XLS/CSV OUTPUT WITH CONSOLIDATING THE CHANGES. **
             ** OTHERWISE, CHANGE IT TO 1. */
            var FinalDataList = formattedRawDataList.Select(s => new FinalData
            {
                Rectype = s.Rectype,
                Reckey = s.Reckey,
                Acct = s.Acct,
                Acctdesc = clientFunctions.LookupCname(getAllMasterAccountsQuery, s.Acct, globals),
                Passlast = s.Passlast,
                Passfrst = s.Passfrst,
                Mtggrpnbr = s.Mtggrpnbr,
                Ticket = s.Ticket,
                Recloc = s.Recloc,
                Bookdate = s.Bookdate,
                Depdate = s.Depdate,
                Airchg = s.Airchg,
                Changstamp = s.Changstamp,
                Changedesc = s.Changedesc,
                Origin = s.Origin,
                Orgdesc = s.Orgdesc,
                Destinat = s.Destinat,
                Destdesc = s.Destdesc,
                Connect = s.Connect,
                Seqno = s.Seqno,
                Airline = s.Airline,
                Rdepdate = s.Rdepdate,
                Fltno = s.Fltno,
                Deptime = SharedProcedures.ConvertTime(s.Deptime.Trim()),
                Arrtime = SharedProcedures.ConvertTime(s.Arrtime.Trim()),
                Segnum = ((globals.OutputFormat == DestinationSwitch.Csv || globals.OutputFormat == DestinationSwitch.Xls) && consolidateChanges)
                            ? (s.Segnum == 0)
                                ? 1
                                : s.Segnum
                            : (s.Segnum == 0)
                                ? 999
                                : s.Segnum
            })
            .OrderBy(s => s.Acct)
            .ThenBy(s => s.Passlast)
            .ThenBy(s => s.Passfrst)
            .ThenBy(s => s.Rectype)
            .ThenBy(s => s.Depdate)
            .ThenBy(s => s.Reckey)
            .ThenBy(s => s.Segnum)
            .ToList();

            return FinalDataList;
        }
    }
}
