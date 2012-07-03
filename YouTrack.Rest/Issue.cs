using System;
using System.Linq;
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

        public Issue(string issueId, IConnection connection) : base(issueId, connection)
        {
            IsLoaded = false;
        }

#if !SILVERLIGHT
        public void Load()
        {
            GetIssueRequest request = new GetIssueRequest(Id);

            Deserialization.Issue issue = Connection.Get<Deserialization.Issue>(request);

            issue.MapTo(this, Connection);

            IsLoaded = true;
        }

        public override void ApplyCommand(string command)
        {
            base.ApplyCommand(command);

            IsLoaded = false;
        }

        public override void ApplyCommands(params string[] commands)
        {
            base.ApplyCommands(commands);

            IsLoaded = false;
        }
#endif

        public void LoadAsync(Action onSuccess, Action<Exception> onError)
        {
            GetIssueRequest request = new GetIssueRequest(Id);

            Connection.GetAsync<Deserialization.Issue>(request, success =>
                                                                    {
                                                                        success.MapTo(this, Connection);
                                                                        IsLoaded = true;
                                                                    }, onError);
        }

        public override void ApplyCommandAsync(string command, Action onSuccess, Action<Exception> onError)
        {
            base.ApplyCommandAsync(command, () =>
                                                {
                                                    IsLoaded = false;
                                                    onSuccess();
                                                }, onError);
        }

        public override void ApplyCommandsAsync(string[] commands, Action onSuccess, Action<Exception> onError)
        {
            base.ApplyCommandsAsync(commands, () =>
                                                  {
                                                      IsLoaded = false;
                                                      onSuccess();
                                                  }, onError);
        }
    }
}