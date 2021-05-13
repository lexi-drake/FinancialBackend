using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Serilog;
using WebService;

namespace Tests
{
    public class AddSupportTicketCommandHandlerShould
    {
        private string _invalidUserId = Guid.NewGuid().ToString();
        private Mock<IUserRepository> _repo;
        private IJwtHelper _jwt = new JwtHelper(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        private AddSupportTicketCommandHandler _handler;

        public AddSupportTicketCommandHandlerShould()
        {
            var logger = new Mock<ILogger>();
            _repo = new Mock<IUserRepository>();
            _repo.SetupUserRepo(_invalidUserId);

            _handler = new AddSupportTicketCommandHandler(logger.Object, _repo.Object, _jwt);
        }

        [Fact]
        public async Task ThrowIfBadJwt()
        {
            var query = CreateValidCommand();
            query.Token.Jwt = Guid.NewGuid().ToString();
            await _handler.AssertThrowsArgumentExceptionWithMessage(query, $"Unable to retrieve user id from jwt {query.Token.Jwt}.");
        }

        [Fact]
        public async Task ThrowIfInvalidUser()
        {
            var command = CreateValidCommand();
            command.Token = _jwt.CreateToken(_invalidUserId, "User");
            await _handler.AssertThrowsArgumentExceptionWithMessage(command, $"Unable to find user with id {_invalidUserId}.");
        }

        [Fact]
        public async Task InsertSupportTicket()
        {
            var command = CreateValidCommand();
            await _handler.Handle(command, new CancellationToken());
            _repo.Verify(x => x.InsertSupportTicketAsync(It.IsAny<SupportTicket>()), Times.Once);
        }

        private AddSupportTicketCommand CreateValidCommand() =>
            new AddSupportTicketCommand()
            {
                Token = CreateUserToken(),
                Request = new SupportTicketRequest()
            };

        private Token CreateUserToken() =>
            _jwt.CreateToken(Guid.NewGuid().ToString(), "User");
    }
}