using System;
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
        private Mock<ILedgerRepository> _repo;
        private GetIncomeGeneratorsQueryHandler _handler;

        public GetIncomeGeneratorsQueryHandlerShould()
        {
            var logger = new Mock<ILogger>();
            _repo = new Mock<ILedgerRepository>();

            _handler = new GetIncomeGeneratorsQueryHandler(logger.Object, _repo.Object);
        }
    }
}