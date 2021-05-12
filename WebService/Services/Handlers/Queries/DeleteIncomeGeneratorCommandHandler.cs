using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using MediatR;

namespace WebService
{
    public class DeleteIncomeGeneratorQueryHandler : IRequestHandler<DeleteIncomeGeneratorQuery, IEnumerable<string>>
    {
        private ILogger _logger;
        private ILedgerRepository _repo;

        public DeleteIncomeGeneratorQueryHandler(ILogger logger, ILedgerRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public async Task<IEnumerable<string>> Handle(DeleteIncomeGeneratorQuery query, CancellationToken cancellation)
        {
            _logger.Information($"Deleting ledger entry with id {query.Id}.");

            var generators = await _repo.GetIncomeGeneratorsByIdAsync(query.Id);
            if (!generators.Any())
            {
                _logger.Throw($"Unable to find income generator with id {query.Id}.");
            }

            var generator = generators.First();
            if (generator.UserId != query.UserId)
            {
                _logger.Throw($"Income generator with id {query.Id} does not belong to user with id {query.UserId}.");
            }

            await _repo.DeleteIncomeGeneratorAsync(query.Id, query.UserId);

            return generator.RecurringTransactions;
        }
    }
}