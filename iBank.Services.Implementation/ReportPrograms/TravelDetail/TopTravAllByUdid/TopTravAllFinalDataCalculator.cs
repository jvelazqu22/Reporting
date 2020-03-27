using Domain.Helper;
using Domain.Models.ReportPrograms.TravelDetail;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Repository;
using iBank.Server.Utilities.Classes;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;
using iBankDomain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using iBank.Services.Implementation.Shared.Client;

namespace iBank.Services.Implementation.ReportPrograms.TravelDetail.TravelDetailByUdid
{
    public class TopTravAllFinalDataCalculator
    {

        private ClientFunctions _clientFunctions;
        private IQuery<IList<MasterAccountInformation>> _getAllMasterAccountsQuery;
        private ReportGlobals _globals;

        private UserBreaks _userBreaks;

        public TopTravAllFinalDataCalculator(ClientFunctions clientFunctions)
        {
            _clientFunctions = clientFunctions;
        }

        // Convert Raw Data List to Final data list
        public List<TopTravAllByUdidFinalData> GetFinalDataFromRawData(List<TravDetRawData> rawDataList, List<CarRawData> cars, List<HotelRawData> hotels,
            IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, ReportGlobals globals, bool homeCountry, bool displayCo2, bool hasRouting)
        {
            _getAllMasterAccountsQuery = getAllMasterAccountsQuery;
            _globals = globals;
            _userBreaks = SharedProcedures.SetUserBreaks(globals.User.ReportBreaks);

            var results = new List<TopTravAllByUdidFinalData>();
            var reckeys = rawDataList.Select(s => s.RecKey).ToList();
            reckeys.AddRange(hotels.Select(s => s.RecKey));
            reckeys.AddRange(cars.Select(s => s.RecKey));
            reckeys = reckeys.Distinct().ToList();
            //add a record for each reckey, grabbing all three parts if necessary. 
            foreach (var reckey in reckeys)
            {
                var tripRec = rawDataList.FirstOrDefault(s => s.RecKey == reckey);
                var carRecs = cars.Where(s => s.RecKey == reckey).ToList();
                var hotelRecs = hotels.Where(s => s.RecKey == reckey).ToList();

                var finalData = new TopTravAllByUdidFinalData { RecKey = reckey };
                if (tripRec != null)
                {
                    finalData = AddTripRecord(finalData, tripRec, homeCountry, displayCo2);
                }
                if (carRecs.Any())
                {
                    finalData = AddCarRecords(finalData, tripRec, carRecs, homeCountry);
                }
                if (hotelRecs.Any())
                {
                    finalData = AddHotelRecords(finalData, tripRec, hotelRecs, carRecs.Any(), homeCountry);
                }

                if (tripRec != null || !hasRouting) results.Add(finalData);
            }

            return results;
        }

        // Checks if a record is for air or rail, since both are stored in airchg/tripcount
        public bool IsItRailRecord(TravDetRawData rawData)
        {
            return rawData.ValCarr.Trim().Length.Equals(4) || rawData.ValCarMode.Equals("R");

        }

        // Generate name of Crystal report to use
        public string GetCrystalReportName(bool includeCarbonEmissions, bool homeCountry)
        {
            return "ibtoptravallbyudid";
        }

        // Return unit string for CO2
        public string GetMetric(bool _useMetric)
        {
            return _useMetric ? "Kgs" : "Lbs.";
        }

        // Adds Hotel Data to final data
        private TopTravAllByUdidFinalData AddHotelRecords(TopTravAllByUdidFinalData finalData, TravDetRawData tripRec, List<HotelRawData> hotelRecs, bool carRecs, bool homeCountry)
        {

            if (tripRec == null && !carRecs)
            {
                var firstHotel = hotelRecs.FirstOrDefault();
                finalData.passfrst = firstHotel.PassFrst;
                finalData.passlast = firstHotel.PassLast;
                finalData.acct = firstHotel.Acct;
                finalData.Break1 = !_userBreaks.UserBreak1 ? "^na^" : ((string.IsNullOrEmpty(firstHotel.Break1.Trim()) ? "NONE".PadRight(30) : firstHotel.Break1.Trim()));
                finalData.Break2 = !_userBreaks.UserBreak2 ? "^na^" : ((string.IsNullOrEmpty(firstHotel.Break2.Trim()) ? "NONE".PadRight(30) : firstHotel.Break2.Trim()));
                finalData.Break3 = !_userBreaks.UserBreak3 ? "^na^" : ((string.IsNullOrEmpty(firstHotel.Break3.Trim()) ? "NONE".PadRight(16) : firstHotel.Break3.Trim()));
                finalData.acctdesc = _clientFunctions.LookupCname(_getAllMasterAccountsQuery, firstHotel.Acct, _globals);
                finalData.RecLoc = firstHotel.RecLoc;
                finalData.TripStart = firstHotel.TripStart ?? DateTime.MinValue;
                finalData.TripEnd = firstHotel.TripEnd ?? DateTime.MinValue;
                finalData.DepDate = firstHotel.DepDate ?? DateTime.MinValue;
                finalData.ArrDate = firstHotel.ArrDate ?? DateTime.MinValue;
                finalData.PlusMin = firstHotel.HPlusMin;
                finalData.homectry = homeCountry ? LookupFunctions.LookupHomeCountryName(firstHotel.SourceAbbr.Trim(), _globals, new MasterDataStore()) : Constants.NotApplicable;
                finalData.Udidtext = firstHotel.Udidtext;
            }


            finalData.hotelcost = hotelRecs.Sum(s => s.BookRate * s.Nights * s.Rooms);
            finalData.hotnights = hotelRecs.Sum(s => s.Nights * s.Rooms);
            return finalData;
        }

        // Adds car data to final data
        private TopTravAllByUdidFinalData AddCarRecords(TopTravAllByUdidFinalData finalData, TravDetRawData tripRec, List<CarRawData> carRecs, bool homeCountry)
        {
            if (tripRec == null)
            {
                var firstCar = carRecs.FirstOrDefault();
                finalData.passfrst = firstCar.PassFrst;
                finalData.passlast = firstCar.PassLast;
                finalData.acct = firstCar.Acct;
                finalData.Break1 = !_userBreaks.UserBreak1 ? "^na^" : ((string.IsNullOrEmpty(firstCar.Break1.Trim()) ? "NONE".PadRight(30) : firstCar.Break1.Trim()));
                finalData.Break2 = !_userBreaks.UserBreak2 ? "^na^" : ((string.IsNullOrEmpty(firstCar.Break2.Trim()) ? "NONE".PadRight(30) : firstCar.Break2.Trim()));
                finalData.Break3 = !_userBreaks.UserBreak3 ? "^na^" : ((string.IsNullOrEmpty(firstCar.Break3.Trim()) ? "NONE".PadRight(16) : firstCar.Break3.Trim()));
                finalData.acctdesc = _clientFunctions.LookupCname(_getAllMasterAccountsQuery, firstCar.Acct, _globals);
                finalData.RecLoc = firstCar.RecLoc;
                finalData.TripStart = firstCar.TripStart ?? DateTime.MinValue;
                finalData.TripEnd = firstCar.TripEnd ?? DateTime.MinValue;
                finalData.DepDate = firstCar.DepDate ?? DateTime.MinValue;
                finalData.ArrDate = firstCar.ArrDate ?? DateTime.MinValue;
                finalData.PlusMin = firstCar.CPlusMin;
                finalData.homectry = homeCountry ? LookupFunctions.LookupHomeCountryName(firstCar.SourceAbbr.Trim(), _globals, new MasterDataStore()) : Constants.NotApplicable;
                finalData.Udidtext = firstCar.Udidtext;
            }


            finalData.carcost = carRecs.Sum(s => s.Abookrat * s.Days);
            finalData.cardays = carRecs.Sum(s => s.Days);
            return finalData;
        }

        // Adds Air/Rail data to final data
        private TopTravAllByUdidFinalData AddTripRecord(TopTravAllByUdidFinalData finalData, TravDetRawData tripRec, bool homeCountry, bool displayCo2)
        {
            finalData.passfrst = tripRec.PassFrst;
            finalData.passlast = tripRec.PassLast;
            finalData.acct = tripRec.Acct;
            finalData.Break1 = !_userBreaks.UserBreak1 ? "^na^" : ((string.IsNullOrEmpty(tripRec.Break1.Trim()) ? "NONE".PadRight(30) : tripRec.Break1.Trim()));
            finalData.Break2 = !_userBreaks.UserBreak2 ? "^na^" : ((string.IsNullOrEmpty(tripRec.Break2.Trim()) ? "NONE".PadRight(30) : tripRec.Break2.Trim()));
            finalData.Break3 = !_userBreaks.UserBreak3 ? "^na^" : ((string.IsNullOrEmpty(tripRec.Break3.Trim()) ? "NONE".PadRight(16) : tripRec.Break3.Trim()));
            finalData.acctdesc = _clientFunctions.LookupCname(_getAllMasterAccountsQuery, finalData.acct, _globals);
            // Combine rail and air records if including carbon calculations
            if (!IsItRailRecord(tripRec) || displayCo2)
            {
                finalData.tripcount = tripRec.PlusMin;
                finalData.airchg = tripRec.AirChg;
            }
            else
            {
                finalData.tripcount = 0;
                finalData.airchg = 0;
            }
            finalData.railcount = IsItRailRecord(tripRec) ? tripRec.PlusMin : 0;
            finalData.railchg = IsItRailRecord(tripRec) ? tripRec.AirChg : 0;
            finalData.TripStart = tripRec.TripStart ?? DateTime.MinValue;
            finalData.TripEnd = tripRec.TripEnd ?? DateTime.MinValue;
            finalData.DepDate = tripRec.DepDate ?? DateTime.MinValue;
            finalData.ArrDate = tripRec.ArrDate ?? DateTime.MinValue;
            finalData.PlusMin = tripRec.PlusMin;
            finalData.RecLoc = tripRec.RecLoc;
            finalData.RecKey = tripRec.RecKey;
            finalData.homectry = homeCountry ? LookupFunctions.LookupHomeCountryName(tripRec.SourceAbbr.Trim(), _globals, new MasterDataStore()) : Constants.NotApplicable;
            finalData.Udidtext = tripRec.Udidtext;
            return finalData;
        }
    }
}
