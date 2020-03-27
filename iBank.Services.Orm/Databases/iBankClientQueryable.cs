using System;
using System.Data.Entity;
using System.Linq;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.Databases
{
    public class iBankClientQueryable : IClientQueryable
    {
        private readonly iBankClientModel _context;
        private string ServerName { get; }
        private string DatabaseName { get; }

        public iBankClientQueryable(string serverName, string databaseName)
        {
            ServerName = serverName;
            DatabaseName = databaseName;
            _context = new iBankClientModel(ServerName, DatabaseName);
        }

        public Database Database
        {
            get
            {
                return _context.Database;
            }
        }

        public IQueryable<hibudid> HibUdid
        {
            get
            {
                return _context.hibudids;
            }
        }

        public IQueryable<ibudid> ibUdid
        {
            get
            {
                return _context.ibudids;
            }
        }

        public IQueryable<ibUserMacroData> ibUserMacroData { get
        {
            return _context.ibUserMacroDatas;
        } }

        public IQueryable<savedrpt2> SavedRpt2 { get
        {
            return _context.savedrpt2;
        } }

        public IQueryable<savedrpt3> SavedRpt3 { get
        {
            return _context.savedrpt3;
        } }

        public IQueryable<acctmast> AcctMast
        {
            get
            {
                return _context.acctmasts;
            }
        }

        public IQueryable<acctparent> AcctParent
        {
            get
            {
                return _context.acctparents;
            }
        }

        public IQueryable<ClassCatMstr> ClassCatMstr
        {
            get
            {
                return _context.ClassCatMstrs;
            }
        }

        public IQueryable<clientsTL> ClientsTl
        {
            get
            {
                return _context.clientsTLS;
            }
        }

        public IQueryable<ibuser> iBUser
        {
            get
            {
                return _context.ibusers;
            }
        }

        public IQueryable<ibUserExtra> iBUserExtra
        {
            get
            {
                return _context.ibUserExtras;
            }
        }

        public IQueryable<ibbatch> iBBatch { get
        {
            return _context.ibbatches;
        } }

        public IQueryable<ibbatch2> iBBatch2 { get
        {
            return _context.ibbatch2;
        } }

        public IQueryable<Organization> Organization { get
        {
            return _context.Organizations;
        }}

        public IQueryable<picklist> PickList
        {
            get
            {
                return _context.picklists;
            }
        }

        public IQueryable<savedrpt1> SavedRpt1
        {
            get
            {
                return _context.savedrpt1;
            }
        }


        public IQueryable<StyleGroupExtra> StyleGroupExtra
        {
            get
            {
                return _context.StyleGroupExtras;
            }
        }

        public IQueryable<StyleGroup> StyleGroup { get
        {
            return _context.StyleGroups;
        } }

        public IQueryable<useracct> UserAcct
        {
            get
            {
                return _context.useraccts;
            }
        }

        public IQueryable<userbrks1> UserBrks1
        {
            get
            {
                return _context.userbrks1;
            }
        }

        public IQueryable<userbrks2> UserBrks2
        {
            get
            {
                return _context.userbrks2;
            }
        }

        public IQueryable<UserSource> UserSource
        {
            get
            {
                return _context.UserSources;
            }
        }

        public IQueryable<ibreascd> iBReasCd
        {
            get
            {
                return _context.ibreascds;
            }
        }

        public IQueryable<ReasonSet> ReasonSet
        {
            get
            {
                return _context.ReasonSets;
            }
        }

        public IQueryable<vendor> Vendors
        {
            get { return _context.vendors; }
        } 

        public void Dispose()
        {
            _context.Dispose();
        }
        public IQueryable<xmluserrpt> XmlUserRpt
        {
            get
            {
                return _context.xmluserrpts;
            }
        }

        public IQueryable<xmluserrpt2> XmlUserRpt2
        {
            get
            {
                return _context.xmluserrpt2;
            }
        }

        public IQueryable<userrpt> UserRpt { get
        {
            return _context.userrpts;
        } }
        
        public IQueryable<userrpt2> UserRpt2
        {
            get
            {
                return _context.userrpt2;
            }
        }

        public IQueryable<UserFieldCat> UserFieldCats
        {
            get
            {
                return _context.UserFieldCats;
            }
        }

        public object Clone()
        {
            return new iBankClientQueryable(ServerName, DatabaseName);
        }
    }
}
