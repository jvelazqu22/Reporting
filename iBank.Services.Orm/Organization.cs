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
    
    public partial class Organization
    {
        public string Agency { get; set; }
        public int OrgKey { get; set; }
        public string OrgName { get; set; }
        public bool PurgeInact { get; set; }
        public short InactDays { get; set; }
        public bool PurgeTemps { get; set; }
        public short TempsDays { get; set; }
        public bool AltAuths { get; set; }
        public string LangCode { get; set; }
        public string OrgAbbrev { get; set; }
        public bool DisabInact { get; set; }
        public short DisabDays { get; set; }
        public bool PWEnfHist { get; set; }
        public byte PWRemember { get; set; }
        public bool PWEnfExpir { get; set; }
        public short PWMaxAge { get; set; }
        public short PWNotify { get; set; }
        public bool EMailReqd { get; set; }
    }
}
