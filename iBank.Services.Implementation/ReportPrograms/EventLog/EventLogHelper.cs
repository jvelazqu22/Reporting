using System.Collections.Generic;
using System.Linq;
using CODE.Framework.Core.Utilities.Extensions;
using Domain.Helper;
using Domain.Orm.iBankAdminQueries;
using Domain.Orm.iBankClientQueries;
using iBank.Repository.SQL.Interfaces;
using iBank.Repository.SQL.Repository;
using iBank.Services.Implementation.Shared;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.Client;
using iBank.Services.Implementation.Utilities.ClientData;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;

namespace iBank.Services.Implementation.ReportPrograms.EventLog
{
    public class EventLogHelper
    {
        public string GetTargetType(string targetType)
        {
            switch (targetType)
            {
                case "ACCT":
                    return "ACCOUNT";
                case "SGRP":
                    return "STYLEGROUP";
                case "AGCY":
                    return "AGENCY";
                case "ORGN":
                    return "ORGANIZATION";
                default:
                    return targetType;
            }
        }

        public List<string> GetExportFields(bool includeIp)
        {
            return includeIp
                ? new List<string> { "Category", "EventDesc", "UserId", "UserName", "IPAddress", "EventDate", "TargetType", "EvntTarget", "TargetName" }
                : new List<string> { "Category", "EventDesc", "UserId", "UserName", "EventDate", "TargetType", "EvntTarget", "TargetName" };
        }

        public string LookupEventTarget(string eventTarget, string targetId, GetAllMasterAccountsQuery mstrQry, IClientDataStore clientStore, ClientFunctions clientFunctions, ReportGlobals globals)
        {
            if (string.IsNullOrEmpty(targetId)) return new string(' ', 60);

            if (globals.ClientType == ClientType.Sharer)
            {
                var dataSources = globals.CorpAccountDataSources.Select(x=>x.DataSource);
                var clientsDbList = dataSources.Select(source => new iBankClientQueryable(source.ServerAddress, source.DatabaseName)).Cast<IClientQueryable>().ToList();
            }

            switch (eventTarget.ToUpper().Trim())
            {
                case "USER":
                    return clientFunctions.LookupUserName(clientStore.ClientQueryDb, targetId);
                case "ORGANIZATION":
                    var orgKey = targetId.TryIntParse(-1);

                    return orgKey < 0
                        ? new string(' ', 60)
                        : clientFunctions.LookupOrganizationName(clientStore.ClientQueryDb, orgKey);
                case "ACCOUNT":
                    return clientFunctions.LookupCname(mstrQry, targetId, globals);
                case "STYLEGROUP":
                    var sgKey = targetId.TryIntParse(-1);

                    if (sgKey > 0)
                    {
                        return new string(' ', 60);
                    }
                    else
                    {
                        var query = new GetStyleGroupNameQuery(clientStore.ClientQueryDb, sgKey);
                        var groupName = query.ExecuteQuery();

                        return string.IsNullOrEmpty(groupName)
                            ? "[NOT FOUND]".PadRight(40)
                            : groupName.PadRight(40);
                    }
                default:
                    return new string(' ', 60);
            }
        }

        public string LookupEventTargetInMultipleAgencies(string eventTarget, string targetId, GetAllMasterAccountsQuery mstrQry, ClientFunctions clientFunctions, ReportGlobals globals)
        {
            if (string.IsNullOrEmpty(targetId)) return new string(' ', 60);
            
            var dataSources = globals.CorpAccountDataSources.Select(x => x.DataSource).ToList();

            var clientsDbList = dataSources.Select(source => new iBankClientQueryable(source.ServerAddress, source.DatabaseName)).Cast<IClientQueryable>().ToList();

            switch (eventTarget.ToUpper().Trim())
            {
                case "USER":
                    return clientFunctions.LookupUserNameInMultipleAgencies(clientsDbList, targetId);
                case "ORGANIZATION":
                    var orgKey = targetId.TryIntParse(-1);

                    return orgKey < 0
                        ? new string(' ', 60)
                        : clientFunctions.LookupOrganizationNameInMultipleAgencies(clientsDbList, orgKey);
                case "ACCOUNT":
                    return clientFunctions.LookupCname(mstrQry, targetId, globals);
                case "STYLEGROUP":
                    var sgKey = targetId.TryIntParse(-1);

                    if (sgKey > 0)
                    {
                        return new string(' ', 60);
                    }
                    else
                    {
                        foreach (var queryDb in clientsDbList)
                        {
                            var query = new GetStyleGroupNameQuery(queryDb, sgKey);
                            var groupName = query.ExecuteQuery();
                            if (!string.IsNullOrEmpty(groupName)) return groupName.PadRight(40);
                        }

                        return "[NOT FOUND]".PadRight(40);
                    }
                default:
                    return new string(' ', 60);
            }
        }

    }
}
