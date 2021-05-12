using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Serilog;
using WebService;

namespace Tests
{
    public class AddRecurringTransactionQueryHandlerShould
    {
        private string _id = Guid.NewGuid().ToString();
        private Mock<ILedgerRepository> _repo;
        private AddRecurringTransactionQueryHandler _handler;

        public AddRecurringTransactionQueryHandlerShould()
        {
            var logger = new Mock<ILogger>();
            _repo = new Mock<ILedgerRepository>();
            _repo.Setup(x => x.InsertRecurringTransactionAsync(It.IsAny<RecurringTransaction>()))
                .Returns<RecurringTransaction>(x =>
                {
                    x.Id = _id;
                    return Task.FromResult(x);
                });

            _handler = new AddRecurringTransactionQueryHandler(logger.Object, _repo.Object);
        }

        [Fact]
        public async Task InsertRecurringTransaction()
        {
            var query = CreateValidQuery();

            var id = await _handler.Handle(query, new CancellationToken());
            Assert.Equal(_id, id);

            _repo.Verify(x => x.InsertRecurringTransactionAsync(It.IsAny<RecurringTransaction>()), Times.Once);
            _repo.Verify(x => x.InsertOrUpdateCategoryAsync(It.IsAny<string>()), Times.Once);
            _repo.Verify(x => x.InsertLedgerEntryAsync(It.IsAny<LedgerEntry>()), Times.Once);
        }

        private AddRecurringTransactionQuery CreateValidQuery() =>
            new AddRecurringTransactionQuery()
            {
                Request = new RecurringTransactionRequest()
            };
    }
}