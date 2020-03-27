using System;
using Domain.Helper;

namespace Domain.Models.ReportPrograms.UserList
{
    
    public class FinalData
    {
        public FinalData()
        {
            Userid = string.Empty;
            Password = string.Empty;
            Firstname = string.Empty;
            Lastname = string.Empty;
            Emailaddr = string.Empty;
            Lastlogin = DateTime.MinValue;
            Purgemsg = string.Empty;
            Orgname = string.Empty;
            Purgeinact = false;
            Inactdays = 0;
            Purgetemps = false;
            Tempsdays = 0;
            Reports = false;
            Adminlvl = 0;
            Accts = string.Empty;
            Analytics = string.Empty;
            Travets = string.Empty;
            Emailfiltr = string.Empty;
            Stylegroup = string.Empty;
            Days = 0;
            AltAuths = string.Empty;
        }
        [Exportable]
        public string Userid { get; set; }
        [Exportable]
        public string Password { get; set; }
        [Exportable]
        public string Firstname { get; set; }
        [Exportable]
        public string Lastname { get; set; }
        [Exportable]
        public string Emailaddr { get; set; }
        [Exportable]
        public DateTime Lastlogin { get; set; }
        [Exportable]
        public string Purgemsg { get; set; }
        [Exportable]
        public string Orgname { get; set; }
        [Exportable]
        public bool Purgeinact { get; set; }
        [Exportable]
        public int Inactdays { get; set; }
        [Exportable]
        public bool Purgetemps { get; set; }
        [Exportable]
        public int Tempsdays { get; set; }
        [Exportable]
        public bool Reports { get; set; }
        [Exportable]
        public int Adminlvl { get; set; }
        [Exportable]
        public string Accts { get; set; }
        [Exportable]
        public string AltAuths { get; set; }
        [Exportable]
        public string Analytics { get; set; }
        [Exportable]
        public string Travets { get; set; }
        [Exportable]
        public string Emailfiltr { get; set; }
        [Exportable]
        public string Stylegroup { get; set; }
        
        public int Days { get; set; }
        
    }
}
