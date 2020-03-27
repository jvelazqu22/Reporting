using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models.ReportPrograms.AdvanceBookAir;

using iBank.Services.Implementation.Utilities;

namespace iBank.Services.Implementation.ReportPrograms.AdvanceBookAir
{
    public class SubReportHandler
    {
        public IList<SubReportData> CreateSubReportCategories(List<TimeBucket> buckets)
        {
            return new List<SubReportData>
                                {
                                    new SubReportData
                                        {
                                            Bookcat = "A",
                                            Category = string.Format("Total # of Trips Purchased {0}-{1} Days Ahead:", buckets[0].StartDay, buckets[0].EndDay),
                                            Shortdesc = string.Format("{0}-{1} Days Ahead", buckets[0].StartDay, buckets[0].EndDay),
                                        },
                                    new SubReportData
                                        {
                                            Bookcat = "B",
                                            Category = string.Format("Total # of Trips Purchased {0}-{1} Days Ahead:", buckets[1].StartDay, buckets[1].EndDay),
                                            Shortdesc = string.Format("{0}-{1} Days Ahead", buckets[1].StartDay, buckets[1].EndDay),
                                        },
                                    new SubReportData
                                        {
                                            Bookcat = "C",
                                            Category = string.Format("Total # of Trips Purchased {0}-{1} Days Ahead:", buckets[2].StartDay, buckets[2].EndDay),
                                            Shortdesc = string.Format("{0}-{1} Days Ahead", buckets[2].StartDay, buckets[2].EndDay),
                                        },
                                    new SubReportData
                                        {
                                            Bookcat = "D",
                                            Category = string.Format("Total # of Trips Purchased {0}-{1} Days Ahead:", buckets[3].StartDay, buckets[3].EndDay),
                                            Shortdesc = string.Format("{0}-{1} Days Ahead", buckets[3].StartDay, buckets[3].EndDay),
                                        },
                                    new SubReportData
                                        {
                                            Bookcat = "E",
                                            Category = string.Format("Total # of Trips Purchased {0}+ Days Ahead:", buckets[3].EndDay),
                                            Shortdesc = string.Format("{0}+ Days Ahead", buckets[3].EndDay),
                                        }
                                };
        }

        public IList<SubReportData> FillSubReportCategories(IList<SubReportData> subReportData, IList<FinalData> finalData)
        {
            //group the records by booking category, reckey, air charge
            var groupedRecs = finalData.GroupBy(x => new { x.Bookcat, x.RecKey, x.Airchg });

            //foreach booking category add air charges, add total number of groups
            foreach (var item in groupedRecs)
            {
                if (subReportData.All(x => x.Bookcat != item.Key.Bookcat))
                {
                    throw new Exception("Booking category not represented");
                }

                var index = subReportData.ToList().FindIndex(x => x.Bookcat == item.Key.Bookcat);

                subReportData[index].Totairchg += item.Key.Airchg;
                subReportData[index].Trips += item.Key.Airchg < 0 ? -1 : 1;
            }

            //get the average air charge for each sub report group
            foreach (var item in subReportData.Where(item => item.Totairchg > 0))
            {
                item.Avgairchg = item.Trips == 0 ? 0 : MathHelper.Round(item.Totairchg / item.Trips, 2);
            }

            return subReportData;
        }
    }
}
