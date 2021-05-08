using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using MediatR;

namespace WebService
{
    public class AddIncomeGeneratorCommandHandler : IRequestHandler<AddIncomeGeneratorCommand>
    {
        private ILogger _logger;
        private ILedgerRepository _repo;

        public AddIncomeGeneratorCommandHandler(ILogger logger, ILedgerRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public async Task<Unit> Handle(AddIncomeGeneratorCommand command, CancellationToken cancellation)
        {
            _logger.Information($"Adding income generator with description {command.Request.Description}.");

            var generator = await _repo.InsertIncomeGeneratorAsync(new IncomeGenerator()
            {
                UserId = command.UserId,
                Description = command.Request.Description,
                SalaryTypeId = command.Request.SalaryTypeId,
                FrequencyId = command.Request.FrequencyId,
                RecurringTransactions = command.TransactionIds,
                CreatedDate = DateTime.Now
            });

            return Unit.Value;
        }
    }
}