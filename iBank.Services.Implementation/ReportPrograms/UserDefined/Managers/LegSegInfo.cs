namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers
{
    public class LegSegInfo
    {
        public int RecKey { get; set; }
        public string LegRouting { get; set; }
        //don't want to mess with LegRouting right now, but will clean it up in the future
        public string DerivedLegRouting { get; set; } = string.Empty;
        public string DerivedSegRouting { get; set; } = string.Empty;
        public string DerivedTransId { get; set; } = string.Empty;
        //prepare for later use on TripClass and TripClassCat
        public string DerivedTripClass { get; set; } = string.Empty;
        public string DerivedTripClassCat { get; set; } = string.Empty;
        public string TripClass { get; set; }
        public string TripClassCat { get; set; }
        public string Mode { get; set; }
        public int Miles { get; set; }
        public bool RoundTrip { get; set; }
        public string Carriers { get; set; }
        public string Classes { get; set; }
        public string ClassCats { get; set; }
        public string FbCodes { get; set; }
    }
}
