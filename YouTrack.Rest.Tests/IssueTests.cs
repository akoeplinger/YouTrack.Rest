using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using YouTrack.Rest.Deserialization;
using YouTrack.Rest.Interception;
using YouTrack.Rest.Requests;
using YouTrack.Rest.Tests.Repositories;

namespace YouTrack.Rest.Tests
{
    class IssueTests : TestFor<Issue>
    {
        private IConnection connection;
        private const string IssueId = "FOO-BAR";
        private FileUrlCollection fileUrlCollection;
        private CommentsCollection commentsCollection;

        protected override Issue CreateSut()
        {
            connection = Mock<IConnection>();
            return new Issue("FOO-BAR", connection);
        }

        protected override void SetupDependencies()
        {
            connection.Get<Rest.Deserialization.Issue>(Arg.Any<GetIssueRequest>()).Returns(Task.Factory.StartNew(() => (Rest.Deserialization.Issue) new DeserializedIssueMock()));
            fileUrlCollection = new FileUrlCollection();
            commentsCollection = new CommentsCollection { Comments = new List<Rest.Deserialization.Comment>() };
        }

        [Test]
        public void IsNotLoadedWhenConstructed()
        {
            Assert.IsFalse(((ILoadable)Sut).IsLoaded);
        }

        [Test]
        public void LoadSetLoaded()
        {
            ILoadable loadable = Sut;

            loadable.Load().Wait();

            Assert.IsTrue(loadable.IsLoaded);
        }

        [Test]
        public void ConnectionIsCalledWithAddComment()
        {
            connection.Post(Arg.Any<AddCommentToIssueRequest>()).Returns(TaskHelper.EmptyTask);

            Sut.AddComment("foobar").Wait();

            connection.Received().Post(Arg.Any<AddCommentToIssueRequest>());
        }

        [Test]
        public void CommentIsDeleted()
        {
            connection.Delete(Arg.Any<RemoveACommentForAnIssueRequest>()).Returns(TaskHelper.EmptyTask);

            Sut.RemoveComment("foobar").Wait();

            connection.Received().Delete(Arg.Any<RemoveACommentForAnIssueRequest>());
        }

        [Test]
        public void CommentsAreFetchedAgainAfterAddingComment()
        {
            connection.Get<CommentsCollection>(Arg.Any<GetCommentsOfAnIssueRequest>()).Returns(Task.Factory.StartNew(() => commentsCollection));
            connection.Post(Arg.Any<AddCommentToIssueRequest>()).Returns(TaskHelper.EmptyTask);

            IEnumerable<IComment> comments = Sut.Comments;
            Sut.AddComment("foobar").Wait();
            comments = Sut.Comments;

            connection.Received(2).Get<CommentsCollection>(Arg.Any<GetCommentsOfAnIssueRequest>());
        }

        [Test]
        public void ConnectionIsCalledWithAttachFile()
        {
            connection.PostWithFile(Arg.Any<AttachFileToAnIssueRequest>()).Returns(TaskHelper.EmptyTask);

            Sut.AttachFile("foo.jpg").Wait();

            connection.Received().PostWithFile(Arg.Any<AttachFileToAnIssueRequest>());
        }

        [Test]
        public void ConnectionIsCalledWithAttachFileWithBytes()
        {
            connection.PostWithFile(Arg.Any<AttachFileToAnIssueRequest>()).Returns(TaskHelper.EmptyTask);

            Sut.AttachFile("foo.txt", new byte[512]).Wait();

            connection.Received().PostWithFile(Arg.Any<AttachFileToAnIssueRequest>());
        }

        [Test]
        public void ConnectionIsCalledWithGetAttachments()
        {
            connection.Get<FileUrlCollection>(Arg.Any<GetAttachmentsOfAnIssueRequest>()).Returns(Task.Factory.StartNew(() => fileUrlCollection));

            Sut.GetAttachments();

            connection.Received().Get<FileUrlCollection>(Arg.Any<GetAttachmentsOfAnIssueRequest>());
        }

        [Test]
        public void ConnectionIsCalledWithGetComments()
        {
            connection.Get<CommentsCollection>(Arg.Any<GetCommentsOfAnIssueRequest>()).Returns(Task.Factory.StartNew(() => commentsCollection));

            IEnumerable<IComment> comments = Sut.Comments;

            connection.Received().Get<CommentsCollection>(Arg.Any<GetCommentsOfAnIssueRequest>());
        }

        [Test]
        public void ConnectionIsCalledWithSetSubsystem()
        {
            connection.Post(Arg.Any<ApplyCommandToAnIssueRequest>()).Returns(TaskHelper.EmptyTask);

            Sut.SetSubsystem("Foobar").Wait();

            connection.Received().Post(Arg.Any<ApplyCommandToAnIssueRequest>());
        }

        [Test]
        public void SubsystemIsApplied()
        {
            connection.Post(Arg.Any<ApplyCommandToAnIssueRequest>()).Returns(TaskHelper.EmptyTask);

            Sut.SetSubsystem("Foobar").Wait();

            AssertThatCommandIsApplied("Subsystem Foobar");
        }

        private void AssertThatCommandIsApplied(string command)
        {
            connection.Received().Post(Arg.Is<ApplyCommandToAnIssueRequest>(x => x.RestResource == String.Format("/rest/issue/{0}/execute?command={1}", IssueId, Uri.EscapeDataString(command))));
        }

        private void AssertThatCommandsAreApplied(string commands)
        {
            connection.Received().Post(Arg.Is<ApplyCommandsToAnIssueRequest>(x => x.RestResource == String.Format("/rest/issue/{0}/execute?command={1}", IssueId, Uri.EscapeDataString(commands))));
        }

        [Test]
        public void ConnectionIsCalledWithSetType()
        {
            connection.Post(Arg.Any<ApplyCommandToAnIssueRequest>()).Returns(TaskHelper.EmptyTask);

            Sut.SetType("Foobar").Wait();

            connection.Received().Post(Arg.Any<ApplyCommandToAnIssueRequest>());
        }

        [Test]
        public void TypeIsApplied()
        {
            connection.Post(Arg.Any<ApplyCommandToAnIssueRequest>()).Returns(TaskHelper.EmptyTask);

            Sut.SetType("foobar").Wait();

            AssertThatCommandIsApplied("Type foobar");
        }

        [Test]
        public void MultipleCommandsAreApplied()
        {
            connection.Post(Arg.Any<ApplyCommandsToAnIssueRequest>()).Returns(TaskHelper.EmptyTask);

            Sut.ApplyCommands("Foo", "Bar").Wait();

            AssertThatCommandsAreApplied("Foo Bar");
        }

        [Test]
        public void IssueStatusNotLoadedAfterApplyingCommand()
        {
            connection.Post(Arg.Any<ApplyCommandsToAnIssueRequest>()).Returns(TaskHelper.EmptyTask);

            Sut.Load().Wait();

            Sut.ApplyCommands("Foo", "Bar").Wait();

            Assert.IsFalse(Sut.IsLoaded);
        }
    }
}
