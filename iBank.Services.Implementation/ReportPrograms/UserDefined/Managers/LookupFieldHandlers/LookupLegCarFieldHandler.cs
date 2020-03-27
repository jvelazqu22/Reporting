using System;
using System.Globalization;
using System.Linq;
using Domain.Helper;
using Domain.Models.ReportPrograms.UserDefinedReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;
using UserDefinedReports;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.LookupFieldHandlers
{
    public class LookupLegCarFieldHandler
    {
        private readonly UserDefinedParameters _userDefinedParams;
        private readonly IMasterDataStore _masterStore;
        private readonly IClientDataStore _clientStore;
        private readonly bool _isTitleCaseRequired;
        private readonly ReportGlobals _globals;
        private ReportLookups _reportLookups;

        public SegmentOrLeg _segmentOrLeg;

        public LookupLegCarFieldHandler(UserDefinedParameters userDefinedParams, IMasterDataStore masterStore, IClientDataStore clientStore,
            bool isTitleCaseRequired, ReportGlobals globals, ReportLookups reportLookups, SegmentOrLeg segmentOrLeg)
        {
            _userDefinedParams = userDefinedParams;
            _masterStore = masterStore;
            _clientStore = clientStore;
            _isTitleCaseRequired = isTitleCaseRequired;
            _globals = globals;
            _reportLookups = reportLookups;
            _segmentOrLeg = segmentOrLeg;
        }

        public string HandleLookupFieldCar(UserReportColumnInformation column, RawData mainRec, int seqNo)
        {
            var rec = _userDefinedParams.CarLookup[mainRec.RecKey].FirstOrDefault(x => x.SeqNo == seqNo);

            if (rec == null) return string.Empty;

            mainRec.HasCarData = true;

            switch (column.Name)
            {
                case "CARCOST":
                    if (rec.CPlusMin < 0 && rec.Abookrat > 0)
                        return string.Format("{0:0.00}", (Math.Abs(rec.Days) * rec.CPlusMin * rec.Abookrat));
                    return string.Format("{0:0.00}", (Math.Abs(rec.Days) * rec.Abookrat));
                case "CARLOST":
                    //abs(curCars.days) * (curCars.abookrat-curCars.aexcprat)
                    return string.Format("{0:0.00}", (Math.Abs(rec.Days) * (rec.Abookrat - rec.Aexcprat)));
                case "CARSTDCOST":
                    //curCars.days * curCars.carstdrate
                    return string.Format("{0:0.00}", (rec.Days * rec.Carstdrate));
                case "CARSTDSVGS":
                    //curCars.days * (curCars.carstdrate-curCars.abookrat)
                    return string.Format("{0:0.00}", (rec.Days * (rec.Carstdrate - rec.Abookrat)).ToString());
                case "AEXCPCOST":
                    //curCars.days * curCars.aexcprat
                    return string.Format("{0:0.00}", (rec.Days * rec.Aexcprat));
                case "AMONEYTYPE":
                    //curCars.moneytype
                    return rec.Moneytype;
                case "AREGNCODE":
                    //luCtryRegionCode(curCars.carctrycod)
                    return LookupFunctions.LookupCountryRegionCode(rec.Carctrycod, _masterStore);
                case "AREGNNAME":
                    //luRegionName(luCtryRegionCode(curCars.carctrycod))
                    return LookupFunctions.LookupRegionName(LookupFunctions.LookupCountryRegionCode(rec.Carctrycod, _masterStore), _masterStore, _isTitleCaseRequired);
                case "ASTATENAME":
                    if (string.IsNullOrEmpty(rec.Carctrycod) || rec.Carctrycod.EqualsIgnoreCase("USA"))
                        return LookupFunctions.LookupStateName(rec.Autostat, _masterStore);
                    return LookupFunctions.LookupCountryName(rec.Carctrycod, _globals, _masterStore).Left(20);
                case "CSEQNO":
                    //curCars.seqno
                    return rec.SeqNo.ToString();
                case "ACURSYMBOL":
                    //luCursymbol(moneytype)
                    return LookupFunctions.LookupCurrencySymbol(rec.Moneytype, _masterStore);
                case "CARREASN":
                    //luReason(t1.reascoda,t1.acct)
                    return _reportLookups.LookupReason(rec.Reascoda, _clientStore.ClientQueryDb, _globals.Agency, _globals.UserLanguage);
                case "CARTYPDESC":
                    //luCarType(t1.cartype)
                    return LookupFunctions.LookupCarType(rec.CarType, _globals.UserLanguage, _masterStore);
                case "CCTRYNBR":
                    //luCtryNbr(T1.carctrycod)
                    return LookupFunctions.LookupCountryNumber(_masterStore, rec.Carctrycod);
                case "CSEGCNTR":
                    //(000)
                    return "000";
                case "DAYCOST":
                    // (abookrat)
                    return string.Format("{0:0.00}", rec.Abookrat.ToString(CultureInfo.InvariantCulture));
                case "RENTDTDOW":
                    //(left(cdow(t1.rentdate),3))
                    return rec.Rentdate.GetValueOrDefault().DayOfWeek.ToString().Left(3);
                case "RETDATEDOW":
                    // (left(cdow(t1.dateback),3))
                    return rec.Dateback.DayOfWeek.ToString().Left(3);
                case "RETURNDATE":
                    //(ttod(t1.dateback))
                    return rec.Dateback.ToShortDateString();
                default:
                    return ReportBuilder.GetValueAsString(rec, column.Name);
            }
        }
    }
}
