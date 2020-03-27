using Domain.Interfaces;
using System;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace iBank.Services.Orm.Databases.Interfaces
{
    public interface IMastersQueryable : IDisposable, IPrototype
    {
        Database Database { get; }

        IQueryable<broadcast_long_running_agencies> BroadcastLongRunningAgencies { get; }
        
        IQueryable<TradingPartners> TradingPartners { get; }
        IQueryable<iBankServers> iBankServers { get; }
        IQueryable<eProfiles> EProfiles { get; }
        IQueryable<eProfileProcs> EProfileProcs { get; }
        IQueryable<eProfExpTypes> EProfExpTypes { get; }

        IQueryable<TimeZones> TimeZones { get; }

        IQueryable<bcstque4> BcstQue4 { get; }
        IQueryable<broadcast_stage_agencies> BroadcastStageAgencies { get; }

        IQueryable<bcstAllowMultiples> BcstAllowMultiples { get; }

        IQueryable<CarbonCalculators> CarbonCalculators { get; }

        IQueryable<airlines> Airlines { get; }

        IQueryable<airports> Airports { get; }

        IQueryable<cartypes> CarTypes { get; }

        IQueryable<chains> Chains { get; }

        IQueryable<ClientExtras> ClientExtras { get; }

        IQueryable<ClientImages> ClientImages { get; }

        IQueryable<collist2> Collist2 { get; }

        IQueryable<Collist2Captions> Collist2Captions { get; }

        IQueryable<Countries> Countries { get; }

        IQueryable<curconversion> CurConversion { get; }

        IQueryable<curcountry> CurCountry { get; }

        IQueryable<iBankDatabases> iBankDatabases { get; }
        IQueryable<ibFuncLangTags> ibFuncLangTags { get; }

        IQueryable<Languages> Languages { get; }

        IQueryable<LanguageTags> LanguageTags { get; }

        IQueryable<LanguageTranslations> LanguageTranslations { get; }

        IQueryable<userTranslations> UserTranslations { get; }

        IQueryable<ibproccrit> iBProccrit { get; }

        IQueryable<ibwhcrit> iBWhcrit { get; }

        IQueryable<ibproces> iBProcess { get; }

        IQueryable<ibRunningRpts> iBRunningRpts { get; }

        IQueryable<ibProcVerbiage> iBProcVerbiage { get; }

        IQueryable<ibRptLog> iBRptLog { get; }

        IQueryable<ibRptLogResults> iBRptLogResults { get; }

        IQueryable<intlparm> IntlParm { get; }

        IQueryable<metro> Metro { get; }

        IQueryable<bcstrptinstance> BcstRptInstance { get; }

        IQueryable<ClientImageXData> ClientImageXData { get; }

        IQueryable<miscparams> MiscParams { get; }

        IQueryable<mstragcy> MstrAgcy { get; }

        IQueryable<mstrAgcySourceExtras> MstrAgcySourceExtras { get; }

        IQueryable<MstrAgcySources> MstrAgcySources { get; }

        IQueryable<JunctionAgcyCorp> JunctionAgcyCorp { get; }
        IQueryable<MstrCorpAccts> MstrCorpAccts { get; }

        IQueryable<reporthandoff> ReportHandoff { get; }

        IQueryable<roomtype> RoomType { get; }

        IQueryable<RROperators> RROperators { get; }

        IQueryable<RRStations> RRStations { get; }

        IQueryable<TripChangeCodes> TripChangeCodes { get; }

        IQueryable<WorldRegions> WorldRegions { get; }

        IQueryable<xmlrpts> XmlRpt { get; }

        IQueryable<xmlrpt2> XmlRpt2 { get; }
    }
}
