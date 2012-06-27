using System;
using System.Collections.Generic;

namespace YouTrack.Rest
{
    public interface IProjectProxy
    {
        string Id { get; }
#if !SILVERLIGHT
        IEnumerable<IIssue> GetIssues();
        IEnumerable<IIssue> GetIssues(string filter);
#endif
        void GetIssuesAsync(Action<IEnumerable<IIssue>> onSuccess, Action<Exception> onError);
        void GetIssuesAsync(string filter, Action<IEnumerable<IIssue>> onSuccess, Action<Exception> onError);
    }
}