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
    public class AddMessageCommandHandlerShould
    {
        private string _validUserId = Guid.NewGuid().ToString();
        private string _invalidUserId = Guid.NewGuid().ToString();
        private string _validTicketId = Guid.NewGuid().ToString();
        private string _invalidTicketId = Guid.NewGuid().ToString();
        private Mock<IUserRepository> _repo;
        private IJwtHelper _jwt = new JwtHelper(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        private AddMessageCommandHandler _handler;

        public AddMessageCommandHandlerShould()
        {
            IEnumerable<SupportTicket> tickets = new List<SupportTicket>()
            {
                new SupportTicket()
                {
                    Id = _validTicketId,
                    SubmittedById = _validUserId
                }
            };

            IEnumerable<SupportTicket> ticketsWithForInvalidUser = new List<SupportTicket>()
            {
                new SupportTicket()
                {
                    Id = _validTicketId,
                    SubmittedById = _invalidUserId
                }
            };
            IEnumerable<SupportTicket> noTickets = new List<SupportTicket>();


            var logger = new Mock<ILogger>();
            _repo = new Mock<IUserRepository>();
            _repo.Setup(x => x.GetSupportTicketsByIdAsync(It.IsAny<string>()))
                .Returns<string>(id =>
                {
                    var response = id == _validTicketId ? tickets : id == _invalidTicketId ? ticketsWithForInvalidUser : noTickets;
                    return Task.FromResult(response);
                });
            _repo.SetupUserRepo(_invalidUserId);


            _handler = new AddMessageCommandHandler(logger.Object, _repo.Object, _jwt);
        }

        [Fact]
        public async Task ThrowIfBadJwt()
        {
            var query = CreateValidCommand();
            query.Token.Jwt = Guid.NewGuid().ToString();
            await _handler.AssertThrowsArgumentExceptionWithMessage(query, $"Unable to retrieve user id from jwt {query.Token.Jwt}.");
        }

        [Fact]
        public async Task ThrowsIfInvalidTicketId()
        {
            var command = CreateValidCommand();
            command.Request.TicketId = Guid.NewGuid().ToString();
            await _handler.AssertThrowsArgumentExceptionWithMessage(command, $"Unable to find support ticket with Id {command.Request.TicketId}.");
        }

        [Fact]
        public async Task ThrowsIfNotAllowed()
        {
            var command = CreateValidCommand();
            var userId = Guid.NewGuid().ToString();
            command.Token = _jwt.CreateToken(userId, "User");
            await _handler.AssertThrowsArgumentExceptionWithMessage(command, $"User with id {userId} attempted to add a message to ticket {command.Request.TicketId}, but is not allowed.");
        }

        [Fact]
        public async Task ThrowsIfInvalidUser()
        {
            var command = CreateValidCommand();
            command.Token = _jwt.CreateToken(_invalidUserId, "User");
            command.Request.TicketId = _invalidTicketId;
            await _handler.AssertThrowsArgumentExceptionWithMessage(command, $"Unable to find user with id {_invalidUserId}.");
        }

        [Fact]
        async Task AddsIfUserMatches()
        {
            var command = CreateValidCommand();
            await _handler.Handle(command, new CancellationToken());

            _repo.Verify(x => x.AddMessageToSupportTicketAsync(command.Request.TicketId, It.IsAny<Message>()), Times.Once);
        }

        [Fact]
        async Task AddsIfAdmin()
        {
            var command = CreateValidCommand();
            command.Token = _jwt.CreateToken(Guid.NewGuid().ToString(), "Admin");
            await _handler.Handle(command, new CancellationToken());

            _repo.Verify(x => x.AddMessageToSupportTicketAsync(command.Request.TicketId, It.IsAny<Message>()), Times.Once);
        }

        private AddMessageCommand CreateValidCommand() =>
            new AddMessageCommand()
            {
                Token = CreateUserToken(),
                Request = new MessageRequest()
                {
                    TicketId = _validTicketId,
                }
            };

        private Token CreateUserToken() =>
            _jwt.CreateToken(_validUserId, "User");

        private Token CreateAdminToken() =>
            _jwt.CreateToken(_validUserId, "Admin");
    }
}