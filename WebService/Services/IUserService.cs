using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebService
{
    public interface IUserService
    {
        Task<long> GetUserCountAsync();
        Task<LoginResponse> GetUserAsync(Token token);
        Task<LoginResponse> CreateUserAsync(CreateUserRequest request);
        Task<LoginResponse> LoginUserAsync(LoginRequest request);
        Task<Token> RefreshLoginAsync(Token token);
        Task LogoutUserAsync(Token token);
        Task<UpdateUsernameResponse> UpdateUsernameAsync(UpdateUsernameRequest request, Token token);
        Task<IEnumerable<MessageResponse>> GetMessagesAsync(Token token);
        Task<SubmitSupportTicketResponse> SubmitSupportTicketAsync(SupportTicketRequest request, Token token);
    }
}