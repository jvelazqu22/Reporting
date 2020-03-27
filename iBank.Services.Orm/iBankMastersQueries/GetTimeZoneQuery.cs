using System;
using System.Linq;
using iBank.Services.Orm.Classes;
using iBank.Services.Orm.Databases.Interfaces;

namespace iBank.Services.Orm.iBankMastersQueries
{
    public class GetTimeZoneQuery : BaseiBankMastersQuery<TimeZoneInformation>
    {
        private string _langCode;
        private string _timeZone;

        public GetTimeZoneQuery(IMastersQueryable db,string timeZone, string langCode)
        {
            _db = db;
            _langCode = langCode;
            _timeZone = timeZone;
        }

        public override TimeZoneInformation ExecuteQuery()
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
