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
    public class ResolveSupportTicketCommandHandlerShould
    {
        private string _validUserId = Guid.NewGuid().ToString();
        private string _adminUserId = Guid.NewGuid().ToString();
        private string _validTicketId = Guid.NewGuid().ToString();
        private string _otherTicketId = Guid.NewGuid().ToString();
        private Mock<IUserRepository> _repo;
        private ResolveSupportTicketCommandHandler _handler;

        public ResolveSupportTicketCommandHandlerShould()
        {
            IEnumerable<User> users = new List<User> { new User() { Role = "User" } };
            IEnumerable<User> admins = new List<User> { new User() { Role = "Admin" } };
            IEnumerable<User> noUsers = new List<User>();

            IEnumerable<SupportTicket> tickets = new List<SupportTicket>() { new SupportTicket() { SubmittedById = _validUserId } };
            IEnumerable<SupportTicket> otherTickets = new List<SupportTicket>() { new SupportTicket() { SubmittedById = Guid.NewGuid().ToString() } };
            IEnumerable<SupportTicket> noTickets = new List<SupportTicket>();

            var logger = new Mock<ILogger>();
            _repo = new Mock<IUserRepository>();
            _repo.Setup(x => x.GetUsersByIdAsync(It.IsAny<string>()))
                .Returns<string>(userId => Task.FromResult(userId == _validUserId ? users : userId == _adminUserId ? admins : noUsers));
            _repo.Setup(x => x.GetSupportTicketsByIdAsync(It.IsAny<string>()))
                .Returns<string>(id => Task.FromResult(id == _validTicketId ? tickets : id == _otherTicketId ? otherTickets : noTickets));
            _handler = new ResolveSupportTicketCommandHandler(logger.Object, _repo.Object);
        }

        [Fact]
        public async Task ThrowIfInvalidUser()
        {
            var command = CreateValidCommand();
            command.UserId = Guid.NewGuid().ToString();
            await _handler.AssertThrowsArgumentExceptionWithMessage(command, $"Unable to find user with Id {command.UserId}.");
            var ex = await Assert.ThrowsAsync<ArgumentException>(async () => await _handler.Handle(command, new CancellationToken()));
            Assert.Equal($"Unable to find user with Id {command.UserId}.", ex.Message);
        }

        [Fact]
        public async Task UpdateIfAdmin()
        {
            var command = CreateValidCommand();
            command.UserId = _adminUserId;

            await _handler.Handle(command, new CancellationToken());
            _repo.Verify(x => x.UpdateSupportTicketResolvedAsync(command.Id, true), Times.Once);
        }

        [Fact]
        public async Task ThrowIfUserAndInvalidTicketId()
        {
            var command = CreateValidCommand();
            command.Id = Guid.NewGuid().ToString();
            await _handler.AssertThrowsArgumentExceptionWithMessage(command, $"Unable to find support ticket with Id {command.Id}.");
        }

        [Fact]
        public async Task ThrowIfUserAndInvalidUserId()
        {
            var command = CreateValidCommand();
            command.Id = _otherTicketId;
            await _handler.AssertThrowsArgumentExceptionWithMessage(command, $"User {command.UserId} cannot resolve submit ticket with Id {command.Id}.");
        }

        public async Task UpdateIfUserAndValidTicket()
        {
            var command = CreateValidCommand();
            await _handler.Handle(command, new CancellationToken());
            _repo.Verify(x => x.UpdateSupportTicketResolvedAsync(command.Id, true), Times.Once);

        }

        private ResolveSupportTicketCommand CreateValidCommand() =>
            new ResolveSupportTicketCommand()
            {
                UserId = _validUserId,
                Id = _validTicketId
            };
    }
}