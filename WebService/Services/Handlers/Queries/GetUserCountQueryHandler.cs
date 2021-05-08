using System.Threading;
using System.Threading.Tasks;
using Serilog;
using MediatR;

namespace WebService
{
    public class GetUserCountQueryHandler : IRequestHandler<GetUserCountQuery, long>
    {
        private ILogger _logger;
        private IUserRepository _repo;

        public GetUserCountQueryHandler(ILogger logger, IUserRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public async Task<long> Handle(GetUserCountQuery query, CancellationToken cancellation) =>
            await _repo.GetUserCountAsync();
    }
}