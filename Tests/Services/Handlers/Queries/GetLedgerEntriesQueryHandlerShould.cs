using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Serilog;
using WebService;

namespace Tests
{
    public class GetLedgerEntriesQueryHandlerShould
    {
        private Mock<ILedgerRepository> _repo;
        private GetLedgerEntriesQueryHandler _handler;

        public GetLedgerEntriesQueryHandlerShould()
        {
            var logger = new Mock<ILogger>();
            _repo = new Mock<ILedgerRepository>();

            _handler = new GetLedgerEntriesQueryHandler(logger.Object, _repo.Object);
        }

        [Fact]
        public async Task ThrowIfInvalidStartDate()
        {
            var query = CreateValidQuery();
            query.Start = Guid.NewGuid().ToString();
            await _handler.AssertThrowsArgumentExceptionWithMessage(query, $"Unable to parse millisecond values {query.Start} and/or {query.End}.");
        }

        [Fact]
        public async Task ThrowIfInvalidEndDate()
        {
            var query = CreateValidQuery();
            query.End = Guid.NewGuid().ToString();
            await _handler.AssertThrowsArgumentExceptionWithMessage(query, $"Unable to parse millisecond values {query.Start} and/or {query.End}.");
        }

        [Fact]
        public async Task ReturnEntries()
        {
            var query = CreateValidQuery();
            var entries = await _handler.Handle(query, new CancellationToken());
            Assert.NotNull(entries);
        }

        private GetLedgerEntriesQuery CreateValidQuery() =>
            new GetLedgerEntriesQuery()
            {
                UserId = Guid.NewGuid().ToString(),
                Start = NowMilliseconds().ToString(),
                End = NowMilliseconds().ToString()
            };

        private double NowMilliseconds()
        {
            DateTime start = new DateTime(1970, 1, 1);
            var span = DateTime.Now - start;
            return span.TotalMilliseconds;
        }
    }
}