using System.Linq;
using System.Threading.Tasks;
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

        public Task<IIssue> CreateIssue(string project, string summary, string description)
        {
            CreateNewIssueRequest createNewIssueRequest = new CreateNewIssueRequest(project, summary, description);

            return connection
                .Put(createNewIssueRequest)
                .ContinueWith(r =>
                                  {
                                      TaskHelper.ThrowIfExceptionOccured(r);

                                      string location = r.Result;
                                      string issueId = location.Split('/').Last();

                                      return GetIssue(issueId);
                                  });
        }

        public IIssue GetIssue(string issueId)
        {
            return issueFactory.CreateIssue(issueId, connection);
        }

        public Task DeleteIssue(string issueId)
        {
            DeleteIssueRequest deleteIssueRequest = new DeleteIssueRequest(issueId);

            return connection.Delete(deleteIssueRequest);
        }

        public Task<bool> IssueExists(string issueId)
        {
            CheckIfIssueExistsRequest checkIfIssueExistsRequest = new CheckIfIssueExistsRequest(issueId);

            return connection
                .Get(checkIfIssueExistsRequest)
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
    }
}
