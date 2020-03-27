using CODE.Framework.Core.Utilities;
using CrystalDecisions.CrystalReports.Engine;
using Domain.Helper;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Helpers;
using iBank.Services.Implementation.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.TravelDetail;
using Domain.Orm.iBankClientQueries;
using System.Reflection;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;

namespace iBank.Services.Implementation.ReportPrograms.TravelDetail
{
    public class TravDet1 : ReportRunner<TravDetRawData, TravDet1FinalData>
    {
        private readonly TravDetShared _travDetShared;
        private readonly com.ciswired.libraries.CISLogger.ILogger LOG = new com.ciswired.libraries.CISLogger.LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public TravDet1()
        {
            CrystalReportName = "ibTravDet1";
            _travDetShared = new TravDetShared();
        }

        private bool IsReservation { get; set; }
        private bool NoCar { get; set; }
        private bool NoHotel { get; set; }

        private UserBreaks UserBreaks { get; set; }

        public override bool InitialChecks()
        {
            _travDetShared.UseServiceFees = true;
            _travDetShared.IsReservation = Globals.ParmValueEquals(WhereCriteria.PREPOST, "1");
            UserBreaks = SharedProcedures.SetUserBreaks(Globals.User.ReportBreaks);

            if (!IsGoodCombo()) return false;

            if (!IsDateRangeValid()) return false;

            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsOnlineReport()) return false;

            return true;
        }

        public override bool GetRawData()
        {
            if (!_travDetShared.GetRawData(BuildWhere)) return false;
            ConvertCurrencies();
            RawDataList = _travDetShared.RawDataList;
            return true;
        }

        public override bool ProcessData()
        {
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);

            if (!_travDetShared.ProcessDataShared(getAllMasterAccountsQuery)) return false;

            var reasonsExclude = Globals.AgencyInformation.ReasonExclude.Split();

            foreach (var t in _travDetShared.FilteredRouteData.ToList())
            {
                var trip = t;

                if (_travDetShared.Legs.All(s => s.RecKey != trip.RecKey || "ZZ,$$".IndexOf(s.Airline.Trim(), StringComparison.Ordinal) >= 0) &&
                    _travDetShared.Cars.All(s => s.RecKey != trip.RecKey) &&
                    _travDetShared.Hotels.All(s => s.RecKey != trip.RecKey)) continue;

                var acctName = clientFunctions.LookupCname(getAllMasterAccountsQuery, trip.Acct, Globals).Trim();

                var excludeDescription = trip.Reascode.IsNullOrWhiteSpace() || reasonsExclude.Contains(trip.Reascode)
                    ? string.Empty
                    : clientFunctions.LookupReason(getAllMasterAccountsQuery, trip.Reascode, trip.Acct, ClientStore, Globals, MasterStore.MastersQueryDb);

                var carrierDescription = trip.ValCarr.IsNullOrWhiteSpace() ||
                    "ZZ,$$".IndexOf(trip.ValCarr.Trim(), StringComparison.Ordinal) >= 0
                    ? string.Empty : LookupFunctions.LookupAline(MasterStore, trip.ValCarr, trip.ValCarMode);

                var svcFee = GetSvcFee(trip);

                var tripCost = IsReservation ? trip.AirChg : trip.AirChg + svcFee;

                var exchangeInfo = GetExchangeInfo(trip);

                var plusMin = trip.PlusMin;
                var airChg = trip.AirChg;
                var stndChg = trip.Stndchg;
                var offRdChg = trip.Offrdchg > 0 && trip.AirChg < 0
                    ? 0 - trip.Offrdchg
                    : trip.Offrdchg == 0
                        ? trip.AirChg
                        : trip.Offrdchg;
                var legs = _travDetShared.Legs.Where(s => s.RecKey == trip.RecKey).OrderBy(x=>x.SeqNo).ToList();
                if (!legs.Any())
                {
                    plusMin = 0;
                    airChg = 0;
                    stndChg = 0;
                    offRdChg = 0;
                }

                FinalDataList.Add(new TravDet1FinalData
                {
                    Acct = trip.Acct,
                    Acctname = acctName,
                    Airchg = airChg,
                    Break1 = trip.Break1,
                    Break2 = trip.Break2,
                    Break3 = trip.Break3,
                    Brkname1 = Globals.User.Break1Name,
                    Brkname2 = Globals.User.Break2Name,
                    Brkname3 = Globals.User.Break2Name,
                    Carrdesc = carrierDescription,
                    ExchInfo = exchangeInfo,
                    Invdate = trip.InvDate ?? DateTime.MinValue,
                    Invoice = trip.Invoice,
                    Offrdchg = offRdChg,
                    PassFrst = trip.PassFrst,
                    PassLast = trip.PassLast,
                    Plusmin = plusMin,
                    ReasCode = trip.Reascode,
                    ReasDesc = excludeDescription,
                    Reckey = trip.RecKey,
                    Recloc = trip.RecLoc,
                    Rectype = "A",
                    Stndchg = stndChg,
                    Svcfee = svcFee,
                    Ticket = trip.Ticket,
                    TranType = trip.TranType,
                    TripCost = tripCost,
                    Valcarr = trip.ValCarr
                });

                foreach (var leg in legs)
                {
                    FinalDataList.Add(new TravDet1FinalData
                    {
                        Rectype = "B",
                        Reckey = trip.RecKey,
                        Recloc = trip.RecLoc,
                        Invoice = trip.Invoice,
                        Ticket = trip.Ticket,
                        Plusmin = plusMin,
                        Acct = trip.Acct,
                        Carrdesc = carrierDescription,
                        Acctname = acctName,
                        Brkname1 = Globals.User.Break1Name,
                        Brkname2 = Globals.User.Break2Name,
                        Brkname3 = Globals.User.Break2Name,
                        Break1 = leg.Break1,
                        Break2 = leg.Break2,
                        Break3 = leg.Break3,
                        PassLast = trip.PassLast,
                        PassFrst = trip.PassFrst,
                        Invdate = trip.InvDate ?? DateTime.MinValue,
                        Origin = leg.Origin,
                        OrgDesc = AportLookup.LookupAport(MasterStore, leg.Origin, leg.Mode, Globals.Agency),
                        Destinat = leg.Destinat,
                        DestDesc = AportLookup.LookupAport(MasterStore, leg.Destinat, leg.Mode, Globals.Agency),
                        Mode = leg.Mode,
                        RdepDate = leg.RDepDate ?? DateTime.MinValue,
                        Airline = leg.Airline,
                        Legcarrdes = LookupFunctions.LookupAline(MasterStore, leg.Airline, leg.Mode),
                        FltNo = leg.fltno,
                        Class = leg.Class,
                        DepTime = leg.DepTime,
                        ArrTime = leg.ArrTime
                    });
                }

                AddCars(trip, plusMin);

                AddHotels(trip, plusMin);
            }

            SortFinalData();

            ProcessExcpCond();

            return DataExists(FinalDataList);
        }

        public override bool GenerateReport()
        {
            if (Globals.OutputFormat == DestinationSwitch.Xls || Globals.OutputFormat == DestinationSwitch.Csv)
            {
                var exportFields = GetExportFields();

                if (Globals.OutputFormat == DestinationSwitch.Csv)
                {
                    ExportHelper.ConvertToCsv(FinalDataList, exportFields, Globals);
                }
                else
                {
                    ExportHelper.ListToXlsx(FinalDataList, exportFields, Globals);
                }

                return true;
            }
            var rptFilePath = StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." +
                              Constants.CrystalReportExt;

            ReportSource = new ReportDocument();
            ReportSource.Load(rptFilePath);

            ReportSource.SetDataSource(FinalDataList);

            CrystalFunctions.SetupCrystalReport(ReportSource, Globals);

            ReportSource.SetParameterValue("lLogGen1", Globals.IsParmValueOn(WhereCriteria.CBEXCLUDELOWFARE));
            ReportSource.SetParameterValue("lbfAirChg", "Air Charges:");
            ReportSource.SetParameterValue("lbfAirTrips", "# of Air Trips:");
            ReportSource.SetParameterValue("lbfAvgCarCost", "Avg Cost per Day:");
            ReportSource.SetParameterValue("lbfAvgCarRate", "Avg Booked Rate:");
            ReportSource.SetParameterValue("lbfAvgCostperTrip", "Avg Cost per Trip:");
            ReportSource.SetParameterValue("lbfAvgDays", "Avg # of Days Rented:");
            ReportSource.SetParameterValue("lbfAvgHotelCost", "Avg Cost/RoomNight:");
            ReportSource.SetParameterValue("lbfAvgHotelRate", "Avg Booked Rate:");
            ReportSource.SetParameterValue("lbfAvgNights", "Avg # of Nights:");
            ReportSource.SetParameterValue("lbfCarChgs", "Car Rental Charges:");
            ReportSource.SetParameterValue("lbfDays", "# of Days Rented:");
            ReportSource.SetParameterValue("lbfHotelChgs", "Hotel Booking Charges:");
            ReportSource.SetParameterValue("lbfNights", "# of Room Nights:");
            ReportSource.SetParameterValue("lbfRentals", "# of Rentals:");
            ReportSource.SetParameterValue("lbfStays", "# of Stays:");
            ReportSource.SetParameterValue("lbfTotAllChgs", "Total All Charges:");
            ReportSource.SetParameterValue("lbfTotSvcFees", "Total Svc Fees:");
            ReportSource.SetParameterValue("lblAccount", "Account:");
            ReportSource.SetParameterValue("lblActual", "Actual:");
            ReportSource.SetParameterValue("lblException", "Exception:");
            ReportSource.SetParameterValue("lblInvDate", "Inv Date:");
            ReportSource.SetParameterValue("lblInvNum", "Invoice #:");
            ReportSource.SetParameterValue("lblLostAmt", "Lost Amt:");
            ReportSource.SetParameterValue("lblLowest", "Lowest:");
            ReportSource.SetParameterValue("lblRecLoc", "Rec Locator:");
            ReportSource.SetParameterValue("lblSavings", "Savings:");
            ReportSource.SetParameterValue("lblSvcFees", "Service Fees:");
            ReportSource.SetParameterValue("lblticket", "Ticket #:");
            ReportSource.SetParameterValue("lblTotalCost", "Total Cost of Trip:");
            ReportSource.SetParameterValue("lblValCarr", "Val Carrier:");
            ReportSource.SetParameterValue("lblVoid", "** Air Travel Voided **");
            ReportSource.SetParameterValue("lExSvcFee", _travDetShared.ExcludeServiceFees);

            //Sum car totals
            if (!NoCar)
            {
                ReportSource.SetParameterValue("nTotRents2",
                    FinalDataList.Sum(s => s.Abookrat == 0 ? 0 : s.CplusMin));
                ReportSource.SetParameterValue("nTotNzDays",
                    FinalDataList.Sum(s => s.Abookrat == 0 ? 0 : s.CplusMin * s.Days));
                ReportSource.SetParameterValue("nTotRents", FinalDataList.Sum(s => s.CplusMin));
                ReportSource.SetParameterValue("nTotDays", FinalDataList.Sum(s => s.CplusMin * s.Days));
                ReportSource.SetParameterValue("nTotCarCost", FinalDataList.Sum(s => s.Abookrat * s.Days));
            }
            else
            {
                ReportSource.SetParameterValue("nTotRents2", 0);
                ReportSource.SetParameterValue("nTotNzDays", 0);
                ReportSource.SetParameterValue("nTotRents", 0);
                ReportSource.SetParameterValue("nTotDays", 0);
                ReportSource.SetParameterValue("nTotCarCost", 0);
            }

            //Sum hotel totals
            if (!NoHotel)
            {
                ReportSource.SetParameterValue("nTotStays2",
                    FinalDataList.Sum(s => s.BookRate == 0 ? 0 : s.HPlusMin));
                ReportSource.SetParameterValue("nTotNzNites",
                    FinalDataList.Sum(s => s.BookRate == 0 ? 0 : s.HPlusMin * s.Nights * s.Rooms));
                ReportSource.SetParameterValue("nTotStays", FinalDataList.Sum(s => s.HPlusMin));
                ReportSource.SetParameterValue("nTotNites", FinalDataList.Sum(s => s.HPlusMin * s.Nights));
                ReportSource.SetParameterValue("nTotHotCost",
                    FinalDataList.Sum(s => s.BookRate * s.Nights * s.Rooms));
            }
            else
            {
                ReportSource.SetParameterValue("nTotStays2", 0);
                ReportSource.SetParameterValue("nTotNzNites", 0);
                ReportSource.SetParameterValue("nTotStays", 0);
                ReportSource.SetParameterValue("nTotNites", 0);
                ReportSource.SetParameterValue("nTotHotCost", 0);

            }
            CrystalFunctions.CreatePdf(ReportSource, Globals);

            return true;
        }

        private void AddCars(TravDetRawData trip, int plusMin)
        {
            foreach (var car in _travDetShared.Cars.Where(s => s.RecKey == trip.RecKey).OrderBy(x=>x.RentDate))
            {
                FinalDataList.Add(new TravDet1FinalData
                {
                    Rectype = "C",
                    Reckey = trip.RecKey,
                    Recloc = trip.RecLoc,
                    Invoice = trip.Invoice,
                    Ticket = trip.Ticket,
                    Plusmin = plusMin,
                    Acct = trip.Acct,
                    Acctname = Globals.AccountName,
                    Brkname1 = Globals.User.Break1Name,
                    Brkname2 = Globals.User.Break2Name,
                    Brkname3 = Globals.User.Break2Name,
                    Break1 = trip.Break1,
                    Break2 = trip.Break2,
                    Break3 = trip.Break3,
                    PassLast = trip.PassLast,
                    PassFrst = trip.PassFrst,
                    Invdate = trip.InvDate ?? DateTime.MinValue,
                    Company = car.Company,
                    Autocity = car.Autocity,
                    Autostat = car.Autostat,
                    Rentdate = car.RentDate ?? DateTime.MinValue,
                    Days = trip.PlusMin * car.Days,
                    Abookrat = car.Abookrat,
                    Cartype = car.CarType,
                    Milecost = car.Milecost,
                    Ratetype = car.RateType,
                    TripCost = car.Abookrat * car.Days,
                    Confirmno = car.Confirmno,
                    TranType = car.Cartrantyp,
                    ReasCodV = car.ReasCoda,
                    CplusMin = car.CPlusMin
                });
            }
        }

        private void AddHotels(TravDetRawData trip, int plusMin)
        {
            foreach (var hotel in _travDetShared.Hotels.Where(s => s.RecKey == trip.RecKey).OrderBy(x=>x.InvDate))
            {
                FinalDataList.Add(new TravDet1FinalData
                {
                    Rectype = "D",
                    Reckey = trip.RecKey,
                    Recloc = trip.RecLoc,
                    Invoice = trip.Invoice,
                    Ticket = trip.Ticket,
                    Plusmin = plusMin,
                    Acct = trip.Acct,
                    Acctname = Globals.AccountName,
                    Brkname1 = Globals.User.Break1Name,
                    Brkname2 = Globals.User.Break2Name,
                    Brkname3 = Globals.User.Break2Name,
                    Break1 = trip.Break1,
                    Break2 = trip.Break2,
                    Break3 = trip.Break3,
                    PassLast = trip.PassLast,
                    PassFrst = trip.PassFrst,
                    Invdate = trip.InvDate ?? DateTime.MinValue,
                    HotelNam = hotel.HotelNam,
                    HotCity = hotel.HotCity,
                    HotState = hotel.HotState,
                    DateIn = hotel.DateIn ?? DateTime.MinValue,
                    Nights = hotel.HPlusMin * hotel.Nights,
                    Rooms = hotel.HPlusMin * hotel.Rooms,
                    BookRate = hotel.BookRate,
                    RoomType = hotel.RoomType,
                    Guarante = hotel.Guarante,
                    TripCost = hotel.BookRate * hotel.Nights * hotel.Rooms,
                    Confirmno = hotel.ConfirmNo,
                    TranType = hotel.HotTranTyp,
                    ReasCodV = hotel.ReasCodh,
                    HPlusMin = hotel.HPlusMin
                });
            }
        }

        private string GetExchangeInfo(TravDetRawData trip)
        {
            string exchangeInfo;
            if (IsReservation || !trip.Exchange)
            {
                exchangeInfo = string.Empty;
            }
            else
            {
                exchangeInfo = @"** This is an ""Exchange"" record.";
                if (!trip.OrigTicket.IsNullOrWhiteSpace())
                    exchangeInfo += @"  Original Ticket # was " +
                                    trip.OrigTicket.Trim();
            }
            return exchangeInfo;
        }

        private List<string> GetExportFields()
        {
            var fieldList = new List<string>();
            //TODO
            return fieldList;
        }

        private decimal GetSvcFee(TravDetRawData trip)
        {
            decimal svcFee;
            if (_travDetShared.UseServiceFees && _travDetShared.ServiceFees.Count > 0)
            {
                var temp = _travDetShared.ServiceFees.Where(s => s.RecKey == trip.RecKey).Select(x=> x);
                svcFee = temp == null ? 0 : temp.Sum(x=> x.SvcFee);
            }
            else
            {
                svcFee = trip.SvcFee;
            }
            return svcFee;
        }

        private void ProcessExcpCond()
        {
            var excpCond = Globals.GetParmValue(WhereCriteria.DDAIRRAILCARHOTELOPTIONS).ToUpperInvariant();
            NoCar = false;
            NoHotel = false;
            List<TravDet1FinalData> tempData;
            //Refactor the following
            if (excpCond.IsNullOrWhiteSpace() || excpCond == "ALL RECORDS") return;

            if (new[] { "AIR ONLY", "AIR/RAIL ONLY", "AIR/RAIL ON" }.Any(s => excpCond.Contains(s)))
            {
                tempData = FinalDataList.Where(s => new[] { "C", "D" }.Any(x => s.Rectype.Contains(x))).ToList();
                FinalDataList.RemoveAll(s => tempData.Exists(x => x.Reckey == s.Reckey));
                NoCar = true;
                NoHotel = true;
            }
            else if (excpCond.EqualsIgnoreCase("CAR ONLY")) //&& TRIPS WITH CAR RENTALS ONLY.
            {
                tempData = FinalDataList.Where(s => new[] { "B", "D" }.Any(x => s.Rectype.Contains(x))).ToList();
                FinalDataList.RemoveAll(s => tempData.Exists(x => x.Reckey == s.Reckey));
                NoHotel = true;
            }
            else if (excpCond.EqualsIgnoreCase("HOTEL ONLY")) //&& TRIPS WITH HOTEL BOOKINGS ONLY.
            {
                tempData = FinalDataList.Where(s => new[] { "B", "C" }.Any(x => s.Rectype.Contains(x))).ToList();
                FinalDataList.RemoveAll(s => tempData.Exists(x => x.Reckey == s.Reckey));
                NoCar = true;
            }
            else if (excpCond.EqualsIgnoreCase("NO HOTEL")) //&& TRIPS WITH NO HOTEL BOOKINGS.
            {
                tempData = FinalDataList.Where(s => s.Rectype == "D").ToList();
                FinalDataList.RemoveAll(s => tempData.Exists(x => x.Reckey == s.Reckey));
                NoHotel = true;
            }
            else if (excpCond.EqualsIgnoreCase("NO CAR")) //&& TRIPS WITH NO HOTEL BOOKINGS.
            {
                tempData = FinalDataList.Where(s => s.Rectype == "C").ToList();
                FinalDataList.RemoveAll(s => tempData.Exists(x => x.Reckey == s.Reckey));
                NoCar = true;
            }
            else if (excpCond.EqualsIgnoreCase("NO AIR")) //&& & TRIPS WITH NO AIR TRAVEL.
            {
                tempData = FinalDataList.Where(s => s.Rectype == "B").ToList();
                FinalDataList.RemoveAll(s => tempData.Exists(x => x.Reckey == s.Reckey));
            }
        }

        private void SortFinalData()
        {
            var brkSortByUserSettings = Globals.IsParmValueOn(WhereCriteria.CBBRKSORTBYUSERSETTINGS);
            if (brkSortByUserSettings)
            {
                //Is there a better way to do conditional OrderBy?
                FinalDataList = FinalDataList.
                    OrderBy(s => Globals.User.AccountBreak ? s.Acct : "").
                    ThenBy(s => UserBreaks.UserBreak1 ? s.Break1 : "").
                    ThenBy(s => UserBreaks.UserBreak2 ? s.Break2 : "").
                    ThenBy(s => UserBreaks.UserBreak3 ? s.Break3 : "").
                    ThenBy(s => s.PassLast).
                    ThenBy(s => s.PassFrst).
                    ThenBy(s => s.Invdate).ToList();
                return;
            }

            FinalDataList = FinalDataList.
                OrderBy(s => s.PassLast).
                ThenBy(s => s.PassFrst).
                ThenBy(s => s.Reckey).ToList();
        }

        private void ConvertCurrencies()
        {
            if (_travDetShared.RawDataList.Any()) _travDetShared.RawDataList = PerformCurrencyConversion(_travDetShared.RawDataList);
            if (_travDetShared.Cars.Any()) _travDetShared.Cars = PerformCurrencyConversion(_travDetShared.Cars);
            if (_travDetShared.Hotels.Any()) _travDetShared.Hotels = PerformCurrencyConversion(_travDetShared.Hotels);
            if (_travDetShared.Legs.Any()) _travDetShared.Legs = PerformCurrencyConversion(_travDetShared.Legs);
            if (_travDetShared.Segments.Any()) _travDetShared.Segments = PerformCurrencyConversion(_travDetShared.Segments);
            if (_travDetShared.ServiceFees.Any()) _travDetShared.ServiceFees = PerformCurrencyConversion(_travDetShared.ServiceFees);
        }
    }
}