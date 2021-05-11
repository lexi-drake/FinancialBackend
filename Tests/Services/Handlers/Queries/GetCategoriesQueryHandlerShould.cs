using System;
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
            var logger = new Mock<ILogger>();
            _repo = new Mock<ILedgerRepository>();

            _handler = new GetCategoriesQueryHandler(logger.Object, _repo.Object);
        }
    }
}