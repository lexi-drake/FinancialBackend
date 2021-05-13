using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using MediatR;
using BC = BCrypt.Net.BCrypt;

namespace WebService
{
    public class LoginQueryHandler : IRequestHandler<LoginQuery, LoginResponse>
    {
        private ILogger _logger;
        private IUserRepository _repo;
        private IJwtHelper _jwt;

        public LoginQueryHandler(ILogger logger, IUserRepository repo, IJwtHelper jwt)
        {
            _logger = logger;
            _repo = repo;
            _jwt = jwt;
        }

        public async Task<LoginResponse> Handle(LoginQuery query, CancellationToken cancellation)
        {
            var users = await _repo.GetUsersByUsernameAsync(query.Request.Username);
            if (!users.Any())
            {
                _logger.Throw($"Attempted login for user {query.Request.Username} failed; user does not exist.");
            }

            var user = users.First();
            if (!BC.Verify(query.Request.Password, user.PasswordHash))
            {
                _logger.Throw($"Attempted login for user {query.Request.Username} failed; incorrect password.");
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
    }
}