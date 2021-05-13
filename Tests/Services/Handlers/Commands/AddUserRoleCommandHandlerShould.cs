using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Serilog;
using WebService;

namespace Tests
{
    public class AddUserRoleCommandHandlerShould
    {
        private Mock<IUserRepository> _repo;
        private AddUserRoleCommandHandler _handler;

        public AddUserRoleCommandHandlerShould()
        {
            var logger = new Mock<ILogger>();
            _repo = new Mock<IUserRepository>();

            _handler = new AddUserRoleCommandHandler(logger.Object, _repo.Object);
        }

        [Fact]
        public async Task InsertUserRole()
        {
            var command = new AddUserRoleCommand();
            await _handler.Handle(command, new CancellationToken());
            _repo.Verify(x => x.InsertUserRoleAsync(It.IsAny<UserRole>()), Times.Once);
        }
    }
}