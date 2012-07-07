using System.Threading.Tasks;

namespace YouTrack.Rest.Repositories
{
    public interface IIssueRepository
    {
        Task<IIssue> CreateIssue(string project, string summary, string description);
        IIssue GetIssue(string issueId);
        Task DeleteIssue(string issueId);
        Task<bool> IssueExists(string issueId);
    }
}