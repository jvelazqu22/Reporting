using Domain.Models.ReportPrograms.InvoiceReport;
using iBank.Server.Utilities.Classes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.Invoice
{
    public class InvoiceFinalTaxesData
    {
        public static void AddTaxes(List<SubReportData> subReportData, List<RawData> rawData, List<SubReportData> svcFeeTaxes, ReportGlobals globals)
        {
            foreach (var tax in rawData.Where(x => x.Tax1 != 0 || x.Tax2 != 0 || x.Tax3 != 0 || x.Tax4 != 0))
            {
                var transactionDate = tax.Invdate ?? DateTime.MinValue;

                if (tax.Tax1 != 0)
                {
                    var temp = subReportData.FirstOrDefault(x => x.Reckey == tax.RecKey);
                    UpdateSubReportData(temp, subReportData, tax.RecKey, transactionDate, globals.User.Tax1Name, tax.Tax1);
                }
                
                if (tax.Tax2 != 0)
                {
                    var temp = subReportData.FirstOrDefault(x => x.Reckey == tax.RecKey && x.Taxamt == 0);
                    UpdateSubReportData(temp, subReportData, tax.RecKey, transactionDate, globals.User.Tax2Name, tax.Tax2);
                }

                if (tax.Tax3 != 0)
                {
                    var temp = subReportData.FirstOrDefault(x => x.Reckey == tax.RecKey && x.Taxamt == 0);
                    UpdateSubReportData(temp, subReportData, tax.RecKey, transactionDate, globals.User.Tax3Name, tax.Tax3);
                }

                if (tax.Tax4 != 0)
                {
                    var temp = subReportData.FirstOrDefault(x => x.Reckey == tax.RecKey && x.Taxamt == 0);
                    UpdateSubReportData(temp, subReportData, tax.RecKey, transactionDate, globals.User.Tax4Name, tax.Tax4);
                }
            }
        }

        public static void AddServiceFeeTaxes(List<SubReportData> subReportData, List<SubReportData> svcFeeTaxes)
        {
            foreach (var svcFeeTax in svcFeeTaxes)
            {
                if (svcFeeTax.Taxamt != 0)
                {
                    var temp = subReportData.FirstOrDefault(s => s.Reckey == svcFeeTax.Reckey && s.Taxamt == 0);
                    UpdateSubReportData(temp, subReportData, svcFeeTax.Reckey, svcFeeTax.Trandate, svcFeeTax.Taxname, svcFeeTax.Taxamt);
                }
            }
        }

        private static void UpdateSubReportData(SubReportData temp, List<SubReportData> subReport, int recKey, DateTime transactionDate, string taxName, decimal taxAmount)
        {
            if (temp == null)
            {
                subReport.Add(new SubReportData
                                        {
                                            Reckey = recKey,
                                            Trandate = transactionDate,
                                            Chargedesc = string.Empty,
                                            Charge = 0,
                                            Taxname = $"{taxName}:",
                                            Taxamt =  taxAmount
                                        });
            }
            else
            {
                temp.Taxname = $"{taxName}:";
                temp.Taxamt = taxAmount;
            }
        }
    }
}
