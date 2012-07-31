using System.Collections.Generic;
using System.Linq;
using YouTrack.Rest.Deserialization;
using YouTrack.Rest.Requests;
using YouTrack.Rest.Requests.Issues;

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
            get { return comments ?? (comments = GetComments()); }
            internal set { comments = value; }
        }

        public string Id { get; private set; }
        protected IConnection Connection { get; private set; }

        private IEnumerable<IComment> GetComments()
        {
            GetCommentsOfAnIssueRequest request = new GetCommentsOfAnIssueRequest(Id);

            CommentCollection commentCollection = Connection.Get<CommentCollection>(request);

            return commentCollection.GetComments(Connection);
        }

        public virtual void ApplyCommand(string command)
        {
            ApplyCommandToAnIssueRequest request = new ApplyCommandToAnIssueRequest(Id, command);

            Connection.Post(request);
        }

        public virtual void ApplyCommands(params string[] commands)
        {
            ApplyCommandsToAnIssueRequest request = new ApplyCommandsToAnIssueRequest(Id, commands);

            Connection.Post(request);
        }

        public void SetSubsystem(string subsystem)
        {
            ApplyCommand(Commands.SetSubsystem(subsystem));
        }

        public void SetType(string type)
        {
            ApplyCommand(Commands.SetType(type));
        }

        public void AttachFile(string fileName, byte[] bytes)
        {
            AttachFileToAnIssueRequest request = new AttachFileToAnIssueRequest(Id, fileName, bytes);

            Connection.PostWithFile(request);
        }

        public void AttachFile(string filePath)
        {
            AttachFileToAnIssueRequest request = new AttachFileToAnIssueRequest(Id, filePath);

            Connection.PostWithFile(request);
        }

        public IEnumerable<IAttachment> GetAttachments()
        {
            GetAttachmentsOfAnIssueRequest request = new GetAttachmentsOfAnIssueRequest(Id);
            FileUrlCollection fileUrlCollection = Connection.Get<FileUrlCollection>(request);

            return fileUrlCollection.FileUrls.Cast<IAttachment>();
        }

        public void AddComment(string comment)
        {
            AddCommentToIssueRequest request = new AddCommentToIssueRequest(Id, comment);

            Connection.Post(request);

            //Force fetching when comments are needed next time.
            comments = null;
        }

        public void RemoveComment(string commentId)
        {
            RemoveACommentForAnIssueRequest request = new RemoveACommentForAnIssueRequest(Id, commentId);

            Connection.Delete(request);

            //Force fetching when comments are needed next time.
            comments = null;
        }
    }
}