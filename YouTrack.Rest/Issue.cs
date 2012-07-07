using System;
using System.Linq;
using System.Threading.Tasks;
using YouTrack.Rest.Interception;
using YouTrack.Rest.Requests;

namespace YouTrack.Rest
{
    class Issue : IssueActions, IIssue, ILoadable
    {
        public string Summary { get; set; }
        public string Type { get; set; }
        public string ProjectShortName { get; set; }
        public string Description { get; internal set; }
        public int NumberInProject { get; internal set; }
        public DateTime Created { get; internal set; }
        public DateTime Updated { get; internal set; }
        public string UpdaterName { get; internal set; }
        public string ReporterName { get; internal set; }
        public int VotesCount { get; internal set; }
        public int CommentsCount { get; internal set; }
        public string Priority { get; internal set; }
        public string State { get; internal set; }
        public string Subsystem { get; internal set; }

        public bool IsLoaded { get; private set; }

        public Task Load()
        {
            GetIssueRequest request = new GetIssueRequest(Id);

            return Connection
                .Get<Deserialization.Issue>(request)
                .ContinueWith(r =>
                                  {
                                      TaskHelper.ThrowIfExceptionOccured(r);

                                      Deserialization.Issue issue = r.Result;

                                      issue.MapTo(this, Connection);

                                      IsLoaded = true;
                                  });
        }

        public Issue(string issueId, IConnection connection) : base(issueId, connection)
        {
            IsLoaded = false;
        }

        public override Task ApplyCommand(string command)
        {
            return base
                .ApplyCommand(command)
                .ContinueWith(r =>
                                  {
                                      TaskHelper.ThrowIfExceptionOccured(r);
                                      IsLoaded = false;
                                  });
        }

        public override Task ApplyCommands(params string[] commands)
        {
            return base
                .ApplyCommands(commands)
                .ContinueWith(r =>
                                  {
                                      TaskHelper.ThrowIfExceptionOccured(r);
                                      IsLoaded = false;
                                  });
        }
    }
}