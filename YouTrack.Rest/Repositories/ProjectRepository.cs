using System.Threading.Tasks;
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

        public IProject GetProject(string projectId)
        {
            return projectFactory.CreateProject(projectId, connection);
        }

        public Task<IProject> CreateProject(string projectId, string projectName, string projectLeadLogin, int startingNumber = 1, string description = null)
        {
            return connection
                .Put(new CreateNewProjectRequest(projectId, projectName, projectLeadLogin, startingNumber, description))
                .ContinueWith(r =>
                                  {
                                      TaskHelper.ThrowIfExceptionOccured(r);

                                      return GetProject(projectId);
                                  });
        }

        public Task<bool> ProjectExists(string projectId)
        {
            //Relies on the "not found" exception if project doesn't exist. Could use some improving.

            return connection
                .Get(new GetProjectRequest(projectId))
                .ContinueWith(r =>
                                  {
                                      if (r.Exception != null)
                                      {
                                          if (r.Exception.GetBaseException() is RequestNotFoundException)
                                              return false;
                                          else
                                              throw r.Exception;
                                      }

                                      return true;
                                  });
        }

        public Task DeleteProject(string projectid)
        {
            return connection.Delete(new DeleteProjectRequest(projectid));
        }
    }
}