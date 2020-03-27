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
using Domain.Orm.Classes;
using Domain.Orm.iBankClientQueries;
using iBank.Repository.SQL.Repository;

using iBankDomain.RepositoryInterfaces;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;

namespace iBank.Services.Implementation.ReportPrograms.TravelDetail
{
    public class TravDet2 : ReportRunner<TravDetRawData, TravDet2FinalData>
    {
        private readonly TravDetShared _travDetShared;

        /// <summary>
        /// This report uses the raw data lists stored on TravDetShared
        /// </summary>
        public TravDet2()
        {
            CrystalReportName = "ibTravDet2";
            _travDetShared = new TravDetShared();
        }

        private bool IsReservation { get; set; }

        public override bool InitialChecks()
        {
            _travDetShared.IsReservation = Globals.ParmValueEquals(WhereCriteria.PREPOST, "1");
            UserBreaks = SharedProcedures.SetUserBreaks(Globals.User.ReportBreaks);

            if (!IsGoodCombo()) return false;

            if (!IsDateRangeValid()) return false;

            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsOnlineReport()) return false;

            return true;
        }

        public UserBreaks UserBreaks { get; set; }

        public override bool GetRawData()
        {
            if (!_travDetShared.GetRawData(BuildWhere)) return false;
            ConvertCurrencies();
            RawDataList = _travDetShared.RawDataList;
            return true;
        }

        public override bool ProcessData()
        {
            var server = Globals.AgencyInformation.ServerName;
            var db = Globals.AgencyInformation.DatabaseName;
            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(new iBankClientQueryable(server, db), Globals.Agency);

            if (!_travDetShared.ProcessDataShared(getAllMasterAccountsQuery)) return false;
            var exchText = "";
            var origTkt = "";
            var carRental = "";
            var hotelBooking = "";

            if (!IsReservation)
            {
                exchText = LookupFunctions.LookupLanguageTranslation("xExchangeBelow",
                    @"This is an ""Exchange"" record.", Globals.LanguageVariables);
                origTkt = LookupFunctions.LookupLanguageTranslation("xOrigTicketWas",
                   "Original Ticket # was", Globals.LanguageVariables);
                carRental = LookupFunctions.LookupLanguageTranslation("lm_CarRental",
                   "Car Rental", Globals.LanguageVariables);
                hotelBooking = LookupFunctions.LookupLanguageTranslation("ll_HotelBookings",
                   "Hotel Booking", Globals.LanguageVariables);
                if (hotelBooking == "Hotel Bookings") hotelBooking = "Hotel Booking";
            }
            var reasonsExclude = Globals.AgencyInformation.ReasonExclude.Split();
            //Note: The different types of data--legs, cars, hotels fill in any prexisiting items in the trip list.

            foreach (var t in _travDetShared.FilteredRouteData.ToList())
            {
                var trip = t;
                var tripList = new List<TravDet2FinalData>();//Holds 1..n rows for each t
                if (
                    _travDetShared.Legs.All(s => s.RecKey != trip.RecKey) &&
                    _travDetShared.Cars.All(s => s.RecKey != trip.RecKey) &&
                    _travDetShared.Hotels.All(s => s.RecKey != trip.RecKey)) continue;

                var offRdChg = trip.Offrdchg == 0 || Math.Abs(trip.Offrdchg) >
                    Math.Abs(trip.AirChg) ? trip.AirChg : trip.Offrdchg;
                var lostAmt = trip.AirChg - offRdChg;
                var reasCode = trip.Reascode.IsNullOrWhiteSpace() ||
                               reasonsExclude.All(s => s == trip.Reascode)
                    ? string.Empty : trip.Reascode;

                var row = new TravDet2FinalData
                {
                    RecKey = trip.RecKey,
                    PassLast = trip.PassLast.Trim(),
                    PassFrst = trip.PassFrst.Trim(),
                    Ticket = trip.Ticket.Trim(),
                    Invoice = trip.Invoice,
                    InvDate = trip.InvDate ?? DateTime.MinValue,
                    AirChg = trip.AirChg,
                    OffrdChg = offRdChg,
                    ReasCode = reasCode.Trim(),
                    LostAmt = lostAmt,
                    PlusMin = trip.PlusMin,
                    Break1 = GetBreak(UserBreaks.UserBreak1, trip.Break1),
                    Break2 = GetBreak(UserBreaks.UserBreak2, trip.Break2),
                    Break3 = GetBreak(UserBreaks.UserBreak3, trip.Break3),
                    ExchInfo = GetExchangeInfo(trip, exchText, origTkt),
                    RecLoc = trip.RecLoc.Trim()
                };
                SetAcct(getAllMasterAccountsQuery, trip, row);

                var legs = _travDetShared.Legs.Where(s => s.RecKey == trip.RecKey).ToList();
                var count = 0;
                foreach (var leg in legs)
                {
                    if (count > 0)
                    {
                        tripList.Add(row);
                        row = new TravDet2FinalData
                        {
                            RecKey = row.RecKey,
                            PassLast = row.PassLast.Trim(),
                            PassFrst = row.PassFrst.Trim(),
                            Ticket = row.Ticket.Trim(),
                            Invoice = row.Invoice,
                            InvDate = row.InvDate,
                            AirChg = row.AirChg,
                            OffrdChg = row.OffrdChg,
                            ReasCode = row.ReasCode.Trim(),
                            LostAmt = row.LostAmt,
                            PlusMin = row.PlusMin,
                            Acct = row.Acct.Trim(),
                            AcctDesc = row.AcctDesc.Trim(),
                            Break1 = row.Break1.Trim(),
                            Break2 = row.Break2.Trim(),
                            Break3 = row.Break3.Trim(),
                            RecType = "L",
                            ExchInfo = row.ExchInfo,
                            RecLoc = trip.RecLoc.Trim(),
                            AirIndicat = true
                        };
                    }
                    count++;
                    row.RdepDate = leg.RDepDate ?? DateTime.MinValue;
                    row.Origin = leg.Origin;
                    row.Destinat = leg.Destinat;
                    row.OrgDesc = AportLookup.LookupAport(MasterStore, leg.Origin, leg.Mode, Globals.Agency);
                    row.DestDesc = AportLookup.LookupAport(MasterStore, leg.Destinat, leg.Mode, Globals.Agency);
                    row.Airline = LookupFunctions.LookupAlineCode(MasterStore, leg.Airline, leg.Mode);
                    row.Class = leg.Class;
                    row.RecType = "L";
                    row.AirIndicat = true;
                }
                tripList.Add(row);
                var cars = _travDetShared.Cars.Where(s => s.RecKey == trip.RecKey).ToList();
                count = 0;
                foreach (var car in cars)
                {
                    if (count >= tripList.Count)
                    {
                        tripList.Add(new TravDet2FinalData
                        {
                            RecKey = row.RecKey,
                            PassLast = row.PassLast.Trim(),
                            PassFrst = row.PassFrst.Trim(),
                            Ticket = row.Ticket.Trim(),
                            Invoice = row.Invoice,
                            InvDate = row.InvDate,
                            AirChg = row.AirChg,
                            OffrdChg = row.OffrdChg,
                            ReasCode = row.ReasCode.Trim(),
                            LostAmt = row.LostAmt,
                            PlusMin = row.PlusMin,
                            Acct = row.Acct,
                            AcctDesc = row.AcctDesc,
                            Break1 = row.Break1,
                            Break2 = row.Break2,
                            Break3 = row.Break3,
                            ExchInfo = row.ExchInfo,
                            RecLoc = trip.RecLoc
                        });
                    }
                    row = tripList[count];
                    if (row.RdepDate == DateTime.MinValue) row.RdepDate = car.RentDate ?? DateTime.MinValue;
                    if (tripList[0].OrgDesc.IsNullOrWhiteSpace() &&
                        tripList[0].DestDesc.IsNullOrWhiteSpace())
                    {
                        tripList[0].OrgDesc = carRental + @" =>:"; //15
                        tripList[0].DestDesc = car.Autocity.Trim() + ", " + car.Autostat.Trim(); //16
                    }
                    row.Vendor = car.Company.Trim(); // 19
                    row.InDate = car.RentDate ?? DateTime.MinValue; //20
                    row.BookRate = car.Abookrat; //21
                    row.TypeCode = car.CarType; //22
                    row.Days = car.Days * trip.PlusMin; //23
                    row.Rooms = trip.PlusMin; //24
                    row.RecType = "C";
                    row.ReasCodV = car.ReasCoda.Trim();
                    row.CarCost = car.Abookrat * car.Days * trip.PlusMin * trip.PlusMin;
                    count++;

                }
                var hotels = _travDetShared.Hotels.Where(s => s.RecKey == trip.RecKey).ToList();
                count = cars.Count;
                foreach (var hotel in hotels)
                {
                    if (count >= tripList.Count)
                    {
                        tripList.Add(new TravDet2FinalData
                        {
                            RecKey = row.RecKey,
                            PassLast = row.PassLast.Trim(),
                            PassFrst = row.PassFrst.Trim(),
                            Ticket = row.Ticket.Trim(),
                            Invoice = row.Invoice.Trim(),
                            InvDate = row.InvDate,
                            AirChg = row.AirChg,
                            OffrdChg = row.OffrdChg,
                            ReasCode = row.ReasCode.Trim(),
                            LostAmt = row.LostAmt,
                            PlusMin = row.PlusMin,
                            Acct = row.Acct.Trim(),
                            AcctDesc = row.AcctDesc.Trim(),
                            Break1 = row.Break1.Trim(),
                            Break2 = row.Break2.Trim(),
                            Break3 = row.Break3.Trim(),
                            ExchInfo = row.ExchInfo.Trim(),
                            RecLoc = trip.RecLoc.Trim()
                        });
                    }
                    row = tripList[count];
                    if (row.RdepDate == DateTime.MinValue) row.RdepDate = hotel.DateIn ?? DateTime.MinValue;
                    if (tripList[0].OrgDesc.IsNullOrWhiteSpace() &&
                        tripList[0].DestDesc.IsNullOrWhiteSpace())
                    {
                        tripList[0].OrgDesc = hotelBooking + @" =>:";
                        tripList[0].DestDesc = hotel.HotCity.Trim() + ", " + hotel.HotState.Trim();
                    }
                    row.Vendor = hotel.HotelNam.Trim();
                    row.InDate = hotel.DateIn ?? DateTime.MinValue;
                    row.BookRate = hotel.BookRate;
                    row.TypeCode = hotel.RoomType.Trim();
                    row.Days = hotel.Nights * trip.PlusMin;
                    row.Rooms = hotel.Rooms * trip.PlusMin;
                    row.RecType = "H";
                    row.ReasCodV = hotel.ReasCodh.Trim();
                    row.HotelCost = hotel.BookRate * hotel.Nights * trip.PlusMin * hotel.Rooms * trip.PlusMin;
                    count++;
                }
                // Order segments by airline and departure date
                tripList = tripList.OrderBy(s => s.Airline.IsNullOrWhiteSpace())
                                    .ThenBy(s => s.RdepDate).ToList();
                // Create a second list to order car/hotel only segments
                var secondlist = tripList.Select(s => new
                {
                    s.Vendor,
                    s.InDate,
                    s.ReasCodV,
                    s.TypeCode,
                    s.Days,
                    s.Rooms,
                    s.CarCost,
                    s.HotelCost,
                    s.BookRate
                }).OrderBy(s => s.Vendor.IsNullOrWhiteSpace()).ThenBy(s => s.InDate).ToList();
                // JOin the two lists again
                foreach (var tr in tripList)
                {
                    var carhotel = secondlist.First();
                    tr.Vendor = carhotel.Vendor;
                    tr.InDate = carhotel.InDate;
                    tr.ReasCodV = carhotel.ReasCodV;
                    tr.TypeCode = carhotel.TypeCode;
                    tr.Days = carhotel.Days;
                    tr.Rooms = carhotel.Rooms;
                    tr.CarCost = carhotel.CarCost;
                    tr.HotelCost = carhotel.HotelCost;
                    tr.BookRate = carhotel.BookRate;
                    secondlist.Remove(carhotel);
                }
                FinalDataList.AddRange(tripList);
            }

            string travBrkOption = Globals.GetParmValue(WhereCriteria.RBTRAVBRKOPTION);
            TravBrk = travBrkOption == "2" || travBrkOption == "3";
            TravPgBrk = travBrkOption == "3";
            PageBrkLvl = 0;
            if (TravPgBrk)
            {
                if (UserBreaks.UserBreak3) PageBrkLvl = 3;
                else if (UserBreaks.UserBreak2) PageBrkLvl = 2;
                else if (UserBreaks.UserBreak1) PageBrkLvl = 1;
            }
            ProcessExcpCond();
            return DataExists(FinalDataList);
        }

        public int PageBrkLvl { get; set; }

        public bool TravBrk { get; set; }

        public bool TravPgBrk { get; set; }

        private void ProcessExcpCond()
        {
            var whereExcpn = string.Empty;
            var excpCond = Globals.GetParmValue(WhereCriteria.DDAIRRAILCARHOTELOPTIONS).ToUpperInvariant();
            List<TravDet2FinalData> tempData;
            if (!excpCond.IsNullOrWhiteSpace() && !excpCond.EqualsIgnoreCase("ALL RECORDS"))
            {
                if (new[] { "AIR ONLY", "AIR/RAIL ONLY", "AIR/RAIL ON" }.Any(s => excpCond.Contains(s)))
                {
                    tempData = FinalDataList.Where(s => new[] { "C", "H" }.Any(x => s.RecType.Contains(x))).ToList();
                    FinalDataList.RemoveAll(s => tempData.Exists(x => x.RecKey == s.RecKey));
                }
                else if (excpCond.EqualsIgnoreCase("CAR ONLY")) //&& TRIPS WITH CAR RENTALS ONLY.
                {
                    tempData = FinalDataList.Where(s => new[] { "L", "H" }.Any(x => s.RecType.Contains(x))).ToList();
                    FinalDataList.RemoveAll(s => tempData.Exists(x => x.RecKey == s.RecKey));
                }
                else if (excpCond.EqualsIgnoreCase("HOTEL ONLY")) //&& TRIPS WITH HOTEL BOOKINGS ONLY.
                {
                    tempData = FinalDataList.Where(s => new[] { "L", "C" }.Any(x => s.RecType.Contains(x))).ToList();
                    FinalDataList.RemoveAll(s => tempData.Exists(x => x.RecKey == s.RecKey));
                }
                else if (excpCond.EqualsIgnoreCase("NO HOTEL")) //&& TRIPS WITH NO HOTEL BOOKINGS.
                {
                    tempData = FinalDataList.Where(s => s.RecType == "H").ToList();
                    FinalDataList.RemoveAll(s => tempData.Exists(x => x.RecKey == s.RecKey));
                }
                else if (excpCond.EqualsIgnoreCase("NO CAR")) //&& TRIPS WITH NO HOTEL BOOKINGS.
                {
                    tempData = FinalDataList.Where(s => s.RecType == "C").ToList();
                    FinalDataList.RemoveAll(s => tempData.Exists(x => x.RecKey == s.RecKey));
                }
                else if (excpCond.EqualsIgnoreCase("NO AIR")) //&& & TRIPS WITH NO AIR TRAVEL.
                {
                    tempData = FinalDataList.Where(s => s.RecType == "L" || s.AirIndicat).ToList();
                    FinalDataList.RemoveAll(s => tempData.Exists(x => x.RecKey == s.RecKey));
                }
            }
            FinalDataList = FinalDataList.
                OrderBy(s => s.AcctDesc).
                ThenBy(s => s.Break1).
                ThenBy(s => s.Break2).
                ThenBy(s => s.Break3).
                ThenBy(s => s.PassLast).
                ThenBy(s => s.PassFrst).
                ThenBy(s => s.InvDate).
                ThenBy(s => s.RecKey).ToList();
            return;
        }

        private string GetBreak(bool useBreak, string breakStr)
        {
            if (!useBreak) return "^na^";
            return breakStr.IsNullOrWhiteSpace() ? "NONE" : breakStr.Trim();
        }
        private void SetAcct(IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, TravDetRawData trip, TravDet2FinalData finalData)
        {

            if (!Globals.User.AccountBreak)
            {
                finalData.Acct = "^na^";
                finalData.AcctDesc = "^na^";
                return;
            }
            if (trip.Acct.IsNullOrWhiteSpace())
            {
                finalData.Acct = "[None]";
                finalData.AcctDesc = "[No Acct #]";
                return;
            }
            finalData.Acct = trip.Acct;
            finalData.AcctDesc = clientFunctions.LookupCname(getAllMasterAccountsQuery, trip.Acct, Globals);
        }

        private string GetExchangeInfo(TravDetRawData trip, string exchText, string origTkt)
        {
            if (IsReservation || !trip.Exchange) return string.Empty;
            var exchangeText = @"** " + exchText;
            if (trip.OrigTicket.IsNullOrWhiteSpace()) return exchangeText;
            return exchangeText + " " + origTkt + " " + trip.OrigTicket.Trim();
        }

        public override bool GenerateReport()
        {
            if (Globals.OutputFormat == DestinationSwitch.Xls ||
                Globals.OutputFormat == DestinationSwitch.Csv)
            {
                var exportFields = GetExportFields();
                FinalDataList = ZeroOut<TravDet2FinalData>.Process(
                    FinalDataList, new List<string> { "airchg", "offrdchg", "lostamt" });
                if (Globals.OutputFormat == DestinationSwitch.Csv)
                    ExportHelper.ConvertToCsv(FinalDataList, exportFields, Globals);
                else
                    ExportHelper.ListToXlsx(FinalDataList, exportFields, Globals);

                return true;
            }
            var rptFilePath =
                StringHelper.AddBS(Globals.CrystalDirectory) + CrystalReportName + "." +
                Constants.CrystalReportExt;

            ReportSource = new ReportDocument();
            ReportSource.Load(rptFilePath);

            ReportSource.SetDataSource(FinalDataList);

            var missing = CrystalFunctions.SetupCrystalReport(ReportSource, Globals);
            ReportSource.SetParameterValue("LTRAVPGBRK", TravPgBrk);
            ReportSource.SetParameterValue("LTRAVBRK", TravBrk);

            CrystalFunctions.CreatePdf(ReportSource, Globals);

            return true;
        }

        private List<string> GetExportFields()
        {
            var fieldList = new List<string>();

            fieldList.Add("recloc");
            fieldList.Add("reckey");

            if (Globals.User.AccountBreak)
            {
                fieldList.Add("acct");
                fieldList.Add("acctdesc");
            }

            if (UserBreaks.UserBreak1)
                fieldList.Add("break1 as " + SharedProcedures.FixDbfColumnName(Globals.User.Break1Name).ToLowerInvariant());
            if (UserBreaks.UserBreak2)
                fieldList.Add("break2 as " + SharedProcedures.FixDbfColumnName(Globals.User.Break2Name).ToLowerInvariant());
            if (UserBreaks.UserBreak3)
                fieldList.Add("break3 as " + SharedProcedures.FixDbfColumnName(Globals.User.Break3Name).ToLowerInvariant());

            fieldList.Add("passlast");
            fieldList.Add("passfrst");
            fieldList.Add("ticket");
            fieldList.Add("invoice");
            fieldList.Add("invdate");
            fieldList.Add("airchg");
            fieldList.Add("offrdchg");
            fieldList.Add("reascode");
            fieldList.Add("lostamt");
            fieldList.Add("rdepdate");
            fieldList.Add("origin");
            fieldList.Add("destinat");
            fieldList.Add("orgdesc");
            fieldList.Add("destdesc");
            fieldList.Add("airline");
            fieldList.Add("class");
            fieldList.Add("vendor");
            fieldList.Add("indate");
            fieldList.Add("bookrate");
            fieldList.Add("typecode");
            fieldList.Add("days");
            fieldList.Add("rooms");
            fieldList.Add("carcost");
            fieldList.Add("hotelcost ");
            return fieldList;
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