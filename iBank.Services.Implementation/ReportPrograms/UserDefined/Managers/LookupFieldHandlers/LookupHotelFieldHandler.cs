using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Helper;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using UserDefinedReports;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.LookupFieldHandlers
{
    public class LookupHotelFieldHandler
    {
        private readonly UserDefinedParameters _userDefinedParams;
        private readonly IMasterDataStore _masterStore;
        private readonly IClientDataStore _clientStore;
        private readonly ReportGlobals _globals;
        private ReportLookups _reportLookups;
        public List<Tuple<string, string>> TripSummaryLevel { get; set; }
        private readonly BuildWhere _buildWhere;

        public SegmentOrLeg _segmentOrLeg;

        public LookupHotelFieldHandler(UserDefinedParameters userDefinedParams, IMasterDataStore masterStore, IClientDataStore clientStore, 
            ReportGlobals globals, SegmentOrLeg segmentOrLeg, ReportLookups reportLookups, BuildWhere buildWhere, List<UserReportColumnInformation> columns)
        {
            _userDefinedParams = userDefinedParams;
            _masterStore = masterStore;
            _clientStore = clientStore;
            _globals = globals;
            _segmentOrLeg = segmentOrLeg;
            _reportLookups = reportLookups;
            TripSummaryLevel = new List<Tuple<string, string>>();
            _reportLookups = new ReportLookups(columns, buildWhere, globals);
        }

        public string HandleLookupFieldHotel(bool isPreview, UserReportColumnInformation column, RawData mainRec, int seqNo = 0)
        {
            var rec = _userDefinedParams.HotelLookup[mainRec.RecKey].FirstOrDefault(x => x.Seqctr == seqNo);

            if (rec == null) return string.Empty;

            mainRec.HasHotelData = true;

            switch (column.Name)
            {
                case "CHECKOUT":
                    //Convert this snippet to .net((ttod(t1.datein)+abs(t1.nights)))
                    return rec.Dateout.GetValueOrDefault().ToShortDateString();
                case "CHKOUTDOW":
                    //(left(cdow(dateout),3))
                    return rec.Dateout.GetValueOrDefault().DayOfWeek.ToString().Left(3);
                case "DATEINDOW":
                    // (left(cdow(datein),3))
                    return rec.Datein.GetValueOrDefault().DayOfWeek.ToString().Left(3);
                case "HCTRYNBR":
                    //luCtryNbr(T1.hotctrycod)
                    return LookupFunctions.LookupCountryNumber(_masterStore, rec.Hotctrycod);
                case "HCURSYMBOL":
                    //luCursymbol(moneytype)
                    return LookupFunctions.LookupCurrencySymbol(rec.Moneytype, _masterStore);
                case "HOTELCHAIN":
                    //luChain(t1.chaincod)
                    return LookupFunctions.LookupChains(rec.Chaincod, _masterStore);
                case "HOTREASN":
                    //luReason(t1.reascodh,t1.acct)
                    return _reportLookups.LookupReason(rec.Reascodh, _clientStore.ClientQueryDb, _globals.Agency, _globals.UserLanguage);
                case "HSEGCNTR":
                    //000
                    return "000";
                case "HSEQNO":
                    return rec.SeqNo.ToString();
                case "NITECOST":
                    //bookrate
                    return string.Format("{0:0.00}", rec.Bookrate);
                case "RMTYPEDESC":
                    //luRtype(t1.roomtype)
                    return LookupFunctions.LookupRoomType(rec.Roomtype, _globals.UserLanguage, _masterStore);
                case "HEXCPCOST":
                    // "curHotels.rooms * curHotels.nights * curHotels.hexcprat"
                    return string.Format("{0:0.00}", (rec.Rooms * rec.Nights * rec.Hexcprat));
                case "HOTCOST":
                    // "curHotels.rooms * curHotels.nights * curHotels.hexcprat"
                    return string.Format("{0:0.00}", (rec.Rooms * rec.Nights * rec.Bookrate));
                case "HOTLOST":
                    return string.Format("{0:0.00}", (Math.Abs(rec.Rooms) * Math.Abs(rec.Nights) * (rec.Bookrate - rec.Hexcprat)));
                case "HOTSTDCOST":
                    return string.Format("{0:0.00}", (rec.Rooms * rec.Nights * rec.Hotstdrate));
                case "HOTSTDSVGS":
                    return string.Format("{0:0.00}", (rec.Rooms * rec.Nights * (rec.Hotstdrate - rec.Bookrate)));
                case "HMONEYTYPE":
                    return rec.Moneytype;
                case "HSTATENAME":
                    return LookupFunctions.LookupStateName(rec.Hotstate, _masterStore);
                case "HCTRYNAME":
                case "HOTCOUNTRY":
                    if (isPreview)
                    {
                        //left(luAportCountryByMetro(curHotels.metro),20)
                        return LookupFunctions.LookupAirportCountryCodeByMetro(rec.Metro, _masterStore).Left(20);
                    }
                    return LookupFunctions.LookupCountryName(rec.Hotctrycod, _globals, _masterStore).PadRight(20).Left(20);
                case "HOTCTRYCOD":
                    if (isPreview)
                    {
                        //left(luAportCtryCodeByMetro(curHotels.metro),20)
                        return LookupFunctions.LookupAirportCountryCodeByMetro(rec.Metro, _masterStore).Left(20);
                    }
                    return rec.Hotctrycod;
            }

            return ReportBuilder.GetValueAsString(rec, column.Name);
        }
    }
}
