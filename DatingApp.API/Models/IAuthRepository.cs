using System.Threading.Tasks;

namespace DatingApp.API.Models
{
    public interface IAuthRepository
    {
        Task<User> Register(User user, string password);
        Task<User> Login(string username, string password);

        Task<bool> UserExisted(string username);
    }
}