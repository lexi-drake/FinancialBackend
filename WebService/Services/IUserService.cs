using System.Threading.Tasks;

namespace WebService
{
    public interface IUserService
    {
        Task<Token> CreateUserAsync(CreateUserRequest request);
        Task<Token> LoginUserAsync(LoginRequest request);
        Task<Token> RefreshLoginAsync(Token token);
        Task<UserRole> AddUserRoleAsync(UserRole role);
    }
}