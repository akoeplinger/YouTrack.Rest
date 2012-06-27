using System;

namespace YouTrack.Rest.Repositories
{
    public interface IUserRepository
    {
#if !SILVERLIGHT
        void CreateUser(string login, string password, string email, string fullname = null);
        void DeleteUser(string login);
        bool UserExists(string login);
        IUser GetUser(string login);
#endif
        void CreateUserAsync(string login, string password, string email, string fullname, Action onSuccess, Action<Exception> onError);
        void DeleteUserAsync(string login, Action onSuccess, Action<Exception> onError);
        void UserExistsAsync(string login, Action<bool> onSuccess, Action<Exception> onError);
        void GetUserAsync(string login, Action<IUser> onSuccess, Action<Exception> onError);
    }
}