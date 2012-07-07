using System.Threading.Tasks;

namespace YouTrack.Rest.Repositories
{
    public interface IUserRepository
    {
        Task CreateUser(string login, string password, string email, string fullname = null);
        Task DeleteUser(string login);
        Task<bool> UserExists(string login);
        Task<IUser> GetUser(string login);
    }
}