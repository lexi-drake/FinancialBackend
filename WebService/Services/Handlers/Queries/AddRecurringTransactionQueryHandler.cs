using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using MediatR;

namespace WebService
{
    public class AddRecurringTransactionQueryHandler : IRequestHandler<AddRecurringTransactionQuery, string>
    {
        private ILogger _logger;
        private ILedgerRepository _repo;

        public AddRecurringTransactionQueryHandler(ILogger logger, ILedgerRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public async Task<string> Handle(AddRecurringTransactionQuery command, CancellationToken cancellation)
        {
            _logger.Information($"Adding recurring transaction with description {command.Request.Description}.");

            var recurringTransaction = await _repo.InsertRecurringTransactionAsync(new RecurringTransaction()
            {
                UserId = command.UserId,
                Category = command.Request.Category,
                Description = command.Request.Description,
                Amount = new Decimal(command.Request.Amount),
                FrequencyId = command.Request.FrequencyId,
                TransactionTypeId = command.Request.TransactionTypeId,
                LastTriggered = command.Request.LastTriggered,
                LastExecuted = command.Request.LastTriggered,
                CreatedDate = DateTime.Now
            });

            await _repo.InsertOrUpdateCategoryAsync(command.Request.Category);
            await _repo.InsertLedgerEntryAsync(new LedgerEntry()
            {
                UserId = command.UserId,
                Category = recurringTransaction.Category,
                Description = recurringTransaction.Description,
                Amount = recurringTransaction.Amount,
                TransactionTypeId = recurringTransaction.TransactionTypeId,
                RecurringTransactionId = recurringTransaction.Id,
                TransactionDate = recurringTransaction.LastTriggered
            });
            return recurringTransaction.Id;
        }
    }
}