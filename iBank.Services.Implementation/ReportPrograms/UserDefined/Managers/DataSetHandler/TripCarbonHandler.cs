using System.Collections.Generic;
using System.Linq;
using Domain.Helper;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Classes;
using iBank.Services.Implementation.Shared;
using iBank.Services.Implementation.Shared.BuildWhereHelpers;
using iBank.Services.Implementation.Shared.CarbonCalculations;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;
using iBank.Services.Implementation.Utilities.ClientData;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.DataSetHandler
{
    public class TripCarbonHandler
    {
        private BuildWhere _buildWhere;

        public TripCarbonHandler(BuildWhere buildWhere)
        {
            _buildWhere = buildWhere;
        }

        public List<TripCo2Information> GetTripCarbon(ReportGlobals globals, bool isTotalEmission)
        {
            var preview = globals.ParmValueEquals(WhereCriteria.PREPOST, "1");
            int udid;
            var hasUdid = int.TryParse(globals.GetParmValue(WhereCriteria.UDIDNBR), out udid) && udid != 0;

            var sqlToExecute = string.Empty;
            sqlToExecute = CarbonSql.CreateAirCarbonSql(preview, hasUdid, _buildWhere);

            var airCarbonList = ClientDataRetrieval.GetOpenQueryData<TripAirCo2Information>(sqlToExecute, globals, _buildWhere.Parameters).ToList();
            if (!airCarbonList.Any()) return new List<TripCo2Information>();

            var useMetric = globals.IsParmValueOn(WhereCriteria.METRIC);
            var useMileageTable = globals.IsParmValueOn(WhereCriteria.MILEAGETABLE);
            var carbonCalculator = globals.GetParmValue(WhereCriteria.CARBONCALC);

            if (useMileageTable) AirMileageCalculator<TripAirCo2Information>.CalculateAirMileageFromTable(airCarbonList);

            var carbonCalc = new CarbonCalculator();
            carbonCalc.SetAirCarbon(airCarbonList, useMetric, carbonCalculator);

            if (useMetric) MetricImperialConverter.ConvertMilesToKilometers(airCarbonList);

            var tripCo2 = airCarbonList.GroupBy(s => s.RecKey, (key, g) =>
            {
                var reclist = g.ToList();
                var firstOrDefault = reclist.FirstOrDefault();
                if (firstOrDefault == null) return new TripCo2Information();

                return new TripCo2Information
                {
                    AirCo2 = reclist.Sum(s => s.AirCo2),
                    TripCo2 = reclist.Sum(s => s.AirCo2),
                    AltCarCo2 = reclist.Sum(s => s.AltCarCo2),
                    AltRailCo2 = reclist.Sum(s => s.AltRailCo2),
                    RecKey = firstOrDefault.RecKey
                };
            }).ToList();

            if (!isTotalEmission) return tripCo2;

            sqlToExecute = string.Empty;
            sqlToExecute = CarbonSql.CreateCarCarbonSql(preview, hasUdid, _buildWhere);

            var carCarbonList = ClientDataRetrieval.GetOpenQueryData<TripCarCo2Information>(sqlToExecute, globals, _buildWhere.Parameters).ToList();
            if (carCarbonList.Any())
            {
                carbonCalc.SetCarCarbon(carCarbonList, useMetric, preview);

                var tripCarCo2List = carCarbonList.GroupBy(s => s.RecKey, (key, g) =>
                {
                    var reclist = g.ToList();
                    var firstOrDefault = reclist.FirstOrDefault();
                    if (firstOrDefault == null) return new TripCo2Information();

                    return new TripCo2Information
                    {
                        TripCo2 = reclist.Sum(s => s.TripCarCo2),
                        TripCarCo2 = reclist.Sum(s => s.TripCarCo2),
                        RecKey = firstOrDefault.RecKey
                    };
                });

                // Add to, or update, the tripCo2List
                foreach (var item in tripCarCo2List)
                {
                    var trip = tripCo2.FirstOrDefault(t => t.RecKey == item.RecKey);
                    if (trip == null)
                        tripCo2.Add(new TripCo2Information
                        {
                            RecKey = item.RecKey,
                            TripCarCo2 = item.TripCarCo2,
                            TripCo2 = item.TripCo2
                        });
                    else
                    {
                        trip.TripCarCo2 = item.TripCarCo2;
                        trip.TripCo2 += item.TripCo2;
                    }
                }
            }

            return tripCo2;
        }

    }
}
