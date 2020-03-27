using Domain.Orm.Classes;

namespace iBank.Services.Implementation.ReportPrograms.Invoice
{
    public class AccountAddressInfo
    {
        public string Name { get; set; } = "";
        public string Address1 { get; set; } = "";
        public string Address2 { get; set; } = "";
        public string Address3 { get; set; } = "";
        public string Address4 { get; set; } = "";

        public AccountAddressInfo(MasterAccountInformation acct, string expectedAcctName)
        {
            if (acct == null)
            {
                Name = $"'{expectedAcctName}' NOT FOUND";
            }
            else
            {
                Name = acct.AccountName;
                Address1 = acct.Address1;
                Address2 = acct.Address2;
                Address3 = acct.Address3;
                Address4 = acct.Address4;
            }
        }

    }
}
