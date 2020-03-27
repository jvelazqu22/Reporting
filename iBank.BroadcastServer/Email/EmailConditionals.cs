using iBank.Entities.MasterEntities;
using iBank.Server.Utilities;

namespace iBank.BroadcastServer.Email
{
    public class EmailConditionals
    {
        public bool IsHtmlVersion(bcstque4 batch)
        {
            return batch.mailformat.Equals("2");
        }

        public bool IsPlainTextWithHtmlAttachment(bcstque4 batch)
        {
            return batch.mailformat.Equals("3");
        }

        public bool AtLeastOneReportHasData(bool? noDataOption, bool allReportsEmpty)
        {
            return (!noDataOption.HasTrueValue() || (noDataOption.HasTrueValue() && !allReportsEmpty));
        }
    }
}
