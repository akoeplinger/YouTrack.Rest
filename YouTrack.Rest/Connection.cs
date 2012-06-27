using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using RestSharp;
using YouTrack.Rest.Exceptions;
using YouTrack.Rest.Requests;

namespace YouTrack.Rest
{
    class Connection : IConnection
    {
        private readonly IRestClient restClient;
        private readonly ISession session;

        public Connection(IRestClient restClient, ISession session)
        {
            this.restClient = restClient;
            this.session = session;
        }

#if !SILVERLIGHT
        private IRestResponse ExecuteRequest(IYouTrackRequest request, Method method)
        {
            IRestRequest restRequest = CreateRestRequest(request, method);
            IRestResponse restResponse = restClient.Execute(restRequest);

            ThrowIfRequestFailed(restResponse);

            return restResponse;
        }

        private IRestResponse ExecuteRequestWithFile(IYouTrackFileRequest request, Method method)
        {
            IRestRequest restRequest = CreateRestRequestWithFile(request, method);
            IRestResponse restResponse = restClient.Execute(restRequest);

            ThrowIfRequestFailed(restResponse);

            return restResponse;
        }

        private IRestResponse<TResponse> ExecuteRequest<TResponse>(IYouTrackRequest request, Method method) where TResponse : new()
        {
            IRestRequest restRequest = CreateRestRequest(request, method);
            IRestResponse<TResponse> restResponse = restClient.Execute<TResponse>(restRequest);

            ThrowIfRequestFailed(restResponse);

            return restResponse;
        }

        private IRestResponse ExecuteRequestWithAuthentication(IYouTrackRequest request, Method method)
        {
            LoginIfNotAuthenticated();

            return ExecuteRequest(request, method);
        }

        private IRestResponse ExecuteRequestWithAuthenticationAndFile(IYouTrackFileRequest request, Method method)
        {
            LoginIfNotAuthenticated();

            return ExecuteRequestWithFile(request, method);
        }

        private IRestResponse<TResponse> ExecuteRequestWithAuthentication<TResponse>(IYouTrackRequest request, Method method) where TResponse : new()
        {
            LoginIfNotAuthenticated();

            return ExecuteRequest<TResponse>(request, method);
        }

        private void LoginIfNotAuthenticated()
        {
            if (!session.IsAuthenticated)
            {
                session.Login();
            }
        }

        public string Put(IYouTrackPutRequest request)
        {
            IRestResponse response = ExecuteRequestWithAuthentication(request, Method.PUT);

            return GetLocationHeaderValue(response);
        }

        public TResponse Get<TResponse>(IYouTrackGetRequest request) where TResponse : new()
        {
            IRestResponse<TResponse> response = ExecuteRequestWithAuthentication<TResponse>(request, Method.GET);

            return response.Data;
        }

        public void Get(IYouTrackGetRequest request)
        {
            ExecuteRequestWithAuthentication(request, Method.GET);
        }

        public void Delete(IYouTrackDeleteRequest request)
        {
            ExecuteRequestWithAuthentication(request, Method.DELETE);
        }

        public void Post(IYouTrackPostRequest request)
        {
            ExecuteRequestWithAuthentication(request, Method.POST);
        }

        public void PostWithFile(IYouTrackPostWithFileRequest request)
        {
            ExecuteRequestWithAuthenticationAndFile(request, Method.POST);
        }
#endif

        private void ExecuteRequestAsync(IYouTrackRequest request, Method method, Action<IRestResponse> onSuccess, Action<Exception> onError)
        {
            IRestRequest restRequest = CreateRestRequest(request, method);
            restClient.ExecuteAsync(restRequest, restResponse =>
                                                     {
                                                         if (restResponse.ResponseStatus != ResponseStatus.Completed)
                                                         {
                                                             onError(restResponse.ErrorException ?? new Exception(restResponse.ErrorMessage));
                                                             return;
                                                         }

                                                         var requestFailedException = GetExceptionIfRequestFailed(restResponse);
                                                         if (requestFailedException != null)
                                                         {
                                                             onError(requestFailedException);
                                                             return;
                                                         }

                                                         onSuccess(restResponse);
                                                     });
        }

        private void ExecuteRequestWithFileAsync(IYouTrackFileRequest request, Method method, Action<IRestResponse> onSuccess, Action<Exception> onError)
        {
            IRestRequest restRequest = CreateRestRequestWithFile(request, method);
            restClient.ExecuteAsync(restRequest, restResponse =>
                                                     {
                                                         if (restResponse.ResponseStatus != ResponseStatus.Completed)
                                                         {
                                                             onError(restResponse.ErrorException ?? new Exception(restResponse.ErrorMessage));
                                                             return;
                                                         }

                                                         var requestFailedException = GetExceptionIfRequestFailed(restResponse);
                                                         if (requestFailedException != null)
                                                         {
                                                             onError(requestFailedException);
                                                             return;
                                                         }

                                                         onSuccess(restResponse);
                                                     });
        }

        private void ExecuteRequestAsync<TResponse>(IYouTrackRequest request, Method method, Action<IRestResponse<TResponse>> onSuccess, Action<Exception> onError) where TResponse : new()
        {
            IRestRequest restRequest = CreateRestRequest(request, method);
            restClient.ExecuteAsync<TResponse>(restRequest, restResponse =>
                                                                {
                                                                    if (restResponse.ResponseStatus != ResponseStatus.Completed)
                                                                    {
                                                                        onError(restResponse.ErrorException ?? new Exception(restResponse.ErrorMessage));
                                                                        return;
                                                                    }

                                                                    var requestFailedException = GetExceptionIfRequestFailed(restResponse);
                                                                    if (requestFailedException != null)
                                                                    {
                                                                        onError(requestFailedException);
                                                                        return;
                                                                    }

                                                                    onSuccess(restResponse);
                                                                });
        }

        private void ThrowIfRequestFailed(IRestResponse response)
        {
            var exception = GetExceptionIfRequestFailed(response);
            if (exception != null)
                throw exception;
        }

        private Exception GetExceptionIfRequestFailed(IRestResponse response)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                case HttpStatusCode.Forbidden:
                case HttpStatusCode.Unauthorized:
                    return new RequestFailedException(response);

                case HttpStatusCode.NotFound:
                    return new RequestNotFoundException(response);
            }

            return null;
        }

        private void LoginIfNotAuthenticatedAsync(Action onSuccess, Action<Exception> onError)
        {
            if (!session.IsAuthenticated)
            {
                session.LoginAsync(onSuccess, onError);
                return;
            }

            onSuccess();
        }

        public void PutAsync(IYouTrackPutRequest request, Action<string> onSuccess, Action<Exception> onError)
        {
            ExecuteRequestWithAuthenticationAsync(request, Method.PUT, success =>
                                                                           {
                                                                               Exception exception = null;
                                                                               var locationHeaderValue = GetLocationHeaderValue(success, out exception);

                                                                               if (exception == null)
                                                                                   onSuccess(locationHeaderValue);
                                                                               else
                                                                                   onError(exception);
                                                                           }, onError);
        }

        public void GetAsync<TResponse>(IYouTrackGetRequest request, Action<TResponse> onSuccess, Action<Exception> onError) where TResponse : new()
        {
            ExecuteRequestWithAuthenticationAsync<TResponse>(request, Method.GET, success => onSuccess(success.Data), onError);
        }

        public void GetAsync(IYouTrackGetRequest request, Action onSuccess, Action<Exception> onError)
        {
            ExecuteRequestWithAuthenticationAsync(request, Method.GET, success => onSuccess(), onError);
        }

        public void DeleteAsync(IYouTrackDeleteRequest request, Action onSuccess, Action<Exception> onError)
        {
            ExecuteRequestWithAuthenticationAsync(request, Method.DELETE, success => onSuccess(), onError);
        }

        public void PostAsync(IYouTrackPostRequest request, Action onSuccess, Action<Exception> onError)
        {
            ExecuteRequestWithAuthenticationAsync(request, Method.POST, success => onSuccess(), onError);
        }

        public void PostWithFileAsync(IYouTrackPostWithFileRequest request, Action onSuccess, Action<Exception> onError)
        {
            ExecuteRequestWithAuthenticationAndFileAsync(request, Method.POST, success => onSuccess(), onError);
        }

        private string GetLocationHeaderValue(IRestResponse response)
        {
            Exception exception;

            var locationHeaderValue = GetLocationHeaderValue(response, out exception);

            if (exception != null)
                throw exception;

            return locationHeaderValue;
        }

        private string GetLocationHeaderValue(IRestResponse response, out Exception exception)
        {
            Func<Parameter, bool> locationPredicate = h => h.Name.ToLowerInvariant() == "location";

            exception = GetExceptionIfHeaderCountInvalid(response, locationPredicate);

            return response.Headers.Single(locationPredicate).Value.ToString();
        }

        private Exception GetExceptionIfHeaderCountInvalid(IRestResponse response, Func<Parameter, bool> locationPredicate)
        {
            if (response.Headers == null)
            {
                return new ArgumentNullException("response.Headers", "Response Headers are null.");
            }

            if (response.Headers.Count(locationPredicate) != 1)
            {
                return new LocationHeaderCountInvalidException(response.Headers);
            }

            return null;
        }

        private void ExecuteRequestWithAuthenticationAsync(IYouTrackRequest request, Method method, Action<IRestResponse> onSuccess, Action<Exception> onError)
        {
            LoginIfNotAuthenticatedAsync(() => ExecuteRequestAsync(request, method, onSuccess, onError), onError);
        }

        private void ExecuteRequestWithAuthenticationAndFileAsync(IYouTrackFileRequest request, Method method, Action<IRestResponse> onSuccess, Action<Exception> onError)
        {
            LoginIfNotAuthenticatedAsync(() => ExecuteRequestWithFileAsync(request, method, onSuccess, onError), onError);
        }

        private void ExecuteRequestWithAuthenticationAsync<TResponse>(IYouTrackRequest request, Method method, Action<IRestResponse<TResponse>> onSuccess, Action<Exception> onError) where TResponse : new()
        {
            LoginIfNotAuthenticatedAsync(() => ExecuteRequestAsync(request, method, onSuccess, onError), onError);
        }

        private IRestRequest CreateRestRequest(IYouTrackRequest request, Method method)
        {
            RestRequest restRequest = new RestRequest(request.RestResource, method);

            SetAcceptToXml(restRequest);

            if (session.IsAuthenticated)
            {
                SetAuthenticationCookies(restRequest);
            }

            return restRequest;
        }

        private IRestRequest CreateRestRequestWithFile(IYouTrackFileRequest request, Method method)
        {
            IRestRequest restRequest = CreateRestRequest(request, method);
            AddFileToRestRequest(request, restRequest);

            return restRequest;
        }

        private void AddFileToRestRequest(IYouTrackFileRequest request, IRestRequest restRequest)
        {
            if (request.HasBytes)
            {
#if SILVERLIGHT
                // NOTE: IRestRequest doesn't contain AddFile() method on WP7 and Silverlight, so cast to RestRequest.
                //       This should be fixed in the next version of RestSharp
                ((RestRequest)restRequest).AddFile(request.Name, request.Bytes, request.FileName);
#else
                restRequest.AddFile(request.Name, request.Bytes, request.FileName);
#endif
            }
            else
            {
#if SILVERLIGHT
                // NOTE: IRestRequest doesn't contain AddFile() method on WP7 and Silverlight, so cast to RestRequest.
                //       This should be fixed in the next version of RestSharp.
                ((RestRequest)restRequest).AddFile(request.Name, request.FilePath);
#else
                restRequest.AddFile(request.Name, request.FilePath);
#endif
            }
        }

        private void SetAcceptToXml(RestRequest restRequest)
        {
            Func<Parameter, bool> acceptPredicate = p => p.Name == "Accept";

            if (restRequest.Parameters.Count(acceptPredicate) > 1)
            {
                throw new InvalidAcceptHeaderCountException(restRequest);
            }

            if (restRequest.Parameters.Count(acceptPredicate) == 1)
            {
                Parameter parameter = restRequest.Parameters.Single(acceptPredicate);
                parameter.Value = "application/xml";
            }
            else
            {
                restRequest.AddHeader("Accept", "application/xml");
            }
        }

        private void SetAuthenticationCookies(RestRequest restRequest)
        {
            foreach (KeyValuePair<string, string> cookie in session.AuthenticationCookies)
            {
                restRequest.AddCookie(cookie.Key, cookie.Value);
            }
        }
    }
}