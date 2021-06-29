using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using MediatR;

namespace WebService
{
    public class GetIncomeGeneratorsQueryHandler : IRequestHandler<GetIncomeGeneratorsQuery, IEnumerable<IncomeGeneratorResponse>>
    {
        private ILogger _logger;
        private ILedgerRepository _repo;

        public GetIncomeGeneratorsQueryHandler(ILogger logger, ILedgerRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public async Task<IEnumerable<IncomeGeneratorResponse>> Handle(GetIncomeGeneratorsQuery query, CancellationToken cancellation)
        {
            var recurringTransactions = await _repo.GetRecurringTransactionsByUserIdAsync(query.UserId);
            var generators = await _repo.GetIncomeGeneratorsByUserIdAsync(query.UserId);
            var transactionTypes = await _repo.GetAllAsync<TransactionType>();

            var responses = from generator in generators
                            select new IncomeGeneratorResponse()
                            {
                                Id = generator.Id,
                                Description = generator.Description,
                                FrequencyId = generator.FrequencyId,
                                RecurringTransactions = recurringTransactions.Compile(generator.RecurringTransactions, transactionTypes)
                            };
            return responses;
        }
    }
}