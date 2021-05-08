using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using MediatR;

namespace WebService
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
    {
        private ILogger _logger;
        private IUserRepository _repo;
        private IJwtHelper _jwt;

        public LogoutCommandHandler(ILogger logger, IUserRepository repo, IJwtHelper jwt)
        {
            _logger = logger;
            _repo = repo;
            _jwt = jwt;
        }

        public async Task<Unit> Handle(LogoutCommand command, CancellationToken cancellation)
        {
            var userId = _jwt.GetUserIdFromToken(command.Token.Jwt);
            if (userId is null)
            {
                _logger.Throw("Cannot logout user with empty jwt token");
            }

            var ids = from data in await _repo.GetRefreshDataByUserIdAsync(userId)
                      select data.Id;
            foreach (var id in ids)
            {
                // In case the user has more than one refresh data stored.
                await _repo.DeleteRefreshDataByIdAsync(id);
            }
            return Unit.Value;
        }
    }
}