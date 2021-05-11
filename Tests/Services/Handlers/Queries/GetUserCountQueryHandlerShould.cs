using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Serilog;
using WebService;

namespace Tests
{
    public class GetUserCountQueryHandlerShould
    {
        private Mock<IUserRepository> _repo;
        private GetUserCountQueryHandler _handler;

        public GetUserCountQueryHandlerShould()
        {
            var logger = new Mock<ILogger>();
            _repo = new Mock<IUserRepository>();

            _handler = new GetUserCountQueryHandler(logger.Object, _repo.Object);
        }
    }
}