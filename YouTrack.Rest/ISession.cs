using System.Collections.Generic;
using System.Threading.Tasks;

namespace YouTrack.Rest
{
    public interface ISession
    {
        bool IsAuthenticated { get; }
        IDictionary<string, string> AuthenticationCookies { get; }
        Task Login();
    }
}