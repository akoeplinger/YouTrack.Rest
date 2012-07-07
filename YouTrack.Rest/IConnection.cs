using System.Threading.Tasks;
using YouTrack.Rest.Requests;

namespace YouTrack.Rest
{
    public interface IConnection
    {
        Task<string> Put(IYouTrackPutRequest request);
        Task<TResponse> Get<TResponse>(IYouTrackGetRequest request) where TResponse : new();
        Task Get(IYouTrackGetRequest request);
        Task Delete(IYouTrackDeleteRequest request);
        Task Post(IYouTrackPostRequest request);
        Task PostWithFile(IYouTrackPostWithFileRequest request);
    }
}