namespace Domain.Helper
{
    public class UserInformation
    {
        public UserInformation()
        {
            UserId = string.Empty;
            LastName = string.Empty;
            FirstName = string.Empty;
            CompanyName = string.Empty;
            AccountName = string.Empty;
            Break1Name = string.Empty;
            Break2Name = string.Empty;
            Break3Name = string.Empty;
            Tax1Name = string.Empty;
            Tax2Name = string.Empty;
            Tax3Name = string.Empty;
            Tax4Name = string.Empty;
        }
        public int UserNumber { get; set; }
        public string UserId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string CompanyName { get; set; }
        public string AccountName { get; set; }
        public int OrganizationKey { get; set; }
        public AllRecords AllAccounts { get; set; }
        public bool AllBreaks1 { get; set; }
        public bool AllBreaks2 { get; set; }
        public string Break1Name { get; set; }
        public string Break2Name { get; set; }
        public string Break3Name { get; set; }
        public int ReportBreaks { get; set; }
        public int PageBreakLevel { get; set; }
        public bool AccountBreak { get; set; }
        public bool AccountPageBreak { get; set; }
        public string Tax1Name { get; set; }
        public string Tax2Name { get; set; }
        public string Tax3Name { get; set; }
        public string Tax4Name { get; set; }
        public AllRecords AllSources { get; set; }
        public int SGroupNumber { get; set; }
        public int AdminLevel { get; set; }
        public bool AllowAgencyReports { get; set; }
        public string TimeZone { get; set; }
    }
}
