using iBank.Server.Utilities;
using iBank.Server.Utilities.Classes;
using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Orm.Classes;
using Domain.Orm.iBankAdminQueries;
using Domain.Orm.iBankClientQueries;
using iBank.Entities.ClientEntities;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace iBank.Services.Implementation.Shared.Client
{
    public class ClientFunctions
    {
        private List<MasterAccountInformation> Accounts { get; set; }
        private List<MasterAccountInformation> ParentAccounts { get; set; }

        private IList<ReasonCode> ReasonCodes { get; set; } = new List<ReasonCode>();

        private IList<ReasonSetInfo> ReasonSets { get; set; } = new List<ReasonSetInfo>();

        private IList<ClassCategoryInformation> ClassCategories { get; set; } = new List<ClassCategoryInformation>();

        private IList<ClientsTLInformation> ClientTLInformation { get; set; } = new List<ClientsTLInformation>();

        private void SetReasonCodes(IClientQueryable queryDb, string agency, string languageCode)
        {
            var query = new GetReasonCodeByAgencyAndLangCodeQuery(queryDb, agency, languageCode);
            ReasonCodes = query.ExecuteQuery();
        }

        private void SetReasonSets(IClientQueryable queryDb, string agency, string languageCode)
        {
            var query = new GetReasonSetByAgencyAndLangCodeQuery(queryDb, agency, languageCode);
            ReasonSets = query.ExecuteQuery();
        }

        public string LookupReason(IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, string code, string accountId, IClientDataStore clientStore, 
            ReportGlobals globals, IMastersQueryable masterDb, bool useLongDescription = false)
        {
            if (!ReasonCodes.Any()) SetReasonCodes(clientStore.ClientQueryDb, globals.Agency, globals.UserLanguage);
            if (!ReasonSets.Any()) SetReasonSets(clientStore.ClientQueryDb, globals.Agency, globals.UserLanguage);

            return new LookupReason().Lookup(code, accountId, clientStore, globals, masterDb, GetMasterAccountInfo(getAllMasterAccountsQuery), ReasonCodes, ReasonSets, useLongDescription);
        }

        public string LookupReason(string code, IClientQueryable queryDb, string agency, string languageCode)
        {
            if (!ReasonCodes.Any()) SetReasonCodes(queryDb, agency, languageCode);

            var rec = ReasonCodes.FirstOrDefault(s => s.ReasCode == code.Trim());

            return rec == null ? " NOT FOUND" : rec.ReasDesc;
        }
        
        public string LookupCname(IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, string key, ReportGlobals globals)
        {
            key = key.Trim();

            var acct = GetMasterAccountInfo(getAllMasterAccountsQuery)
                .FirstOrDefault(x => x.AccountId.Trim().EqualsIgnoreCase(key.Trim()) && x.Agency.Trim().EqualsIgnoreCase(globals.Agency.Trim()));

            return acct == null || key == string.Empty ? key + " NOT FOUND" : acct.AccountName;
        }

        public string LookupUserName(IClientQueryable queryDb, string userId)
        {
            var query = new GetAllUsersQuery(queryDb);
            var users = query.ExecuteQuery();

            var matchingUsers = users.Where(x => x.userid.EqualsIgnoreCase(userId)).ToList();

            switch (matchingUsers.Count)
            {
                case 0:
                    return "[NOT FOUND]".PadRight(30);
                case 1:
                    var user = users.FirstOrDefault();
                    // ReSharper disable once PossibleNullReferenceException
                    return $"{user.lastname.Trim()}, {user.firstname.Trim()}".PadRight(30);
                default:
                    return "[Multiple matches on User ID]".PadRight(30);
            }
        }

        public string LookupUserNameInMultipleAgencies(List<IClientQueryable> queryDbs, string userId)
        {
            var users = new List<ibuser>();
            queryDbs.ForEach(queryDb => users.AddRange(new GetAllUsersQuery(queryDb).ExecuteQuery().ToList()));

            var matchingUsers = users.Where(x => x.userid.EqualsIgnoreCase(userId)).ToList();

            switch (matchingUsers.Count)
            {
                case 0:
                    return "[NOT FOUND]".PadRight(30);
                case 1:
                    var user = users.FirstOrDefault();
                    // ReSharper disable once PossibleNullReferenceException
                    return $"{user.lastname.Trim()}, {user.firstname.Trim()}".PadRight(30);
                default:
                    return "[Multiple matches on User ID]".PadRight(30);
            }
        }

        public string LookupOrganizationNameInMultipleAgencies(List<IClientQueryable> queryDbs, int key)
        {
            foreach (var queryDb in queryDbs)
            {
                var name = new GetOrganizationNameQuery(queryDb, key).ExecuteQuery();
                if(!string.IsNullOrEmpty(name)) return name.PadRight(40);
            }

            return "[NOT FOUND]".PadRight(40);
        }

        public string LookupOrganizationName(IClientQueryable queryDb, int key)
        {
            var query = new GetOrganizationNameQuery(queryDb, key);
            var name = query.ExecuteQuery();

            return string.IsNullOrEmpty(name) ? "[NOT FOUND]".PadRight(40) : name.PadRight(40);
        }

        public List<MasterAccountInformation> GetMasterAccountInfo(IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery)
        {
            if (Accounts == null || !Accounts.Any()) Accounts = getAllMasterAccountsQuery.ExecuteQuery().ToList();

            return Accounts;
        }

        /*must add agency, otherwise it will always get the first available match
        recordno    agency      acct                    acctname
        150827	    GSA       	9000900             	GSA ACCOUNT #9000900                    
        159016	    SHRSRC1   	9000900             	DEMO ACCOUNT # 9000900                  
        142951	    DEMO      	9000900             	DEMO ACCOUNT # 9000900                  
        143003	    DEMO3     	9000900             	DEMO ACCOUNT # 9000900   
        */
        public ParentClassQuickInformation LookupParent(IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, string key, IQuery<IList<MasterAccountInformation>> getAllParentAccountsQuery, string agency)
        {
            var acct = GetMasterAccountInfo(getAllMasterAccountsQuery).FirstOrDefault(s => s.AccountId == key.Trim() && s.Agency == agency);
            if (acct == null) return new ParentClassQuickInformation { AccountId = key, AccountDescription = key + " NOT FOUND" };

            var parentAcct = acct.ParentAccount;
            var parentDesc = acct.AccountName;

            if (string.IsNullOrEmpty(parentAcct))
            {
                parentAcct = key;
            }
            else
            {
                var parAcctRec = GetParentAccountInfo(getAllParentAccountsQuery).FirstOrDefault(s => s.AccountId == parentAcct && s.Agency == agency);
                if (parAcctRec == null)
                {
                    parentDesc = parentAcct + " NOT FOUND";
                }
                else
                {
                    parentAcct = parAcctRec.AccountId;
                    parentDesc = parAcctRec.AccountName;
                }
            }

            return new ParentClassQuickInformation { AccountId = parentAcct, AccountDescription = parentDesc };
        }

        //TODO: This method should be replaced with above verloading method. probably for tech debt
        //Affected reports are: AccountSummary, CarTopButtomAccounts, CoAirSummary, HotelAccountTopBottom, TopAccoutAir 
        public ParentClassQuickInformation LookupParent(IQuery<IList<MasterAccountInformation>> getAllMasterAccountsQuery, string key, IQuery<IList<MasterAccountInformation>> getAllParentAccountsQuery)
        {
            var acct = GetMasterAccountInfo(getAllMasterAccountsQuery).FirstOrDefault(s => s.AccountId == key.Trim());
            if (acct == null) return new ParentClassQuickInformation { AccountId = key, AccountDescription = key + " NOT FOUND" };

            var parentAcct = acct.ParentAccount;
            var parentDesc = acct.AccountName;

            if (string.IsNullOrEmpty(parentAcct))
            {
                parentAcct = key;
            }
            else
            {
                var parAcctRec = GetParentAccountInfo(getAllParentAccountsQuery).FirstOrDefault(s => s.AccountId == parentAcct);
                if (parAcctRec == null)
                {
                    parentDesc = parentAcct + " NOT FOUND";
                }
                else
                {
                    parentAcct = parAcctRec.AccountId;
                    parentDesc = parAcctRec.AccountName;
                }
            }

            return new ParentClassQuickInformation { AccountId = parentAcct, AccountDescription = parentDesc };
        }

        public List<MasterAccountInformation> GetParentAccountInfo(IQuery<IList<MasterAccountInformation>> getAllParentAccountsQuery)
        {
            if (ParentAccounts == null || !ParentAccounts.Any()) ParentAccounts = getAllParentAccountsQuery.ExecuteQuery().ToList();

            return ParentAccounts;
        }

        public string LookupClassCategoryDescription(string classCategory, string agency, IClientQueryable queryDb, bool isTitleCase = false)
        {
            string classCategoryDescription;
            if (string.IsNullOrEmpty(classCategory)) classCategoryDescription = LookupConstants.Unspecified;
            else
            {
                if (!ClassCategories.Any()) SetClassCategories(queryDb);

                var category = ClassCategories.FirstOrDefault(s => s.Agency.EqualsIgnoreCase(agency) && s.Category.EqualsIgnoreCase(classCategory));

                classCategoryDescription = category != null ? category.Description : string.Format("Unknown ClassCat \"{0}\"", classCategory.Trim());
            }

            if (isTitleCase) return classCategoryDescription.PadRight(20).TitleCaseString();

            return classCategoryDescription.PadRight(20);
        }

        private void SetClassCategories(IClientQueryable queryDb)
        {
            var query = new GetAllClassCategoriesQuery(queryDb);
            ClassCategories = query.ExecuteQuery();
        }

        public int LookupClassCategoryHierarchy(string category, IClientQueryable queryDb)
        {
            if (!ClassCategories.Any()) SetClassCategories(queryDb);

            var cat = ClassCategories.FirstOrDefault(x => x.Category.Trim().EqualsIgnoreCase(category.Trim()));

            return cat?.Heirarchy ?? 99;
        }

        public string LookupClientName(string clientId, string agency, IClientQueryable queryDb)
        {
            if (string.IsNullOrEmpty(clientId)) return "";

            if (!ClientTLInformation.Any()) SetClientTlInformation(queryDb);

            var client = ClientTLInformation.FirstOrDefault(s => s.ClientId.EqualsIgnoreCase(clientId) && s.Agency.EqualsIgnoreCase(agency));

            return (client != null) ? client.ClientName : "";
        }

        private void SetClientTlInformation(IClientQueryable queryDb)
        {
            var query = new GetAllClientsTlsQuery(queryDb);
            ClientTLInformation = query.ExecuteQuery();
        }
    }
}
