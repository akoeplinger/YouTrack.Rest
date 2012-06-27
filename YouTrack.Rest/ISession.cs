using System;
using System.Collections.Generic;

namespace YouTrack.Rest
{
    public interface ISession
    {
        bool IsAuthenticated { get; }
        IDictionary<string, string> AuthenticationCookies { get; }
#if !SILVERLIGHT
        void Login();
#endif
        void LoginAsync(Action onSuccess, Action<Exception> onError);
    }
}