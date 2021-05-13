using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Serilog;
using WebService;

namespace Tests
{
    public class AddLedgerEntryCommandHandlerShould
    {
        private Mock<ILedgerRepository> _repo;
        private AddLedgerEntryCommandHandler _handler;

        public AddLedgerEntryCommandHandlerShould()
        {
            var logger = new Mock<ILogger>();
            _repo = new Mock<ILedgerRepository>();

            _handler = new AddLedgerEntryCommandHandler(logger.Object, _repo.Object);
        }

        [Fact]
        public async Task InsertLedgerEntry()
        {
            var command = new AddLedgerEntryCommand()
            {
                UserId = Guid.NewGuid().ToString(),
                Request = new LedgerEntryRequest()
                {
                    Category = Guid.NewGuid().ToString()
                }
            };

            await _handler.Handle(command, new CancellationToken());

            _repo.Verify(x => x.InsertOrUpdateCategoryAsync(command.Request.Category), Times.Once);
            _repo.Verify(x => x.InsertLedgerEntryAsync(It.IsAny<LedgerEntry>()), Times.Once);
        }
    }
}