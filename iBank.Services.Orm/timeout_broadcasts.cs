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
    
    public partial class timeout_broadcasts
    {
        public int id { get; set; }
        public int batchnum { get; set; }
        public string batchname { get; set; }
        public int UserNumber { get; set; }
        public string agency { get; set; }
        public string database_name { get; set; }
        public Nullable<System.DateTime> nextrun { get; set; }
        public bool notification_sent { get; set; }
        public System.DateTime time_stamp { get; set; }
    }
}