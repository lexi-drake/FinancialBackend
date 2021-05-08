using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using MediatR;

namespace WebService
{
    public class AddLedgerEntryCommandHandler : IRequestHandler<AddLedgerEntryCommand>
    {
        private ILogger _logger;
        private ILedgerRepository _repo;

        public AddLedgerEntryCommandHandler(ILogger logger, ILedgerRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public async Task<Unit> Handle(AddLedgerEntryCommand command, CancellationToken cancellation)
        {
            _logger.Information($"Adding ledger entry with description {command.Request.Description}.");

            await _repo.InsertOrUpdateCategoryAsync(command.Request.Category);
            var entry = await _repo.InsertLedgerEntryAsync(new LedgerEntry()
            {
                UserId = command.UserId,
                Category = command.Request.Category,
                Description = command.Request.Description,
                Amount = new Decimal(command.Request.Amount),
                TransactionTypeId = command.Request.TransactionTypeId,
                TransactionDate = command.Request.TransactionDate,
                RecurringTransactionId = command.Request.RecurringTransactionId,
                CreatedDate = DateTime.Now
            });

            return Unit.Value;
        }
    }
}