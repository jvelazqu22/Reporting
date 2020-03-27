using Domain.Helper;
using Domain.Models.ReportPrograms.CCReconReport;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.Client;
using iBankDomain.RepositoryInterfaces;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.CCRecon
{
    public class CcTripRecordsMapping
    {
        public IList<FinalData> AddMissingRecordsFromTrips(IList<FinalData> tripCcRecords, IList<FinalData> finalDataList, Dictionary<int, string> routeItineraries, bool useAccountBreaks,
            ClientFunctions clientFunctions, IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, ReportGlobals globals, UserBreaks userBreaks, bool displayBreaks, IMasterDataStore store)
        {
            foreach (var row in tripCcRecords)
            {
                var recsFound = finalDataList.Where(w => w.Passfrst.Equals(row.Passfrst))
                    .Where(w => w.Passlast.Equals(row.Passlast))
                    .Where(w => w.Cardnum.Equals(row.Cardnum))
                    .Where(w => w.Ticket.Equals(row.Ticket))
                    .Any();

                if (!recsFound)
                {
                    var airlineLookup = LookupFunctions.LookupAirline(store, row.Valcarr.Trim(), row.Mode.Trim());

                    string itinerary;
                    if (!routeItineraries.TryGetValue(row.RecKey, out itinerary))
                    {
                        itinerary = new string(' ', 240);
                    }

                    row.Acctdesc = !useAccountBreaks ? Constants.NotApplicable : clientFunctions.LookupCname(getAllMasterAccountsQuery, row.Acct, globals);
                    row.Break1 = !userBreaks.UserBreak1 ? Constants.NotApplicable : string.IsNullOrEmpty(row.Break1.Trim()) ? "NONE".PadRight(30) : row.Break1;
                    row.Break2 = !userBreaks.UserBreak2 ? Constants.NotApplicable : string.IsNullOrEmpty(row.Break2.Trim()) ? "NONE".PadRight(30) : row.Break2;
                    row.Break3 = !userBreaks.UserBreak3 ? Constants.NotApplicable : string.IsNullOrEmpty(row.Break3.Trim()) ? "NONE".PadRight(30) : row.Break3;
                    row.BreaksCol = GetBreaksCol(displayBreaks, row.Break1, row.Break2, row.Break3);
                    row.Airlndesc = airlineLookup.Item2;
                    row.Airlinenbr = airlineLookup.Item1.ToString(CultureInfo.InvariantCulture).PadLeft(3, '0');
                    row.Descript = itinerary;

                    finalDataList.Add(row);
                }
            }
            return finalDataList;
        }

        public string GetBreaksCol(bool displayBreaks, string break1, string break2, string break3)
        {
            string retValue = string.Empty;

            if (string.IsNullOrWhiteSpace(break1)) break1 = "NONE";
            if (string.IsNullOrWhiteSpace(break2)) break2 = "NONE";
            if (string.IsNullOrWhiteSpace(break3)) break3 = "NONE";

            if (displayBreaks)
            {
                if (break1 != Constants.NotApplicable)
                {
                    retValue += break1.Trim();
                }
                if (break2 != Constants.NotApplicable)
                {
                    if (!string.IsNullOrEmpty(retValue)) retValue += "/";
                    retValue += break2.Trim();
                }
                if (break3 != Constants.NotApplicable)
                {
                    if (!string.IsNullOrEmpty(retValue)) retValue += "/";
                    retValue += break3.Trim();
                }
            }
            return retValue;
        }
    }
}