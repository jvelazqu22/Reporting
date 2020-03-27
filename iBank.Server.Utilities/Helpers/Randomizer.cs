using System;
using Domain.Helper;

namespace iBank.Server.Utilities.Helpers
{
    public class Randomizer
    {
        public Timer GetRandomNumberOfMillisecondsBeforeNextReStart()
        {
            var rnd = new Random();
            var timer = new Timer();
            // pick a random hour between 6 and 12
            timer.Hours = rnd.Next(6, 12);
            // convert hours to milliseconds
            var restartMilliseconds = timer.Hours * 60 * 60 * 1000; // hours * minutes * seconds * milliseconds
            // pick a random minute between 0 and 59
            timer.Minutes = rnd.Next(0, 59);
            // convert minutes to milliseconds
            var numberOfMinutesInMilliseconds = timer.Minutes * 60 * 1000; //  minutes * seconds * milliseconds

            // If we re-start the services every 6:00 - 12:00 hours, there is a 1/6 chance that they will start at the same time
            // However, if we include the minutes so the range is 6:00 to 12:59 then there is a 1/66 chance.
            restartMilliseconds += numberOfMinutesInMilliseconds;

            timer.TotalTimeInMilliseconds = restartMilliseconds;

            return timer;
        }
    }
}
