using System;
using YouTrack.Rest.Exceptions;
using YouTrack.Rest.Factories;
using YouTrack.Rest.Requests.Projects;

namespace YouTrack.Rest.Repositories
{
    class ProjectRepository : IProjectRepository
    {
        private readonly IConnection connection;
        private readonly IProjectFactory projectFactory;

        public ProjectRepository(IConnection connection, IProjectFactory projectFactory)
        {
            this.connection = connection;
            this.projectFactory = projectFactory;
        }

        public IProjectProxy GetProjectProxy(string projectid)
        {
            return new ProjectProxy(projectid, connection);
        }

#if !SILVERLIGHT
        public IProject CreateProject(string projectId, string projectName, string projectLeadLogin, int startingNumber = 1, string description = null)
        {
            connection.Put(new CreateNewProjectRequest(projectId, projectName, projectLeadLogin, startingNumber, description));

            return projectFactory.CreateProject(projectId, connection);
        }

        public bool ProjectExists(string projectId)
        {
            //Relies on the "not found" exception if project doesn't exist. Could use some improving.

            try
            {
                connection.Get(new GetProjectRequest(projectId));

                return true;
            }
            catch (RequestNotFoundException)
            {
                return false;
            }
        }

        public void DeleteProject(string projectid)
        {
            connection.Delete(new DeleteProjectRequest(projectid));
        }
#endif

        public void CreateProjectAsync(string projectId, string projectName, string projectLeadLogin, int startingNumber, string description, Action<IProject> onSuccess, Action<Exception> onError)
        {
            connection.PutAsync(
                new CreateNewProjectRequest(projectId, projectName, projectLeadLogin, startingNumber, description),
                success => onSuccess(projectFactory.CreateProject(projectId, connection)),
                onError);
        }

        public void ProjectExistsAsync(string projectId, Action<bool> onSuccess, Action<Exception> onError)
        {
            //Relies on the "not found" exception if project doesn't exist. Could use some improving.

            connection.GetAsync(new GetProjectRequest(projectId), () => onSuccess(true), error =>
                                                                                             {
                                                                                                 if (error is RequestNotFoundException)
                                                                                                     onSuccess(false);
                                                                                                 else
                                                                                                     onError(error);
                                                                                             });
        }

        public void DeleteProjectAsync(string projectid, Action onSuccess, Action<Exception> onError)
        {
            connection.DeleteAsync(new DeleteProjectRequest(projectid), onSuccess, onError);
        }
    }
}