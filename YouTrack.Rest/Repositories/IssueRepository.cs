using System;
using System.Linq;
using YouTrack.Rest.Deserialization;
using YouTrack.Rest.Exceptions;
using YouTrack.Rest.Factories;
using YouTrack.Rest.Requests;

namespace YouTrack.Rest.Repositories
{
    class IssueRepository : IIssueRepository
    {
        private readonly IConnection connection;
        private readonly IIssueFactory issueFactory;

        public IssueRepository(IConnection connection, IIssueFactory issueFactory)
        {
            this.connection = connection;
            this.issueFactory = issueFactory;
        }

#if !SILVERLIGHT
        public IIssue CreateIssue(string project, string summary, string description)
        {
            CreateNewIssueRequest createNewIssueRequest = new CreateNewIssueRequest(project, summary, description);

            string location = connection.Put(createNewIssueRequest);
            string issueId = location.Split('/').Last();

            return issueFactory.CreateIssue(issueId, connection);
        }

        public IIssue GetIssue(string issueId)
        {
            return issueFactory.CreateIssue(issueId, connection);
        }

        public void DeleteIssue(string issueId)
        {
            DeleteIssueRequest deleteIssueRequest = new DeleteIssueRequest(issueId);

            connection.Delete(deleteIssueRequest);
        }

        public bool IssueExists(string issueId)
        {
            try
            {
                CheckIfIssueExistsRequest checkIfIssueExistsRequest = new CheckIfIssueExistsRequest(issueId);

                connection.Get(checkIfIssueExistsRequest);

                return true;
            }
            catch (RequestNotFoundException)
            {
                return false;
            }
        }
#endif

        public void CreateIssueAsync(string project, string summary, string description, Action<IIssue> onSuccess, Action<Exception> onError)
        {
            CreateNewIssueRequest createNewIssueRequest = new CreateNewIssueRequest(project, summary, description);

            connection.PutAsync(createNewIssueRequest, location =>
                                                           {
                                                               string issueId = location.Split('/').Last();

                                                               onSuccess(issueFactory.CreateIssue(issueId, connection));
                                                           }, onError);
        }

        public void GetIssueAsync(string issueId, Action<IIssue> onSuccess, Action<Exception> onError)
        {
            GetIssueRequest getIssueRequest = new GetIssueRequest(issueId);

            connection.GetAsync<Deserialization.Issue>(getIssueRequest, success => onSuccess(success.GetIssue(connection)), onError);
        }

        public void DeleteIssueAsync(string issueId, Action onSuccess, Action<Exception> onError)
        {
            DeleteIssueRequest deleteIssueRequest = new DeleteIssueRequest(issueId);

            connection.DeleteAsync(deleteIssueRequest, onSuccess, onError);
        }

        public void IssueExistsAsync(string issueId, Action<bool> onSuccess, Action<Exception> onError)
        {
            CheckIfIssueExistsRequest checkIfIssueExistsRequest = new CheckIfIssueExistsRequest(issueId);

            connection.GetAsync(checkIfIssueExistsRequest, () => onSuccess(true), error =>
                                                                                      {
                                                                                          if (error is RequestNotFoundException)
                                                                                              onSuccess(false);
                                                                                          else
                                                                                              onError(error);
                                                                                      });
        }
    }
}
