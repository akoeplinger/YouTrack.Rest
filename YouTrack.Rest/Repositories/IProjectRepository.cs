using System.Threading.Tasks;

namespace YouTrack.Rest.Repositories
{
    public interface IProjectRepository
    {
        IProject GetProject(string projectId);
        Task<IProject> CreateProject(string projectId, string projectName, string projectLeadLogin, int startingNumber = 1, string description = null);
        Task<bool> ProjectExists(string projectId);
        Task DeleteProject(string projectid);
    }
}