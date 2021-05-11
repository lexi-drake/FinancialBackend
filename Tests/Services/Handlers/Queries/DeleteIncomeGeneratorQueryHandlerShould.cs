using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Serilog;
using WebService;

namespace Tests
{
    public class DeleteIncomeGeneratorQueryHandlerShould
    {
        private Mock<ILedgerRepository> _repo;
        private DeleteIncomeGeneratorQueryHandler _handler;

        public DeleteIncomeGeneratorQueryHandlerShould()
        {
            var logger = new Mock<ILogger>();
            _repo = new Mock<ILedgerRepository>();

            _handler = new DeleteIncomeGeneratorQueryHandler(logger.Object, _repo.Object);
        }
    }
}