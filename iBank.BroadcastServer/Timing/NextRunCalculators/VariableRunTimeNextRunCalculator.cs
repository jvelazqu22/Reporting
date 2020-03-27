using System;
using Domain.Constants;
using iBank.Entities.MasterEntities;
using iBankDomain.RepositoryInterfaces;

namespace iBank.BroadcastServer.Timing.NextRunCalculators
{
    public class VariableRunTimeNextRunCalculator : INextRunCalculator
    {
        private DateTime _nextRun;

        private int _frequencyOfRun;

        private DateTime _now;

        private IQuery<mstragcy> _getAgencyRecordByAgencyNameQuery;

        private IQuery<MstrCorpAccts> _getCorpAcctByAgencyQuery;

        public VariableRunTimeNextRunCalculator(DateTime nextRun, int frequencyOfRun, DateTime now, IQuery<mstragcy> getAgencyRecordByAgencyNameQuery,
            IQuery<MstrCorpAccts> getCorpAcctByAgencyQuery)
        {
            _nextRun = nextRun;
            _frequencyOfRun = frequencyOfRun;
            _now = now;
            _getAgencyRecordByAgencyNameQuery = getAgencyRecordByAgencyNameQuery;
            _getCorpAcctByAgencyQuery = getCorpAcctByAgencyQuery;
        }

        public DateTime CalculateNextRun()
        {
            var timingCalculator = new TimingCalculator();

            var seconds = BroadcastFrequencyOfRun.IsDailyEveryXHours(_frequencyOfRun)
                              ? timingCalculator.GetSecondsFromHours(_frequencyOfRun)
                              : timingCalculator.GetSecondsFromMinutes(_frequencyOfRun);

            var nextRun = _nextRun.AddSeconds(seconds);

            //TODO: this piece will need to be removed so that we have the catch up option for anything
            var timeZoneOffset = timingCalculator.GetTimeZoneOffset(_getAgencyRecordByAgencyNameQuery, _getCorpAcctByAgencyQuery);
            var tzOffsetInSeconds = timingCalculator.GetSecondsFromHours(timeZoneOffset);
            if (nextRun < _now.AddSeconds(timeZoneOffset))
            {
                //if the next run is in the past this will keep the broadcast from running every x minutes over and over again till it catches up
                nextRun = _now.AddSeconds(seconds + tzOffsetInSeconds);
            }

            return nextRun;
        }
    }
}
