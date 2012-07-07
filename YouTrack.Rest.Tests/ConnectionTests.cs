using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using RestSharp;
using YouTrack.Rest.Exceptions;
using YouTrack.Rest.Requests;

namespace YouTrack.Rest.Tests
{
    public class ConnectionTestItem
    {

    }

    class ConnectionTests : TestFor<Connection>
    {
        private IRestClient restClient;
        private IRestResponse restResponse;
        private IRestResponse<ConnectionTestItem> restResponseWithTestItem;
        private List<Parameter> parametersWithLocationHeader;
        private ISession session;
        private Dictionary<string, string> authenticationCookies;
        private IYouTrackPostWithFileRequest postWithFileRequest;

        protected override Connection CreateSut()
        {
            restClient = Mock<IRestClient>();
            session = Mock<ISession>();

            return new Connection(restClient, session);
        }

        protected override void SetupDependencies()
        {
            restResponse = Mock<IRestResponse>();
            restResponseWithTestItem = Mock<IRestResponse<ConnectionTestItem>>();
            parametersWithLocationHeader = CreateParametersWithLocationHeader();

            authenticationCookies = CreateAuthenticationCookies();

            restClient.ExecuteAsync(Arg.Any<IRestRequest>(), Arg.Invoke(restResponse, Mock<RestRequestAsyncHandle>()));
            restClient.ExecuteAsync<ConnectionTestItem>(Arg.Any<IRestRequest>(), Arg.Invoke(restResponseWithTestItem, Mock<RestRequestAsyncHandle>()));

            postWithFileRequest = Mock<IYouTrackPostWithFileRequest>();
        }

        private Dictionary<string, string> CreateAuthenticationCookies()
        {
            Dictionary<string, string> cookies = new Dictionary<string, string>();
            cookies.Add("foo", "bar");

            return cookies;
        }

        private List<Parameter> CreateParametersWithLocationHeader()
        {
            List<Parameter> parameters = new List<Parameter>();
            parameters.Add(CreateParameterWithLocationHeader());


            return parameters;
        }

        private Parameter CreateParameterWithLocationHeader()
        {
            Parameter parameter = new Parameter();
            parameter.Name = "Location";
            parameter.Type = ParameterType.HttpHeader;
            parameter.Value = "foobar";

            return parameter;
        }

        [Test]
        public void AuthenticationCookiesAreSetWhenAuthenticated()
        {
            session.IsAuthenticated.Returns(true);
            session.AuthenticationCookies.Returns(authenticationCookies);
            restResponse.ResponseStatus.Returns(ResponseStatus.Completed);

            Sut.Get(Mock<IYouTrackGetRequest>()).Wait();

            AssertThatRestRequestContainsAuthenticationCookie();
        }

        private void AssertThatRestRequestContainsAuthenticationCookie()
        {
            restClient.Received().ExecuteAsync(Arg.Is<IRestRequest>(x => x.Parameters.Any(p => p.Type == ParameterType.Cookie && p.Name == "foo")), Arg.Any<Action<IRestResponse, RestRequestAsyncHandle>>());
        }

        [Test]
        public void RestClientIsCalledWithGetMethod()
        {
            restResponse.ResponseStatus.Returns(ResponseStatus.Completed);

            Sut.Get(Mock<IYouTrackGetRequest>()).Wait();

            AssertThatRestClientExecuteWasCalledWithMethod(Method.GET);
        }

        [Test]
        public void RequestNotFoundExceptionThrownOnNotFound()
        {
            restResponse.ResponseStatus.Returns(ResponseStatus.Completed);
            restResponse.StatusCode.Returns(HttpStatusCode.NotFound);

            AssertThatThrowsAndAggregateExceptionContains<RequestNotFoundException>(() => Sut.Get(Mock<IYouTrackGetRequest>()).Wait());
        }

        [Test]
        public void RequestFailedExceptionThrownOnBadRequest()
        {
            restResponse.StatusCode.Returns(HttpStatusCode.BadRequest);

            AssertThatThrowsAndAggregateExceptionContains<RequestFailedException>(() => Sut.Get(Mock<IYouTrackGetRequest>()).Wait());
        }

        [Test]
        public void RequestFailedExceptionThrownOnForbidden()
        {
            restResponse.StatusCode.Returns(HttpStatusCode.Forbidden);

            AssertThatThrowsAndAggregateExceptionContains<RequestFailedException>(() => Sut.Get(Mock<IYouTrackGetRequest>()).Wait());
        }

        [Test]
        public void RequestFailedExceptionThrownOnUnauthorized()
        {
            restResponse.StatusCode.Returns(HttpStatusCode.Unauthorized);

            AssertThatThrowsAndAggregateExceptionContains<RequestFailedException>(() => Sut.Get(Mock<IYouTrackGetRequest>()).Wait());
        }

        private void AssertThatRestClientExecuteWasCalledWithMethod(Method method)
        {
            restClient.Received().ExecuteAsync(Arg.Is<IRestRequest>(x => x.Method == method), Arg.Any<Action<IRestResponse, RestRequestAsyncHandle>>());
        }

        private void AssertThatRestClientExecuteWasCalledWithMethod<TResponse>(Method method) where TResponse : new()
        {
            restClient.Received().ExecuteAsync<TResponse>(Arg.Is<IRestRequest>(x => x.Method == method), Arg.Any<Action<IRestResponse<TResponse>, RestRequestAsyncHandle>>());
        }

        [Test]
        public void RestClientCalledWitGetMethodAndResponseType()
        {
            restResponseWithTestItem.ResponseStatus.Returns(ResponseStatus.Completed);

            Sut.Get<ConnectionTestItem>(Mock<IYouTrackGetRequest>()).Wait();

            AssertThatRestClientExecuteWasCalledWithMethod<ConnectionTestItem>(Method.GET);
        }

        [Test]
        public void RestClientCalledWithDeleteMethod()
        {
            restResponse.ResponseStatus.Returns(ResponseStatus.Completed);

            Sut.Delete(Mock<IYouTrackDeleteRequest>()).Wait();

            AssertThatRestClientExecuteWasCalledWithMethod(Method.DELETE);
        }

        [Test]
        public void RestClientCalledWithPostMethod()
        {
            restResponse.ResponseStatus.Returns(ResponseStatus.Completed);

            Sut.Post(Mock<IYouTrackPostRequest>()).Wait();

            AssertThatRestClientExecuteWasCalledWithMethod(Method.POST);
        }

        [Test]
        public void LocationHeaderCountInvalidThrownOnMissingLocationHeader()
        {
            restResponse.ResponseStatus.Returns(ResponseStatus.Completed);

            AssertThatThrowsAndAggregateExceptionContains<LocationHeaderCountInvalidException>(() => Sut.Put(Mock<IYouTrackPutRequest>()).Wait());
        }

        [Test]
        public void RestClientCalledWithPutMethod()
        {
            restResponse.ResponseStatus.Returns(ResponseStatus.Completed);
            restResponse.Headers.Returns(parametersWithLocationHeader);

            Sut.Put(Mock<IYouTrackPutRequest>()).Wait();

            AssertThatRestClientExecuteWasCalledWithMethod(Method.PUT);
        }

        [Test]
        public void ArgumentNullExceptionThrownOnMissingResponseHeaders()
        {
            restResponse.ResponseStatus.Returns(ResponseStatus.Completed);
            restResponse.Headers.Returns(null as IList<Parameter>);

            AssertThatThrowsAndAggregateExceptionContains<ArgumentNullException>(() => Sut.Put(Mock<IYouTrackPutRequest>()).Wait());
        }

        [Test]
        public void PostingFileIsCalledWithPostMethod()
        {
            restResponse.ResponseStatus.Returns(ResponseStatus.Completed);

            Sut.PostWithFile(postWithFileRequest).Wait();

            AssertThatRestClientExecuteWasCalledWithMethod(Method.POST);
        }

        [Test]
        public void FileIsPostedWithName()
        {
            postWithFileRequest.Name.Returns("files");
            restResponse.ResponseStatus.Returns(ResponseStatus.Completed);

            Sut.PostWithFile(postWithFileRequest).Wait();

            AssertThatRestClientExecuteWasCalledWithFileAndName("files");
        }

        [Test]
        public void FileIsPostedWithFilePath()
        {
            postWithFileRequest.FilePath.Returns("foo.jpg");
            restResponse.ResponseStatus.Returns(ResponseStatus.Completed);

            Sut.PostWithFile(postWithFileRequest).Wait();

            AssertThatRestClientExecuteWasCalledWithFile("foo.jpg");
        }

        [Test]
        public void FileIsPostedWithBytes()
        {
            byte[] bytes = new byte[512];
            string filename = "foo.txt";

            postWithFileRequest.HasBytes.Returns(true);
            postWithFileRequest.Bytes.Returns(bytes);
            postWithFileRequest.FileName.Returns(filename);
            restResponse.ResponseStatus.Returns(ResponseStatus.Completed);

            Sut.PostWithFile(postWithFileRequest).Wait();

            AssertThatRestClientExecuteWasCalledWithBytes(filename, bytes);
        }

        private void AssertThatThrowsAndAggregateExceptionContains<T>(Action action)
        {
            var ex = Assert.Throws<AggregateException>(() => action());
            Assert.IsInstanceOf<T>(ex.GetBaseException());
        }

        private void AssertThatRestClientExecuteWasCalledWithBytes(string filename, byte[] bytes)
        {
            restClient.Received().ExecuteAsync(Arg.Is<IRestRequest>(x => x.Files.Any(f => f.FileName == filename && f.ContentLength == bytes.Length)), Arg.Any<Action<IRestResponse, RestRequestAsyncHandle>>());
        }

        private void AssertThatRestClientExecuteWasCalledWithFile(string filePath)
        {
            restClient.Received().ExecuteAsync(Arg.Is<IRestRequest>(x => x.Files.Any(f => f.FileName == filePath)), Arg.Any<Action<IRestResponse, RestRequestAsyncHandle>>());
        }

        private void AssertThatRestClientExecuteWasCalledWithFileAndName(string name)
        {
            restClient.Received().ExecuteAsync(Arg.Is<IRestRequest>(x => x.Files.Any(f => f.Name == name)), Arg.Any<Action<IRestResponse, RestRequestAsyncHandle>>());
        }
    }
}
