using System.Threading;
using System.Threading.Tasks;
using Serilog;
using MediatR;

namespace WebService
{
    public class DeleteRecurringTransactionCommandHandler : IRequestHandler<DeleteRecurringTransactionCommand>
    {
        private ILogger _logger;
        private ILedgerRepository _repo;

        public DeleteRecurringTransactionCommandHandler(ILogger logger, ILedgerRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public async Task<Unit> Handle(DeleteRecurringTransactionCommand command, CancellationToken cancellation)
        {
            _logger.Information($"Deleting recurring transaction with id {command.Id}.");
            await _repo.DeleteRecurringTransactionAsync(command.Id, command.UserId);
            return Unit.Value;
        }
    }
}