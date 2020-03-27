
using Domain.Helper;
using Domain.Orm.iBankClientQueries;
using Domain.Models.ReportPrograms.TravelOptixReport;
using System;
using com.ciswired.libraries.CISLogger;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using Repository.Api;
using Domain.Interfaces.Repository.Api;
using Domain.Models.Repository.Api;
using Domain.Constants;

namespace iBank.Services.Implementation.ReportPrograms.TravelOptixReport
{
    public class TravelOptix : ReportRunner<RawData, FinalData>
    {
        private readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public int ReportKey { get; set; }
        public UserInfo _userInfo { get; set; }
        public AppInfo _appInfo { get; set; }
        public List<FilterInfo> _filterInfoList { get; set; }
        public ServiceBody _serviceBody { get; set; }

        public override bool InitialChecks()
        {
            ReportKey = Convert.ToInt32(Globals.GetParmValue(WhereCriteria.UDRKEY));
            _filterInfoList = new List<FilterInfo>();
            return IsValidReportKey();
        }
        public override bool GetRawData()
        {
            using (var timer = new CustomTimer(Globals.ProcessKey, Globals.UserNumber, Globals.Agency, Globals.ReportLogKey, LOG, "TravelOptix: GetRawData", ReportKey))
            {
                var report = new GetTravelOptixReportQuery(ClientStore.ClientQueryDb, ReportKey).ExecuteQuery();
                RawDataList = new List<RawData>
                {
                    new RawData {
                        Agency = report.Agency,
                        AppId = report.AppId,
                        AppName = report.AppName,
                        StoryId = report.StoryId,
                        StoryName = report.StoryName,
                        ReportName = report.ReportName,
                        UserId = report.UserId,
                        TOXUserId = report.TOXUserId.Trim(),
                        TOXUserDirectory = report.TOXUserDirectory.Trim(),
                        UserNumber = report.UserNumber
                    }
                };
                //right now just one user story.
                _userInfo = new UserInfo
                {
                    UserDirectory = report.TOXUserDirectory,
                    UserId = report.TOXUserId,
                };
                _appInfo = new AppInfo
                {
                    AppId = RawDataList[0].AppId,
                    AppName = RawDataList[0].AppName
                };

            return true;
            }
        }
        public override bool ProcessData()
        {
            var item = new FinalData
            {
                Agency = Globals.Agency,
                AppId = RawDataList[0].AppId,
                AppName = RawDataList[0].AppName,
                StoryId = RawDataList[0].StoryId,
                StoryName = RawDataList[0].StoryName,
                ReportName = RawDataList[0].ReportName,
                UserId = RawDataList[0].UserId,
                TOXUserId = RawDataList[0].TOXUserId.Trim(),
                TOXUserDirectory = RawDataList[0].TOXUserDirectory.Trim(),
                UserNumber = RawDataList[0].UserNumber
            };

            var reports = new GetTravelOptixReport2Query(ClientStore.ClientQueryDb, ReportKey).ExecuteQuery();
            if (reports.Count > 0)
            {
                var report = reports[0];
                item.FilterName = report.FilterName;
                item.FilterDataType = report.FilterDataType;
                item.FilterValues = report.FilterValues;
                item.FilterOrder = report.FilterOrder;

                if (!string.IsNullOrWhiteSpace(report.FilterName))
                {
                    var filterInfo = new FilterInfo
                    {
                        FilterName = report.FilterName,
                        FilterValues = GetFitlerValuesList(report.FilterValues)
                    };
                    _filterInfoList.Add(filterInfo);
                }
            }
            FinalDataList.Add(item);           

            return true;
        }
        public override bool GenerateReport()
        {
            LOG.Debug("GenerateReport - Start");
            var sw = Stopwatch.StartNew();

            var repository = new QlikRepository();

            _serviceBody = new ServiceBody()
            {
                AppInfo = _appInfo,
                StoryId = FinalDataList[0].StoryId, //only request one story pour report
                OutputType =  Globals.OutputFormat == DestinationSwitch.ClassicPdf 
                    ? "pdf" : 
                    "ppt",
                FilterInfo = _filterInfoList 
            };

            var serverRequest = new ServiceRequest {
                ServiceBody = _serviceBody,
                ExportSubDirectory = Globals.Agency,
                ServiceType = "printStories",
                UserInfo = _userInfo  
            };

            var printRequest = repository.PrintStoryRequest(serverRequest);
            
            if (printRequest.StatusID != QlikApiStatus.SUCCESS)
            {
                LOG.Error($"GenerateReport - printRequest | StatusID:[{printRequest.StatusID}]");
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = Globals.ReportMessages.GenericErrorMessage;
                sw.Stop();
                return false;
            }
            else
            {
                LOG.Debug($"GenerateReport - Report Name: {printRequest.ReportName}");
                LOG.Debug($"GenerateReport - Request return url: {printRequest.ReportUrl}");
                Globals.SavedReportName = printRequest.ReportName;
                Globals.ReportInformation.Href = printRequest.ReportUrl;
            }

            sw.Stop();
            LOG.Debug($"GenerateReport - End | Elapsed:[{sw.Elapsed}]");
            return true;
        }

        private bool IsValidReportKey()
        {
            if (ReportKey < 0)
            {
                Globals.ReportInformation.ReturnCode = 2;
                Globals.ReportInformation.ErrorMessage = $"Invalid ReportKey Globals.GetParmValue(WhereCriteria.UDRKEY)";
                return false;
            }
            return true;
        }

        private List<string> GetFitlerValuesList(string values)
        {
            var result = new List<string>();

            var arrays = values.Split(',');
            foreach(var item in arrays)
            {
                result.Add(item);
            }
            return result;
        }
    }
}
