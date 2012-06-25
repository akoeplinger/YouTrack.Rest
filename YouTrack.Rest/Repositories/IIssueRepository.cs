using System.Collections.Generic;

namespace YouTrack.Rest.Repositories
{
    public interface IIssueRepository
    {
        IIssueProxy CreateIssue(string project, string summary, string description);
        IIssue CreateAndGetIssue(string project, string summary, string description);
        IIssue GetIssue(string issueId);
        void DeleteIssue(string issueId);
        bool IssueExists(string issueId);
        IIssueProxy GetIssueProxy(string issueId);
        ICollection<IIssue> GetIssuesOfAnProject(string project, string filter);
    }
}