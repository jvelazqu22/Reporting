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
    
    public partial class errorlog
    {
        public Nullable<int> UserNumber { get; set; }
        public string agency { get; set; }
        public Nullable<System.DateTime> errdate { get; set; }
        public Nullable<short> errornbr { get; set; }
        public string errorpgm { get; set; }
        public string errormsg { get; set; }
        public Nullable<short> pgmlinenbr { get; set; }
        public int recordno { get; set; }
        public string iBankVer { get; set; }
        public byte svrnbr { get; set; }
        public string serverName { get; set; }
    }
}
