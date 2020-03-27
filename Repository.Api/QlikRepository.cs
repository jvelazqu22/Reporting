using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using Domain.Interfaces.Repository.Api;
using Domain.Models.Repository.Api;
using com.ciswired.libraries.CISLogger;
using Domain.Constants;

namespace Repository.Api
{
    public class QlikRepository : IQlikRepository
    {
        private static readonly ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private HttpClient GetClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["QlikApiBaseAddress"]); 
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        public HttpResponseMessage RequestStories(ServiceRequest request)
        {
            var response = new HttpResponseMessage(); 
            try
            {
                var client = GetClient();
                response = client.PostAsJsonAsync("api/StoryExport", request).Result;
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
            }
            return response;
        }

        public HttpResponseMessage RequestAppInfo(ServiceRequest request)
        {
            var response = new HttpResponseMessage();
            try
            {
                var client = GetClient();
                response = client.PostAsJsonAsync("api/StoryExport", request).Result;
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
            }
            return response;
        }

        public Result PrintStoryRequest(ServiceRequest request)
        {
            Result result = null;
            try
            {
                var client = GetClient();
                var response = client.PostAsJsonAsync("api/StoryExport", request).Result;
                if (response.IsSuccessStatusCode)
                {
                    var printResponse = response.Content.ReadAsAsync<PrintResponse>().Result;
                    result = ProcessStoryRequestResponseAndRetry(request, printResponse);
                }
                else
                {
                    result = new Result() { ServiceRequest = request, PrintResponse = response.Content.ReadAsAsync<PrintResponse>().Result };
                }
                return new QlikRepositoryProcessResults().PrintStoryProcessResultAndErrorChecking(result);
            }
            catch (Exception ex)
            {
                return new QlikRepositoryProcessResults().UpdateStatusToFailureFromPrintStory(result, ex);
            }
        }

        private Result ProcessStoryRequestResponseAndRetry(ServiceRequest originalRequest, PrintResponse printResponse)
        {
            var result = new Result() { ServiceRequest = originalRequest };

            var lastResponse = printResponse;
            var newRequest = new ServiceRequest()
            {
                ServiceType = "printStatus",
                ExportSubDirectory = originalRequest.ExportSubDirectory,
                UserInfo = new UserInfo() { UserId = originalRequest.UserInfo.UserId, UserDirectory = originalRequest.UserInfo.UserDirectory },
                ServiceBody = new ServiceBody() { RequestId = printResponse.RequestId }
            };
            return Retry(lastResponse, newRequest, result);
        }

        private Result Retry(PrintResponse lastResponse, ServiceRequest newRequest, Result result)
        {
            var retry = 30;
            var waitTime = 30 * 1000; // seconds
            while (lastResponse.Status.Equals("Processing", StringComparison.OrdinalIgnoreCase) || lastResponse.Status.Equals("Processed", StringComparison.OrdinalIgnoreCase))
            {
                var client = GetClient();

                var httpResponseMessage = client.PostAsJsonAsync("api/StoryExport", newRequest).Result;
                lastResponse = httpResponseMessage.Content.ReadAsAsync<PrintResponse>().Result;
                result.PrintResponse = lastResponse;
                if (lastResponse.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
                {
                    result.PrintResponse = lastResponse;
                    result.StatusID = QlikApiStatus.SUCCESS;
                    return result;
                }

                Thread.Sleep(waitTime);
                retry--;
                if (retry != 0) continue;
                result.StatusID = QlikApiStatus.FAIL_TIMEOUT;
                return result;
            }

            result.PrintResponse = lastResponse;
            if (lastResponse.Status.Equals("Exception", StringComparison.OrdinalIgnoreCase)) result.StatusID = QlikApiStatus.FAIL_EXCEPTION;
            result.StatusID = lastResponse.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase)
                ? QlikApiStatus.SUCCESS
                : QlikApiStatus.FAIL_UNKNOWN;

            return result;
        }
    }
}
