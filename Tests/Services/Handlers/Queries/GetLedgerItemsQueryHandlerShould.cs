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
    public class GetLedgerItemsQueryHandlerShould
    {
        private Mock<ILedgerRepository> _repo;
        private GetLedgerItemsQueryHandler<TransactionType> _handler;

        public GetLedgerItemsQueryHandlerShould()
        {
            IEnumerable<TransactionType> salaryTypes = new List<TransactionType>();

            var logger = new Mock<ILogger>();
            _repo = new Mock<ILedgerRepository>();
            _repo.Setup(x => x.GetAllAsync<TransactionType>())
                .Returns(Task.FromResult(salaryTypes));

            _handler = new GetLedgerItemsQueryHandler<TransactionType>(logger.Object, _repo.Object);
        }

        [Fact]
        public async Task GetsAll()
        {
            var types = await _handler.Handle(new GetLedgerItemsQuery<TransactionType>(), new CancellationToken());
            Assert.NotNull(types);
        }
    }
}