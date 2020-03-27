using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces;
using Domain.Models.ReportPrograms.UserDefinedReport;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers
{
    public static class LegSegOrderFixer
    {
        public static void UpdateSequenceToCorrectOrder(List<LegRawData> list) 
        {
            //must be sort by reckey and seqno first
            list = list.OrderBy(x => x.RecKey).ThenBy(x => x.SeqNo).ToList();

            var groupedReckeys = list.GroupBy(x => x.RecKey).Select(grp => new { Reckey = grp.Key, Trip = grp });

            foreach (var t in groupedReckeys)
            {
                var segCounter = 1;
                foreach (var trip in t.Trip)
                {
                    trip.Seg_Cntr = segCounter;
                    trip.SeqNo = segCounter;
                    segCounter++;
                }
            }            
        }
    }
}
