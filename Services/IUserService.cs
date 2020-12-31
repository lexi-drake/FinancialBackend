using System.Threading.Tasks;

namespace financial_backend
{
    public interface IUserService
    {
        Task<Token> CreateUserAsync(UserRequest request);
        Task<Token> LoginUserAsync(UserRequest request);
        Task<Token> RefreshLoginAsync(Token token);
    }
}