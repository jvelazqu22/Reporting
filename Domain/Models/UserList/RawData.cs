using System;

namespace Domain.Models.UserList
{
    public class RawData
    {
        public RawData()
        {
            UserNumber = 0;
            Userid = string.Empty;
            Password = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Emailaddr = string.Empty;
            Allaccts = 0;
            Lastlogin = DateTime.MinValue;
            Orgname = string.Empty;
            Purgeinact = false;
            Inactdays = 0;
            Purgetemps = false;
            Tempsdays = 0;
            Reports = false;
            AdminLvl = 0;
            Pwencrypt = false;
            SGroupNbr = 0;
            SGroupName = string.Empty;
        }
        public int UserNumber { get; set; }
        public string Userid { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Emailaddr { get; set; }
        public int Allaccts { get; set; }
        public DateTime? Lastlogin { get; set; }
        public string Orgname { get; set; }
        public bool Purgeinact { get; set; }
        public int Inactdays { get; set; }
        public bool Purgetemps { get; set; }
        public int Tempsdays { get; set; }
        public bool Reports { get; set; }
        public int AdminLvl { get; set; }
        public bool Pwencrypt { get; set; }
        public int SGroupNbr { get; set; }
        public string SGroupName { get; set; }
    }
}
