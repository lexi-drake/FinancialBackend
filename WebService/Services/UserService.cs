using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using BC = BCrypt.Net.BCrypt;

namespace WebService
{
    public class UserService : IUserService
    {
        private const int SALT_SIZE = 32;
        private const string USER_ROLE = "User";
        private readonly ILogger<UserService> _logger;
        private IUserRepository _repo;
        private JwtHelper _jwt;

        public UserService(ILogger<UserService> logger, IUserRepository repo, JwtHelper jwt)
        {
            _logger = logger;
            _repo = repo;
            _jwt = jwt;
        }

        public async Task<Token> CreateUserAsync(CreateUserRequest request)
        {
            // Validator ensures non-duplicate username
            var salt = GenerateSalt();
            var passwordHash = BC.HashPassword(salt + request.Password);

            // Validator ensures role is "User"
            await _repo.InsertUserAsync(new User()
            {
                Role = USER_ROLE,
                Username = request.Username,
                PasswordHash = passwordHash,
                Salt = salt,
                CreatedDate = DateTime.Now
            });
            return await LoginUserAsync(new LoginRequest()
            {
                Username = request.Username,
                Password = request.Password
            });
        }

        private string GenerateSalt(int size = SALT_SIZE)
        {
            var bytes = new byte[size];
            new RNGCryptoServiceProvider().GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public async Task<Token> LoginUserAsync(LoginRequest request)
        {
            var users = await _repo.GetUsersByUsernameAsync(request.Username);
            if (!users.Any())
            {
                return null;
            }

            var user = users.First();
            if (!BC.Verify(user.Salt + request.Password, user.PasswordHash))
            {
                return null;
            }

            var token = _jwt.CreateToken(user.Id, user.Role);
            await _repo.InsertRefreshDataAsync(new RefreshData()
            {
                Refresh = token.Refresh,
                UserId = user.Id,
                CreatedDate = DateTime.Now
            });
            return token;
        }

        public async Task<Token> RefreshLoginAsync(Token token)
        {
            if (!string.IsNullOrEmpty(token.Jwt) || string.IsNullOrEmpty(token.Refresh))
            {
                return null;
            }

            try
            {
                var userId = _jwt.GetUserIdFromToken(token.Jwt);
                var role = _jwt.GetRoleFromToken(token.Jwt);

                var refreshData = from data in await _repo.GetRefreshDataByUserIdAsync(userId)
                                  where data.Refresh == token.Refresh
                                  select data;

                if (!refreshData.Any())
                {
                    return null;
                }

                var newToken = _jwt.CreateToken(userId, role);
                await _repo.DeleteRefreshDataByIdAsync(refreshData.First().Id);
                await _repo.InsertRefreshDataAsync(new RefreshData()
                {
                    Refresh = newToken.Refresh,
                    UserId = userId,
                    CreatedDate = DateTime.Now
                });
                return newToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<UserRole> AddUserRoleAsync(UserRole role)
        {
            return await _repo.InsertUserRoleAsync(role);
        }
    }
}