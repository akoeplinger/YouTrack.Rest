using System;
using YouTrack.Rest.Exceptions;
using YouTrack.Rest.Requests.Users;

namespace YouTrack.Rest.Repositories
{
    class UserRepository : IUserRepository
    {
        private readonly IConnection connection;

        public UserRepository(IConnection connection)
        {
            this.connection = connection;
        }

#if !SILVERLIGHT
        public void CreateUser(string login, string password, string email, string fullname = null)
        {
            connection.Put(new CreateANewUserRequest(login, password, email, fullname));
        }

        public void DeleteUser(string login)
        {
            connection.Delete(new DeleteUserRequest(login));
        }

        public bool UserExists(string login)
        {
            //Relies on the "not found" exception if user doesn't exist. Could use some improving.

            try
            {
                connection.Get(new GetUserRequest(login));

                return true;
            }
            catch(RequestNotFoundException)
            {
                return false;
            }
        }

        public IUser GetUser(string login)
        {
            Deserialization.User user = connection.Get<Deserialization.User>(new GetUserRequest(login));

            return user.GetUser();
        }
#endif

        public void CreateUserAsync(string login, string password, string email, string fullname, Action onSuccess, Action<Exception> onError)
        {
            connection.PutAsync(new CreateANewUserRequest(login, password, email, fullname), success => onSuccess(), onError);
        }

        public void DeleteUserAsync(string login, Action onSuccess, Action<Exception> onError)
        {
            connection.DeleteAsync(new DeleteUserRequest(login), onSuccess, onError);
        }

        public void UserExistsAsync(string login, Action<bool> onSuccess, Action<Exception> onError)
        {
            //Relies on the "not found" exception if user doesn't exist. Could use some improving.

            connection.GetAsync(new GetUserRequest(login), () => onSuccess(true), error =>
                                                                                      {
                                                                                          if (error is RequestNotFoundException)
                                                                                              onSuccess(false);
                                                                                          else
                                                                                              onError(error);
                                                                                      });
        }

        public void GetUserAsync(string login, Action<IUser> onSuccess, Action<Exception> onError)
        {
            connection.GetAsync<Deserialization.User>(new GetUserRequest(login), success => onSuccess(success.GetUser()), onError);
        }
    }
}