using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using MediatR;

namespace WebService
{
    public class GetLedgerItemsQueryHandler<T> : IRequestHandler<GetLedgerItemsQuery<T>, IEnumerable<T>> where T : AbstractLedgerItem
    {
        private ILogger _logger;
        private ILedgerRepository _repo;

        public GetLedgerItemsQueryHandler(ILogger logger, ILedgerRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public async Task<IEnumerable<T>> Handle(GetLedgerItemsQuery<T> query, CancellationToken cancellation) =>
            await _repo.GetAllAsync<T>();
    }
}