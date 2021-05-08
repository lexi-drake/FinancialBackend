using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Serilog;
using WebService;

namespace Tests
{
    public class AddIncomeGeneratorCommandHandlerShould
    {
        private Mock<ILedgerRepository> _repo;
        private AddIncomeGeneratorCommandHandler _handler;

        public AddIncomeGeneratorCommandHandlerShould()
        {
            var logger = new Mock<ILogger>();
            _repo = new Mock<ILedgerRepository>();

            _handler = new AddIncomeGeneratorCommandHandler(logger.Object, _repo.Object);
        }

        [Fact]
        public async Task InsertIncomeGenerator()
        {
            var command = new AddIncomeGeneratorCommand()
            {
                UserId = Guid.NewGuid().ToString(),
                Request = new IncomeGeneratorRequest()
            };

            await _handler.Handle(command, new CancellationToken());

            _repo.Verify(x => x.InsertIncomeGeneratorAsync(It.IsAny<IncomeGenerator>()), Times.Once);
        }
    }
}