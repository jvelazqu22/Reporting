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
    
    public partial class ibUserMacroData
    {
        public int recordno { get; set; }
        public int macrokey { get; set; }
        public string agency { get; set; }
        public int usernumber { get; set; }
        public bool active { get; set; }
        public short version { get; set; }
        public System.DateTime uploaddate { get; set; }
        public string macrodata { get; set; }
        public bool certified { get; set; }
        public string certstatus { get; set; }
        public Nullable<System.DateTime> certifdate { get; set; }
        public bool certified_active { get; set; }
    }
}
