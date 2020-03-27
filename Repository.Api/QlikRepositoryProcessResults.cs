using System;
using System.Configuration;
using System.Reflection;
using com.ciswired.libraries.CISLogger;
using Domain.Constants;
using Domain.Models.Repository.Api;

namespace Repository.Api
{
    public class QlikRepositoryProcessResults
    {
        //
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Result PrintStoryProcessResultAndErrorChecking(Result result)
        {
            if (result == null) return new Result() { StatusID = QlikApiStatus.FAIL_UNKNOWN };

            if (result.StatusID == QlikApiStatus.SUCCESS) return GetResultIWthFinalDetails(result);

            result.StatusID = QlikApiStatus.FAIL_ERROR;
            result.ErrorMsg = result.PrintResponse.Error ?? "result.PrintResponse.Error is null";

            var msg = string.Empty;
            if (result.ServiceRequest == null)
                msg = "result.ServiceRequest is null is null";
            else if (result.ServiceRequest.ServiceBody == null)
                msg = "result.ServiceRequest.ServiceBody is null";
            else if (result.ServiceRequest.ServiceBody.AppInfo == null)
                msg = "result.ServiceRequest.ServiceBody.AppInfo is null";
            else
                msg =
                    $"User id: {result.ServiceRequest.UserInfo} App Name: {result.ServiceRequest.ServiceBody.AppInfo.AppName} " +
                    $"AppId: {result.ServiceRequest.ServiceBody.AppInfo.AppId} StoryId: {result.ServiceRequest.ServiceBody.StoryId}" +
                    $"Output type: {result.ServiceRequest.ServiceBody.OutputType} Filter Names: {string.Join(",", result.ServiceRequest.ServiceBody.FilterNames)}" +
                    $"Error: {result.ErrorMsg}";

            LOG.Error(msg);

            return result;
        }

        private Result GetResultIWthFinalDetails(Result result)
        {
            try
            {
                // qURL=../tempcontent/13e2ad95-9d65-4853-8460-4a0043057d37/9c1970786c224a66915687757b75db5d.pptx?serverNodeId=832bbee0-5bbc-43b7-8ba7-961daca47027
                var url = result.PrintResponse.QUrl;
                var requestId = result.PrintResponse.RequestId.Replace("-", ""); 

                var array = url.Split('?');
                var urlStrWithFileName = array[0];
                var startPosition = urlStrWithFileName.IndexOf(requestId, 0, StringComparison.Ordinal);
                result.ReportName = urlStrWithFileName.Substring(startPosition);
                var reportNameArray = result.ReportName.Split('.');
                result.ReportName = $"{result.PrintResponse.RequestId}.{reportNameArray[1]}";
                var subDirectory = result.ServiceRequest.ExportSubDirectory;

                if (string.IsNullOrEmpty(subDirectory))
                {
                    result.ReportUrl = ConfigurationManager.AppSettings["QlikApiExportUrl"] + result.ReportName;
                }
                else
                {
                    result.ReportUrl = $"{ConfigurationManager.AppSettings["QlikApiExportUrl"]}/{subDirectory}/{result.ReportName}";
                }
            }
            catch (Exception ex)
            {
                result.StatusID = QlikApiStatus.FILE_NAME_ERROR;
                LOG.Error(ex);
            }

            return result;
        }

        public Result UpdateStatusToFailureFromPrintStory(Result result, Exception ex)
        {
            if (result == null) result = new Result() { StatusID = QlikApiStatus.FAIL_EXCEPTION };

            result.StatusID = QlikApiStatus.FAIL_EXCEPTION;
            var errorMsg = string.Empty;
            if (result.ServiceRequest == null)
                errorMsg = "result.ServiceRequest is null is null";
            else if (result.ServiceRequest.ServiceBody == null)
                errorMsg = "result.ServiceRequest.ServiceBody is null";
            else if (result.ServiceRequest.ServiceBody.AppInfo == null)
                errorMsg = "result.ServiceRequest.ServiceBody.AppInfo is null";
            else
                errorMsg =
                    $"User id: {result.ServiceRequest.UserInfo} App Name: {result.ServiceRequest.ServiceBody.AppInfo.AppName} " +
                    $"AopId: {result.ServiceRequest.ServiceBody.AppInfo.AppId} StoryId: {result.ServiceRequest.ServiceBody.StoryId}" +
                    $"Output type: {result.ServiceRequest.ServiceBody.OutputType} Filter Names: {string.Join(",", result.ServiceRequest.ServiceBody.FilterNames)}";

            LOG.Error(errorMsg, ex);
            
            return result;
        }

    }
}
