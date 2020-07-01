using System.Threading.Tasks;
using DattingApp.api.Models;

namespace DattingApp.api.Data
{
    public interface IAuthRepository
    {
        Task<User> Register(User user, string password, bool reset);
        Task<User> Login(string userName, string password);
        Task<bool> UserExists(string userName);
        Task<User> ResetUser(User user, string password);
    }
}