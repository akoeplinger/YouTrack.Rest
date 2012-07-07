using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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

        public Task Login()
        {
            return ExecuteRequest(new LoginRequest(login, password), Method.POST)
                .ContinueWith(r =>
                                  {
                                      TaskHelper.ThrowIfExceptionOccured(r);

                                      IRestResponse loginResponse = r.Result;
                                      AuthenticationCookies = loginResponse.Cookies.ToDictionary(c => c.Name, c => c.Value);
                                  });
        }

        private Task<IRestResponse> ExecuteRequest(IYouTrackRequest request, Method method)
        {
            IRestRequest restRequest = new RestRequest(request.RestResource, method);

            var tcs = new TaskCompletionSource<IRestResponse>();
            restClient.ExecuteAsync(restRequest, restResponse =>
                                                     {
                                                         try
                                                         {
                                                             // TODO: get rid of try-catch
                                                             ThrowIfRequestFailed(restResponse);
                                                             tcs.SetResult(restResponse);
                                                         }
                                                         catch (Exception ex)
                                                         {
                                                             tcs.SetException(ex);
                                                         }
                                                     });

            return tcs.Task;
        }

        private void ThrowIfRequestFailed(IRestResponse response)
        {
            if (response.ResponseStatus != ResponseStatus.Completed)
                throw new RequestFailedException(response);

            switch(response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                case HttpStatusCode.Forbidden:
                case HttpStatusCode.Unauthorized:
                    throw new RequestFailedException(response);
            }
        }
    }
}
