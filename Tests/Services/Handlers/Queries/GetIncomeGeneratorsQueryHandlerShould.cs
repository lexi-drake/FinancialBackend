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
    public class GetIncomeGeneratorsQueryHandlerShould
    {
        private string _recurringTransactionId = Guid.NewGuid().ToString();
        private Mock<ILedgerRepository> _repo;
        private GetIncomeGeneratorsQueryHandler _handler;

        public GetIncomeGeneratorsQueryHandlerShould()
        {
            IEnumerable<RecurringTransaction> transactions = new List<RecurringTransaction>()
            {
                new RecurringTransaction() { Id = _recurringTransactionId}
            };

            IEnumerable<IncomeGenerator> generators = new List<IncomeGenerator>()
            {
                new IncomeGenerator()
                {
                    RecurringTransactions = new List<string>()
                    {
                        transactions.First().Id
                    }
                }
            };

            var logger = new Mock<ILogger>();
            _repo = new Mock<ILedgerRepository>();
            _repo.Setup(x => x.GetRecurringTransactionsByUserIdAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(transactions));
            _repo.Setup(x => x.GetIncomeGeneratorsByUserIdAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(generators));

            _handler = new GetIncomeGeneratorsQueryHandler(logger.Object, _repo.Object);
        }

        [Fact]
        public async Task ReturnIncomeGeneratorResponses()
        {
            var query = CreateValidQuery();
            var responses = await _handler.Handle(query, new CancellationToken());
            Assert.Single(responses);
            Assert.Single(responses.First().RecurringTransactions);
            Assert.Equal(_recurringTransactionId, responses.First().RecurringTransactions.First().Id);
        }

        private GetIncomeGeneratorsQuery CreateValidQuery() =>
            new GetIncomeGeneratorsQuery()
            {
                UserId = Guid.NewGuid().ToString()
            };
    }
}