using iBank.Server.Utilities.Classes;
using Domain.Models.ReportPrograms.TripChangesAir;
using System.Collections.Generic;
using System.Linq;
using iBank.Services.Implementation.Shared;
using iBank.Server.Utilities;
using iBank.Repository.SQL.Interfaces;

namespace iBank.Services.Implementation.ReportPrograms.TripChanges.TripChangesAir
{
    public class RawDataHandler
    {
        public RawDataParams SetUpRawDataLists(ReportGlobals globals, List<RawData> rawDataList, IMasterDataStore masterStore,
            bool includeCancelledTrips, TripChangeDataProcessor dataProcessor)
        {
            var rawDataParams = new RawDataParams();
            rawDataParams.RawDataList = GetJoinRawDataListWithChangeCodes(globals, rawDataList, masterStore);

            if (includeCancelledTrips) rawDataParams = UpdateRawListWithMissingCancelledRecords(dataProcessor, rawDataParams);

            rawDataParams = GetRoutingData(includeCancelledTrips, dataProcessor, rawDataParams);

            return rawDataParams;
        }

        private List<RawData> GetJoinRawDataListWithChangeCodes(ReportGlobals Globals, List<RawData> RawDataList, IMasterDataStore MasterStore)
        {
            var changeCodes =
                LookupFunctions.GetAllTripChangeCodes(MasterStore)
                    .Where(s => s.LanguageCode.EqualsIgnoreCase(Globals.UserLanguage))
                    .ToList();

            RawDataList = (from r in RawDataList
                           join c in changeCodes on r.ChangeCode equals c.ChangeCode
                           where (c.ChangeGroup.Trim() == "T" || c.ChangeGroup.Trim() == "S")
                           select
                               new RawData
                               {
                                   RecKey = r.RecKey,
                                   Acct = r.Acct,
                                   Passlast = r.Passlast,
                                   Passfrst = r.Passfrst,
                                   AirCurrTyp = r.AirCurrTyp,
                                   InvDate = r.InvDate,
                                   RDepDate = r.RDepDate,
                                   Mtggrpnbr = r.Mtggrpnbr,
                                   Ticket = r.Ticket,
                                   Recloc = r.Recloc,
                                   Bookdate = r.Bookdate,
                                   Depdate = r.Depdate,
                                   Airchg = r.Airchg,
                                   Segnum = r.Segnum,
                                   ChangStamp = r.ChangStamp,
                                   ChangeDesc = LookupFunctions.LookupChangeDescirption(r.ChangeCode, c.CodeDescription, r.ChangeFrom, r.ChangeTo, r.PriorItin, Globals, MasterStore)
                               }).OrderBy(s => s.RecKey).ThenBy(s => s.ChangStamp).ToList();

            return RawDataList;
        }

        private RawDataParams UpdateRawListWithMissingCancelledRecords(TripChangeDataProcessor dataProcessor, RawDataParams rawDataParams)
        {
            rawDataParams.CancelledRawDataList = dataProcessor.GetCancelledRawData();

            //Remove duplicates
            rawDataParams.CancelledRawDataList =
                rawDataParams.CancelledRawDataList.Where(s => rawDataParams.RawDataList.All(t => t.RecKey != s.RecKey)).ToList();

            //Add to the rawdatalist
            rawDataParams.RawDataList.AddRange(rawDataParams.CancelledRawDataList.Select(r => new RawData
            {
                RecKey = r.RecKey,
                Acct = r.Acct,
                Passlast = r.Passlast,
                Passfrst = r.Passfrst,
                AirCurrTyp = r.AirCurrTyp,
                InvDate = r.InvDate,
                RDepDate = r.RDepDate,
                Mtggrpnbr = r.Mtggrpnbr,
                Ticket = r.Ticket,
                Recloc = r.Recloc,
                Bookdate = r.Bookdate,
                Depdate = r.Depdate,
                Airchg = r.Airchg,
                Segnum = r.Segnum,
                ChangStamp = r.ChangStamp,
                ChangeDesc = "This trip has been CANCELLED"
            }));

            return rawDataParams;
        }

        private RawDataParams GetRoutingData(bool IncludeCancelledTrips, TripChangeDataProcessor dataProcessor, RawDataParams rawDataParams)
        {
            //Get Routing Data
            rawDataParams.RoutingRawDataList = dataProcessor.GetRoutingData();

            //Get Routing Data for cancelled trips
            if (IncludeCancelledTrips)
            {
                rawDataParams.CancelledRawDataList = dataProcessor.GetCancelledRoutingData();

                //Remove duplicates
                rawDataParams.CancelledRawDataList =
                    rawDataParams.CancelledRawDataList.Where(s => rawDataParams.RoutingRawDataList.All(t => t.RecKey != s.RecKey)).ToList();

                //Add to the routing data list
                rawDataParams.RoutingRawDataList.AddRange(rawDataParams.CancelledRawDataList);
            }

            return rawDataParams;
        }

    }
}
