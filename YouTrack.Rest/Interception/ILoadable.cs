using System.Threading.Tasks;

namespace YouTrack.Rest.Interception
{
    public interface ILoadable
    {
        bool IsLoaded { get; }
        Task Load();
    }
}