using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using RestSharp;
using YouTrack.Rest.Exceptions;
using YouTrack.Rest.Requests;

namespace YouTrack.Rest
{
    class Session : ISession
    {
        private readonly IRestClient restClient;
        private readonly string login;
        private readonly string password;

        public Session(IRestClient restClient, string login, string password)
        {
            this.restClient = restClient;
            this.login = login;
            this.password = password;
        }

        public bool IsAuthenticated
        {
            get { return AuthenticationCookies != null; }
        }

        public IDictionary<string, string> AuthenticationCookies { get; private set; }

#if !SILVERLIGHT
        public void Login()
        {
            IRestResponse loginResponse = ExecuteRequest(new LoginRequest(login, password), Method.POST);

            AuthenticationCookies = loginResponse.Cookies.ToDictionary(c => c.Name, c => c.Value);
        }

        private IRestResponse ExecuteRequest(IYouTrackRequest request, Method method)
        {
            IRestRequest restRequest = new RestRequest(request.RestResource, method);
            IRestResponse restResponse = restClient.Execute(restRequest);

            ThrowIfRequestFailed(restResponse);

            return restResponse;
        }
#endif
        public void LoginAsync(Action onSuccess, Action<Exception> onError)
        {
            ExecuteRequestAsync(new LoginRequest(login, password), Method.POST, success =>
                                                                                    {
                                                                                        AuthenticationCookies = success.Cookies.ToDictionary(c => c.Name, c => c.Value);
                                                                                        onSuccess();
                                                                                    }, onError);
        }

        private void ExecuteRequestAsync(IYouTrackRequest request, Method method, Action<IRestResponse> onSuccess, Action<Exception> onError)
        {
            IRestRequest restRequest = new RestRequest(request.RestResource, method);
            restClient.ExecuteAsync(restRequest, restResponse =>
                                                     {
                                                         var exception = GetExceptionIfRequestFailed(restResponse);

                                                         if (exception == null)
                                                             onSuccess(restResponse);
                                                         else
                                                             onError(exception);
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
            }

            return null;
        }
    }
}
