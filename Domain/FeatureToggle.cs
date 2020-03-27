using Domain.Orm.iBankAdministrationQueries;
using iBank.Repository.SQL.Repository;
using System;
using System.Configuration;
using Domain.Exceptions;

namespace Domain
{
    public enum Feature
    {
        AdvancedParameterAcctPicklistCheck,
        MinutesToWaitForFoxProToReturnToDB,
        BroadcastNextRunFlag,
        GsaTripTransactionIdFeatureFlag,
        WhereTextFeautureFlag,
        SegtransactionidFeautureFlag,
        AdvancedClauseBuilderFeatureFlag,
        AutoMapperInitializer,
        OriginTranslatingUseMode,
        HandleLookupFieldTripRefactor,
        HandleLookupFieldLegRefactor,
        HandleLookupFieldMktSegsRefactor,
        HandleLookupFieldMiscSegsRailTicketRefactor,
        ExcelDateTimeFormatUsePropertyInfo,
        ExcludeParseStampWhereClause,
        TravelOptixImplimentation,
        RoutingUseTripsDerivedDataTable,
        MissedHotelUseReclocCheck,
        LoadLegLookupWhenIsLegLevel,
        BuildLegSegSqlHandler,
        FieldPreFix,
        UseBuildAdvancedClauses,
        FareType,
        GsaCurrencyCheck,
        XmlExtractWhereClauseFull,
        HighAlerts, 
        XmlReportBuilder,
        AdvanceParamsReplaceReckeyInWithAnd
    }

    public static class Features
    {
        public static IFeatureToggle AdvanceParamsReplaceReckeyInWithAnd { get; set; } = new FeatureFlag(Feature.AdvanceParamsReplaceReckeyInWithAnd);
        public static IFeatureToggle XmlReportBuilder { get; set; } = new FeatureFlag(Feature.XmlReportBuilder);
        public static IFeatureToggle HighAlerts { get; set; } = new FeatureFlag(Feature.HighAlerts);
        public static IFeatureToggle XmlExtractWhereClauseFull { get; set; } = new FeatureFlag(Feature.XmlExtractWhereClauseFull);
        public static IFeatureToggle HandleLookupFieldMiscSegsRailTicketRefactor { get; set; } = new FeatureFlag(Feature.HandleLookupFieldMiscSegsRailTicketRefactor);
        public static IFeatureToggle HandleLookupFieldMktSegsRefactor { get; set; } = new FeatureFlag(Feature.HandleLookupFieldMktSegsRefactor);
        public static IFeatureToggle HandleLookupFieldLegRefactor { get; set; } = new FeatureFlag(Feature.HandleLookupFieldLegRefactor);
        public static IFeatureToggle GsaCurrencyCheck { get; set; } = new FeatureFlag(Feature.GsaCurrencyCheck);
        public static IFeatureToggle FareType { get; set; } = new FeatureFlag(Feature.FareType);
        public static IFeatureToggle AdvancedParameterAcctPicklistCheck { get; set; } = new FeatureFlag(Feature.AdvancedParameterAcctPicklistCheck);
        public static IFeatureToggle FieldPreFix { get; set; } = new FeatureFlag(Feature.FieldPreFix);
        public static IFeatureToggle BuildLegSegSqlHandler { get; set; } = new FeatureFlag(Feature.BuildLegSegSqlHandler);
        public static IFeatureToggle HandleLookupFieldTripRefactor { get; set; } = new FeatureFlag(Feature.HandleLookupFieldTripRefactor);
        public static IFeatureToggle AutoMapperInitializer { get; set; } = new FeatureFlag(Feature.AutoMapperInitializer);
        public static IFeatureToggle AdvancedClauseBuilderFeatureFlag { get; set; } = new FeatureFlag(Feature.AdvancedClauseBuilderFeatureFlag);
        public static IFeatureToggle SegtransactionidFeautureFlag { get; set; } = new FeatureFlag(Feature.SegtransactionidFeautureFlag);
        public static IFeatureToggle WhereTextFeautureFlag { get; set; } = new FeatureFlag(Feature.WhereTextFeautureFlag);
        public static IFeatureToggle GsaTripTransactionIdFeatureFlag { get; set; } = new FeatureFlag(Feature.GsaTripTransactionIdFeatureFlag);

        public static IFeatureToggle MinutesToWaitForFoxProToReturnToDB { get; set; } = new FeatureFlag(Feature.MinutesToWaitForFoxProToReturnToDB);

        public static IFeatureToggle BroadcastNextRunFlag { get; set; } = new FeatureFlag(Feature.BroadcastNextRunFlag);

        public static IFeatureToggle OriginTranslatingUseMode { get; set; } = new FeatureFlag(Feature.OriginTranslatingUseMode);

        public static IFeatureToggle ExcelDateTimeFormatUsePropertyInfo { get; set; } = new FeatureFlag(Feature.ExcelDateTimeFormatUsePropertyInfo);
        public static IFeatureToggle ExcludeParseStampWhereClause { get; set; } = new FeatureFlag(Feature.ExcludeParseStampWhereClause);
        public static IFeatureToggle TravelOptixImplimentation { get; set; } = new FeatureFlag(Feature.TravelOptixImplimentation);
        public static IFeatureToggle RoutingUseTripsDerivedDataTable { get; set; } = new FeatureFlag(Feature.RoutingUseTripsDerivedDataTable);
        public static IFeatureToggle MissedHotelUseReclocCheck { get; set; } = new FeatureFlag(Feature.MissedHotelUseReclocCheck);
        public static IFeatureToggle LoadLegLookupWhenIsLegLevel { get; set; } = new FeatureFlag(Feature.LoadLegLookupWhenIsLegLevel);
        public static IFeatureToggle UseBuildAdvancedClauses { get; set; } = new FeatureFlag(Feature.UseBuildAdvancedClauses);
        

        private class FeatureFlag : IFeatureToggle
        {
            private readonly Feature _featureName;

            public FeatureFlag(Feature featureName)
            {
                _featureName = featureName;
            }

            public bool IsEnabled()
            {
                if (ConfigurationManager.AppSettings["ServerNumber"] == null)
                {
                    throw new Exception("Server number configuation does not exist.");
                }

                var serverNumberTemp = ConfigurationManager.AppSettings["ServerNumber"];
                var serverNumber = 0;
                if (!int.TryParse(serverNumberTemp, out serverNumber))
                {
                    throw new Exception("Server number configuration malformed.");
                }

                try
                {
                    var query = new IsFeatureFlagOnQuery(new iBankAdministrationQueryable(), _featureName, serverNumber);
                    return query.ExecuteQuery();
                }
                catch (FeatureFlagDoesNotExistException)
                {
                    //if we can't find the feature flag we will just act like it is turned off, rather than bomb out
                    return false;
                }
            }
        }

        /// <summary>
        /// Use this in unit testing to turn on a feature flag w/o requiring a trip to the database.
        /// [Example usage: Features.MyAwesomeFeature = Features.AlwaysOnFlag;]
        /// </summary>
        /// <value>
        /// The always on flag.
        /// </value>
        public static IFeatureToggle AlwaysOnFlag => new AlwaysOn();

        private class AlwaysOn : IFeatureToggle
        {
            public bool IsEnabled() => true;
        }


        public static IFeatureToggle AlwaysOffFlag => new AlwaysOff();

        private class AlwaysOff : IFeatureToggle
        {
            public bool IsEnabled() => false;
        }

    }

    public interface IFeatureToggle
    {
        bool IsEnabled();
    }
}
