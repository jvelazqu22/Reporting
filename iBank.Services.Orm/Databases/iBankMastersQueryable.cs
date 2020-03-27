using System.Data.Entity;
using System.Linq;
using System.Transactions;

using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.Databases
{
    public class iBankMastersQueryable : IMastersQueryable
    {
        private readonly iBankMastersEntities _context;
        
        public iBankMastersQueryable()
        {
            _context = new iBankMastersEntities();
        }

        public IQueryable<broadcast_long_running_agencies> BroadcastLongRunningAgencies
        {
            get
            {
                return _context.broadcast_long_running_agencies;
            }
        }

        public IQueryable<broadcast_stage_agencies> BroadcastStageAgencies
        {
            get
            {
                return _context.broadcast_stage_agencies;
            }
        }

        public Database Database { get
        {
            return _context.Database;
        } }

        public IQueryable<TradingPartners> TradingPartners { get
        {
            return _context.TradingPartners;
        } }

        public IQueryable<iBankServers> iBankServers { get
        {
            return _context.iBankServers;
        } }

        public IQueryable<eProfiles> EProfiles { get
        {
            return _context.eProfiles;
        } }

        public IQueryable<eProfileProcs> EProfileProcs { get
        {
            return _context.eProfileProcs;
        } }
        public IQueryable<eProfExpTypes> EProfExpTypes { get
        {
            return _context.eProfExpTypes;
        } }

        public IQueryable<TimeZones> TimeZones { get
        {
            return _context.TimeZones;
        } }

        public IQueryable<bcstque4> BcstQue4 { get
        {
            return _context.bcstque4;
        } }

        public IQueryable<bcstAllowMultiples> BcstAllowMultiples { get
        {
            return _context.bcstAllowMultiples;
        } }

        public IQueryable<CarbonCalculators> CarbonCalculators { get
        {
            return _context.CarbonCalculators;
        } }

        public IQueryable<airlines> Airlines { get
        {
            return _context.airlines;
        } }

        public IQueryable<airports> Airports { get { return _context.airports; } }

        public IQueryable<cartypes> CarTypes
        {
            get
            {
                return _context.cartypes;
            }
        }

        public IQueryable<chains> Chains { get
        {
            return _context.chains;
        } }

        public IQueryable<ClientExtras> ClientExtras { get
        {
            return _context.ClientExtras;
        } }

        public IQueryable<ClientImages> ClientImages { get
        {
            return _context.ClientImages;
        } }

        public IQueryable<collist2> Collist2 { get
        {
            return _context.collist2;
        } }

        public IQueryable<Collist2Captions> Collist2Captions { get
        {
            return _context.Collist2Captions;
        } }

        public IQueryable<Countries> Countries { get
        {
            return _context.Countries;
        } }

        public IQueryable<curconversion> CurConversion { get
        {
            return _context.curconversion;
        } }

        public IQueryable<curcountry> CurCountry { get
        {
            return _context.curcountry;
        } }

        public IQueryable<iBankDatabases> iBankDatabases { get
        {
            return _context.iBankDatabases;
        } }

        public IQueryable<ibFuncLangTags> ibFuncLangTags { get
        {
            return _context.ibFuncLangTags;
        } }

        public IQueryable<Languages> Languages { get
        {
            return _context.Languages;
        } }

        public IQueryable<LanguageTags> LanguageTags { get
        {
            return _context.LanguageTags;
        } }

        public IQueryable<LanguageTranslations> LanguageTranslations { get
        {
            return _context.LanguageTranslations;
        } }

        public IQueryable<userTranslations> UserTranslations { get
        {
            return _context.userTranslations;
        } }

        public IQueryable<ibproccrit> iBProccrit { get
        {
            return _context.ibproccrit;
        } }

        public IQueryable<ibwhcrit> iBWhcrit { get
        {
            return _context.ibwhcrit;
        } }

        public IQueryable<ibproces> iBProcess { get
        {
            return _context.ibproces;
        } }

        public IQueryable<ibRunningRpts> iBRunningRpts { get
            {
                return _context.ibRunningRpts;
            }
        }

        public IQueryable<ibProcVerbiage> iBProcVerbiage { get
        {
            return _context.ibProcVerbiage;
        } }

        public IQueryable<ibRptLog> iBRptLog { get
        {
            return _context.ibRptLog;
        } }

        public IQueryable<ibRptLogResults> iBRptLogResults { get
        {
            return _context.ibRptLogResults;
        } }

        public IQueryable<intlparm> IntlParm { get
        {
            return _context.intlparm;
        } }

        public IQueryable<metro> Metro { get
        {
            return _context.metro;
        } }

        public IQueryable<bcstrptinstance> BcstRptInstance { get
        {
            return _context.bcstrptinstance;
        } }

        public IQueryable<ClientImageXData> ClientImageXData { get
        {
            return _context.ClientImageXData;
        } }

        public IQueryable<miscparams> MiscParams { get
        {
            return _context.miscparams;
        } }

        public IQueryable<mstragcy> MstrAgcy { get
        {
            return _context.mstragcy;
        } }

        public IQueryable<mstrAgcySourceExtras> MstrAgcySourceExtras { get
        {
            return _context.mstrAgcySourceExtras;
        } }

        public IQueryable<MstrAgcySources> MstrAgcySources { get
        {
            return _context.MstrAgcySources;
        } }

        public IQueryable<JunctionAgcyCorp> JunctionAgcyCorp { get
        {
            return _context.JunctionAgcyCorp;
        } }
        public IQueryable<MstrCorpAccts> MstrCorpAccts { get
        {
            return _context.MstrCorpAccts;
        } }

        public IQueryable<reporthandoff> ReportHandoff { get
        {
            return _context.reporthandoff;
        } }

        public IQueryable<roomtype> RoomType { get
        {
            return _context.roomtype;
        } }

        public IQueryable<RROperators> RROperators { get
        {
            return _context.RROperators;
        } }

        public IQueryable<RRStations> RRStations { get
        {
            return _context.RRStations;
        } }

        public IQueryable<TripChangeCodes> TripChangeCodes { get
        {
            return _context.TripChangeCodes;
        } }

        public IQueryable<WorldRegions> WorldRegions { get
        {
            return _context.WorldRegions;
        } }

        public IQueryable<xmlrpts> XmlRpt { get
        {
            return _context.xmlrpts;
        } }

        public IQueryable<xmlrpt2> XmlRpt2 { get
        {
            return _context.xmlrpt2;
        } }

        public void Dispose()
        {
            _context.Dispose();
        }

        public object Clone()
        {
            return new iBankMastersQueryable();
        }
    }
}
