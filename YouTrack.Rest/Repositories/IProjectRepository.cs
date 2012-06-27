using System;

namespace YouTrack.Rest.Repositories
{
    public interface IProjectRepository
    {
        IProjectProxy GetProjectProxy(string projectid);
#if !SILVERLIGHT
        IProject CreateProject(string projectId, string projectName, string projectLeadLogin, int startingNumber = 1, string description = null);
        bool ProjectExists(string projectId);
        void DeleteProject(string projectid);
#endif
        void CreateProjectAsync(string projectId, string projectName, string projectLeadLogin, int startingNumber, string description, Action<IProject> onSuccess, Action<Exception> onError);
        void ProjectExistsAsync(string projectId, Action<bool> onSuccess, Action<Exception> onError);
        void DeleteProjectAsync(string projectid, Action onSuccess, Action<Exception> onError);
    }
}