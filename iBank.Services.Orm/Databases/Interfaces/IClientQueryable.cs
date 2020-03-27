using Domain.Interfaces;
using System;
using System.Data.Entity;
using System.Linq;

namespace iBank.Services.Orm.Databases.Interfaces
{
    public interface IClientQueryable : IDisposable, IPrototype
    {
        Database Database { get; }

        IQueryable<StyleGroup> StyleGroup { get; }
        IQueryable<hibudid> HibUdid { get; }

        IQueryable<ibudid> ibUdid { get; }
        IQueryable<ClassCatMstr> ClassCatMstr { get; }
        IQueryable<ibUserMacroData> ibUserMacroData { get; }
        IQueryable<savedrpt1> SavedRpt1 { get; }
        IQueryable<savedrpt2> SavedRpt2 { get; }
        IQueryable<savedrpt3> SavedRpt3 { get; }
        IQueryable<acctmast> AcctMast { get; }
        IQueryable<acctparent> AcctParent { get; }
        IQueryable<clientsTL> ClientsTl { get; }
        IQueryable<ibuser> iBUser { get; }
        IQueryable<ibUserExtra> iBUserExtra { get; }
        IQueryable<ibbatch> iBBatch { get; }
        IQueryable<ibbatch2> iBBatch2 { get; }
        IQueryable<Organization> Organization { get; }
        IQueryable<picklist> PickList { get; }
        IQueryable<StyleGroupExtra> StyleGroupExtra { get; }
        IQueryable<useracct> UserAcct { get; }
        IQueryable<userbrks1> UserBrks1 { get; }
        IQueryable<userbrks2> UserBrks2 { get; }
        IQueryable<UserSource> UserSource { get; }
        IQueryable<ibreascd> iBReasCd { get; }
        IQueryable<ReasonSet> ReasonSet { get; }
        IQueryable<vendor> Vendors { get; }
        IQueryable<xmluserrpt> XmlUserRpt { get; }
        IQueryable<xmluserrpt2> XmlUserRpt2 { get; }
        IQueryable<userrpt> UserRpt { get; }
        IQueryable<userrpt2> UserRpt2 { get; }
        IQueryable<UserFieldCat> UserFieldCats { get; }
    }
}
