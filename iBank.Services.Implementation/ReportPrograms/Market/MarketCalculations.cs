using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Domain.Constants;
using Domain.Helper;
using Domain.Models.ReportPrograms.MarketReport;

using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared;

using Domain.Orm.Classes;

using iBank.Entities.MasterEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace iBank.Services.Implementation.ReportPrograms.Market
{
    public class MarketCalculations
    {
        public enum CarrierNumber
        {
            Carrier1, 
            Carrier2, 
            Carrier3
        }

        public string GetCrystalReportName(int numberOfCarriers, bool useAirportCodes)
        {
            if (useAirportCodes)
            {
                switch (numberOfCarriers)
                {
                    case 1:
                        return ReportNames.MARKET_RPT_1A;
                    case 2:
                        return ReportNames.MARKET_RPT_2A;
                    case 3:
                        return ReportNames.MARKET_RPT_3A;
                    default:
                        throw new ArgumentOutOfRangeException(string.Format("Number of carriers must be 1, 2, or 3. Supplied [{0}]", numberOfCarriers));
                }
            }
            else
            {
                switch (numberOfCarriers)
                {
                    case 1:
                        return ReportNames.MARKET_RPT_1;
                    case 2:
                        return ReportNames.MARKET_RPT_2;
                    case 3:
                        return ReportNames.MARKET_RPT_3;
                    default:
                        throw new ArgumentOutOfRangeException(string.Format("Number of carriers must be 1, 2, or 3. Supplied [{0}]", numberOfCarriers));
                }
            }
        }

        public bool UseAirportCodes(ReportGlobals globals)
        {
            return globals.IsParmValueOn(WhereCriteria.CBUSEAIRPORTCODES);
        }

        public int GetNumberCarriers(Carrier carrier1, Carrier carrier2, Carrier carrier3)
        {
            var numberCarriers = 0;

            if (carrier1?.Carriers.Count > 0) numberCarriers++;
            if (carrier2?.Carriers.Count > 0) numberCarriers++;
            if (carrier3?.Carriers.Count > 0) numberCarriers++;

            return numberCarriers;
        }
        
        public Carrier GetCarrier(ReportGlobals globals, CarrierNumber carrierNumber, string userMode, IMasterDataStore store)
        {
            var carrierName = GetCarrierName(globals, carrierNumber);
            var pickList = new PickListParms(globals);

            if (carrierName == string.Empty) { return new Carrier(); }
            pickList.ProcessList(carrierName, string.Empty, "AIRLINES");
            var carrier = new Carrier
            {
                Carriers = carrierName.Split(',').Select(s => s.Trim()).ToList(),
                ExpandedCarriers = pickList.PickList, 
            };

            carrier.Description = string.IsNullOrEmpty(pickList?.PickName) 
                ? string.Empty 
                : pickList.PickName;

            // Used to amend the description with a lookup on the mode (Air or Rail) 
            if (carrier.Description == string.Empty)
            {
                var mode = string.Empty;
                if (!userMode.IsNullOrWhiteSpace())
                {
                    mode = userMode;
                }
                else if (carrierName.Length == 4) { mode = "R"; }

                if (!carrierName.IsNullOrWhiteSpace() && !carrierName.EqualsIgnoreCase("[NONE]") && carrierName.IndexOf(",", StringComparison.InvariantCulture) < 0)
                {
                    carrier.Description = GetCarrierDescription(store, carrierName, mode);
                }
                else
                {
                    carrier.Description = carrierName;
                }
            }
            return carrier;
        }

        public IList<RawData> AssignAccountFare(IList<RawData> rawData, IList<GroupedData> groups)
        {
            foreach (var record in rawData)
            {
                var group = groups.FirstOrDefault(s => s.RecKey == record.RecKey);
                if (group == null ||
                    group.RecCount == 0 ||
                    group.BaseAirChg == 0 ||
                    group.RecCount == 0) continue;

                record.ActFare = group.BaseAirChg / group.RecCount;
            }

            return rawData;
        }

        public string GetSortBy(ReportGlobals globals)
        {
            return globals.GetParmValue(WhereCriteria.SORTBY);
        }

        public TotalSegsAndFares CalculateTotalSegmentsAndFares(IList<FinalData> finalData)
        {
            var totals = new TotalSegsAndFares()
                             {
                                 TotalSegs = 0,
                                 TotalFare = 0,
                                 TotalCarrier1Segs = 0,
                                 TotalCarrier1Fare = 0,
                                 TotalCarrier2Segs = 0,
                                 TotalCarrier2Fare = 0,
                                 TotalCarrier3Fare = 0,
                                 TotalCarrier3Segs = 0
                             };

            foreach (var row in finalData)
            {
                totals.TotalSegs += row.Segments;
                totals.TotalFare += row.Fare;
                totals.TotalCarrier1Segs += (row.Carr1Segs < 0 ? 0 : row.Carr1Segs);
                totals.TotalCarrier1Fare += row.Carr1Fare;
                totals.TotalCarrier2Segs += (row.Carr2Segs < 0 ? 0 : row.Carr2Segs);
                totals.TotalCarrier2Fare += row.Carr2Fare;
                totals.TotalCarrier3Segs += (row.Carr3Segs < 0 ? 0 : row.Carr3Segs);
                totals.TotalCarrier3Fare += row.Carr3Fare;
            }

            return totals;
        }

        public IList<string> GetExportFields()
        {
            return new List<string>
            {
                "Carr1Fare",
                "Carr1Segs",
                "Carr2Fare",
                "Carr2Segs",
                "Carr3Fare",
                "Carr3Segs",
                "DestDesc",
                "Destinat",
                "Fare",
                "Flt_Mkt",
                "Flt_Mkt2",
                "Mode",
                "OrgDesc",
                "Origin",
                "Segments"
            };
        }

        private string GetCarrierDescription(IMasterDataStore store, string carrier, string mode)
        {
            return CultureInfo.CurrentCulture.TextInfo
                                             .ToTitleCase(LookupFunctions.LookupAline(store, carrier, mode)
                                             .PadRight(Constants.CarrierDescLength)
                                             .Trim()
                                             .ToLower());
        }

        private string GetCarrierName(ReportGlobals globals, CarrierNumber carrierNumber)
        {
            switch (carrierNumber)
            {
                case CarrierNumber.Carrier1:
                    return globals.GetParmValue(WhereCriteria.SEGCARR1);
                case CarrierNumber.Carrier2:
                    return globals.GetParmValue(WhereCriteria.SEGCARR2);
                case CarrierNumber.Carrier3:
                    return globals.GetParmValue(WhereCriteria.SEGCARR3);
                default:
                    throw new ArgumentOutOfRangeException(nameof(carrierNumber), carrierNumber, null);
            }
        }
    }
}
