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
    class Connection : IConnection
    {
        private readonly IRestClient restClient;
        private readonly ISession session;

        public Connection(IRestClient restClient, ISession session)
        {
            this.restClient = restClient;
            this.session = session;
        }

        private Task<IRestResponse> ExecuteRequest(IYouTrackRequest request, Method method)
        {
            IRestRequest restRequest = CreateRestRequest(request, method);

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

        private Task<IRestResponse> ExecuteRequestWithFile(IYouTrackFileRequest request, Method method)
        {
            IRestRequest restRequest = CreateRestRequestWithFile(request, method);

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

        private Task<IRestResponse<TResponse>> ExecuteRequest<TResponse>(IYouTrackRequest request, Method method) where TResponse : new()
        {
            IRestRequest restRequest = CreateRestRequest(request, method);
            
            var tcs = new TaskCompletionSource<IRestResponse<TResponse>>();
            restClient.ExecuteAsync<TResponse>(restRequest, restResponse =>
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

            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                case HttpStatusCode.Forbidden:
                case HttpStatusCode.Unauthorized:
                    throw new RequestFailedException(response);

                case HttpStatusCode.NotFound:
                    throw new RequestNotFoundException(response);
            }
        }

        public Task<string> Put(IYouTrackPutRequest request)
        {
            return ExecuteRequestWithAuthentication(request, Method.PUT)
                .ContinueWith(r =>
                                  {
                                      TaskHelper.ThrowIfExceptionOccured(r);

                                      var response = r.Result;
                                      return GetLocationHeaderValue(response);
                                  });
        }

        public Task<TResponse> Get<TResponse>(IYouTrackGetRequest request) where TResponse : new()
        {
            return ExecuteRequestWithAuthentication<TResponse>(request, Method.GET)
                .ContinueWith(r =>
                                  {
                                      TaskHelper.ThrowIfExceptionOccured(r);

                                      var response = r.Result;
                                      return response.Data;
                                  });
        }

        public Task Get(IYouTrackGetRequest request)
        {
            return ExecuteRequestWithAuthentication(request, Method.GET);
        }

        public Task Delete(IYouTrackDeleteRequest request)
        {
            return ExecuteRequestWithAuthentication(request, Method.DELETE);
        }

        public Task Post(IYouTrackPostRequest request)
        {
            return ExecuteRequestWithAuthentication(request, Method.POST);
        }

        public Task PostWithFile(IYouTrackPostWithFileRequest request)
        {
            return ExecuteRequestWithAuthenticationAndFile(request, Method.POST);
        }

        private string GetLocationHeaderValue(IRestResponse response)
        {
            Func<Parameter, bool> locationPredicate = h => h.Name.ToLowerInvariant() == "location";

            ThrowIfHeaderCountInvalid(response, locationPredicate);

            return response.Headers.Single(locationPredicate).Value.ToString();
        }

        private void ThrowIfHeaderCountInvalid(IRestResponse response, Func<Parameter, bool> locationPredicate)
        {
            if (response.Headers == null)
            {
                throw new ArgumentNullException("response.Headers", "Response Headers are null.");
            }

            if (response.Headers.Count(locationPredicate) != 1)
            {
                throw new LocationHeaderCountInvalidException(response.Headers);
            }
        }

        private Task<IRestResponse> ExecuteRequestWithAuthentication(IYouTrackRequest request, Method method)
        {
            return Task.Factory.StartNew(() =>
                                             {
                                                 LoginIfNotAuthenticated().Wait();
                                                 return ExecuteRequest(request, method).Result;
                                             });
        }

        private Task<IRestResponse> ExecuteRequestWithAuthenticationAndFile(IYouTrackFileRequest request, Method method)
        {
            return Task.Factory.StartNew(() =>
                                             {
                                                 LoginIfNotAuthenticated().Wait();
                                                 return ExecuteRequestWithFile(request, method).Result;
                                             });
        }

        private Task<IRestResponse<TResponse>> ExecuteRequestWithAuthentication<TResponse>(IYouTrackRequest request, Method method) where TResponse : new()
        {
            return Task.Factory.StartNew(() =>
                                             {
                                                 LoginIfNotAuthenticated().Wait();
                                                 return ExecuteRequest<TResponse>(request, method).Result;
                                             });
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
                restRequest.AddFile(request.Name, request.Bytes, request.FileName);
            }
            else
            {
                restRequest.AddFile(request.Name, request.FilePath);
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

        private Task LoginIfNotAuthenticated()
        {
            if (!session.IsAuthenticated)
            {
                return session.Login() ?? TaskHelper.EmptyTask;
            }

            return TaskHelper.EmptyTask;
        }
    }
}