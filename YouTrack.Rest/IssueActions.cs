using System;
using System.Collections.Generic;
using System.Linq;
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

        public string Id { get; private set; }
        protected IConnection Connection { get; private set; }

        public IEnumerable<IComment> Comments
        {
            get 
            { 
#if !SILVERLIGHT
                return comments ?? (comments = GetComments());
#else
                return comments;
#endif
            }
            internal set { comments = value; }
        }

#if !SILVERLIGHT
        private IEnumerable<IComment> GetComments()
        {
            GetCommentsOfAnIssueRequest request = new GetCommentsOfAnIssueRequest(Id);

            CommentsCollection commentsCollection = Connection.Get<CommentsCollection>(request);

            return commentsCollection.GetComments(Connection);
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

            return fileUrlCollection.FileUrls;
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
#endif
        public void FetchCommentsAsync(Action onSuccess, Action<Exception> onError)
        {
            GetCommentsOfAnIssueRequest request = new GetCommentsOfAnIssueRequest(Id);

            Connection.GetAsync<CommentsCollection>(request, success =>
                                                                 {
                                                                     this.Comments = success.GetComments(Connection);
                                                                     onSuccess();
                                                                 }, onError);
        }

        public virtual void ApplyCommandAsync(string command, Action onSuccess, Action<Exception> onError)
        {
            ApplyCommandToAnIssueRequest request = new ApplyCommandToAnIssueRequest(Id, command);

            Connection.PostAsync(request, onSuccess, onError);
        }

        public virtual void ApplyCommandsAsync(string[] commands, Action onSuccess, Action<Exception> onError)
        {
            ApplyCommandsToAnIssueRequest request = new ApplyCommandsToAnIssueRequest(Id, commands);

            Connection.PostAsync(request, onSuccess, onError);
        }

        public void SetSubsystemAsync(string subsystem, Action onSuccess, Action<Exception> onError)
        {
            ApplyCommandAsync(Commands.SetSubsystem(subsystem), onSuccess, onError);
        }

        public void SetTypeAsync(string type, Action onSuccess, Action<Exception> onError)
        {
            ApplyCommandAsync(Commands.SetType(type), onSuccess, onError);
        }

        public void AttachFileAsync(string fileName, byte[] bytes, Action onSuccess, Action<Exception> onError)
        {
            AttachFileToAnIssueRequest request = new AttachFileToAnIssueRequest(Id, fileName, bytes);

            Connection.PostWithFileAsync(request, onSuccess, onError);
        }

        public void AttachFileAsync(string filePath, Action onSuccess, Action<Exception> onError)
        {
            AttachFileToAnIssueRequest request = new AttachFileToAnIssueRequest(Id, filePath);

            Connection.PostWithFileAsync(request, onSuccess, onError);
        }

        public void GetAttachmentsAsync(Action<IEnumerable<IAttachment>> onSuccess, Action<Exception> onError)
        {
            GetAttachmentsOfAnIssueRequest request = new GetAttachmentsOfAnIssueRequest(Id);
            Connection.GetAsync<FileUrlCollection>(request, success => onSuccess(success.FileUrls.Cast<IAttachment>()), onError);
        }

        public void AddCommentAsync(string comment, Action onSuccess, Action<Exception> onError)
        {
            AddCommentToIssueRequest request = new AddCommentToIssueRequest(Id, comment);

            Connection.PostAsync(request, onSuccess, onError);
        }

        public void RemoveCommentAsync(string commentId, Action onSuccess, Action<Exception> onError)
        {
            RemoveACommentForAnIssueRequest request = new RemoveACommentForAnIssueRequest(Id, commentId);

            Connection.DeleteAsync(request, onSuccess, onError);
        }
    }
}