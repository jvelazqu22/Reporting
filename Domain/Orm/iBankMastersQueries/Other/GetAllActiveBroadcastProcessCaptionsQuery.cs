using System.Collections.Generic;
using System.Linq;
using Domain.Helper;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetAllActiveBroadcastProcessCaptionsQuery : IQuery<IList<ProcessCaptionInformation>>
    {
        private IMastersQueryable _db;
        public GetAllActiveBroadcastProcessCaptionsQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public IList<ProcessCaptionInformation> ExecuteQuery()
        {
            using (_db)
            {
                return _db.iBProcess.Select(s =>
                                        new ProcessCaptionInformation
                                            {
                                                ProcessKey = s.processkey,
                                                Caption = s.v4caption,
                                                Usage = s.usage
                                            }).ToList();
            }
        }
    }
}
