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
    public class LogoutCommandHandlerShould
    {
        private IJwtHelper _jwt = new JwtHelper(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        private Mock<IUserRepository> _repo;
        private LogoutCommandHandler _handler;

        public LogoutCommandHandlerShould()
        {
            IEnumerable<RefreshData> data = new List<RefreshData>()
            {
                new RefreshData(),
                new RefreshData()
            };
            var logger = new Mock<ILogger>();
            _repo = new Mock<IUserRepository>();
            _repo.Setup(x => x.GetRefreshDataByUserIdAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(data));

            _handler = new LogoutCommandHandler(logger.Object, _repo.Object, _jwt);
        }

        // TODO (alexa): Figure out how to trigger the first exception condition 
        // (null/empty userId).

        [Fact]
        public async Task DeleteAllRefreshData()
        {
            var command = CreateValidCommand();
            await _handler.Handle(command, new CancellationToken());
            _repo.Verify(x => x.GetRefreshDataByUserIdAsync(It.IsAny<string>()), Times.Once);
            _repo.Verify(x => x.DeleteRefreshDataByIdAsync(It.IsAny<String>()), Times.Exactly(2));
        }

        private LogoutCommand CreateValidCommand() =>
            new LogoutCommand()
            {
                Token = _jwt.CreateToken(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
            };
    }
}