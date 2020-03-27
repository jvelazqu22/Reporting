using System.Collections.Generic;

namespace iBank.Services.Implementation.ReportPrograms.AirActivity
{
    public class ReportDocumentParameters
    {
        public Dictionary<string, object> Parameters { get; set; }

        public readonly string ColumnHeader = "cColHead1";

        public readonly string TotalAirCharge = "nTotChg";

        public readonly string CarbonCalcReportFooter = "cCarbCalcRptFtr";

        public readonly string UnitOfMeasurement = "cPoundsKilos";

        public readonly string IncludeVoids = "lInclVoids";

        public readonly string ExcludeServiceFees = "lExSvcFee";

        public ReportDocumentParameters(string columnHeader, decimal totalAirCharge, string unitOfMeasurement, bool includeVoids, bool excludeServiceFees)
        {
            Parameters = CreateParameterStructure(columnHeader, totalAirCharge, unitOfMeasurement, includeVoids, excludeServiceFees);
        }

        private Dictionary<string, object> CreateParameterStructure(string columnHeader, decimal totalAirCharge, string unitOfMeasurement, 
            bool includeVoids, bool excludeServiceFees)
        {
            return new Dictionary<string, object>
            {
                { ColumnHeader, columnHeader},
                { TotalAirCharge, totalAirCharge },
                { CarbonCalcReportFooter, ""},
                { UnitOfMeasurement, unitOfMeasurement },
                { IncludeVoids, includeVoids },
                { ExcludeServiceFees, excludeServiceFees }
            };
        }
    }
}

