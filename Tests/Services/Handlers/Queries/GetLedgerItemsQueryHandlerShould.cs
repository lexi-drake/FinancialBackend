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
        private GetLedgerItemsQueryHandler<SalaryType> _handler;

        public GetLedgerItemsQueryHandlerShould()
        {
            IEnumerable<SalaryType> salaryTypes = new List<SalaryType>();

            var logger = new Mock<ILogger>();
            _repo = new Mock<ILedgerRepository>();
            _repo.Setup(x => x.GetAllAsync<SalaryType>())
                .Returns(Task.FromResult(salaryTypes));

            _handler = new GetLedgerItemsQueryHandler<SalaryType>(logger.Object, _repo.Object);
        }

        [Fact]
        public async Task GetsAll()
        {
            var types = await _handler.Handle(new GetLedgerItemsQuery<SalaryType>(), new CancellationToken());
            Assert.NotNull(types);
        }
    }
}