using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using MediatR;

namespace WebService
{
    public class GetRecurringTransactionsQueryHandler : IRequestHandler<GetRecurringTransactionsQuery, IEnumerable<RecurringTransactionResponse>>
    {
        private ILogger _logger;
        private ILedgerRepository _repo;

        public GetRecurringTransactionsQueryHandler(ILogger logger, ILedgerRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public async Task<IEnumerable<RecurringTransactionResponse>> Handle(GetRecurringTransactionsQuery query, CancellationToken cancellation)
        {
            var transactionTypes = await _repo.GetAllAsync<TransactionType>();
            return from transaction in await _repo.GetRecurringTransactionsByUserIdAsync(query.UserId)
                   select RecurringTransactionResponse.FromDBObject(transaction, transactionTypes);
        }
    }
}