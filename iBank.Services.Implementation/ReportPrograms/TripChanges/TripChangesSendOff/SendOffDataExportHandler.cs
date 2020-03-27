using Domain.Models.ReportPrograms.TripChangesSendOffReport;
using System.Collections.Generic;
using System.Linq;

namespace iBank.Services.Implementation.ReportPrograms.TripChanges.TripChangesSendOff
{
    public class SendOffDataExportHandler
    {
        SendOffDataProcessor sendOffDataProcessor = new SendOffDataProcessor();
        SendOffCalculations sendOffCalculations = new SendOffCalculations();
        /// <summary>
        /// Get the formatted final data and set the export fields for the CSV/Excel export of the report
        /// </summary>
        /// <returns></returns>
        public List<FinalDataExport> SetupFinalReportAndSetExportFields(bool consolidateChanges, List<FinalData> finalDataList, bool useAirportCodes, 
           bool includeEmailAddress, string udidLabel1, string udidLabel2, ref List<string> exportFields)
        {

            var displayFields = 1;
            var finalDataExportList = new List<FinalDataExport>();
            if (!consolidateChanges)
            {
                finalDataExportList = sendOffDataProcessor.MapFinalDataToFinalDataExportWithoutConsolidatingChanges(finalDataList);
            }
            else
            {
                finalDataExportList = sendOffDataProcessor.MapFinalDataToFinalDataExportConsolidateChanges(finalDataList);

                foreach (var exportData in finalDataExportList)
                {
                    var useChangeFields = 1;
                    string changeDesc1 = "", changeDesc2 = "", changeDesc3 = "";
                    string changeStamp1 = "", changeStamp2 = "", changeStamp3 = "";

                    var records = sendOffDataProcessor.GetMatchingFinalDataRecords(exportData, finalDataList);

                    UpdateChangeFields(records, ref useChangeFields, ref changeDesc1, ref changeStamp1, ref changeDesc2, ref changeStamp2, ref changeDesc3, ref changeStamp3); 

                    displayFields = displayFields > useChangeFields ? displayFields : useChangeFields;

                    exportData.Changedesc = sendOffCalculations.RemovePipe(changeDesc1);
                    exportData.Changedesc2 = sendOffCalculations.RemovePipe(changeDesc2);
                    exportData.Changedesc3 = sendOffCalculations.RemovePipe(changeDesc3);

                    exportData.Changstamp = sendOffCalculations.RemovePipe(changeStamp1);
                    exportData.Changstamp2 = sendOffCalculations.RemovePipe(changeStamp2);
                    exportData.Changstamp3 = sendOffCalculations.RemovePipe(changeStamp3);

                }

                finalDataExportList = finalDataExportList.OrderBy(s => s.Origdesc).ThenBy(s => s.Destinat).ToList();
            }

            if (useAirportCodes)
            {
                finalDataExportList.ForEach(s =>
                {
                    s.Origin = s.Origdesc;
                    s.LastOrigin = s.LastOrigin;
                });
            }

            var udid1ColumnName = sendOffCalculations.GetUdidColumnOneName(udidLabel1);
            var udid2ColumnName = sendOffCalculations.GetUdidColumnTwoName(udidLabel2, udid1ColumnName);
            exportFields = sendOffCalculations.GetExportFields(includeEmailAddress, useAirportCodes, consolidateChanges, displayFields, udid1ColumnName, udid2ColumnName);

            return finalDataExportList;
        }

        private void UpdateChangeFields(List<FinalData> records, ref int useChangeFields, ref string changeDesc1, ref string changeStamp1, 
            ref string changeDesc2, ref string changeStamp2, ref string changeDesc3, ref string changeStamp3) 
        {
            foreach (var record in records)
            {
                var originalChangeDescription = sendOffCalculations.ShortenChangeDescription(record.Changedesc);
                if (!string.IsNullOrEmpty(originalChangeDescription))
                {
                    useChangeFields = sendOffCalculations.GetUseChangeFields(useChangeFields, originalChangeDescription, changeDesc1, changeDesc2);

                    if (sendOffCalculations.ChangeDescriptionsNotPresent(changeDesc1, changeDesc2, changeDesc3, originalChangeDescription))
                    {
                        switch (useChangeFields)
                        {
                            case 1:
                                changeDesc1 = sendOffCalculations.AddPipe(changeDesc1, originalChangeDescription);
                                changeStamp1 = sendOffCalculations.AddPipe(changeStamp1, record.Changstamp);
                                break;
                            case 2:
                                changeDesc2 = sendOffCalculations.AddPipe(changeDesc2, originalChangeDescription);
                                changeStamp2 = sendOffCalculations.AddPipe(changeStamp2, record.Changstamp);
                                break;
                            case 3:
                                changeDesc3 = sendOffCalculations.AddPipe(changeDesc3, originalChangeDescription);
                                changeStamp3 = sendOffCalculations.AddPipe(changeStamp3, record.Changstamp);
                                break;
                        }
                    }
                }
            }

        }
    }
}
