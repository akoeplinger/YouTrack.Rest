using System.Collections.Generic;
using System.Threading.Tasks;

namespace YouTrack.Rest
{
    public interface IProjectActions
    {
        string Id { get; }
        Task<IEnumerable<IIssue>> GetIssues();
        Task<IEnumerable<IIssue>> GetIssues(string filter);
    }
}