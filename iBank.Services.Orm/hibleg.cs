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
    
    public partial class hibleg
    {
        public int recordno { get; set; }
        public string agency { get; set; }
        public int reckey { get; set; }
        public string origin { get; set; }
        public string destinat { get; set; }
        public string airline { get; set; }
        public string fltno { get; set; }
        public Nullable<System.DateTime> rdepdate { get; set; }
        public string deptime { get; set; }
        public Nullable<System.DateTime> rarrdate { get; set; }
        public string arrtime { get; set; }
        public string @class { get; set; }
        public string connect { get; set; }
        public string mode { get; set; }
        public Nullable<short> seqno { get; set; }
        public Nullable<short> miles { get; set; }
        public Nullable<decimal> actfare { get; set; }
        public Nullable<decimal> miscamt { get; set; }
        public string farebase { get; set; }
        public Nullable<short> rplusmin { get; set; }
        public string ditcode { get; set; }
        public string tktdesig { get; set; }
        public string OrigOrigin { get; set; }
        public string OrigDest { get; set; }
        public string OrigCarr { get; set; }
        public string classcat { get; set; }
        public Nullable<int> flduration { get; set; }
        public Nullable<decimal> fltGrsFare { get; set; }
        public Nullable<decimal> fltProFare { get; set; }
        public string mktPair { get; set; }
        public Nullable<short> smartCtrFl { get; set; }
        public string carrierTyp { get; set; }
        public string segStatus { get; set; }
        public string tktSegStat { get; set; }
        public string endODFlag { get; set; }
    }
}
