using Domain.Models.ReportPrograms.TripChangesMeetAndGreetReport;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.TripChanges.TripChangesMeetAndGreet
{
    public class MeetGreetFinalDataGroupAndSort
    {
        public List<FinalData> GroupFinalData(List<FinalData> FinalDataList)
        {
            return FinalDataList.GroupBy(
                s =>
                    new
                    {
                        s.Reckey,
                        s.Mtggrpnbr,
                        s.Passlast,
                        s.Passfrst,
                        s.Emailaddr,
                        s.Destinat,
                        s.Destdesc,
                        s.Origdesc,
                        s.Lastorgdes,
                        s.Rarrdate,
                        s.Alinedesc,
                        s.Airline,
                        s.Fltno,
                        s.Arrtime,
                        s.Deptime,
                        s.Sorttime,
                        s.Recloc,
                        s.Ticketed,
                        s.Bookdate
                    },
                (k, g) => new FinalData
                {
                    Reckey = k.Reckey,
                    Mtggrpnbr = k.Mtggrpnbr,
                    Passlast = k.Passlast,
                    Passfrst = k.Passfrst,
                    Emailaddr = k.Emailaddr,
                    Destinat = k.Destinat,
                    Destdesc = k.Destdesc,
                    Origdesc = k.Origdesc,
                    Lastorgdes = k.Lastorgdes,
                    Rarrdate = k.Rarrdate,
                    Alinedesc = k.Alinedesc,
                    Fltno = k.Fltno,
                    Arrtime = k.Arrtime,
                    Deptime = k.Deptime,
                    Sorttime = k.Sorttime,
                    Recloc = k.Recloc,
                    Ticketed = k.Ticketed,
                    Bookdate = k.Bookdate,
                    Changedesc = g.Min(t => t.Changedesc),
                    Changstamp = g.Max(t => t.Changstamp)
                }).ToList();
        }

        public List<FinalData> GetSortedFinalList(List<FinalData> FinalDataList, string sortBy)
        {
            if (sortBy.Equals("2"))
            {
                return FinalDataList =
                    FinalDataList.OrderBy(s => s.Destdesc)
                        .ThenBy(s => s.Rarrdate)
                        .ThenBy(s => s.Sorttime)
                        .ThenBy(s => s.Airline)
                        .ThenBy(s => s.Alinedesc)
                        .ThenBy(s => s.Fltno)
                        .ThenBy(s => s.Passlast)
                        .ThenBy(s => s.Passfrst)
                        .ThenBy(s => s.Changstamp).ToList();
            }

            if (sortBy.Equals("3"))
            {
                return FinalDataList =
                    FinalDataList.OrderBy(s => s.Destdesc)
                        .ThenBy(s => s.Rarrdate)
                        .ThenBy(s => s.Passlast)
                        .ThenBy(s => s.Passfrst)
                        .ThenBy(s => s.Sorttime)
                        .ThenBy(s => s.Airline)
                        .ThenBy(s => s.Alinedesc)
                        .ThenBy(s => s.Fltno)
                        .ThenBy(s => s.Changstamp).ToList();
            }

            return FinalDataList =
                FinalDataList.OrderBy(s => s.Destdesc)
                    .ThenBy(s => s.Rarrdate)
                    .ThenBy(s => s.Airline)
                    .ThenBy(s => s.Alinedesc)
                    .ThenBy(s => s.Arrtime)
                    .ThenBy(s => s.Fltno)
                    .ThenBy(s => s.Passlast)
                    .ThenBy(s => s.Passfrst)
                    .ThenBy(s => s.Changstamp).ToList();
        }
    }
}
