﻿using System;
using System.Linq;

using iBank.Entities.AdministrationEntities;
using iBank.Repository.SQL.Interfaces;

using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankAdministrationQueries
{
    public class GetSvrStatusRecordQuery : IQuery<SvrStatus>
    {
        private string ServerName { get; }

        private readonly IAdministrationQueryable _db;

        public GetSvrStatusRecordQuery(IAdministrationQueryable db, string serverName)
        {
            _db = db;
            ServerName = serverName.Trim();
        }
        public SvrStatus ExecuteQuery()
        {
            using (_db)
            {
                return _db.SvrStatus.FirstOrDefault(x => x.SvrName.Trim().Equals(ServerName, StringComparison.OrdinalIgnoreCase));
                
            }
        }
    }
}