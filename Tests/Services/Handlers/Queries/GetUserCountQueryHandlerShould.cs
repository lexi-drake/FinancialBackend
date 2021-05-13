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
        private long _count = (long)new Random().Next();
        private Mock<IUserRepository> _repo;
        private GetUserCountQueryHandler _handler;

        public GetUserCountQueryHandlerShould()
        {
            var logger = new Mock<ILogger>();
            _repo = new Mock<IUserRepository>();
            _repo.Setup(x => x.GetUserCountAsync())
                .Returns(Task.FromResult(_count));

            _handler = new GetUserCountQueryHandler(logger.Object, _repo.Object);
        }

        [Fact]
        public async Task ReturnsCount()
        {
            var count = await _handler.Handle(new GetUserCountQuery(), new CancellationToken());
            Assert.Equal(_count, count);
        }
    }
}