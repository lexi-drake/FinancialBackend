using System;
using System.Threading.Tasks;

namespace financial_backend
{
    public class UserService : IUserService
    {
        public async Task<Token> CreateUserAsync(UserRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<Token> LoginUserAsync(UserRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<Token> RefreshLoginAsync(Token token)
        {
            throw new NotImplementedException();
        }
    }
}