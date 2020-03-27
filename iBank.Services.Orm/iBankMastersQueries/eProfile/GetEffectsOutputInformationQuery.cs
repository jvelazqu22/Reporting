using Domain.Helper;
using Domain.Interfaces;
using iBank.Services.Orm.Databases.Interfaces;
using System;
using System.Linq;

namespace iBank.Services.Orm.iBankMastersQueries.eProfile
{
    public class GetEffectsOutputInformationQuery : IQuery<EffectsOutputInformation>
    {
        private readonly IMastersQueryable _db;
        private int EProfileNumber { get; }
        private string Agency { get; }

        public GetEffectsOutputInformationQuery(IMastersQueryable db, int eProfileNumber, string agency)
        {
            _db = db;
            EProfileNumber = eProfileNumber;
            Agency = agency;
        }

        public EffectsOutputInformation ExecuteQuery()
        {
            using (_db)
            {
                var rec = _db.EProfiles.Where(x => x.eProfileNo == EProfileNumber && x.agency.Equals(Agency, StringComparison.OrdinalIgnoreCase))
                                        .Join(_db.TradingPartners, 
                                                e => e.tradingPartNo, 
                                                t => t.TradingPartNo, 
                                                (e, t) =>
                                                            new EffectsOutputInformation
                                                                {
                                                                    EProfileNumber = e.eProfileNo,
                                                                    TradingPartnerNumber = e.tradingPartNo,
                                                                    DirectDelivery = e.directDelivery,
                                                                    ProfileName = e.profilename.Trim(),
                                                                    FileNameMask = e.fileNameMask.Trim(),
                                                                    ZipOutput = e.ZipOutput,
                                                                    TradingPartnerName = t.TradingPartName.Trim(),
                                                                    Outbox = t.outbox.Trim()
                                                                }).FirstOrDefault();

                return rec ?? new EffectsOutputInformation();
            }
        }
    }
}
