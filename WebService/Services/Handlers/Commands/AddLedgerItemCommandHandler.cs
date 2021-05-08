using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using MediatR;

namespace WebService
{
    public class AddLedgerItemCommandHandler : IRequestHandler<AddLedgerItemCommand>
    {
        private ILogger _logger;
        private ILedgerRepository _repo;

        public AddLedgerItemCommandHandler(ILogger logger, ILedgerRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public async Task<Unit> Handle(AddLedgerItemCommand command, CancellationToken cancellation)
        {
            _logger.Information($"Adding ledger item with description {command.LedgerItem.Description}.");

            command.LedgerItem.CreatedBy = command.UserId;
            command.LedgerItem.CreatedDate = DateTime.Now;

            await _repo.InsertOneAsync(command.LedgerItem);
            return Unit.Value;
        }
    }
}