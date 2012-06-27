using System;

namespace YouTrack.Rest.Interception
{
    public interface ILoadable
    {
        bool IsLoaded { get; }
#if !SILVERLIGHT
        void Load();
#endif
        void LoadAsync(Action onSuccess, Action<Exception> onError);
    }
}