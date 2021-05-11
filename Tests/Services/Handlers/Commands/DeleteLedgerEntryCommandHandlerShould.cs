using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Serilog;
using WebService;

namespace Tests
{
    public class DeleteLedgerEntryCommandHandlerShould
    {
        private Mock<ILedgerRepository> _repo;
        private DeleteLedgerEntryCommandHandler _handler;

        public DeleteLedgerEntryCommandHandlerShould()
        {
            var logger = new Mock<ILogger>();
            _repo = new Mock<ILedgerRepository>();

            _handler = new DeleteLedgerEntryCommandHandler(logger.Object, _repo.Object);
        }

        [Fact]
        public async Task DeleteLedgerEntry()
        {
            var command = new DeleteLedgerEntryCommand()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString()
            };
            await _handler.Handle(command, new CancellationToken());
            _repo.Verify(x => x.DeleteLedgerEntryAsync(command.Id, command.UserId), Times.Once);
        }
    }
}