using System.Collections.Generic;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined
{
    public static class UserReportCheckLists
    {

        public static List<string> TripClassColumns = new List<string> { "TRIPCLASS", "TRPCLASDSC", "TRPCLASCAT", "LEGSVCS", "LEGAIRCODS", "LEGCLSCATS", "FAREBASE1" };
        public static List<string> RouteColumns = new List<string> { "TRIPDEST", "TRIPORIGIN"
        ,"SEGROUTING","LEGAIRCODS","LEGSVCS","FAREBASE1","LEGCLSCATS","TRIPDSTCOD","TRPDSTCTRY","TRPDSTRGN","TRIPORGCOD","IATAORG", "IATADEST","ORGCTYCOD","ORGDETCNTY", "TRPORGCTRY","TRPORGRGN","TRIPMILES","TRIPCLASS","TRPCLASCAT","ROUNDTRIP","TRPCLASDSC"};

        //Lists of table columns
        public static List<string> TablesToCheck = new List<string> { "TRIPS", "LEGS", "RAIL", "AIRLEG", "TTRTRIPS", "TTRSEGS" };
        public static List<string> TripCarbonColumns = new List<string> { "TRIPCO2", "TRPAIRCO2", "TRPALTRCO2", "TRPALTCCO2", "TRIPAIRCO2" };
        public static List<string> AirCarbonColumns = new List<string> { "AIRCO2", "ALTRAILCO2", "ALTCARCO2", "AIR_AIRCO2", "AIR_ARALC2", "AIR_ACARC2", "RAL_AIRCO2", "RAL_ARALC2", "RAL_ACARC2" };
        public static List<string> CarCarbonColumns = new List<string> { "CARCO2", "TRIPCO2" };
        public static List<string> HotelCarbonColumns = new List<string> { "HOTELCO2", "TRIPCO2" };
        public static List<string> AltRailCarbonColumns = new List<string> { "ALTRAILCO2", "TRPALTRCO2", "RAL_ARALC2", "AIR_ARALC2" };
        public static List<string> AltCarbonColumns = new List<string> { "ALTCARCO2", "TRPALTCCO2", "RAL_ACARC2", "AIR_ACARC2" };
        public static List<string> WeightColumns = new List<string> { "TRIPCO2", "TRPAIRCO2", "TRPALTRCO2", "TRPALTCCO2", "TRIPAIRCO2", "AIRCO2", "ALTRAILCO2", "ALTCARCO2", "AIR_AIRCO2", "AIR_ARALC2", "AIR_ACARC2", "RAL_AIRCO2", "RAL_ARALC2", "RAL_ACARC2", "CARCO2", "TRIPCO2", "HOTELCO2", "TRIPCO2", "ALTRAILCO2", "TRPALTRCO2", "RAL_ARALC2", "AIR_ARALC2", "ALTCARCO2", "TRPALTCCO2", "RAL_ACARC2", "AIR_ACARC2" };

        public static List<string> ComboTables = new List<string> { "TRIPS", "HIBTRIPS", "IBTRIPS", "TRIPTLS", "LEGS", "AUTO", "HOTEL", "SVCFEE", "RAIL", "AIRLEG", "MISCSEGS", "CHGLOG", "AUTHRZR", "TRAVAUTH", "TRAVAUTH", "ONDTRIPS", "ONDMSEGS", "ONDLEGS", "MSTUR", "MSSEA", "MSLIM", "MSRAL" };
        public static List<string> AcctTables = new List<string> { "TRACS", "ACCTSPCL", "AXISPCL" };
        public static List<string> OldCols = new List<string> { "SACCT", "SACCOUNT", "SINVOICE", "SRECLOC", "SPASSLAST", "SPASSFRST", "SPAXNAME" };
        public static List<string> TripTables = new List<string> { "TRIPS", "HIBTRIPS", "IBTRIPS", "TTRTRIPS" };
        public static List<string> SummaryReports = new List<string> { "SUMCOMB", "SUMFSEGD", "SUMFSEGO", "SUMSEGD", "SUMSEGO", "SUMAIR", "SUMCAR", "SUMHOTEL" };
        
        //Lists of advanced parameters that can't be applied when gets RawData
        public static List<string> TripTlsAdvancedCrit = new List<string> { "VENDTYPES", "VENDNAMES", "TRPVENDCOD", "CCCOMPANY", "INVAMTNFEE", "FLTCOUNT", "HTLCOUNT", "CARCOUNT", "HTLAVGRATE", "CARAVGRATE", "INVQTR", "INVMONTH", "INVMTHNAM", "INVMTHABBR", "BKQTR", "BKMONTH", "BKMTHNAM", "BKMTHABBR", "BKYEAR" };
        public static List<string> MiscSegsAdvancedCrit = new List<string> { "MSARRDATE", "MSARRTIME", "MSDEPDATE", "MSDEPTIME", "CABINSEAT", "MXCHAINCOD", "MEALDESC", "NITECOUNT", "OPT", "ARRIVERMKS", "DEPARTRMKS", "MXTOURCODE", "MXTOURNAME", "TRNSFRRMKS", "MXVENDNAME", "MXSGSTATUS", "TOURCOUNT", "MSCONFIRM", "SEGAMT", "MSCLASS", "MSMONEYTYP", "MSDESTINAT", "MSEXCPRATE", "MSLOSSCODE", "MSORIGIN", "MSSVGCODE", "MSSEQNO", "SVCIDNBR", "MSSTNDRATE", "MSTRANTYPE", "SEGTYPE", "MSVENDCODE" };
        public static List<string> HotelAdvancedCrit = new List<string> { "ROOMTYPE", "HOTBKTYPE", "SMARTCTRH", "COMISABLEH", "HOTRATETYP", "HOTSEGSTAT", "HOTVOIDDAT", "TRDTRXNUMH", "HOTPROPID", "HSEQNO", "HSTATENAME", "HCTRYNAME", "METRO", "HOTSEGNUM", "HOTELADDR1", "HOTELADDR2", "HOTELZIP", "HOTCOUNTRY", "HCOMMISN" };
        public static List<string> CarAdvancedCrit = new List<string> { "CARBKTYPE", "SMARTCTRC", "COMISABLEC", "CARRATETYP", "CARSEGSTAT", "CARVOIDDAT", "TRDTRXNUMC", "CSEQNO", "AREGNCODE", "AREGNNAME", "ASTATENAME", "CARTYPE", "CARSEGNUM", "DROPCITYCD", "PICKCITYCD", "CCOMMISN", "ACONFIRMNO" };
        public static List<string> MarketSegsAdvancedCrit = new List<string> { "GROSSAMT", "SFARETAX", "SEGCOMMISN", "PRDCLASS", "PRDCLSCAT", "FIRSTALINE", "FIRSTFLTNO", "PRDFBASE", "SAMEALINE", "SAMEFBASE", "CONNECTIME", "SFLTSTATUS", "PRDAIRLINE" };
        public static List<string> TripsAdvancedCrit = new List<string> { "TKTCOUNT", "PAXCOUNT", "POTLNITES", "PRDOCTYNAM", "PRDCTYNAME", "INVCTRYCOD", "PRDCARRNAM", "CLIENTNAME" };
        public static List<string> OnDmSegsAdvancedCrit = new List<string> { "GROSAVGCPM", "ODOTRMETRO", "SEGDEST", "ODOTRCTRY", "MKTDSTATE", "MKTDCNAMS", "MKTDANAMS", "EQUIPDESC", "MKTNDCNAMS", "MKNDMETRO", "MKNDANAMS", "MFLTNO", "MDITCODE", "MAIRLINE", "MKTSEG", "SEGFARE", "MKTSEGBOTH" };
        public static List<string> LimoAdvancedCrit = new List<string> { "LIMARRDATE", "LIMARRTIM", "LIMBASPR1", "LIMBASPR2", "LIMCOMMSN", "LIMDESTNAM", "LIMRATTYP", "LIMSGSTAT", "LIMTOTAMT", "LIMVENDNAM", "LIMSEGNUM" };
        public static List<string> RailTicketAdvancedCrit = new List<string> { "RALARRDATE", "RALARRTIM", "RALBASPR1", "RALBASPR2", "RALCLASCOD", "RALCONFNBR", "RALDEPDATE", "RALDEPTIM", "RALDESTNAM", "RALDSTSTN", "RALMEALS", "RALORGSTN", "RALORIGNAM", "RALSEGNUM", "RALSEGSTAT", "RALTOTAMT", "RALTRNNBR", "RALVENDCOD", "RALVENDNAM" };
        public static List<string> CruiseAdvancedCrit = new List<string> { "SEABASPR1", "SEABASPR2", "SEACABNCAT", "SEACABNLOC", "SEACABNNBR", "SEACABNTYP", "SEACOMMSN", "SEADURATN", "SEAPGMID", "SEAREGION", "SEAREGNID", "SEAROOMS", "SEASEGNUM", "SEASGSTAT", "SEASHIPNAM", "SEASPCLINF", "SEATOTAMT", "SEAVENDCOD", "SEAVENDNAM" };
        public static List<string> TourAdvancedCrit = new List<string> { "TURARRDATE", "TURBASPR1", "TURBASPR2", "TURBKNGS", "TURCOMMSN", "TURDEST", "TURORGCODE", "TURDESTNAM", "TURDURATN", "TURORIGNAM", "TURPGMID", "TURREGION", "TURREGNID", "TURROOMS", "TURSEGNUM", "TURSGSTAT", "TURSPCLINF", "TURTOTAMT", "TURVENDNAM" };

    }
}
