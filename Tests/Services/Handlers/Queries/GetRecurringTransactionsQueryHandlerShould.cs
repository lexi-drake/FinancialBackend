using System;
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
            var logger = new Mock<ILogger>();
            _repo = new Mock<ILedgerRepository>();

            _handler = new GetRecurringTransactionsQueryHandler(logger.Object, _repo.Object);
        }
    }
}