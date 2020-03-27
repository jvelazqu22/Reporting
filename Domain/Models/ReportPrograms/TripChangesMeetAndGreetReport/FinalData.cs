﻿using System;

namespace Domain.Models.ReportPrograms.TripChangesMeetAndGreetReport
{
    public class FinalData
    {
        public int Reckey { get; set; } = 0;
        public string Mtggrpnbr { get; set; } = string.Empty;
        public string Acct { get; set; } = string.Empty;
        public string Passlast { get; set; } = string.Empty;
        public string Passfrst { get; set; } = string.Empty;
        public string Emailaddr { get; set; } = string.Empty;
        public string Destinat { get; set; } = string.Empty;
        public string Destdesc { get; set; } = string.Empty;
        public string Origdesc { get; set; } = string.Empty;
        public string Lastorgdes { get; set; } = string.Empty;
        public DateTime Rarrdate { get; set; } = DateTime.MinValue;
        public string RArrDateDisplay
        {
            get
            {
                return Rarrdate.ToString("dddd, MMMM dd, yyyy");
            }
            set
            {

            }
        }
        public string Airline { get; set; } = string.Empty;
        public string Alinedesc { get; set; } = string.Empty;
        public string Fltno { get; set; } = string.Empty;
        public string Arrtime { get; set; } = string.Empty;
        public string Deptime { get; set; } = string.Empty;
        public string Sorttime { get; set; } = string.Empty;
        public string Recloc { get; set; } = string.Empty;
        public string Ticketed { get; set; } = string.Empty;
        public DateTime Bookdate { get; set; } = DateTime.MinValue;
        public string Changedesc { get; set; } = string.Empty;
        public DateTime Changstamp { get; set; }

        public string ChangeStampDisplay
        {
            get
            {
                return Changstamp.ToShortDateString();
            }
            set {}
        }

        public decimal Udidnbr1 { get; set; } = 0m;
        public string Udidtext1 { get; set; } = string.Empty;
        public decimal Udidnbr2 { get; set; } = 0m;
        public string Udidtext2 { get; set; } = string.Empty;
        public string Ticket { get; set; } = string.Empty;
        public int SegNum { get; set; }

    }
}
