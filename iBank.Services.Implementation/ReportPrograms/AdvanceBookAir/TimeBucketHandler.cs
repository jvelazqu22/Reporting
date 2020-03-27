using System.Collections.Generic;

using Domain.Exceptions;
using Domain.Models.ReportPrograms.AdvanceBookAir;

using iBankDomain.RepositoryInterfaces;

namespace iBank.Services.Implementation.ReportPrograms.AdvanceBookAir
{
    public class TimeBucketHandler
    {
        public IList<TimeBucket> FillTimeBuckets(IList<FinalData> finalData, IQuery<string> getClientExtrasByFieldFunctionAndAgencyQuery)
        {
            var buckets = new List<TimeBucket>
                              {
                                  new TimeBucket { StartDay = 0, EndDay = 2 },
                                  new TimeBucket { StartDay = 3, EndDay = 6 },
                                  new TimeBucket { StartDay = 7, EndDay = 13 },
                                  new TimeBucket { StartDay = 14, EndDay = 20 }
                              };
            
            var clientBuckets = getClientExtrasByFieldFunctionAndAgencyQuery.ExecuteQuery();

            //replace default buckets with client specific buckets
            if (!string.IsNullOrEmpty(clientBuckets))
            {
                var pairsAsString = clientBuckets.Split(':');
                for (int i = 0; i < 4; i++)
                {
                    var pairAsString = pairsAsString[i].Split(',');

                    int pair1;
                    int pair2;
                    if (!int.TryParse(pairAsString[0], out pair1)) throw new TimeBucketException("BAD VALUE IN ADVANCEDAYSBUCKETS");
                    if (!int.TryParse(pairAsString[1], out pair2)) throw new TimeBucketException("BAD VALUE IN ADVANCEDAYSBUCKETS");
                    buckets[i].StartDay = pair1;
                    buckets[i].EndDay = pair2;
                }
            }

            return buckets;
        }
    }
}
