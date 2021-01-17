using System;
using System.Linq;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;
using Serilog;

namespace WebService
{
    public class UserService : IUserService
    {
        // We always want to default the user to the role of User
        private const string USER_ROLE = "User";
        private readonly ILogger _logger;
        private IUserRepository _repo;
        private IJwtHelper _jwt;

        public UserService(ILogger logger, IUserRepository repo, IJwtHelper jwt)
        {
            _logger = logger;
            _repo = repo;
            _jwt = jwt;
        }

        public async Task<long> GetUserCountAsync() => await _repo.GetUserCountAsync();

        public async Task<LoginResponse> GetUserAsync(Token token)
        {
            var id = _jwt.GetUserIdFromToken(token.Jwt);
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            var users = await _repo.GetUsersByIdAsync(id);
            if (!users.Any())
            {
                return null;
            }
            var user = users.First();
            return new LoginResponse()
            {
                Username = user.Username,
                Role = user.Role
            };
        }

        public async Task<LoginResponse> CreateUserAsync(CreateUserRequest request)
        {
            var passwordHash = BC.HashPassword(request.Password);

            // Validator ensures non-duplicate username
            await _repo.InsertUserAsync(new User()
            {
                Role = USER_ROLE,
                Username = request.Username,
                PasswordHash = passwordHash,
                CreatedDate = DateTime.Now
            });

            // To prevent requiring a second call to the back end, we're just going to 
            // log the user in and return the login info to the front end.
            return await LoginUserAsync(new LoginRequest()
            {
                Username = request.Username,
                Password = request.Password
            });
        }

        public async Task<LoginResponse> LoginUserAsync(LoginRequest request)
        {
            var users = await _repo.GetUsersByUsernameAsync(request.Username);
            if (!users.Any())
            {
                return null;
            }

            var user = users.First();
            if (!BC.Verify(request.Password, user.PasswordHash))
            {
                return null;
            }

            await _repo.UpdateUserLastLoggedInAsync(user.Id, DateTime.Now);

            var token = _jwt.CreateToken(user.Id, user.Role);
            await _repo.InsertRefreshDataAsync(new RefreshData()
            {
                Refresh = token.Refresh,
                UserId = user.Id,
                CreatedDate = DateTime.Now
            });
            return new LoginResponse()
            {
                Username = user.Username,
                Role = user.Role,
                Token = token
            };
        }

        public async Task<Token> RefreshLoginAsync(Token token)
        {
            if (string.IsNullOrEmpty(token.Jwt) || string.IsNullOrEmpty(token.Refresh))
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
                _logger.Error(ex.Message);
                throw;
            }
        }

        public async Task LogoutUserAsync(Token token)
        {
            if (string.IsNullOrEmpty(token.Jwt))
            {
                throw new ArgumentException("Cannot logout user with empty jwt token");
            }

            var userId = _jwt.GetUserIdFromToken(token.Jwt);

            var ids = from data in await _repo.GetRefreshDataByUserIdAsync(userId)
                      select data.Id;
            foreach (var id in ids)
            {
                await _repo.DeleteRefreshDataByIdAsync(id);
            }
        }

        public async Task<UpdateUsernameResponse> UpdateUsernameAsync(UpdateUsernameRequest request, Token token)
        {
            var userId = _jwt.GetUserIdFromToken(token.Jwt);
            var updated = await _repo.UpdateUsernameAsync(userId, request.Username);
            if (updated == 0)
            {
                return null;
            }
            return new UpdateUsernameResponse()
            {
                Username = request.Username
            };
        }
    }
}