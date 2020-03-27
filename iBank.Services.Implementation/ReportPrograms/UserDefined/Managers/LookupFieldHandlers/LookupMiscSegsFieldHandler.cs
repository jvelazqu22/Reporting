using System.Linq;
using Domain.Models.ReportPrograms.UserDefinedReport;
using UserDefinedReports;

namespace iBank.Services.Implementation.ReportPrograms.UserDefined.Managers.LookupFieldHandlers
{
    public class LookupMiscSegsFieldHandler
    {
        private readonly UserDefinedParameters _userDefinedParams;

        public LookupMiscSegsFieldHandler(UserDefinedParameters userDefinedParams)
        {
            _userDefinedParams = userDefinedParams;
        }

        public string HandleLookupFieldMiscSegsLegacy(UserReportColumnInformation column, RawData mainRec)
        {
            var rec = _userDefinedParams.MiscLookup[mainRec.RecKey].FirstOrDefault();

            if (rec == null) return string.Empty;

            switch (column.Name)
            {
                case "MSVENDCODE":
                    return rec.Vendorcode;
                case "MSCLASS":
                    return rec.Class;
                case "MSMONEYTYP":
                    return rec.Moneytype;
                case "MSTAX1":
                    return rec.Tax1.ToString();
                case "MSTAX2":
                    return rec.Tax2.ToString();
                case "MSTAX3":
                    return rec.Tax3.ToString();
                case "MSTAX4":
                    return rec.Tax4.ToString();
                case "MSCONFIRM":
                    return rec.Confirmno;
                default:
                    return ReportBuilder.GetValueAsString(rec, column.Name);
            }
        }
    }
}
