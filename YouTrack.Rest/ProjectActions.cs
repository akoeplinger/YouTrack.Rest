using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YouTrack.Rest.Requests;

namespace YouTrack.Rest
{
    class ProjectActions : IProjectActions
    {
        public ProjectActions(string projectId, IConnection connection)
        {
            Id = projectId;
            Connection = connection;
        }

        protected IConnection Connection { get; private set; }
        public string Id { get; private set; }

        public Task<IEnumerable<IIssue>> GetIssues()
        {
            GetIssuesInAProjectRequest request = new GetIssuesInAProjectRequest(Id);

            return GetIssues(request);
        }

        public Task<IEnumerable<IIssue>> GetIssues(string filter)
        {
            GetIssuesInAProjectRequest request = new GetIssuesInAProjectRequest(Id, filter);

            return GetIssues(request);
        }

        private Task<IEnumerable<IIssue>> GetIssues(GetIssuesInAProjectRequest request)
        {
            return Connection
                .Get<List<Deserialization.Issue>>(request)
                .ContinueWith(r =>
                                  {
                                      TaskHelper.ThrowIfExceptionOccured(r);

                                      List<Deserialization.Issue> issues = r.Result;

                                      return issues.Select(i => i.GetIssue(Connection));
                                  });
        }
    }
}