#if SILVERLIGHT
using System;
using System.Threading;
using RestSharp;

namespace YouTrack.Rest.Requests
{
    public static class RestSharpSilverlightExtensions
    {
        public static IRestResponse Execute(this IRestClient restClient, IRestRequest restRequest)
        {
            IRestResponse restResponse = null;
            Exception exceptionDuringRequest = null;
            var waitHandle = new ManualResetEvent(false);

            restClient.ExecuteAsync(restRequest, cb =>
            {
                exceptionDuringRequest = cb.ErrorException;
                restResponse = cb;
                waitHandle.Set();
            });

            waitHandle.WaitOne();

            if (exceptionDuringRequest != null)
                throw exceptionDuringRequest;

            return restResponse;
        }

        public static IRestResponse<TResponse> Execute<TResponse>(this IRestClient restClient, IRestRequest restRequest) where TResponse : new()
        {
            IRestResponse<TResponse> restResponse = null;
            Exception exceptionDuringRequest = null;
            var waitHandle = new ManualResetEvent(false);

            restClient.ExecuteAsync<TResponse>(restRequest, cb =>
            {
                exceptionDuringRequest = cb.ErrorException;
                restResponse = cb;
                waitHandle.Set();
            });

            waitHandle.WaitOne();

            if (exceptionDuringRequest != null)
                throw exceptionDuringRequest;

            return restResponse;
        }

        public static IRestRequest AddFile(this IRestRequest restRequest, string name, byte[] bytes, string fileName)
        {
            // NOTE: IRestRequest doesn't contain the AddFile() method on WP7 and Silverlight, so cast to RestRequest.           
            //       This should be fixed in the next version of RestSharp
            var slRestRequest = (RestRequest)restRequest;

            return slRestRequest.AddFile(name, bytes, fileName);
        }

        public static IRestRequest AddFile(this IRestRequest restRequest, string name, string path)
        {
            // NOTE: IRestRequest doesn't contain the AddFile() method on WP7 and Silverlight, so cast to RestRequest.           
            //       This should be fixed in the next version of RestSharp
            var slRestRequest = (RestRequest)restRequest;

            return slRestRequest.AddFile(name, path);
        }
    }
}
#endif