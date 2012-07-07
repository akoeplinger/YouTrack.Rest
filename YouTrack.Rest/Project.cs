using System.Threading.Tasks;
using YouTrack.Rest.Interception;
using YouTrack.Rest.Requests.Projects;

namespace YouTrack.Rest
{
    class Project : ProjectActions, IProject, ILoadable
    {
        public Project(string projectId, IConnection connection) : base(projectId, connection)
        {
        }

        public string Description { get; internal set; }
        public string Name { get; internal set; }
        public int StartingNumber { get; internal set; }
        public string ProjectLeadLogin { get; internal set; }

        public bool IsLoaded { get; private set; }

        public Task Load()
        {
            GetProjectRequest request = new GetProjectRequest(Id);

            return Connection
                .Get<Deserialization.Project>(request)
                .ContinueWith(r =>
                                  {
                                      TaskHelper.ThrowIfExceptionOccured(r);

                                      Deserialization.Project project = r.Result;

                                      project.MapTo(this);

                                      IsLoaded = true;
                                  });
        }
    }
}
