using System;
using System.Collections.Generic;
using System.Linq;
using YouTrack.Rest.Requests;

namespace YouTrack.Rest
{
    class ProjectProxy : IProjectProxy
    {
        private readonly IConnection connection;

        public ProjectProxy(string projectId, IConnection connection)
        {
            Id = projectId;
            this.connection = connection;
        }

        public string Id { get; private set; }

#if !SILVERLIGHT
        public IEnumerable<IIssue> GetIssues()
        {
            GetIssuesInAProjectRequest request = new GetIssuesInAProjectRequest(Id);

            return GetIssues(request);
        }

        public IEnumerable<IIssue> GetIssues(string filter)
        {
            GetIssuesInAProjectRequest request = new GetIssuesInAProjectRequest(Id, filter);

            return GetIssues(request);
        }

        private IEnumerable<IIssue> GetIssues(GetIssuesInAProjectRequest request)
        {
            List<Deserialization.Issue> issues = connection.Get<List<Deserialization.Issue>>(request);

            return issues.Select(i => i.GetIssue(connection));
        }
#endif

        public void GetIssuesAsync(Action<IEnumerable<IIssue>> onSuccess, Action<Exception> onError)
        {
            GetIssuesInAProjectRequest request = new GetIssuesInAProjectRequest(Id);

            GetIssuesAsync(request, onSuccess, onError);
        }

        public void GetIssuesAsync(string filter, Action<IEnumerable<IIssue>> onSuccess, Action<Exception> onError)
        {
            GetIssuesInAProjectRequest request = new GetIssuesInAProjectRequest(Id, filter);

            GetIssuesAsync(request, onSuccess, onError);
        }

        private void GetIssuesAsync(GetIssuesInAProjectRequest request, Action<IEnumerable<IIssue>> onSuccess, Action<Exception> onError)
        {
            connection.GetAsync<List<Deserialization.Issue>>(request, success =>
                                                                          {
                                                                              var issues = success.Select(i => i.GetIssue(connection));
                                                                              onSuccess(issues);
                                                                          }, onError);
        }
    }
}