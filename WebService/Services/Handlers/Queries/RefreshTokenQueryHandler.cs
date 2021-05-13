using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using MediatR;

namespace WebService
{
    public class RefreshTokenQueryHandler : IRequestHandler<RefreshTokenQuery, Token>
    {
        private ILogger _logger;
        private IUserRepository _repo;
        private IJwtHelper _jwt;

        public RefreshTokenQueryHandler(ILogger logger, IUserRepository repo, IJwtHelper jwt)
        {
            _logger = logger;
            _repo = repo;
            _jwt = jwt;
        }

        public async Task<Token> Handle(RefreshTokenQuery query, CancellationToken cancellation)
        {
            if (string.IsNullOrEmpty(query.Token.Jwt) || string.IsNullOrEmpty(query.Token.Refresh))
            {
                _logger.Throw($"Invalid token with jwt {query.Token.Jwt} and refresh {query.Token.Refresh}.");
            }

            var userId = _jwt.GetUserIdFromToken(query.Token.Jwt);
            var role = _jwt.GetRoleFromToken(query.Token.Jwt);
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(role))
            {
                _logger.Throw($"Unable to retrieve user id or role from jwt {query.Token.Jwt}.");
            }

            var refreshData = from data in await _repo.GetRefreshDataByUserIdAsync(userId)
                              where data.Refresh == query.Token.Refresh
                              select data;

            if (!refreshData.Any())
            {
                _logger.Throw($"Unable to find refresh data with refresh {query.Token.Refresh}.");
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
    }
}