using System;

using iBank.Entities.MasterEntities;

using iBankDomain.RepositoryInterfaces;

namespace iBank.BroadcastServer.Timing
{
    public class TimingCalculator
    {
        public DateTime NextReportPeriodEnd { get; set; }
        public DateTime NextReportPeriodStart { get; set; }

        public TimingCalculator()
        {
            NextReportPeriodEnd = new DateTime();
            NextReportPeriodStart = new DateTime();
        }
        
        public int GetSecondsFromHours(int hours)
        {
            return 60 * 60 * Math.Abs(hours);
        }

        public int GetSecondsFromMinutes(int minutes)
        {
            return 60 * Math.Abs(minutes);
        }
        
        public DateTime GetValidDate(int year, int month, int day)
        {
            day = GetCorrectDayBasedOnMonth(day, month);
            return new DateTime(year, month, day);
        }

        private int GetCorrectDayBasedOnMonth(int day, int month)
        {
            switch (month)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    return day > 31 ? 31 : day;
                case 2:
                    return day > 28 ? 28 : day;
                case 4:
                case 6:
                case 9:
                case 11:
                    return day > 30 ? 30 : day;
            }

            return day;
        }

        public int GetTimeZoneOffset(IQuery<mstragcy> getAgencyRecordByAgencyNameQuery, IQuery<MstrCorpAccts> getCorpAcctsByAgencyQuery)
        {
            var agencyRecord = getAgencyRecordByAgencyNameQuery.ExecuteQuery();

            if (agencyRecord != null)
            {
                return agencyRecord.tzoffset.Value;
            }
            else
            {
                var corpAcct = getCorpAcctsByAgencyQuery.ExecuteQuery();

                if (corpAcct != null)
                {
                    return corpAcct.tzoffset.Value;
                }
            }

            return 0;
        }
    }
}
