using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Serilog;
using WebService;

namespace Tests
{
    public class GetRecurringTransactionsQueryHandlerShould
    {
        private Mock<ILedgerRepository> _repo;
        private GetRecurringTransactionsQueryHandler _handler;

        public GetRecurringTransactionsQueryHandlerShould()
        {
            IEnumerable<TransactionType> types = new List<TransactionType>()
            {
                new TransactionType() { Id = Guid.NewGuid().ToString() }
            };

            IEnumerable<RecurringTransaction> transactions = new List<RecurringTransaction>()
            {
                new RecurringTransaction()
                {
                    TransactionTypeId = types.First().Id
                }
            };

            var logger = new Mock<ILogger>();
            _repo = new Mock<ILedgerRepository>();
            _repo.Setup(x => x.GetAllAsync<TransactionType>())
                .Returns(Task.FromResult(types));
            _repo.Setup(x => x.GetRecurringTransactionsByUserIdAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(transactions));

            _handler = new GetRecurringTransactionsQueryHandler(logger.Object, _repo.Object);
        }

        [Fact]
        public async Task ReturnRecurringTransactions()
        {
            var query = CreateValidQuery();
            var transactions = await _handler.Handle(query, new CancellationToken());
            Assert.Single(transactions);
        }

        private GetRecurringTransactionsQuery CreateValidQuery() =>
            new GetRecurringTransactionsQuery()
            {
                UserId = Guid.NewGuid().ToString()
            };
    }
}