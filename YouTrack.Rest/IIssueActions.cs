using System;
using System.Collections.Generic;

namespace YouTrack.Rest
{
    public interface IIssueActions
    {
        string Id { get; }
        IEnumerable<IComment> Comments { get; }

#if !SILVERLIGHT
        void AttachFile(string filePath);
        IEnumerable<IAttachment> GetAttachments();
        void AddComment(string comment);
        void RemoveComment(string commentId);
        void SetSubsystem(string subsystem);
        void SetType(string type);
        void AttachFile(string fileName, byte[] bytes);
        void ApplyCommand(string command);
        void ApplyCommands(params string[] commands);
#endif
        void FetchCommentsAsync(Action onSuccess, Action<Exception> onError);
        void AttachFileAsync(string filePath, Action onSuccess, Action<Exception> onError);
        void GetAttachmentsAsync(Action<IEnumerable<IAttachment>> onSuccess, Action<Exception> onError);
        void AddCommentAsync(string comment, Action onSuccess, Action<Exception> onError);
        void RemoveCommentAsync(string commentId, Action onSuccess, Action<Exception> onError);
        void SetSubsystemAsync(string subsystem, Action onSuccess, Action<Exception> onError);
        void SetTypeAsync(string type, Action onSuccess, Action<Exception> onError);
        void AttachFileAsync(string fileName, byte[] bytes, Action onSuccess, Action<Exception> onError);
        void ApplyCommandAsync(string command, Action onSuccess, Action<Exception> onError);
        void ApplyCommandsAsync(string[] commands, Action onSuccess, Action<Exception> onError);
    }
}