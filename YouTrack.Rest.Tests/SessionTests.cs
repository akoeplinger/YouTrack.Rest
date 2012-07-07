using System;
using System.Collections.Generic;
using System.Net;
using NSubstitute;
using NUnit.Framework;
using RestSharp;
using YouTrack.Rest.Exceptions;

namespace YouTrack.Rest.Tests
{
    class SessionTests : TestFor<Session>
    {
        private IRestClient restClient;
        private IRestResponse badRequestResponse;
        private IRestResponse loginSuccessResponse;

        protected override Session CreateSut()
        {
            restClient = Mock<IRestClient>();
            
            return new Session(restClient, "login", "password");
        }

        protected override void SetupDependencies()
        {
            badRequestResponse = CreateBadRequestResponse();
            loginSuccessResponse = CreateLoginSuccessResponse();
        }

        private IRestResponse CreateBadRequestResponse()
        {
            IRestResponse response = Mock<IRestResponse>();

            response.StatusCode.Returns(HttpStatusCode.BadRequest);

            return response;
        }

        private IRestResponse CreateLoginSuccessResponse()
        {
            IRestResponse response = Mock<IRestResponse>();

            response.ResponseStatus.Returns(ResponseStatus.Completed);
            response.StatusCode.Returns(HttpStatusCode.OK);
            response.Cookies.Returns(Mock<IList<RestResponseCookie>>());

            return response;
        }

        [Test]
        public void LoginExceptionIsThrownOnLoginFailed()
        {
            restClient.ExecuteAsync(Arg.Any<IRestRequest>(), Arg.Invoke(badRequestResponse, Mock<RestRequestAsyncHandle>()));

            var ex = Assert.Throws<AggregateException>(() => Sut.Login().Wait());
            Assert.IsInstanceOf<RequestFailedException>(ex.GetBaseException());
        }

        [Test]
        public void LoginRequestIsUsedOnLogin()
        {
            restClient.ExecuteAsync(Arg.Any<IRestRequest>(), Arg.Invoke(loginSuccessResponse, Mock<RestRequestAsyncHandle>()));

            Sut.Login().Wait();
        }
    }
}