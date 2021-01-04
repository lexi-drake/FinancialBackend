using System.Threading.Tasks;

namespace WebService
{
    public interface IUserService
    {
        Task<Token> CreateUserAsync(CreateUserRequest request);
        Task<Token> LoginUserAsync(LoginRequest request);
        Task<Token> RefreshLoginAsync(Token token);
        Task LogoutUserAsync(Token token);
    }
}