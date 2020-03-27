using CODE.Framework.Core.Utilities;
using CODE.Framework.Core.Utilities.Extensions;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using Domain.Models.ReportPrograms.TravelDetail;
using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared;

using System;
using System.Collections.Generic;
using System.Linq;

using Domain.Orm.iBankClientQueries;
using Domain.Orm.iBankMastersQueries;
using Domain.Orm.iBankMastersQueries.Other;
using iBank.Repository.SQL.Repository;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;

namespace iBank.Services.Implementation.ReportPrograms.TravelDetail
{
    //TODO: Move data classes into the domain structure. See QView for example.
    public class TravelAtAGlance : ReportRunner<TravDetRawData, TravelAtAGlanceFinalData>
    {
        private readonly TravDetShared _travDetShared;

        public TravelAtAGlance()
        {
            CrystalReportName = "ibTravelGlance";
            _travDetShared = new TravDetShared();
        }
        
        public override bool InitialChecks()
        {
            Globals.SetParmValue(WhereCriteria.PREPOST, "1");
            _travDetShared.IsReservation = true;

            if (!IsGoodCombo()) return false;

            if (!IsDateRangeValid()) return false;

            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsOnlineReport()) return false;

            return true;
        }

        public override bool GetRawData()
        {
            if (!_travDetShared.GetRawData(BuildWhere)) return false;
            RawDataList = _travDetShared.RawDataList;
            return true;
        }

        //** IN ORDER TO GET CAR/HOTEL RECORDS TO SHOW UP ON THE REPORT ON THE SAME**
        //** REPORT LINES AS THE ROUTING, WE'RE GOING TO HAVE TO "MIX" DATA IN ROWS  **
        //** IN THE CURSOR.A FURTHER COMPILCATION IS THAT FACT THAT THE NUMBER OF  **
        //** LEGS WILL RARELY MATCH THE NUMBER OF CAR/HOTEL RECORDS.FOR EACH TRIP, **
        //** WE'RE GOING TO BUILD AN ARRAY WITH A SET # OF COLUMNS AND A "FLEXIBLE"  **
        //** # OF ROWS TO HOLD THE DATA.  (THIS SHOULD BE A BLAST !#&!!)             **
        public override bool ProcessData()
        {
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (!_travDetShared.ProcessDataShared(getAllMasterAccountsQuery)) return false;

            var urlPath = GetUrlPath();

            FinalDataList = new List<TravelAtAGlanceFinalData>();
            var filteredRouteData = _travDetShared.FilteredRouteData.
               OrderBy(s => s.PassLast).
               ThenBy(s => s.PassFrst).
               ThenBy(s => s.TripStart).ToList();

            foreach (var row in filteredRouteData)
            {
                if (_travDetShared.Legs.All(s => s.RecKey != row.RecKey) &&
                    _travDetShared.Cars.All(s => s.RecKey != row.RecKey) &&
                    _travDetShared.Hotels.All(s => s.RecKey != row.RecKey)) continue;

                var trip = _travDetShared.FilteredRouteData.FirstOrDefault(s => s.RecKey == row.RecKey);

                if (trip == null) continue;

                var country = "";

                var finalData = new TravelAtAGlanceFinalData
                {
                    Reckey = trip.RecKey,
                    Passlast = trip.PassLast.Trim(),
                    Passfrst = trip.PassFrst.Trim(),
                    Recloc = trip.RecLoc,
                    Authlink = urlPath + trip.RecKey,
                    Destctry = string.Empty
                };

                var legs = _travDetShared.Legs.Where(s => s.RecKey == trip.RecKey).OrderBy(s => s.RDepDate).ThenBy(s => s.DepTime).ToList();
                var nCount = 0;
                foreach (var leg in legs)
                {
                    if (nCount > 0)
                    {
                        FinalDataList.Add(finalData);
                        finalData = new TravelAtAGlanceFinalData
                        {
                            Reckey = trip.RecKey,
                            Passlast = trip.PassLast.Trim(),
                            Passfrst = trip.PassFrst.Trim(),
                            Recloc = trip.RecLoc,
                            Authlink = urlPath + trip.RecKey,
                            Destctry = string.Empty
                        };

                    }

                    nCount++;

                    finalData.Orgdesc = AportLookup.LookupAport(MasterStore, leg.Origin, leg.Mode, Globals.Agency).Trim();
                    finalData.Destdesc = AportLookup.LookupAport(MasterStore, leg.Destinat, leg.Mode, Globals.Agency).Trim();
                    finalData.Rdepdate = leg.RDepDate ?? DateTime.MinValue;
                    finalData.Deptime = leg.DepTime;
                    finalData.Arrtime = leg.ArrTime;
                    finalData.Airlinedes = LookupFunctions.LookupAline(MasterStore, leg.Airline, leg.Mode);
                    finalData.Fltno = leg.fltno;
                    finalData.Destctry = country;
                    finalData.Rectype = "L";
                    finalData.Mode = leg.Mode;

                    if (!country.IsNullOrWhiteSpace() || leg.Connect.EqualsIgnoreCase("X")) continue;
                    var ctry = LookupFunctions.LookupCountry(MasterStore, leg.Destinat, false, leg.Mode, Globals);
                    if (ctry.IsNullOrWhiteSpace()) continue;
                    country = ctry.IndexOf("United States", StringComparison.CurrentCultureIgnoreCase) >= 0 ?
                        "United States" :
                        ctry;
                    //Is there a better way to do this? I think this only sets the first trip record to the country.
                    foreach (var f in FinalDataList.Where(s => s.Reckey == trip.RecKey).ToList())
                    {
                        f.Destctry = country;
                    }
                    finalData.Destctry = country;
                }
                //Check that cars doesn't fill in columns in already existing finalData rows, like legs does.
                var cars = _travDetShared.Cars.Where(s => s.RecKey == trip.RecKey).ToList();
                foreach (var car in cars)
                {
                    FinalDataList.Add(finalData);
                    finalData = new TravelAtAGlanceFinalData
                    {
                        Passlast = trip.PassLast.Trim(),
                        Passfrst = trip.PassFrst.Trim(),
                        Recloc = trip.RecLoc,
                        Authlink = urlPath + trip.RecKey,
                        Destctry = country,
                        Company = car.Company.Trim(),
                        Autocityst = car.Autocity.Trim() + ", " + car.Autostat.Trim(),
                        Rentdate = car.RentDate ?? DateTime.MinValue,
                        Days = car.Days * car.CPlusMin,
                        Reckey = trip.RecKey,
                        Rectype = "C"
                    };
                    if (!country.IsNullOrWhiteSpace()) continue;
                    var ctry = LookupFunctions.LookupCountry(MasterStore, car.Citycode, false, "A", Globals);
                    if (ctry.IsNullOrWhiteSpace()) continue;
                    country = ctry.IndexOf("United States", StringComparison.CurrentCultureIgnoreCase) >= 0 ?
                        "United States" :
                        ctry;
                    foreach (var f in FinalDataList.Where(s => s.Reckey == trip.RecKey).ToList())
                    {
                        f.Destctry = country;
                    }
                }
                var hotels = _travDetShared.Hotels.Where(s => s.RecKey == trip.RecKey).ToList();
                foreach (var hotel in hotels)
                {
                    FinalDataList.Add(finalData);
                    finalData = new TravelAtAGlanceFinalData
                    {
                        Reckey = trip.RecKey,
                        Passlast = trip.PassLast.Trim(),
                        Passfrst = trip.PassFrst.Trim(),
                        Recloc = trip.RecLoc,
                        Authlink = urlPath + trip.RecKey,
                        Destctry = country,
                        Hotelnam = hotel.HotelNam.Trim(),
                        Hotcityst = hotel.HotCity.Trim() + ", " + hotel.HotState.Trim(),
                        Datein = hotel.DateIn ?? DateTime.MinValue,
                        Nights = hotel.Nights * hotel.HPlusMin,
                        Rectype = "H"
                    };

                    //I don't understand the following FoxPro code. DateIn is a date, and the field that corresponds
                    //to column 12 in the array is a string. This program seems to be assuming DateIn will
                    //be a string.
                    //If Empty(laTemp[lnArrRow,12])
                    //	laTemp[lnArrRow,12] = curHotels.DateIn
                    //Endif

                    if (!country.IsNullOrWhiteSpace()) continue;
                    var ctry = LookupFunctions.LookupCountry(MasterStore, hotel.Metro, false, "A", Globals);
                    if (ctry.IsNullOrWhiteSpace()) continue;
                    country = ctry.IndexOf("United States", StringComparison.CurrentCultureIgnoreCase) >= 0 ?
                       "United States" :
                       ctry;
                    foreach (var f in FinalDataList.Where(s => s.Reckey == trip.RecKey).ToList())
                    {
                        f.Destctry = country;
                    }
                }
                FinalDataList.Add(finalData);
            }
            FinalDataList = FinalDataList.OrderBy(s => s.Passlast).ThenBy(s => s.Passfrst).ThenBy(s => s.Rdepdate).ThenBy(s => s.Deptime).ToList();
            //Filter by mode
            if (!Globals.GetParmValue(WhereCriteria.MODE).IsNullOrWhiteSpace())
            {
                var mode = (Mode)(Globals.GetParmValue(WhereCriteria.MODE).TryIntParse(0));
                var trips = new List<string>();

                if (mode == Mode.RAIL)
                {
                    Globals.WhereText = string.IsNullOrEmpty(Globals.WhereText) ? "Rail Only;" : $"{Globals.WhereText} Rail Only";
                    trips = FinalDataList.Where(x => x.Mode.EqualsIgnoreCase("R")).GroupBy(s => s.Recloc).Select(s => s.Key).ToList();
                    FinalDataList = FinalDataList.Where(s => trips.Contains(s.Recloc)).ToList();
                }
                else if (mode == Mode.AIR)
                {
                    Globals.WhereText = string.IsNullOrEmpty(Globals.WhereText) ? "Air Only;" : $"{Globals.WhereText} Air Only";
                    trips = FinalDataList.Where(x => x.Mode.EqualsIgnoreCase("A")).GroupBy(s => s.Recloc).Select(s => s.Key).ToList();
                    FinalDataList = FinalDataList.Where(s => trips.Contains(s.Recloc)).ToList();
                }
            }

            return DataExists(FinalDataList);
        }

        public override bool GenerateReport()
        {
            var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." +
                                      Constants.CrystalReportExt;

            ReportSource = new ReportDocument();
            ReportSource.Load(rptFilePath);

            ReportSource.SetDataSource(FinalDataList);

            var missing = CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
            ReportSource.SetParameterValue("lblCountry", "Destination Country: ");


            CrystalFunctions.CreatePdf(ReportSource, Globals);
            return true;
        }

        private string GetUrlPath()
        {
            var parm = new GetMasterUrlPathQuery(new iBankMastersQueryable());
            var urlPath = parm.ExecuteQuery();
            return urlPath.UrlPath +
                       "redirect1.cfm?nextpage=kslogin.cfm&agclient=" + Globals.AgencyInformation.ClientURL +
                       @"&frpage=svcsTravelerLookup.cfm&frstyle=0&frappkey=";
        }
    }
}