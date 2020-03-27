using iBank.Server.Utilities;
using iBank.Services.Implementation.Shared.LookupFunctionHelpers;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers
{
    public class LegSegBuilder
    {
        public int PrimRecKey { get; set; } = 0;
        public string SegRouting { get; set; } = "";
        public string ClassThisHierarchy { get; set; } = "";
        public string ClassCategoryThisHierarchy { get; set; } = "";

        public string ModeThisHierarchy { get; set; } = "";
        public int TripMiles { get; set; } = 0;
        public string Carriers { get; set; } = "";
        public string Classes { get; set; } = "";
        public string ClassCategories { get; set; } = "";
        public string FareBaseCodes { get; set; } = "";
        public string PreviousDestination { get; set; } = "";
        public int LegClassHierarchy { get; set; } = 99;
        public string AirlineThisHierarchy { get; set; } = "";
        
        //Should use these two to replace SegRouting and LegRouting in the future
        public string DerivedSegRouting { get; set; } = string.Empty;
        public string DerivedLegRouting { get; set; } = string.Empty;
        public string DerivedTransId { get; set; } = string.Empty;
        public string DerivedTripClass { get; set; } = string.Empty;
        public string DerivedTripClassCat { get; set; } = string.Empty;

        public void SetFromClassCategory(LegSegTemp rec, string classCategoryBuffer, int hierarchy)
        {
            ClassThisHierarchy = rec.ClassCat ?? "";
            AirlineThisHierarchy = rec.Airline ?? "";
            ClassCategoryThisHierarchy = classCategoryBuffer;
            ModeThisHierarchy = rec.Mode ?? "";
            LegClassHierarchy = hierarchy;
        }

        public LegSegInfo BuildLegSegInfo()
        {
            return new LegSegInfo
            {
                RecKey = PrimRecKey,
                LegRouting = SegRouting.Length > 240 ? SegRouting.PadLeft(240) : SegRouting,
                TripClass = ClassThisHierarchy,
                TripClassCat = ClassCategoryThisHierarchy,
                Mode = ModeThisHierarchy,
                Miles = TripMiles,
                RoundTrip =  RoundTripCalculator.IsRoundTrip(SegRouting),
                Carriers = this.Carriers,
                Classes = this.Classes,
                ClassCats = ClassCategories,
                FbCodes =  this.FareBaseCodes,
                DerivedLegRouting = this.DerivedLegRouting,
                DerivedSegRouting = this.DerivedSegRouting,
                DerivedTransId = this.DerivedTransId,
                DerivedTripClass = this.DerivedTripClass,
                DerivedTripClassCat = this.DerivedTripClassCat
            };
        }

        public void SetSegRoutingDeprecate(string originDestination)
        {
            var orgDest = originDestination.Trim();

            if (orgDest.Length > 3 && orgDest.Substring(3, 1).Equals("-"))
            {
                orgDest = orgDest.Left(3);
            }

            SegRouting += orgDest + " ";
        }

        public void SetSegRouting(string originDestination)
        {           
            SegRouting += originDestination.Trim() + " ";
        }
    }
}
