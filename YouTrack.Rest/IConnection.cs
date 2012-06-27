using System;
using YouTrack.Rest.Requests;

namespace YouTrack.Rest
{
    public interface IConnection
    {
#if !SILVERLIGHT
        string Put(IYouTrackPutRequest request);
        TResponse Get<TResponse>(IYouTrackGetRequest request) where TResponse : new();
        void Get(IYouTrackGetRequest request);
        void Delete(IYouTrackDeleteRequest request);
        void Post(IYouTrackPostRequest request);
        void PostWithFile(IYouTrackPostWithFileRequest request);
#endif
        void PutAsync(IYouTrackPutRequest request, Action<string> onSuccess, Action<Exception> onError);
        void GetAsync<TResponse>(IYouTrackGetRequest request, Action<TResponse> onSuccess, Action<Exception> onError) where TResponse : new();
        void GetAsync(IYouTrackGetRequest request, Action onSuccess, Action<Exception> onError);
        void DeleteAsync(IYouTrackDeleteRequest request, Action onSuccess, Action<Exception> onError);
        void PostAsync(IYouTrackPostRequest request, Action onSuccess, Action<Exception> onError);
        void PostWithFileAsync(IYouTrackPostWithFileRequest request, Action onSuccess, Action<Exception> onError);
    }
}