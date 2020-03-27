using Domain.Helper;
using iBank.Services.Orm.Databases.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetAllActiveBroadcastProcessCaptionsQuery : BaseiBankMastersQuery<IList<ProcessCaptionInformation>>
    {
        public GetAllActiveBroadcastProcessCaptionsQuery(IMastersQueryable db)
        {
            _db = db;
        }

        public override IList<ProcessCaptionInformation> ExecuteQuery()
        {
            using (_db)
            {
                return _db.iBProcess.Where(s => s.bcactive).Select(s =>
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
