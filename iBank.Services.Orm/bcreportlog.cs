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
    
    public partial class bcreportlog
    {
        public System.DateTime rundatetime { get; set; }
        public bool runokay { get; set; }
        public string agency { get; set; }
        public Nullable<int> UserNumber { get; set; }
        public Nullable<int> batchnum { get; set; }
        public Nullable<System.DateTime> startdate { get; set; }
        public Nullable<System.DateTime> enddate { get; set; }
        public string emailaddr { get; set; }
        public string emailmsg { get; set; }
        public int recordno { get; set; }
        public string emaillog { get; set; }
        public string emailpda { get; set; }
    }
}