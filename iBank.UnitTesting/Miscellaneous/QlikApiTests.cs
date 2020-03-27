using System.Collections.Generic;
using System.Net;
using Domain.Constants;
using Domain.Models.Repository.Api;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Repository.Api;

namespace iBank.UnitTesting.Miscellaneous
{
    [TestClass]
    public class QlikApiTests
    {
        [TestMethod]
        public void TestRequestStories()
        {
            var request = new ServiceRequest();
            request.ServiceType = "requestStories";
            request.UserInfo = new UserInfo() { UserId = "84", UserDirectory = "CIS" };

#if DEBUG
            var result = new QlikRepository().RequestStories(request);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
#endif

        }

        [TestMethod]
        public void TestRequestAppInfo()
        {
            var request = new ServiceRequest();
            request.ServiceType = "requestAppInfo";
            request.UserInfo = new UserInfo() { UserId = "84", UserDirectory = "CIS" };
            request.ServiceBody = new ServiceBody()
            {
                AppInfo = new AppInfo() { AppId = "b15b1802-4d7a-4560-ad35-1f2cefefd4ad" },
                FilterNames = new List<string>() { "agency" }
            };

#if DEBUG
            var result = new QlikRepository().RequestAppInfo(request);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
#endif

        }

        [TestMethod]
        public void TestRequestDimmensions()
        {
            var request = new ServiceRequest();
            request.ServiceType = "requestAppDimensions";
            request.UserInfo = new UserInfo() { UserId = "84", UserDirectory = "CIS" };
            request.ServiceBody = new ServiceBody()
            {
                AppInfo = new AppInfo() { AppId = "b15b1802-4d7a-4560-ad35-1f2cefefd4ad" },
            };
#if DEBUG
            var result = new QlikRepository().RequestAppInfo(request);
            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
#endif

        }

        [TestMethod]
        public void TestStaticPrintStory()
        {
            var request = new ServiceRequest();
            request.ServiceType = "printStories";
            request.ExportSubDirectory = "DEMO";
            request.UserInfo = new UserInfo() { UserId = "84", UserDirectory = "CIS" };
            request.ServiceBody = new ServiceBody()
            {
                AppInfo = new AppInfo() { AppName = "Jacob-Sandbox", AppId = "b15b1802-4d7a-4560-ad35-1f2cefefd4ad" },
                StoryId = "631df3b9-2f41-4516-be55-f0fb0c570c68",
                OutputType = "ppt",
            };

            #if DEBUG
                var result = new QlikRepository().PrintStoryRequest(request);
                Assert.IsFalse(string.IsNullOrEmpty(result.ReportName));
                Assert.IsTrue(result.ReportUrl.Contains(request.ExportSubDirectory));
            #endif
        }

        [TestMethod]
        public void TestStaticPrintStoryPdf()
        {
            var request = new ServiceRequest();
            request.ServiceType = "printStories";
            request.ExportSubDirectory = "DEMO";
            request.UserInfo = new UserInfo() { UserId = "84", UserDirectory = "CIS" };
            request.ServiceBody = new ServiceBody()
            {
                AppInfo = new AppInfo() { AppName = "Jacob-Sandbox", AppId = "b15b1802-4d7a-4560-ad35-1f2cefefd4ad" },
                StoryId = "631df3b9-2f41-4516-be55-f0fb0c570c68",
                OutputType = "pdf",
            };

            #if DEBUG
                var result = new QlikRepository().PrintStoryRequest(request);
                Assert.IsFalse(string.IsNullOrEmpty(result.ReportName));
                Assert.IsTrue(result.ReportUrl.Contains(request.ExportSubDirectory));
                Assert.IsTrue(result.ReportUrl.Contains(".pdf"));
            #endif
        }


        [TestMethod]
        public void TestPrintStoryWithFilters()
        {
            var request = new ServiceRequest();
            request.ServiceType = "printStories";
            request.ExportSubDirectory = "DEMO";
            request.UserInfo = new UserInfo() { UserId = "84", UserDirectory = "CIS" };
            request.ServiceBody = new ServiceBody()
            {
                AppInfo = new AppInfo() { AppName = "Jacob-Sandbox", AppId = "79c0b428-e0dd-457a-8b15-b99ddfcf40f8" },
                StoryId = "d66e1a0c-aa7b-45c8-99a2-02c42047b539",
                OutputType = "ppt",
                FilterInfo = new List<FilterInfo>()
                {
                    new FilterInfo()
                    {
                        FilterName = "Year",
                        FilterValues = new List<string>() { "2017", "2018"}
                    }
                }
            };

            #if DEBUG
                var result = new QlikRepository().PrintStoryRequest(request);
                Assert.IsFalse(string.IsNullOrEmpty(result.ReportName));
                Assert.IsTrue(result.ReportUrl.Contains(request.ExportSubDirectory));
            #endif
        }

        [TestMethod]
        public void TestPrintStoryWithFiltersPageAndSourceSizes()
        {
            var request = new ServiceRequest();
            request.ServiceType = "printStories";
            request.ExportSubDirectory = "DEMO";
            request.UserInfo = new UserInfo() { UserId = "84", UserDirectory = "CIS" };
            request.ServiceBody = new ServiceBody()
            {
                AppInfo = new AppInfo() { AppName = "Jacob-Sandbox", AppId = "79c0b428-e0dd-457a-8b15-b99ddfcf40f8" },
                StoryId = "d66e1a0c-aa7b-45c8-99a2-02c42047b539",
                OutputType = "ppt",
                FilterInfo = new List<FilterInfo>()
                {
                    new FilterInfo()
                    {
                        FilterName = "Year",
                        FilterValues = new List<string>() { "2017", "2018"}
                    }
                },
                PageSize = new PageSize()
                {
                    Width = 1665,
                    Height = 937,
                    Dpi = 120
                },
                SourceSize = new SourceSize()
                {
                    Width = 985,
                    Height = 554,
                    Dpi = 96
                }
            };

            #if DEBUG
                var result = new QlikRepository().PrintStoryRequest(request);
                Assert.IsFalse(string.IsNullOrEmpty(result.ReportName));
                Assert.IsTrue(result.ReportUrl.Contains(request.ExportSubDirectory));
            #endif
        }

        [TestMethod]
        public void TestPrintStoryWithFiltersPageAndSourceSizesInPdfFormat()
        {
            var request = new ServiceRequest();
            request.ServiceType = "printStories";
            request.ExportSubDirectory = "DEMO";
            request.UserInfo = new UserInfo() { UserId = "84", UserDirectory = "CIS" };
            request.ServiceBody = new ServiceBody()
            {
                AppInfo = new AppInfo() { AppName = "Jacob-Sandbox", AppId = "79c0b428-e0dd-457a-8b15-b99ddfcf40f8" },
                StoryId = "d66e1a0c-aa7b-45c8-99a2-02c42047b539",
                OutputType = "pdf",
                FilterInfo = new List<FilterInfo>()
                {
                    new FilterInfo()
                    {
                        FilterName = "Year",
                        FilterValues = new List<string>() { "2017", "2018"}
                    }
                },
                PageSize = new PageSize()
                {
                    Width = 1665,
                    Height = 937,
                    Dpi = 120
                },
                SourceSize = new SourceSize()
                {
                    Width = 985,
                    Height = 554,
                    Dpi = 96
                }
            };

            #if DEBUG
                var result = new QlikRepository().PrintStoryRequest(request);
                Assert.IsFalse(string.IsNullOrEmpty(result.ReportName));
                Assert.IsTrue(result.ReportUrl.Contains(request.ExportSubDirectory));
                Assert.IsTrue(result.ReportUrl.Contains(".pdf"));
            #endif
        }

        [TestMethod]
        public void TimeoutExceptionDefectReproductionAndTesting()
        {
            var request = new ServiceRequest();
            request.ServiceType = "printStories";
            request.ExportSubDirectory = "DEMO";
            request.UserInfo = new UserInfo() { UserId = "84", UserDirectory = "CIS" };
            request.ServiceBody = new ServiceBody()
            {
                AppInfo = new AppInfo() { AppName = "Spend Analysis", AppId = "79c0b428-e0dd-457a-8b15-b99ddfcf40f8" },
                StoryId = "d66e1a0c-aa7b-45c8-99a2-02c42047b539",
                OutputType = "pdf",
                FilterInfo = new List<FilterInfo>()
                {
                    new FilterInfo()
                    {
                        FilterName = "Year",
                        FilterValues = new List<string>() { "2017", "2018"}
                    }
                },
            };

#if DEBUG
            var result = new QlikRepository().PrintStoryRequest(request);
            Assert.AreEqual(QlikApiStatus.SUCCESS, result.StatusID);
            Assert.IsFalse(string.IsNullOrEmpty(result.ReportName));
            Assert.IsTrue(result.ReportUrl.Contains(request.ExportSubDirectory));
            Assert.IsTrue(result.ReportUrl.Contains(".pdf"));
#endif
        }


    }
}
