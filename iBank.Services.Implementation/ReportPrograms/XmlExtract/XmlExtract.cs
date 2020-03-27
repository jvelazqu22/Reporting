using CODE.Framework.Core.Utilities.Extensions;
using com.ciswired.libraries.CISLogger;
using Domain.Helper;
using Domain.Models.ReportPrograms.XmlExtractReport;
using iBank.Server.Utilities;
using iBank.Services.Implementation.ReportPrograms.XmlExtract.Managers;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.XmlReport;

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

using Domain.Orm.iBankClientQueries;

using iBank.Services.Implementation.Shared.AdvancedClause;
using Domain;

namespace iBank.Services.Implementation.ReportPrograms.XmlExtract
{
    public class XmlExtract : ReportRunner<RawData, FinalData>
    {
        private static readonly ILogger Log = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private int _reportKey;
        private bool _isReservation;
        private bool _udidExists;

        private DataManager _dataManager;
        private XmlDataStructure _xmlDataStructure { get; set; }

        public XDocument XDoc { get; set; }

        public XmlExtractParameters XmlExtractParams = new XmlExtractParameters();

        public XmlExtract()
        {
        }

        public override bool InitialChecks()
        {
            if (!IsGoodCombo()) return false;

            if (!IsDateRangeValid()) return false;

            if (!IsUdidNumberSuppliedWithUdidText()) return false;

            if (!IsOnlineReport()) return false;

            if (!IsValidateKey()) return false;

            return true;
        }

        public override bool GetRawData()
        {
            Log.Debug($"GetRawData - Start | Agency:[{Globals.Agency}] | User:[{Globals.User.UserNumber}] | Report Key:[{_reportKey}]");
            var sw = Stopwatch.StartNew();

            _udidExists = Globals.GetParmValue(WhereCriteria.UDIDNBR).TryIntParse(-1) > 0;

            SetReportProperties();

            var bldDate = !Globals.ParmValueEquals(WhereCriteria.DATERANGE, "19");

            var getAllMasterAccountsQuery = new GetAllMasterAccountsQuery(ClientStore.ClientQueryDb, Globals.Agency);
            if (!BuildWhere.BuildAll(getAllMasterAccountsQuery, buildTripWhere: true, buildRouteWhere: true, buildCarWhere: false, buildHotelWhere: false, buildUdidWhere: true, buildDateWhere: bldDate, inMemory: true, isRoutingBidirectional: false, legDit: false, ignoreTravel: true)) return false;

            if (Features.UseBuildAdvancedClauses.IsEnabled())
            {
                BuildWhere.BuildAdvancedClauses();
            }
            else
            {
                BuildWhere.AddAdvancedClauses();
            }
            BuildWhere.AddSecurityChecks();

            _xmlDataStructure = new XmlDataStructure(_reportKey, Globals);
            _xmlDataStructure.RetrieveStructure();          
            if (_xmlDataStructure.HasError) return false;
            Log.Info($"GetRawData - RetrieveStructure Tags Count:[{_xmlDataStructure.Tags.Count}] | XmlDetails Name[{_xmlDataStructure.XmlDetails.Name.Trim()}] | Type:[{_xmlDataStructure.XmlDetails.Type.Trim()}] | Title:[{_xmlDataStructure.XmlDetails.Title.Trim()}]");

            _isReservation = GlobalCalc.IsReservationReport();
            
            _dataManager = new DataManager(Globals, BuildWhere, _xmlDataStructure.DataSwitches, XmlExtractParams, _udidExists, _isReservation, _xmlDataStructure.NeedCancelledTrips);
            var includeVoids = Globals.IsParmValueOn(WhereCriteria.CBINCLVOIDS);
            if ( Features.XmlExtractWhereClauseFull.IsEnabled() )
            {
                // this is related to US: 9676. Even if the trip is marked as voided some items such as hotel, car, svc, etcetera 
                // may not be marked as voided.
                var whereClauseFullWihtoutTranType = BuildWhere.WhereClauseFull;
                if (!includeVoids) whereClauseFullWihtoutTranType = BuildWhere.WhereClauseFull.Replace("T1.trantype <> @t1TranType1 AND ", "");

                _dataManager.SetRawData(whereClauseFullWihtoutTranType, _xmlDataStructure.XmlDetails.Type.EqualsIgnoreCase("ISOS"), _xmlDataStructure.XmlDetails.Type.EqualsIgnoreCase("USRISOS"));
            }
            else
            {
                _dataManager.SetRawData(BuildWhere.WhereClauseFull, _xmlDataStructure.XmlDetails.Type.EqualsIgnoreCase("ISOS"), _xmlDataStructure.XmlDetails.Type.EqualsIgnoreCase("USRISOS"));
            }

            if (!DataExists(XmlExtractParams.TripRawDataList)) return false;

            if (!IsUnderOfflineThreshold(XmlExtractParams.TripRawDataList)) return false;

            _dataManager.SetXmlExtractParameters(BuildWhere.WhereClauseFull, includeVoids);            

            //Now that we have Leg data, we can apply Route Where
            if (BuildWhere.HasRoutingCriteria)
            {
                XmlExtractParams.LegRawDataList = GlobalCalc.IsAppliedToLegLevelData() ? BuildWhere.ApplyWhereRoute(XmlExtractParams.LegRawDataList, true) : BuildWhere.ApplyWhereRoute(XmlExtractParams.LegRawDataList, false);
                //We also will need to limit all the other lists to the bookRateCounte reckeys
                var recKeys = XmlExtractParams.LegRawDataList.Select(s => s.RecKey).Distinct();
                XmlExtractParams.TripRawDataList = XmlExtractParams.TripRawDataList.Where(s => recKeys.Contains(s.RecKey)).ToList();
                XmlExtractParams.CarRawDataList = XmlExtractParams.CarRawDataList.Where(s => recKeys.Contains(s.RecKey)).ToList();
                XmlExtractParams.HotelRawDataList = XmlExtractParams.HotelRawDataList.Where(s => recKeys.Contains(s.RecKey)).ToList();
                XmlExtractParams.UdidRawDataList = XmlExtractParams.UdidRawDataList.Where(s => recKeys.Contains(s.RecKey)).ToList();
                XmlExtractParams.SvcFeeRawDataList = XmlExtractParams.SvcFeeRawDataList.Where(s => recKeys.Contains(s.RecKey)).ToList();
                XmlExtractParams.MktSegRawDataList = XmlExtractParams.MktSegRawDataList.Where(s => recKeys.Contains(s.RecKey)).ToList();
            }

            //So it won't be empty data list, populate FinalDataList for total record count
            RawDataList = XmlExtractParams.TripRawDataList;
            FinalDataList = XmlExtractParams.LegRawDataList.Select(s => new FinalData { RecKey = s.RecKey }).ToList();
            FinalDataList.AddRange(XmlExtractParams.CarRawDataList.Select(s => new FinalData { RecKey = s.RecKey }).ToList());
            FinalDataList.AddRange(XmlExtractParams.HotelRawDataList.Select(s => new FinalData { RecKey = s.RecKey }).ToList());
            FinalDataList.AddRange(XmlExtractParams.SvcFeeRawDataList.Select(s => new FinalData { RecKey = s.RecKey }).ToList());
            FinalDataList.AddRange(XmlExtractParams.MktSegRawDataList.Select(s => new FinalData { RecKey = s.RecKey }).ToList());

            sw.Stop();
            Log.Debug($"GetRawData End - Elapsed'[{ sw.Elapsed}]");

            return true;
        }

        public override bool ProcessData()
        {
            Log.Debug("ProcessData - Start");
            var sw = Stopwatch.StartNew();

            if (_isReservation)
            {
                foreach (var row in XmlExtractParams.TripRawDataList)
                {
                    row.Recloc = row.Recloc.PadRight(8);
                    row.RecLoc6 = row.Recloc.Trim().Left(6);
                    row.PassNbr = row.Recloc.Right(2).Trim();
                    row.PnrPaxCnt = 1;
                    row.CommonKey = row.RecKey;
                }
            }
            else
            {
                var paxCount = 0;
                var priorRecLoc = string.Empty;
                foreach (var row in XmlExtractParams.TripRawDataList)
                {
                    if (row.Recloc != priorRecLoc)
                    {
                        paxCount = 0;
                        priorRecLoc = row.Recloc;
                    }
                    paxCount++;
                    row.Recloc = row.Recloc.PadRight(8);
                    row.RecLoc6 = row.Recloc.Trim().Left(6);
                    row.PassNbr = paxCount == 1? string.Empty:(paxCount-1).ToString();
                    row.PnrPaxCnt = 1;
                    row.CommonKey = row.RecKey;
                    //row.PassNbr = XmlExtractParams.TripRawDataList.Count(s => s.Recloc.Trim().Equals(row.Recloc.Trim())).ToString();
                }

            }

            //Handle the advanced crit against all subtables, and remove all rows from raw data that have been dropped from others. 
            AdvancedWhere<LegRawData>.ApplyAdvancedWhere(XmlExtractParams.LegRawDataList, Globals.AdvancedParameters);

            var builder = new XMLReportBuilder(Globals, _xmlDataStructure, XmlExtractParams, _isReservation, MasterStore, ClientStore);

            //ISOS has a different schema, which can't be customized. 
            XDoc = _xmlDataStructure.XmlDetails.Type.EqualsIgnoreCase("ISOS") || _xmlDataStructure.XmlDetails.Type.EqualsIgnoreCase("USRISOS")
                ? builder.BuildIsosXml()
                : builder.BuildXmlDocument();

            sw.Stop();
            Log.Debug($"ProcessData - End | Elapsed:[{sw.Elapsed}]");

            return true;
        }

        public override bool GenerateReport()
        {
            string filename = Globals.GetFileName();
            XDoc.Save(filename);
            CustomReportEffects.ApplyEffects(filename, Globals);
            return true;
        }
        
        public void SetReportProperties()
        {

            if (Globals.IsOfflineServer && Globals.IsParmValueOn(WhereCriteria.CBSELECTDATACHANGED))
            {
                Globals.SetParmValue(WhereCriteria.DATERANGE, "21");

                var interval = Globals.GetParmValue(WhereCriteria.TXTLASTSELECTDATACHANGED).TryIntParse(0);
                var hrsMin = Globals.GetParmValue(WhereCriteria.DDDATACHANGEDHRSMINS).Trim();

                interval = hrsMin.EqualsIgnoreCase("HRS") ? interval * 60 * 60 : interval * 60;

                var dateNow = DateTime.Now;
                var dateThen = dateNow.AddSeconds(0 - interval);

                if (dateNow.Hour > 12)
                {
                    Globals.SetParmValue(WhereCriteria.ENDHOUR, (dateNow.Hour - 12).ToString());
                    Globals.SetParmValue(WhereCriteria.ENDAMPM, "2");
                }
                else
                {
                    Globals.SetParmValue(WhereCriteria.ENDHOUR, dateNow.Hour.ToString());
                    Globals.SetParmValue(WhereCriteria.ENDAMPM, dateNow.Hour == 12 ? "2" : "1");
                }
                Globals.SetParmValue(WhereCriteria.ENDMINUTE, dateNow.Minute.ToString());

                if (dateThen.Hour > 12)
                {
                    Globals.SetParmValue(WhereCriteria.BEGHOUR, (dateThen.Hour - 12).ToString());
                    Globals.SetParmValue(WhereCriteria.BEGAMPM, "2");
                }
                else
                {
                    Globals.SetParmValue(WhereCriteria.BEGHOUR, dateThen.Hour.ToString());
                    Globals.SetParmValue(WhereCriteria.BEGAMPM, dateThen.Hour == 12 ? "2" : "1");
                }
                Globals.SetParmValue(WhereCriteria.BEGMINUTE, dateThen.Minute.ToString());
            }
        }    
        
        public string BuildwhereDate()
        {
            var begDate = Globals.BeginDate.Value;
            var endDate = Globals.EndDate.Value;

            //string whereDate;
            //if (!Globals.ParmHasValue(WhereCriteria.BEGHOUR) && !Globals.ParmHasValue(WhereCriteria.BEGMINUTE) &&
            //    !Globals.ParmHasValue(WhereCriteria.ENDHOUR) && !Globals.ParmHasValue(WhereCriteria.ENDMINUTE))
            //{
            //     whereDate = string.Format("lastupdate between '{0}' and '{1}'", begDate.ToShortDateString(), endDate.ToShortDateString());
            //}
            //else
            //{
            var begHr = Globals.ParmHasValue(WhereCriteria.BEGHOUR) ?
            Globals.GetParmValue(WhereCriteria.BEGHOUR) :
            "00";

            var begMin = Globals.ParmHasValue(WhereCriteria.BEGMINUTE) ?
            Globals.GetParmValue(WhereCriteria.BEGMINUTE) :
            "00";
            var begAmPm = Globals.ParmValueEquals(WhereCriteria.BEGAMPM, "2") ? "PM" : "AM";


            if (begAmPm.EqualsIgnoreCase("PM") && (begHr.Equals("00") || begHr.Equals("0")))
            {
                begHr = "12";
            }

            var endHr = Globals.ParmHasValue(WhereCriteria.ENDHOUR) ?
            Globals.GetParmValue(WhereCriteria.ENDHOUR) :
            "00";
            var endMin = Globals.ParmHasValue(WhereCriteria.ENDMINUTE) ?
                Globals.GetParmValue(WhereCriteria.BEGMINUTE) :
                "00";
            var endAmPm = Globals.ParmValueEquals(WhereCriteria.ENDAMPM, "2") ? "PM" : "AM";

            if (endAmPm.EqualsIgnoreCase("PM") && (endHr.Equals("00") || endHr.Equals("0")))
            {
                endHr = "12";
            }
            return string.Format("lastupdate between '{0} {1}:{2}:00 {3}' and '{4} {5}:{6}:00 {7}'", begDate.ToShortDateString(), begHr, begMin, begAmPm, endDate.ToShortDateString(), endHr, endMin, endAmPm);
            //}
        }
        public bool IsValidateKey()
        {
            var udrKeyParm = Globals.GetParmValue(WhereCriteria.UDRKEY);
            //verify that data elements have been requested 
            _reportKey = udrKeyParm.TryIntParse(0);


            if (_reportKey == 0)
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = "Invalid report key!";
                return false;
            }

            return true;
        }
    }
}
