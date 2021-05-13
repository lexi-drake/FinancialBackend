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
    public class GetCategoriesQueryHandlerShould
    {
        private Mock<ILedgerRepository> _repo;
        private GetCategoriesQueryHandler _handler;

        public GetCategoriesQueryHandlerShould()
        {
            IEnumerable<LedgerEntryCategory> categories = new List<LedgerEntryCategory>()
            {
                new LedgerEntryCategory(){Category = Guid.NewGuid().ToString()}
            };

            var logger = new Mock<ILogger>();
            _repo = new Mock<ILedgerRepository>();
            _repo.Setup(x => x.GetLedgerEntryCategoriesLikeAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(categories));

            _handler = new GetCategoriesQueryHandler(logger.Object, _repo.Object);
        }

        [Fact]
        public async Task ReturnCategories()
        {
            var query = CreateValidQuery();
            var categories = await _handler.Handle(query, new CancellationToken());
            Assert.Single(categories);
            Assert.False(string.IsNullOrEmpty(categories.First()));
        }

        private GetCategoriesQuery CreateValidQuery() =>
            new GetCategoriesQuery()
            {
                Partial = Guid.NewGuid().ToString()
            };
    }
}