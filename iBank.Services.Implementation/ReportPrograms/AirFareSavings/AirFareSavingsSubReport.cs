using System.Collections.Generic;
using System.Linq;
using Domain.Helper;
using Domain.Models.ReportPrograms.AirFareSavingsReport;
using iBank.Repository.SQL.Interfaces;
using iBank.Server.Utilities.Classes;
using iBank.Services.Implementation.Shared.Client;

namespace iBank.Services.Implementation.ReportPrograms.AirFareSavings
{
    public class AirFareSavingsSubReport
    {
        private readonly List<SubReportData> _subReportDataList = new List<SubReportData>();
        private readonly AirFareSavingsSubReportSavings _airFareSavingsSubReportSavings = new AirFareSavingsSubReportSavings();
        private readonly AirFareSavingsSubReportLosses _airFareSavingsSubReportLosses = new AirFareSavingsSubReportLosses();

        public List<SubReportData> GetSubReportDataList()
        {
            return _subReportDataList;
        }

        public bool BuildSummaryData(ClientFunctions clientFunctions, bool lExSavings, bool lExNegoSvgs, List<FinalData> FinalDataList, ReportGlobals Globals, bool SummaryPageOnly, 
            IClientDataStore clientStore, IMasterDataStore masterStore)
        {
            var nCnt1SavCodes = 0;
            var aSavCodes = new List<SummaryDataInformation>();

            if (!lExSavings)
            {
                aSavCodes = _airFareSavingsSubReportSavings.GetSavingCodesSummaryData(clientFunctions, FinalDataList, Globals, clientStore, masterStore);
                nCnt1SavCodes = aSavCodes.Count;
            }

            var aLossCodes = _airFareSavingsSubReportLosses.GetLossCodesSummaryData(clientFunctions, FinalDataList, Globals, clientStore, masterStore);
            var nCnt2LossCodes = aLossCodes.Count;

            var nCnt3NegoSvgs = 0;
            var aNegoSvngs = new List<SummaryDataInformation>();
            if (!lExNegoSvgs && !lExSavings)
            {
                aNegoSvngs = _airFareSavingsSubReportSavings.GetNegotiatedSavingsSummaryData(FinalDataList);
                nCnt3NegoSvgs = aNegoSvngs.Count;
            }

            SummaryPageOnly = Globals.IsParmValueOn(WhereCriteria.CBSUMPAGEONLY);
            if (SummaryPageOnly &&
                nCnt1SavCodes + nCnt2LossCodes + nCnt3NegoSvgs == 0)
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = Globals.ReportMessages.RptMsg_NoData;
                return false;
            }

            //Skip this section of code and just combine the three sets below. The intermediate step isn't necessary.
            //Lines 721-753, builds aRptSum from combining aSavCodes and aNegoSvngs, and, if the ReasDesc is blank, fills it in with "None," translated if needed. See also: LookupFunctions.LookupLanguageTranslation and replaces <BR> tag with space.

            //Skip this and just combine the three sets below. The intermediate step isn't necessary.
            //Lines 755-775, builds aLossSum. Similar but only processes aLossCodes. See also: LookupFunctions.LookupLanguageTranslation

            PopulateSubReportDataList(aSavCodes, aLossCodes, aNegoSvngs, lExSavings, lExNegoSvgs);
            return true;
        }


        public void PopulateSubReportDataList(List<SummaryDataInformation> aSavCodes, List<SummaryDataInformation> aLossCodes, List<SummaryDataInformation> aNegoSvngs, bool lExSavings, bool lExNegoSvgs)
        {
            int saveCodesCounter = aSavCodes.Count;
            int lossCodesCounter = aLossCodes.Count;
            aSavCodes = aSavCodes.OrderBy(o => o.Description).ToList();
            aLossCodes = aLossCodes.OrderBy(o => o.Description).ToList();
            if (lossCodesCounter >= saveCodesCounter || (lossCodesCounter > 0 && lExSavings))
            {
                _airFareSavingsSubReportLosses.LossCodesFirstSavingsCodesSecondSetUp(aSavCodes, aLossCodes, lExSavings, _subReportDataList);
            }
            else if (!lExSavings)
            {
                _airFareSavingsSubReportSavings.SavingsCodesFirstLossCodesSecondSetUp(aSavCodes, aLossCodes, _subReportDataList);
            }

            if (!lExNegoSvgs)
            {
                _airFareSavingsSubReportSavings.NegotiatedSavings(aNegoSvngs, _subReportDataList);
            }
        }

    }
}
