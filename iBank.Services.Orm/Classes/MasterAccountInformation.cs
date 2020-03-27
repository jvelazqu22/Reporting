namespace iBank.Services.Orm.Classes
{
    public class MasterAccountInformation
    {
        public MasterAccountInformation()
        {
            AccountId = string.Empty;
            AccountName = string.Empty;
            ParentAccount = string.Empty;
            ReasSetNbr = 0;
            Agency = string.Empty;
            AcctCat1 = string.Empty;
            AcctCat2 = string.Empty;
            AcctCat3 = string.Empty;
            AcctCat4 = string.Empty;
            AcctCat5 = string.Empty;
            AcctCat6 = string.Empty;
            AcctCat7 = string.Empty;
            AcctCat8 = string.Empty;
            Address1 = string.Empty;
            Address2 = string.Empty;
            Address3 = string.Empty;
            Address4 = string.Empty;
        }

        public int ReasSetNbr { get; set; }
        public string AccountId { get; set; }
        public string AccountName { get; set; }
        public string ParentAccount { get; set; }
        public string Agency { get; set; }
        public string AcctCat1 { get; set; }
        public string AcctCat2 { get; set; }
        public string AcctCat3 { get; set; }
        public string AcctCat4 { get; set; }
        public string AcctCat5 { get; set; }
        public string AcctCat6 { get; set; }
        public string AcctCat7 { get; set; }
        public string AcctCat8 { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
    }
}
