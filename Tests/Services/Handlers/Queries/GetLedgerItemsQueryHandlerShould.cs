using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Serilog;
using WebService;

namespace Tests
{
    public class GetLedgerItemsQueryHandlerShould
    {
        private Mock<ILedgerRepository> _repo;
        private GetLedgerEntriesQueryHandler _handler;

        public GetLedgerItemsQueryHandlerShould()
        {
            var logger = new Mock<ILogger>();
            _repo = new Mock<ILedgerRepository>();

            _handler = new GetLedgerEntriesQueryHandler(logger.Object, _repo.Object);
        }
    }
}