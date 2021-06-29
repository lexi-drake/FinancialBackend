using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Serilog;
using WebService;

namespace Tests
{
    public class AddLedgerItemCommandHandlerShould
    {
        private Mock<ILedgerRepository> _repo;
        private AddLedgerItemCommandHandler _handler;

        public AddLedgerItemCommandHandlerShould()
        {
            var logger = new Mock<ILogger>();
            _repo = new Mock<ILedgerRepository>();

            _handler = new AddLedgerItemCommandHandler(logger.Object, _repo.Object);
        }

        [Fact]
        public async Task InsertIncomeGenerator()
        {
            var command = new AddLedgerItemCommand()
            {
                UserId = Guid.NewGuid().ToString(),
                LedgerItem = new TransactionType()
                {
                    Description = Guid.NewGuid().ToString()
                }
            };

            await _handler.Handle(command, new CancellationToken());

            _repo.Verify(x => x.InsertOneAsync<AbstractLedgerItem>(It.IsAny<TransactionType>()), Times.Once);
        }
    }
}