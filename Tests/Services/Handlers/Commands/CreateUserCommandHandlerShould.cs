using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Serilog;
using WebService;

namespace Tests
{
    public class CreateUserCommandHandlerShould
    {
        private Mock<IUserRepository> _repo;
        private CreateUserCommandHandler _handler;

        public CreateUserCommandHandlerShould()
        {
            var logger = new Mock<ILogger>();
            _repo = new Mock<IUserRepository>();

            _handler = new CreateUserCommandHandler(logger.Object, _repo.Object);
        }

        [Fact]
        public async Task InsertUser()
        {
            var command = new CreateUserCommand()
            {
                Request = new CreateUserRequest()
                {
                    Password = Guid.NewGuid().ToString()
                }
            };
            await _handler.Handle(command, new CancellationToken());
            _repo.Verify(x => x.InsertUserAsync(It.IsAny<User>()), Times.Once);
        }
    }
}