﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace iBank.Services.Orm
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class iBankMastersEntities : DbContext
    {
        public iBankMastersEntities()
            : base("name=iBankMastersEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<reporthandoff> reporthandoff { get; set; }
        public virtual DbSet<ibwhcrit> ibwhcrit { get; set; }
        public virtual DbSet<Collist2Captions> Collist2Captions { get; set; }
        public virtual DbSet<ClientImages> ClientImages { get; set; }
        public virtual DbSet<ClientExtras> ClientExtras { get; set; }
        public virtual DbSet<mstragcy> mstragcy { get; set; }
        public virtual DbSet<MstrCorpAccts> MstrCorpAccts { get; set; }
        public virtual DbSet<ibRptLog> ibRptLog { get; set; }
        public virtual DbSet<ibProcVerbiage> ibProcVerbiage { get; set; }
        public virtual DbSet<ibFuncLangTags> ibFuncLangTags { get; set; }
        public virtual DbSet<LanguageTags> LanguageTags { get; set; }
        public virtual DbSet<LanguageTranslations> LanguageTranslations { get; set; }
        public virtual DbSet<userTranslations> userTranslations { get; set; }
        public virtual DbSet<collist2> collist2 { get; set; }
        public virtual DbSet<errorlog> errorlog { get; set; }
        public virtual DbSet<Languages> Languages { get; set; }
        public virtual DbSet<intlparm> intlparm { get; set; }
        public virtual DbSet<curcountry> curcountry { get; set; }
        public virtual DbSet<ibproccrit> ibproccrit { get; set; }
        public virtual DbSet<ibRptLogCrit> ibRptLogCrit { get; set; }
        public virtual DbSet<ibRptLogResults> ibRptLogResults { get; set; }
        public virtual DbSet<curconversion> curconversion { get; set; }
        public virtual DbSet<transtrack> transtrack { get; set; }
        public virtual DbSet<miscparams> miscparams { get; set; }
        public virtual DbSet<RRStations> RRStations { get; set; }
        public virtual DbSet<CarbonCalculators> CarbonCalculators { get; set; }
        public virtual DbSet<RROperators> RROperators { get; set; }
        public virtual DbSet<iBankDatabases> iBankDatabases { get; set; }
        public virtual DbSet<iBankServers> iBankServers { get; set; }
        public virtual DbSet<TimeZones> TimeZones { get; set; }
        public virtual DbSet<bcstAllowMultiples> bcstAllowMultiples { get; set; }
        public virtual DbSet<eProfiles> eProfiles { get; set; }
        public virtual DbSet<xmlrpts> xmlrpts { get; set; }
        public virtual DbSet<bcstque4> bcstque4 { get; set; }
        public virtual DbSet<airlines> airlines { get; set; }
        public virtual DbSet<ClientImageXData> ClientImageXData { get; set; }
        public virtual DbSet<bcstrptinstance> bcstrptinstance { get; set; }
        public virtual DbSet<cartypes> cartypes { get; set; }
        public virtual DbSet<roomtype> roomtype { get; set; }
        public virtual DbSet<mstrAgcySourceExtras> mstrAgcySourceExtras { get; set; }
        public virtual DbSet<MstrAgcySources> MstrAgcySources { get; set; }
        public virtual DbSet<airports> airports { get; set; }
        public virtual DbSet<Countries> Countries { get; set; }
        public virtual DbSet<ibproces> ibproces { get; set; }
        public virtual DbSet<metro> metro { get; set; }
        public virtual DbSet<chainClass> chainClass { get; set; }
        public virtual DbSet<chainParents> chainParents { get; set; }
        public virtual DbSet<chains> chains { get; set; }
        public virtual DbSet<JunctionAgcyCorp> JunctionAgcyCorp { get; set; }
        public virtual DbSet<TripChangeCodes> TripChangeCodes { get; set; }
        public virtual DbSet<WorldRegions> WorldRegions { get; set; }
        public virtual DbSet<xmlrpt2> xmlrpt2 { get; set; }
        public virtual DbSet<ibRunningRpts> ibRunningRpts { get; set; }
        public virtual DbSet<ibRptLogSQL> ibRptLogSQL { get; set; }
        public virtual DbSet<eProfExpTypes> eProfExpTypes { get; set; }
        public virtual DbSet<eProfileProcs> eProfileProcs { get; set; }
        public virtual DbSet<TradingPartners> TradingPartners { get; set; }
        public virtual DbSet<broadcast_stage_agencies> broadcast_stage_agencies { get; set; }
        public virtual DbSet<broadcast_long_running_agencies> broadcast_long_running_agencies { get; set; }
        public virtual DbSet<CorpAcctNbrs> CorpAcctNbrs { get; set; }
        public virtual DbSet<report_server_stage> report_server_stage { get; set; }
        public virtual DbSet<report_rollout_stage> report_rollout_stage { get; set; }
        public virtual DbSet<broadcast_high_alert_agency> broadcast_high_alert_agency { get; set; }
        public virtual DbSet<overdue_broadcasts> overdue_broadcasts { get; set; }
        public virtual DbSet<timeout_broadcasts> timeout_broadcasts { get; set; }
        public virtual DbSet<state_names> state_names { get; set; }
    }
}
