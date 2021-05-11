using System;
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
        private Mock<IUserRepository> _repo;
        private ResolveSupportTicketCommandHandler _handler;

        public ResolveSupportTicketCommandHandlerShould()
        {
            var logger = new Mock<ILogger>();
            _repo = new Mock<IUserRepository>();

            _handler = new ResolveSupportTicketCommandHandler(logger.Object, _repo.Object);
        }

        [Fact]
        public async Task ThrowIfInvalidUser()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task UpdateIfAdmin()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task ThrowIfUserAndInvalidTicketId()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public async Task ThrowIfUserAndInvalidUserId()
        {
            throw new NotImplementedException();
        }

        public async Task UpdateIfUserAndValidTicket()
        {
            throw new NotImplementedException();
        }
    }
}