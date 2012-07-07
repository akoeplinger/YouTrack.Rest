using System.Threading.Tasks;
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

        public Task CreateUser(string login, string password, string email, string fullname = null)
        {
            return connection.Put(new CreateANewUserRequest(login, password, email, fullname));
        }

        public Task DeleteUser(string login)
        {
            return connection.Delete(new DeleteUserRequest(login));
        }

        public Task<bool> UserExists(string login)
        {
            //Relies on the "not found" exception if user doesn't exist. Could use some improving.

            return connection
                .Get(new GetUserRequest(login))
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

        public Task<IUser> GetUser(string login)
        {
            return connection
                .Get<Deserialization.User>(new GetUserRequest(login))
                .ContinueWith(r =>
                                  {
                                      TaskHelper.ThrowIfExceptionOccured(r);

                                      Deserialization.User user = r.Result;
                                      return user.GetUser();
                                  });
        }
    }
}