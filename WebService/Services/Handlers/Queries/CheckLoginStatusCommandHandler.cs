using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using MediatR;

namespace WebService
{
    public class CheckLoginStatusQueryHandler : IRequestHandler<CheckLoginStatusQuery, LoginResponse>
    {
        private ILogger _logger;
        private IUserRepository _repo;
        private IJwtHelper _jwt;

        public CheckLoginStatusQueryHandler(ILogger logger, IUserRepository repo, IJwtHelper jwt)
        {
            _logger = logger;
            _repo = repo;
            _jwt = jwt;
        }

        public async Task<LoginResponse> Handle(CheckLoginStatusQuery query, CancellationToken cancellation)
        {
            var id = _jwt.GetUserIdFromToken(query.Token.Jwt);
            if (string.IsNullOrEmpty(id))
            {
                _logger.Throw($"Unable to retrieve id from jwt {query.Token.Jwt}.");
            }
            var users = await _repo.GetUsersByIdAsync(id);
            if (!users.Any())
            {
                _logger.Throw($"Unable to find user with id {id}.");
            }
            var user = users.First();
            return new LoginResponse()
            {
                Username = user.Username,
                Role = user.Role
            };
        }
    }
}