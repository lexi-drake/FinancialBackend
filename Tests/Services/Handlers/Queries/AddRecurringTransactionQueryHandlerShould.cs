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
        private Mock<ILedgerRepository> _repo;
        private AddRecurringTransactionQueryHandler _handler;

        public AddRecurringTransactionQueryHandlerShould()
        {
            var logger = new Mock<ILogger>();
            _repo = new Mock<ILedgerRepository>();

            _handler = new AddRecurringTransactionQueryHandler(logger.Object, _repo.Object);
        }
    }
}