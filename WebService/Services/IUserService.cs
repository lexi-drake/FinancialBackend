using System.Threading.Tasks;

namespace WebService
{
    public interface IUserService
    {
        Task<LoginResponse> GetUserAsync(Token token);
        Task<LoginResponse> CreateUserAsync(CreateUserRequest request);
        Task<LoginResponse> LoginUserAsync(LoginRequest request);
        Task<Token> RefreshLoginAsync(Token token);
        Task LogoutUserAsync(Token token);
        Task<UpdateUsernameResponse> UpdateUsernameAsync(UpdateUsernameRequest request, Token token);
    }
}