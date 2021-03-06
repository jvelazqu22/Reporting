﻿namespace Domain.Helper
{
    public enum RoutingCriteriaType
    {
        Airport,
        Metro,
        Region,
        Country
    }

    public enum AndOr
    {
        And = 1,
        Or = 2
    }

    public enum Operator
    {
        Equal,
        GreaterThan,
        GreaterOrEqual,
        Between,
        Empty,
        InList,
        NotBetween,
        NotInList,
        NotEmpty,
        Lessthan,
        LessThanOrEqual,
        NotEqual,
        Like,
        NotLike
    }

    public enum AllRecords
    {
        All = 1,
        Specified = 2,
        AllExceptSpecified = 3
    }

    public enum ClientType
    {
        Agency,
        Sharer
    }

    public enum DestinationSwitch // Correlates to ReportHandoff ParmValue for ParmName = "OUTPUTTYPE" 
    {
        None = 0,
        CrystalReport = 1,
        PortableDocFormat = 3,
        Xls = 2,
        Csv = 5,
        XML = 9,
        PPT = 10, //For TravelOptix broadcast report
        XlsxFormatted = 98, //actual output type is "XX", but this is iBank so why should anything make sense
        ClassicPdf = 99 // Should be deprecated, but is it possible to get this type?
    }
    public enum BackofficeOrReservation
    {
        Reservation = 1,
        Backoffice = 2
    }

    public enum Mode
    {
        BOTH = 0,
        AIR = 1,
        RAIL = 2
    }

    //Handles the first 290 criteria. Others will not be able to use the enum, as they are not contiguous. 
    public enum WhereCriteria
    {
        BEGDATE = 1,
        ENDDATE = 2,
        DATERANGE = 3,
        PREPOST = 4,
        INVCRED = 5,
        ACCT = 6,
        INACCT = 7,
        BREAK1 = 8,
        INBREAK1 = 9,
        BREAK2 = 10,
        INBREAK2 = 11,
        BREAK3 = 12,
        INBREAK3 = 13,
        VALCARR = 14,
        INVALCARR = 15,
        PASSLAST = 16,
        PASSFIRST = 17,
        DOMINTL = 18,
        BRANCH = 19,
        AGENTID = 20,
        INVOICE = 21,
        TICKET = 22,
        REASCODE = 23,
        PSEUDOCITY = 24,
        CREDCARD = 25,
        CARDNUM = 26,
        CBGENERIC1 = 27,
        RECLOC = 28,
        SEGCARR2 = 29,
        SEGCARR3 = 30,
        UDIDNBR = 31,
        UDIDTEXT = 32,
        COUNTRY = 33,
        SORTBY = 34,
        RBGENERIC1 = 35,
        TXTGENERIC1 = 36,
        HOWMANY = 37,
        RBGENERIC2 = 38,
        ORIGIN = 39,
        INORGS = 40,
        DESTINAT = 41,
        INDESTS = 42,
        AIRLINE = 43,
        INAIRLINE = 44,
        UDRKEY = 45,
        DDGENERIC1 = 46,
        INREASCODE = 47,
        STARTMONTH = 48,
        STARTYEAR = 49,
        BEGDATE2 = 50,
        ENDDATE2 = 51,
        COMPNAME = 52,
        CBGENERIC2 = 53,
        TITLEACCT2 = 54,
        CBGENERIC3 = 55,
        NOTINACCT = 56,
        NOTINBRK1 = 57,
        NOTINBRK2 = 58,
        NOTINBRK3 = 59,
        NOTINREAS = 60,
        HOTCHAIN = 61,
        INHOTCHAINS = 62,
        NOTINHOTCHAINS = 63,
        NOTINROOMTYPE = 64,
        ROOMTYPE = 65,
        INROOMTYPES = 66,
        HOTELNAM = 67,
        HOTCITY = 68,
        HOTSTATE = 69,
        SUBLOGTYPE = 70,
        INSUBLOGTYPE = 71,
        NOTINSUBLOGTYPE = 72,
        PREPOSTHOT = 73,
        RBGENERIC3 = 74,
        CBGENERIC4 = 75,
        DDGENERIC2 = 76,
        DDGENERIC3 = 77,
        CBGENERIC5 = 78,
        RPTTITLE2 = 79,
        NOTINCARCOMP = 80,
        CARCOMP = 81,
        INCARCOMPS = 82,
        CARTYPE = 83,
        INCARTYPES = 84,
        AUTOCITY = 85,
        AUTOSTAT = 86,
        RATETYPE = 87,
        NOTINCARTYPE = 88,
        PREPOSTCAR = 89,
        CBGENERIC6 = 90,
        REPORTKEY = 91,
        INREPORTKEY = 92,
        NOTINVALCARR = 93,
        NOTINORIGIN = 94,
        NOTINDESTS = 95,
        NOTINAIRLINES = 96,
        CBINCLVOIDS = 97,
        RUNOFFLINE = 98,
        PROCESSID = 99,
        OFFLINEEMAIL = 100,
        USERNMBR = 101,
        MAILFORMAT = 102,
        PARENTACCT = 103,
        INPARENTACCT = 104,
        NOTINPARENTACCT = 105,
        GROUPBY = 106,
        CHANGECODE = 107,
        INCHANGECODE = 108,
        NOTINCHANGECODE = 109,
        MODE = 110,
        ORIGCOUNTRY = 111,
        INORIGCOUNTRY = 112,
        NOTINORIGCOUNTRY = 113,
        DESTCOUNTRY = 114,
        INDESTCOUNTRY = 115,
        NOTINDESTCOUNTRY = 116,
        ORIGREGION = 117,
        INORIGREGION = 118,
        NOTINORIGREGION = 119,
        DESTREGION = 120,
        INDESTREGION = 121,
        NOTINDESTREGION = 122,
        CBGENERIC7 = 123,
        OBSERVEDST = 124,
        TIMEZONE = 125,
        UDIDLBL1 = 126,
        UDIDLBL2 = 127,
        UDIDONRPT1 = 128,
        UDIDONRPT2 = 129,
        MTGGRPNBR = 130,
        BEGHOUR = 131,
        BEGMINUTE = 132,
        BEGAMPM = 133,
        CBGENERIC8 = 134,
        TRAVREAS = 135,
        INTRAVREAS = 136,
        NOTINTRAVREAS = 137,
        MONEYTYPE = 140,
        GLOBALDATEFMT = 141,
        ENDHOUR = 142,
        ENDMINUTE = 143,
        ENDAMPM = 144,
        ENDMONTH = 145,
        ENDYEAR = 146,
        VAPTRANCOD = 147,
        INVAPTRANCOD = 148,
        NOTINVAPTRANCOD = 149,
        AXISITE = 151,
        INAXISITES = 152,
        NOTINAXISITES = 153,
        CALLERTYPE = 157,
        INCALLERTYPE = 158,
        NOINCALLERTYPE = 159,
        RESTYPE = 160,
        INRESTYPE = 161,
        NOTINRESTYPE = 162,
        SVCTYPE = 163,
        INSVCTYPE = 164,
        NOTINSVCTYPE = 165,
        ACTIONTYPE = 166,
        INACTIONTYPE = 167,
        NOTINACTIONTYPE = 168,
        AREASNTYPE = 169,
        INAREASNTYPE = 170,
        NOTINAREASNTYPE = 171,
        ATAKENTYPE = 172,
        INATAKENTYPE = 173,
        NOTINATAKENTYPE = 174,
        IAOPER = 175,
        INIAOPER = 176,
        NOTINIAOPER = 177,
        EPROFILENO = 178,
        EXPORTTYPE = 179,
        CRITOUT = 180,
        TXTGENERIC2 = 181,
        TXTGENERIC3 = 182,
        INGENERIC1 = 183,
        NOTINGENERIC1 = 184,
        SOURCEABBR = 185,
        INSOURCEABBR = 186,
        NOTINSOURCEABBR = 187,
        METROORG = 188,
        INMETROORGS = 189,
        NOTINMETROORGS = 190,
        METRODEST = 191,
        INMETRODESTS = 192,
        NOTINMETRODESTS = 193,
        FIRSTORIGIN = 194,
        FIRSTDEST = 195,
        CBGENERIC9 = 196,
        CBGENERIC10 = 197,
        CBGENERIC11 = 198,
        CBGENERIC12 = 199,
        GICCODE = 200,
        UDIDLBL3 = 201,
        UDIDONRPT3 = 202,
        UDIDLBL4 = 203,
        UDIDONRPT4 = 204,
        UDIDLBL5 = 205,
        UDIDONRPT5 = 206,
        UDIDLBL6 = 207,
        UDIDONRPT6 = 208,
        UDIDLBL7 = 209,
        UDIDONRPT7 = 210,
        UDIDLBL8 = 211,
        UDIDONRPT8 = 212,
        UDIDLBL9 = 213,
        UDIDONRPT9 = 214,
        UDIDLBL10 = 215,
        UDIDONRPT10 = 216,
        ACCTCAT1 = 221,
        INACCTCAT1 = 222,
        NOTINACCTCAT1 = 223,
        ACCTCAT2 = 224,
        INACCTCAT2 = 225,
        NOTINACCTCAT2 = 226,
        ACCTCAT3 = 227,
        INACCTCAT3 = 228,
        NOTINACCTCAT3 = 229,
        ACCTCAT4 = 230,
        INACCTCAT4 = 231,
        NOTINACCTCAT4 = 232,
        ACCTCAT5 = 233,
        INACCTCAT5 = 234,
        NOTINACCTCAT5 = 235,
        PCC = 236,
        INPCC = 237,
        NOTINPCC = 238,
        GDS = 239,
        INGDS = 240,
        NOTINGDS = 241,
        BKNGTOOL = 242,
        INBKNGTOOL = 243,
        NOTINBKNGTOOL = 244,
        RAILCARR = 245,
        LLRECNO = 246,
        MILEAGETABLE = 251,
        SEGFAREMILEAGE = 252,
        CARBONEMISSIONS = 253,
        ALTERNATEEMISSNS = 254,
        CARBONCALC = 255,
        METRIC = 256,
        USERID = 260,
        INUSERID = 261,
        NOTINUSERID = 262,
        TARGETUSERID = 263,
        INTARGETUSERID = 264,
        NOTINTARGUSERID = 265,
        TARGETORG = 266,
        INTARGETORG = 267,
        NOTINTARGETORG = 268,
        TARGSTYLEGRP = 269,
        INTARGSTYLEGRP = 270,
        NOTINTARGSTYLGRP = 271,
        ORGANIZATION = 272,
        INORGANIZATION = 273,
        NOTINORGANIZTN = 274,
        HOMECTRY = 275,
        INHOMECTRY = 276,
        NOTINHOMECTRY = 277,
        URBANMKT = 278,
        INURBANMKT = 279,
        NOTINURBANMKT = 280,
        CHPARENT = 281,
        INCHPARENT = 282,
        NOTINCHPARENT = 283,
        HOTELBRAND = 284,
        INHOTELBRAND = 285,
        NOTINHOTELBRAND = 286,
        PHOTSETNBR = 287,
        INPHOTSETNBR = 288,
        NOTINPHOTSETNBR = 289,
        AGENCY = 801,
        OUTPUTLANGUAGE = 850,
        OUTPUTTYPE = 901,
        OUTPUTDEST = 902,
        CBEXCLUDESVCFEES = 1101,
        CBSHOWBRKBYDOMINTL = 1102,
        CBINCLUDEYTDTOTALS = 1103,
        CBEXCLUDEMILEAGE = 1104,
        RBCITYPAIRCOMBINEORNOT = 1105,
        RBSORTDESCASC = 1106,
        RBAPPLYTOLEGORSEG = 1107,
        TXTFLTSEGMENTS = 1108,
        TXTFYSTARTMTH = 1109,
        RBONEWAYBOTHWAYS = 1110,
        RBRPTVERSION = 1111,
        DDRPTOPTION = 1112,
        DDCOSMETIC = 1113,
        CBEXCLUDESVGS = 1114,
        CBINCLBREAKBYDATE = 1115,
        CBUSEYTDNBRS = 1116,
        CBCOUNTTKTSNOTSEGS = 1117,
        CBPGBRKHOMECTRY = 1118,
        CBEXCLUDEEXCEPTNS = 1119,
        CBTRANDATEWITHINRANGE = 1120,
        CBRPTVERSION = 1121,
        CBEXCLUDENEGOT = 1122,
        CBINCLSVCFEENOMATCH = 1123,
        CBEXCLEXCHINFO = 1124,
        CBINCLUDEDATASUBSEQ = 1125,
        CBEXCLUDEONLINEADOPT = 1126,
        CBUSEBASEFARE = 1127,
        CBSEPARATERAIL = 1128,
        CBINCLUDEONEWAY = 1129,
        CBPRINTBRKINFOINBODY = 1130,
        CBSUMPAGEONLY = 1131,
        CBINCLUDELOSTSVGS = 1132,
        CBBRKSORTBYUSERSETTINGS = 1133,
        CBINCLTRIPSWITHHOTELS = 1134,
        CBUSEAIRPORTCODES = 1135,
        RBINADVANCERECORDS = 1136,
        DDAIRRAILCARHOTELOPTIONS = 1137,
        CBALLEXCEPTFLTSEGS = 1138,
        CBSUPPRESSAVGDIFF = 1139,
        CBIGNOREBRKSETTINGS = 1140,
        CBSUPPRESSALLOTHERS = 1141,
        CBEXCLUDELOWFARE = 1142,
        CBEXCLUDECARONLY = 1143,
        CBDISLPAYHOMECTRY = 1144,
        CBBRKBYAGENTID = 1145,
        CBGRPBYHOMECTRY = 1146,
        SUPPRESSCRIT = 1147,
        SEGCARR1 = 1148,
        PREPOSTAIR = 1149,
        RBONGRAPHSSHOW = 1150,
        RBFLTMKTONEWAYBOTHWAYS = 1151,
        NBRPASSENGERS = 1152,
        NBRDAYSDURATION = 1153,
        NBRDAYSINADVANCE = 1154,
        DDTRAVETSYESNO = 1155,
        DDEMAILFILTERCHECKED = 1156,
        CBINCLUDEPAXCOUNTBYFLT = 1157,
        CBINCLUDEPGBRKBYDATE = 1158,
        CBINCLUDEALLLEGS = 1159,
        CBUSECONNECTLEGS = 1160,
        CBEXCLUDEHOTELINFO = 1161,
        CBSUPPRPTBRKS = 1162,
        TXTALERTCITIES = 1163,
        CBINCLPCTLOSS = 1164,
        CBEXCLPUBFARE = 1165,
        CBDONOTDERIVESVGCODE = 1166,
        RBTRIPDURATION = 1167,
        NBRDAYSTRIPDUR = 1168,
        RBTRAVBRKOPTION = 1169,
        CBINCLTAXBRKDN = 1170,
        CBINCLHOTCARCOSTS = 1171,
        DDSUPPDUPETRIPFLDS = 1172,
        DDTIMEFORMAT = 1173,
        NBRTTEXPIREDAYS = 1174,
        NBRTTDAYSBEFORETODAY = 1175,
        CBTTTKTSWITHOPENSEG = 1176,
        DDTTRTKTSEGSTATUS = 1177,
        CBTTRINCLCANCTRIPS = 1178,
        DDTTRREFUNDTYPE = 1179,
        CBFINDTRAVELERLOCN = 1180,
        CBEXCLTRAVSARRIVINGHOME = 1181,
        CBSELECTDATACHANGED = 1182,
        TXTLASTSELECTDATACHANGED = 1183,
        DDDATACHANGEDHRSMINS = 1184,
        DDPASSENGERXMLRECORD = 1185,
        CBONLYSEGSCTRYDIFF = 1186,
        INSPECMASKINGAIRLINES = 1187,
        CBUSECLASSCATS = 1188,
        CBINCLUDERPTTOTALS = 1189,
        CBAPPLYBOTHDIRS = 1190,
        DDGRPFIELD = 1191,
        CBINCLLOWFARELOSTSVGS = 1192,
        DDWHICHDEST = 1193,
        CBINCLIPADDRESS = 1194,
        CBEXCLEVENTCODE101 = 1195,
        TXTIPADDRESS = 1196,
        DDEVENTCATEGORY = 1197,
        DDEVENTCODE = 1198,
        DDAGENTTYPE = 1199,
        DDSHOWCOUNTSORFARE = 1200,
        DDADMINLVL = 1201,
        DDRPTACCESS = 1202,
        DDANALYTICS = 1203,
        CBSUPPPASSWORDS = 1204,
        CBINCLALTAUTHS = 1205,
        CBONLYBCASTERRORS = 1206,
        CBEXCLOFFLINERPTS = 1207,
        DDLASTRPTSUCCESS = 1208,
        CBSHOWCLIENTDETAIL = 1209,
        CBBREAKBYSOURCE = 1210,
        CBSUPPSUBTOTS = 1211,
        DDCREDCARDCOMP = 1212,
        TXTCCNUM = 1213,
        CBBRKBYUSERSETTINGS = 1214,
        CBBRKSPERUSERSETTINGS = 1215,
        CBRECONCILETOCCDATA = 1216,
        CBSUPPDETCHANGE = 1217,
        CBINCLEMAILADDR = 1218,
        CBINCLSUBTOTSBYFLT = 1219,
        DDCONCURRENTSEGS = 1220,
        DDTOPBRKCAT = 1221,
        CBINCLAPPRTVL = 1222,
        CBINCLDECLINEDTVL = 1223,
        CBINCLCANCTVL = 1224,
        CBINCLEXPREQS = 1225,
        CBINCLNOTIFONLY = 1226,
        CBINCLFAREINFO = 1227,
        CBINCLAUTHCOMMS = 1228,
        DDTRAVAUTH = 1229,
        TXTAPPRDECLCOMMS = 1230,
        CBONLYMSGSWITHERRS = 1231,
        TXTAUTHREQNBR = 1232,
        CBEXCLAPPRTVL = 1233,
        CBEXCLDECLINEDTVL = 1234,
        CBEXPIREDREQS = 1235,
        CBEXCLNOTIFONLY = 1236,
        DDHOTPROPMATCHSTAT = 1237,
        DDHOTPREFSTATUS = 1238,
        CBDISPTKTEXPDATE = 1239,
        CBDISPFAREBASISCODE = 1240,
        CBDISPAGENTID = 1241,
        CBINCLTBSTATUS = 1242,
        CBINCLAGTID = 1243,
        CBINCLMCOFLAG = 1244,
        CBINCLITINCANCEL = 1245,
        CBEXCLAIRLINE = 1246,
        DDTTRRPTBRKOPT = 1247,
        DDFAREDISPOPT = 1248,
        DDTTREVENTSUCCESS = 1249,
        DDLOGRPTTYPE = 1250,
        CBTTREXCLEVENT1 = 1251,
        CBTTREXCLEVENT2 = 1252,
        CBTTREXCLEVENT3 = 1253,
        CBTTREXCLEVENT4 = 1254,
        CBTTREXCLEVENT5 = 1255,
        CBSORTBYSVGSCODE = 1256,
        TXTTRIPPURPOSEUDID = 1257,
        TXTBILLABLEUDID = 1258,
        DDQUARTER = 1259,
        DDMTHQTR = 1260,
        DDRPTSUBTOTLVL = 1261,
        TXTACCT_BRKCRIT = 1262,
        BEGDATE3 = 1263,
        ENDDATE3 = 1264,
        CBBASECHARTONTRIPS = 1266,
        TRANDATECOMPARELABEL = 1267,
        INOOPCODES = 1268,
        NOTINOOPCODES = 1269,
        POWERMACRO = 1270,
        CBINCLCARRIERZZ = 1271,
        CBCONSOLIDATECHNGES = 1272,
        CBSUPPRESSSOURCESUM = 1273,
        CBSUPPRESSACCTSUM = 1274,
        CBSUPPRESSBRK1SUM = 1275,
        CBSUPPRESSBRK2SUM = 1276,
        CBSUPPRESSBRK3SUM = 1277,
        DDINVBYAGCY = 1278,
        CBCITYPAIRBYMETRO = 1279,
        TXTPARSEDTSTART = 1280,
        TXTPARSEDTEND = 1281,
        CHANGESTAMP = 1282,
        AVAILABLE = 1283,
        CANCELCODE = 1284,
        CANCELTIME = 1285,
        CHANGESTAMP2 = 1286,
        CANCELTIME2 = 1287,
        ACCTTYPE = 1288
    }

    public enum DateType
    {
        DepartureDate = 1,
        InvoiceDate = 2,
        BookedDate = 3,
        RoutingDepartureDate = 4,
        RoutingArrivalDate = 5,
        CarRentalDate = 6,
        HotelCheckInDate = 7,
        TransactionDate = 8,
        OnTheRoadDatesSpecial = 9,
        OnTheRoadDatesCarRental = 10,
        OnTheRoadDatesHotel = 11,
        AuthorizationStatusDate = 12,
        PostDate = 13,
        LastUpdate = 21
    }
    public enum SegmentOrLeg
    {
        Leg = 1,
        Segment = 2
    }

    /// <summary>
    /// Derived from ibproces.v4caption | ibproces.processkey
    /// </summary>
    public enum ReportTitles
    {
        AirActivity = 2,
        RailActivity = 3,
        Arrival = 4,
        Departures = 6,
        UpcomingPlans = 8,
        WeeklyTravelerActivity = 9,
        MarketShare = 10,
        PassengersOnAPlane = 12,
        AdvanceBookingAir = 14,
        AirActivityByUdid = 16,
        FareSavingsAir = 18,
        SameCity = 20,
        TripDuration = 21,
        ServiceFees = 22,
        ItineraryDetailCombined = 23,
        TripDetail = 24,
        ElectronicInvoiceCombined = 27,
        ClassOfService = 28,
        CarrierConcentration = 29,
        QuickSummaryByMonth = 30,
        ExecutiveSummaryByHomeCountry = 31,
        QuickSummary = 32,
        ExecutiveSummaryYearToYear = 34,
        ExecutiveSummaryWithGraphs = 36,
        TravelManagementSummary = 38,
        TopBottomValidatingCarriers = 40,
        TopBottomSegmentAirlines = 42,
        TopBottomCityPair = 44,
        TopBottomCars = 46,
        TopBottomHotels = 48,
        TopBottomAccountsAir = 50,
        TopBottomAccountsCar = 52,
        TopBottomCostCenter = 53,
        TopBottomAccountsHotel = 54,
        TopBottomTravelersCar = 55,
        TopBottomTravelersAir = 56,
        TopBottomTravelersHotel = 57,
        TopBottomUserDefinedFields = 58,
        TopTravelersCombined = 59,
        TopBottomExceptionReasons = 60,
        TopBottomExceptionTravelers = 62,
        TopBottomDestinations = 64,
        TopBottomCityPairRail = 66,
        AnalysisByCityCar = 70,
        AdvanceBookingsCar = 72,
        AnalysisByVendorCar = 74,
        CarActivity = 76,
        AnalysisByCityHotel = 80,
        AdvanceBookingsHotel = 82,
        AnalysisByVendorHotel = 84,
        HotelActivity = 86,
        MissedHotelOpportunities = 88,
        FareSavingsCar = 90,
        FareSavingsHotel = 92,
        ExceptionsAir = 100,
        ExceptionsCar = 102,
        ExceptionsHotel = 104,
        iBankEventLog = 118,
        SpecialProductivityAir = 120,
        ReportLog = 122,
        CompanyUsers = 124,
        ValidatingCarrier = 126,
        BroadcastReportsStatus = 127,
        SpecialistProductivityCombined = 128,
        AirActivityBySpecialistId = 130,
        AccountSummaryAirYTYComparison = 132,
        AccountSummaryAir12MonthTrend = 134,
        BroadcastReportOverview = 136,
        TransactionSummary = 138,
        ServiceFeeDetailByTrip = 140,
        ServiceFeeDetailByTransaction = 142,
        ServiceFeesSummary = 144,
        TransactionAnalysisByCreditCard = 146,
        PublishedSavings = 160,
        NegotiatedSavings = 162,
        TripChangesMeetAndGreet = 180,
        TripChangesAir = 182,
        TripChangesTypeSummary = 184,
        TripChangesSendOff = 186,
        ConcurrentSegmentsBooked = 201,
        TravelerByCountry = 205,
        CO2CombinedSummary = 221,
        CO2AirSummary = 222,
        PTARequestActivity = 241,
        PTATravelerDetail = 243,
        DocumentDeliveryLogReport = 245,
        TopTravelersAudited = 247,
        TravelAuditReasonsByMonth = 249,
        PTAKeyPerformanceIndicators = 251,
        TravelAtAGlance = 261,
        CreditCardTransactionDetail = 281,
        MasterHotelPropertyReview = 301,
        PreferredHotelPropertyReport = 302,
        AirUserDefinedReports = 501,
        HotelUserDefinedReports = 502,
        CarUserDefinedReports = 503,
        ServiceFeeUserDefinedReports = 504,
        TicketTrackerUserDefinedReports = 505,
        SavedFiltersList = 508,
        CustomReportLayout = 509,
        SummaryLevelUserDefinedReports1 = 511,
        SummaryLevelUserDefinedReports2 = 512,
        SummaryLevelUserDefinedReports3 = 513,
        SummaryLevelUserDefinedReports4 = 518,
        SummaryLevelUserDefinedReports5 = 519,
        CombinedUserDefinedReports = 520,
        iXMLUserDefinedExport = 581,
        TicketTrackerUnusedTickets = 601,
        TicketTrackerCustomerNotifications = 602,
        TicketTrackerLostValue = 611,
        TicketTrackerNonRefundableAlerts = 612,
        TicketTrackerDetailReport = 621,
        TicketTrackerLogReport = 651,
        TicketTrackerEmailLog = 652,
        TicketTrackerOverview = 653,
        TTAFareSavingsReport = 701,
        RadiusExecutiveSummary = 702,
        FirstPassYieldSummary_IM005 = 7001,
        TransactionDetail_IM205 = 7002,
        TransactionTypeSummary_IM010 = 7003,
        CallDetailByTraveler_IM215 = 7004,
        CallOutcomeSummary_IM015 = 7005,
        CallOutcomeBySpecialist_IM305 = 7006,
        CallOutcomeGrandSummary_IM315 = 7007,
        ProductivityDetail_IM325 = 7008,
        ProductivitySummary_IM330 = 7009,
        ValueAddedProducts = 7010,
        AXOTransactionUserDefined = 7051,
        AXOCallsUserDefined = 7052,
        AXOProductivityUserDefined = 7053,
        SpecialRequestDetail_IM225 = 7101,
        AXOExecutiveSummaryReport = 7111,
        OneScoreTravelScorecard = 7200,
        ExpandedOneScoreTravelScorecard = 7201,
        OneScoreDetail = 7202,
        iBankStandardExtract = 9101,
        TravelOptixReport = 10500,

    }

    public enum BroadcastServerFunction
    {
        /// <summary>
        /// Processes both standard and offline batches
        /// </summary>
        Primary = 1,
        /// <summary>
        /// Processes offline batches
        /// </summary>
        Offline = 2,
        /// <summary>
        /// Processes eFFECTS batches
        /// </summary>
        Hot = 3,
        /// <summary>
        /// Handles stage 
        /// </summary>
        Stage = 4,
        /// <summary>
        /// Server with increased logging
        /// </summary>
        Logging = 5,
        /// <summary>
        /// Handles broadcasts that have a date range over x months,
        ///  where x is held in ibankmasters.dbo.broadcast_long_running_threshold
        /// </summary>
        LongRunning = 6
    }

    public enum ReportServerFunction
    {
        Primary = 1,
        Stage = 2
    }

    public enum ReportType
    {
        StandardReport,
        BcstReport
    }

    public enum SuppressTripDuplicateType
    {
        TextDateAndNumberFields,
        TextAndDateFieldsOnly,
        NumberFieldsOnly,
        NoSuppress
    }
    
}
