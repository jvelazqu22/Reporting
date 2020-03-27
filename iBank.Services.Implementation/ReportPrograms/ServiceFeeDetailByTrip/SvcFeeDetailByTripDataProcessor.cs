using Domain.Helper;
using Domain.Models.ReportPrograms.ServiceFeeDetailByTripReport;
using Domain.Orm.Classes;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBankDomain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using iBank.Services.Implementation.Shared.Client;

namespace iBank.Services.Implementation.ReportPrograms.ServiceFeeDetailByTrip
{
    public class SvcFeeDetailByTripDataProcessor
    {
        private readonly string _noAirData = "No Air Travel Data";
        public IList<FinalData> MapRawDataToFinal(IList<RawData> rawData, Dictionary<int, string> routeItineraries, 
            IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, bool accountBreak, ClientFunctions clientFunctions, ReportGlobals globals)
        {
            var finalData = rawData.Select(s => new FinalData
            {
                FeeCurrTyp = s.FeeCurrTyp,
                Reckey = s.RecKey,
                Acct = accountBreak ? s.Acct : Constants.NotApplicable,
                Acctdesc = accountBreak ? clientFunctions.LookupCname(getAllMasterAccountsQuery, s.Acct, globals) : Constants.NotApplicable,
                Passlast = s.Passlast,
                Passfrst = s.Passfrst,
                Recloc = s.Recloc,
                Invoice = s.Invoice,
                Ticket = s.Ticket,
                Airchg = s.Airchg ?? 0m,
                Depdate = s.Depdate ?? DateTime.MinValue,
                Invdate = s.Invdate ?? DateTime.MinValue,
                Itinerary = GetItinerary(routeItineraries, s.RecKey),
                Svcfee = s.Svcfee,
                Trandate = s.Trandate ?? DateTime.MinValue,
                Descript = s.Descript
            });

            return finalData
                    .OrderBy(s => s.Acctdesc)
                    .ThenBy(s => s.Acct)
                    .ThenBy(s => s.Passlast)
                    .ThenBy(s => s.Passfrst)
                    .ThenBy(s => s.Recloc)
                    .ThenBy(s => s.Invoice)
                    .ThenBy(s => s.Trandate).ToList();
        }

        public string GetItinerary(Dictionary<int, string> routeItineraries, int RecKey)
        {
            if(routeItineraries.ContainsKey(RecKey))
                if( string.IsNullOrEmpty(routeItineraries[RecKey]))
                    return _noAirData;
                else
                    return routeItineraries[RecKey];
            else
                return _noAirData;
        }
    }
}
