//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace iBank.Services.Orm
{
    using System;
    using System.Collections.Generic;
    
    public partial class TimeZones
    {
        public int RecordNo { get; set; }
        public string LangCode { get; set; }
        public string TimeZoneCode { get; set; }
        public string TimeZoneName { get; set; }
        public string Region { get; set; }
        public double GMTDiff { get; set; }
        public string DSTAbbrev { get; set; }
    }
}