using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using RestSharp;
using YouTrack.Rest.Deserialization;
using YouTrack.Rest.Exceptions;
using YouTrack.Rest.Factories;
using YouTrack.Rest.Repositories;
using YouTrack.Rest.Requests;
using YouTrack.Rest.Tests.Requests;

namespace YouTrack.Rest.Tests.Repositories
{

    internal class IssueRepositoryTests : TestFor<IssueRepository>
    {
        private const string Project = "project";
        private const string Summary = "summary";
        private const string Description = "description";
        private const string IssueId = "FOO-BAR";
        private IConnection connection;
        private Rest.Deserialization.Issue deSerializedIssueMock;
        private IIssue issue;
        private CommentsCollection commentsCollection;
        private IIssueFactory issueFactory;

        protected override IssueRepository CreateSut()
        {
            connection = Mock<IConnection>();
            issue = Mock<IIssue>();
            deSerializedIssueMock = new DeserializedIssueMock(issue);
            commentsCollection = CreateCommentsWrapper();
            issueFactory = Mock<IIssueFactory>();

            return new IssueRepository(connection, issueFactory);
        }

        private static CommentsCollection CreateCommentsWrapper()
        {
            CommentsCollection commentsCollection = new CommentsCollection();
            commentsCollection.Comments = new List<Rest.Deserialization.Comment>();

            return commentsCollection;
        }

        [Test]
        public void IssueIsCreated()
        {
            connection.Put(Arg.Any<IYouTrackPutRequest>()).Returns(Task.Factory.StartNew(() => "foobar"));

            Sut.CreateIssue(Project, Summary, Description).Wait();

            issueFactory.Received().CreateIssue("foobar", connection);
        }

        [Test]
        public void ConnectionIsCalledOnCreateIssue()
        {
            connection.Put(Arg.Any<CreateNewIssueRequest>()).Returns(Task.Factory.StartNew(() => ""));

            Sut.CreateIssue(Project, Summary, Description).Wait();

            connection.Received().Put(Arg.Any<CreateNewIssueRequest>());
        }

        [Test]
        public void ConnectionIsCalledOnDeleteIssue()
        {
            connection.Delete(Arg.Any<DeleteIssueRequest>()).Returns(TaskHelper.EmptyTask);

            Sut.DeleteIssue(IssueId).Wait();

            connection.Received().Delete(Arg.Any<DeleteIssueRequest>());
        }

        [Test]
        public void FactoryIsCalledOnGetIssue()
        {
            Sut.GetIssue(IssueId);

            issueFactory.Received().CreateIssue(IssueId, connection);
        }

        [Test]
        public void ConnectionIsCalledOnIssueExists()
        {
            connection.Get(Arg.Any<CheckIfIssueExistsRequest>()).Returns(TaskHelper.EmptyTask);

            Sut.IssueExists(IssueId).Wait();

            connection.Received().Get(Arg.Any<CheckIfIssueExistsRequest>());
        }

        [Test]
        public void IssueExists()
        {
            connection.Get(Arg.Any<CheckIfIssueExistsRequest>()).Returns(TaskHelper.EmptyTask);

            Assert.IsTrue(Sut.IssueExists(IssueId).Result);
        }

        [Test]
        public void IssueDoesNotExist()
        {
            connection.Get(Arg.Any<CheckIfIssueExistsRequest>()).Returns(Task.Factory.StartNew(() =>
                                                                                                   {
                                                                                                       throw new RequestNotFoundException(Mock<IRestResponse>());
                                                                                                   }));

            Assert.IsFalse(Sut.IssueExists(IssueId).Result);
        }
    }
}
