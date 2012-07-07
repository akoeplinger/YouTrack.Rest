using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouTrack.Rest.Deserialization;
using YouTrack.Rest.Requests;

namespace YouTrack.Rest
{
    abstract class IssueActions : IIssueActions
    {
        private IEnumerable<IComment> comments;

        protected IssueActions(string issueId, IConnection connection)
        {
            Id = issueId;
            Connection = connection;
        }

        public IEnumerable<IComment> Comments
        {
            get { return comments ?? (comments = GetComments().Result); }
            internal set { comments = value; }
        }

        public string Id { get; private set; }
        protected IConnection Connection { get; private set; }

        private Task<IEnumerable<IComment>> GetComments()
        {
            GetCommentsOfAnIssueRequest request = new GetCommentsOfAnIssueRequest(Id);

            return Connection
                .Get<CommentsCollection>(request)
                .ContinueWith(r =>
                                  {
                                      TaskHelper.ThrowIfExceptionOccured(r);

                                      CommentsCollection commentsCollection = r.Result;
                                      return commentsCollection.GetComments(Connection);
                                  });
        }

        public virtual Task ApplyCommand(string command)
        {
            ApplyCommandToAnIssueRequest request = new ApplyCommandToAnIssueRequest(Id, command);

            return Connection.Post(request);
        }

        public virtual Task ApplyCommands(params string[] commands)
        {
            ApplyCommandsToAnIssueRequest request = new ApplyCommandsToAnIssueRequest(Id, commands);

            return Connection.Post(request);
        }

        public Task SetSubsystem(string subsystem)
        {
            return ApplyCommand(Commands.SetSubsystem(subsystem));
        }

        public Task SetType(string type)
        {
            return ApplyCommand(Commands.SetType(type));
        }

        public Task AttachFile(string fileName, byte[] bytes)
        {
            AttachFileToAnIssueRequest request = new AttachFileToAnIssueRequest(Id, fileName, bytes);

            return Connection.PostWithFile(request);
        }

        public Task AttachFile(string filePath)
        {
            AttachFileToAnIssueRequest request = new AttachFileToAnIssueRequest(Id, filePath);

            return Connection.PostWithFile(request);
        }

        public Task<IEnumerable<IAttachment>> GetAttachments()
        {
            GetAttachmentsOfAnIssueRequest request = new GetAttachmentsOfAnIssueRequest(Id);

            return Connection
                .Get<FileUrlCollection>(request)
                .ContinueWith(r =>
                                  {
                                      TaskHelper.ThrowIfExceptionOccured(r);

                                      FileUrlCollection fileUrlCollection = r.Result;
                                      return fileUrlCollection.FileUrls.Cast<IAttachment>();
                                  });
        }

        public Task AddComment(string comment)
        {
            AddCommentToIssueRequest request = new AddCommentToIssueRequest(Id, comment);

            return Connection
                .Post(request)
                .ContinueWith(r =>
                                  {
                                      TaskHelper.ThrowIfExceptionOccured(r);

                                      //Force fetching when comments are needed next time.
                                      comments = null;
                                  });
        }

        public Task RemoveComment(string commentId)
        {
            RemoveACommentForAnIssueRequest request = new RemoveACommentForAnIssueRequest(Id, commentId);

            return Connection
                .Delete(request)
                .ContinueWith(r =>
                                  {
                                      TaskHelper.ThrowIfExceptionOccured(r);

                                      //Force fetching when comments are needed next time.
                                      comments = null;
                                  });
        }
    }
}