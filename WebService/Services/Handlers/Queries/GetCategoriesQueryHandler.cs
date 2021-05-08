using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using MediatR;

namespace WebService
{
    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, IEnumerable<string>>
    {
        private ILogger _logger;
        private ILedgerRepository _repo;

        public GetCategoriesQueryHandler(ILogger logger, ILedgerRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public async Task<IEnumerable<string>> Handle(GetCategoriesQuery query, CancellationToken cancellation) =>
            from catgory in await _repo.GetLedgerEntryCategoriesLikeAsync(query.Partial)
            select catgory.Category;
    }
}