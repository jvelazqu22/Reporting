using Domain.Models.ReportPrograms.InvoiceReport;
using iBank.Server.Utilities.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.Invoice
{
    public class InvoiceFinalSvcFeeData
    {
        public static void AddSvcFeeDataToSubReport(List<SubReportData> subReportData, List<SvcFeeRawData> svcFeeData, RawData row,
                                                    string acceptableTransactionTypes, ReportGlobals globals, List<SubReportData> svcFeeTaxlist, string xServiceFee)
        {
            var svcfees = svcFeeData.Where(s => s.RecKey == row.RecKey && acceptableTransactionTypes.Contains(row.Trantype.Trim()))
                .OrderBy(s => s.Trandate);

            foreach (var svcFee in svcfees)
            {
                subReportData.Add(GetTaxData(row, svcFee, "UNUSED", 0, $"{svcFee.SvcDesc.Trim()}:", svcFee.SvcAmt));
                if (svcFee.Tax1 != 0) svcFeeTaxlist.Add(GetTaxData(row, svcFee, $"{xServiceFee} {globals.User.Tax1Name}", svcFee.Tax1, "", 0));
                if (svcFee.Tax2 != 0) svcFeeTaxlist.Add(GetTaxData(row, svcFee, $"{xServiceFee} {globals.User.Tax2Name}", svcFee.Tax2, "", 0));
                if (svcFee.Tax3 != 0) svcFeeTaxlist.Add(GetTaxData(row, svcFee, $"{xServiceFee} {globals.User.Tax3Name}", svcFee.Tax3, "", 0));
                if (svcFee.Tax4 != 0) svcFeeTaxlist.Add(GetTaxData(row, svcFee, $"{xServiceFee} {globals.User.Tax4Name}", svcFee.Tax4, "", 0));
            }
        }

        private static SubReportData GetTaxData(RawData row, SvcFeeRawData svcFee, string taxName, decimal tax, string chargeDescription, decimal charge)
        {
            return new SubReportData
                       {
                           Reckey = row.RecKey,
                           Trandate = svcFee.Trandate ?? DateTime.MinValue,
                           Chargedesc = chargeDescription,
                           Charge = charge,
                           Taxname = taxName,
                           Taxamt = tax
                       };
        }

        public static SubReportData GetDefaultData(RawData row, string acceptableTransactionTypes, ReportGlobals globals, string xServiceFee)
        {
            return new SubReportData
                       {
                           Reckey = row.RecKey,
                           Trandate = row.Invdate ?? DateTime.MinValue,
                           Chargedesc = globals.AgencyInformation.AgencyName + " " + xServiceFee + ":",
                           Charge = row.SvcFee,
                           Taxname = "UNUSED",
                           Taxamt = 0
                       };
        }
    }
}
