
using Domain.Helper;
using iBank.Server.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined
{
    public class UserReportInformation
    {
        public int ReportKey { get; set; }
        public string Agency { get; set; }
        public string ReportName { get; set; }
        public int UserNumber { get; set; }
        public string UserId { get; set; }
        public string ReportType { get; set; }
        public string ReportTitle { get; set; }
        public string ReportSubtitle { get; set; }
        public SegmentOrLeg SegmentOrLeg { get; set; }
        public bool NoDetail { get; set; }
        public DateTime? LastUsed { get; set; }
        public string PageFooterText { get; set; }
        public string Theme { get; set; }
        public int TripSummaryLevel { get; set; }
        public string IataNum { get; set; }
        public bool IsLookupColumn { get; set; }
        public string LookupFunction { get; set; }
        public string SourceTable { get; set; }
        public string LookupSourceTable { get; set; }
        public bool SuppressDuplicates { get; set; }
        public bool HasServiceFee { get; set; }
        public string CarbonCalculator { get; set; }
        public bool HasTripCarbon { get; set; }
        public bool HasAirCarbon { get; set; }
        public bool HasCarCarbon { get; set; }
        public bool HasHotelCarbon { get; set; }
        public bool HasAltRailCarbon { get; set; }
        public bool HasAltCarCarbon { get; set; }
        public bool HasCarbonFields { get { return HasTripCarbon || HasAirCarbon || HasCarCarbon || HasHotelCarbon || HasAltRailCarbon || HasAltCarCarbon; } }
        public bool UseMileageTable { get { return HasCarbonFields && !string.IsNullOrEmpty(CarbonCalculator); } }        
        public List<UserReportColumnInformation> Columns { get; set; }
        public int DefinedColumnCount { get; set; } 
        //width is the width of all columns, with a one character space between each.
        public int CrystalReportWidth { get { return Columns.Sum(s => s.Width) + Columns.Count; } }
        public bool HasPlusMinColumn { get { return Columns.Any(s => s.Name.EqualsIgnoreCase("PLUSMIN")); } }
        public bool SuppressDetail { get; set; }
        public bool IsValidReport { get; set; }
        public bool AreAllColumnsInTripTables { get; set; } = false;
    }
}
