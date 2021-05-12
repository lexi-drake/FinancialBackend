using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Serilog;
using WebService;

namespace Tests
{
    public class DeleteIncomeGeneratorQueryHandlerShould
    {
        private string _validUserId = Guid.NewGuid().ToString();
        private string _validId = Guid.NewGuid().ToString();
        private Mock<ILedgerRepository> _repo;
        private DeleteIncomeGeneratorQueryHandler _handler;

        public DeleteIncomeGeneratorQueryHandlerShould()
        {
            IEnumerable<IncomeGenerator> generators = new List<IncomeGenerator>() { new IncomeGenerator() { UserId = _validUserId, RecurringTransactions = new List<string>() } };
            IEnumerable<IncomeGenerator> noGenerators = new List<IncomeGenerator>();

            var logger = new Mock<ILogger>();
            _repo = new Mock<ILedgerRepository>();
            _repo.Setup(x => x.GetIncomeGeneratorsByIdAsync(It.IsAny<string>()))
                .Returns<string>(id => Task.FromResult(id == _validId ? generators : noGenerators));

            _handler = new DeleteIncomeGeneratorQueryHandler(logger.Object, _repo.Object);
        }

        [Fact]
        public async Task ThrowIfInvalidId()
        {
            var query = CreateValidQuery();
            query.Id = Guid.NewGuid().ToString();
            await _handler.AssertThrowsArgumentExceptionWithMessage(query, $"Unable to find income generator with id {query.Id}.");
        }

        [Fact]
        public async Task ThrowIfWrongUser()
        {
            var query = CreateValidQuery();
            query.UserId = Guid.NewGuid().ToString();
            await _handler.AssertThrowsArgumentExceptionWithMessage(query, $"Income generator with id {query.Id} does not belong to user with id {query.UserId}.");
        }

        [Fact]
        public async Task DeleteIncomeGenerator()
        {
            var query = CreateValidQuery();
            var transactions = await _handler.Handle(query, new CancellationToken());
            Assert.NotNull(transactions);
        }

        private DeleteIncomeGeneratorQuery CreateValidQuery() =>
            new DeleteIncomeGeneratorQuery()
            {
                Id = _validId,
                UserId = _validUserId
            };
    }
}