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
    
    public partial class ibtravauth
    {
        public int travauthno { get; set; }
        public string agency { get; set; }
        public int reckey { get; set; }
        public string recloc { get; set; }
        public string acct { get; set; }
        public string authstatus { get; set; }
        public string msgpending { get; set; }
        public System.DateTime statustime { get; set; }
        public int sgroupnbr { get; set; }
        public string trvlremail { get; set; }
        public string rtvlcode { get; set; }
        public string outpolcods { get; set; }
        public string authcomm { get; set; }
        public string gds { get; set; }
        public System.DateTime bookedgmt { get; set; }
        public System.DateTime parsedgmt { get; set; }
        public string cliauthnbr { get; set; }
        public string notifylvl { get; set; }
        public string tvlrccaddr { get; set; }
        public string chgNotify { get; set; }
        public string qcruleset { get; set; }
        public Nullable<System.DateTime> pnrcrdtgmt { get; set; }
        public byte loadflag { get; set; }
        public byte Click2Auth { get; set; }
        public byte xmlflag { get; set; }
        public System.DateTime xmltime { get; set; }
        public short xmltries { get; set; }
        public bool errflag { get; set; }
        public string sourceabbr { get; set; }
    }
}
