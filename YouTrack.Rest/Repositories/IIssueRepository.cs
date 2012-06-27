using System;

namespace YouTrack.Rest.Repositories
{
    public interface IIssueRepository
    {
#if !SILVERLIGHT
        IIssue CreateIssue(string project, string summary, string description);
        IIssue GetIssue(string issueId);
        void DeleteIssue(string issueId);
        bool IssueExists(string issueId);
#endif
        void CreateIssueAsync(string project, string summary, string description, Action<IIssue> onSuccess, Action<Exception> onError);
        void GetIssueAsync(string issueId, Action<IIssue> onSuccess, Action<Exception> onError);
        void DeleteIssueAsync(string issueId, Action onSuccess, Action<Exception> onError);
        void IssueExistsAsync(string issueId, Action<bool> onSuccess, Action<Exception> onError);
    }
}