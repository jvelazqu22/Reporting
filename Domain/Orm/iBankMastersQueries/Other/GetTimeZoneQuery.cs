using System;
using System.Linq;
using Domain.Orm.Classes;
using iBank.Repository.SQL.Interfaces;
using iBankDomain.RepositoryInterfaces;

namespace Domain.Orm.iBankMastersQueries.Other
{
    public class GetTimeZoneQuery : IQuery<TimeZoneInformation>
    {
        private string _langCode;
        private string _timeZone;
        private readonly IMastersQueryable _db;

        public GetTimeZoneQuery(IMastersQueryable db,string timeZone, string langCode)
        {
            _db = db;
            _langCode = langCode;
            _timeZone = timeZone;
        }

        public TimeZoneInformation ExecuteQuery()
        {
            using (_db)
            {
                var tz = _db.TimeZones.FirstOrDefault(x => x.TimeZoneCode.Equals(_timeZone, StringComparison.InvariantCultureIgnoreCase) && x.LangCode.Equals(_langCode, StringComparison.InvariantCultureIgnoreCase));
                if(tz == null) return new TimeZoneInformation();//defaults are for EST
                return new TimeZoneInformation
                {
                    RecordNo = tz.RecordNo,
                    TimeZoneCode = tz.TimeZoneCode,
                    TimeZoneName = tz.TimeZoneName,
                    Region = tz.Region,
                    GmtDiff = (int) tz.GMTDiff,
                    DstAbbrev = tz.DSTAbbrev
                };

            }
        }
    }
}
