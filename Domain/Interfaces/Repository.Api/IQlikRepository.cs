using System.Net.Http;
using Domain.Models.Repository.Api;

namespace Domain.Interfaces.Repository.Api
{
    public interface IQlikRepository
    {
        HttpResponseMessage RequestStories(ServiceRequest request);

        HttpResponseMessage RequestAppInfo(ServiceRequest request);

        Result PrintStoryRequest(ServiceRequest request);
    }
}
