using System.Threading;
using System.Threading.Tasks;
using Serilog;
using MediatR;

namespace WebService
{
    public class DeleteLedgerEntryCommandHandler : IRequestHandler<DeleteLedgerEntryCommand>
    {
        private ILogger _logger;
        private ILedgerRepository _repo;

        public DeleteLedgerEntryCommandHandler(ILogger logger, ILedgerRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public async Task<Unit> Handle(DeleteLedgerEntryCommand command, CancellationToken cancellation)
        {
            _logger.Information($"Deleting ledger entry with id {command.Id}.");
            await _repo.DeleteLedgerEntryAsync(command.Id, command.UserId);
            return Unit.Value;
        }
    }
}