using System;
using System.Collections.Generic;
using System.Reflection;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupLegFieldHandler;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupMktSegsFieldHandler;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupMktSegsRailTickerFieldHandler;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.ColumnValueRules.LookupTripFieldHandler;
using iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.LookupFieldHandlers;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers
{
    public class ColumnValueRulesFactory
    {
        Dictionary<string, Type> columns;

        public ColumnValueRulesFactory()
        {
            LoadTypesIColumnValueReturn();
        }

        public IColumnValue CreateInstance(string colName, object handler)
        {
            Type t = GetTypeToCreate(colName);
            if (t == null)
            {
                if (handler is LookupTripFieldHandler) return new NullColLookupTripFieldHandler();
                if (handler is LookupLegFieldHandler) return new NullColLookupLegFieldHandler();
                if (handler is LookupMktSegsFieldHandler) return new NullColLookupMktSegsFieldHandler();
                if (handler is LookupMktSegsRailTickerFieldHandler) return new NullColLookupMktSegsRailTickerFieldHandler();

                throw new Exception("missing lookup handler type");
            }

            return Activator.CreateInstance(t) as IColumnValue;
        }

        Type GetTypeToCreate(string colName)
        {
            foreach (var column in columns)
            {
                if (column.Key.Equals(colName))
                {
                    return columns[column.Key];
                }
            }

            return null;
        }

        void LoadTypesIColumnValueReturn()
        {
            columns = new Dictionary<string, Type>();

            Type[] typesInThisAssembly = Assembly.GetExecutingAssembly().GetTypes();

            foreach (Type type in typesInThisAssembly)
            {
                if (type.GetInterface(typeof(IColumnValue).ToString()) != null)
                {
                    columns.Add(type.Name.ToUpper(), type);
                }
            }
        }
    }
}