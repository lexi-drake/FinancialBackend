using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Serilog;
using WebService;

namespace Tests
{
    public class DeleteRecurringTransactionCommandHandlerShould
    {
        private Mock<ILedgerRepository> _repo;
        private DeleteRecurringTransactionCommandHandler _handler;

        public DeleteRecurringTransactionCommandHandlerShould()
        {
            var logger = new Mock<ILogger>();
            _repo = new Mock<ILedgerRepository>();

            _handler = new DeleteRecurringTransactionCommandHandler(logger.Object, _repo.Object);
        }

        [Fact]
        public async Task DeleteRecurringTransaction()
        {
            var command = new DeleteRecurringTransactionCommand()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString()
            };
            await _handler.Handle(command, new CancellationToken());
            _repo.Verify(x => x.DeleteRecurringTransactionAsync(command.Id, command.UserId), Times.Once);
        }
    }
}