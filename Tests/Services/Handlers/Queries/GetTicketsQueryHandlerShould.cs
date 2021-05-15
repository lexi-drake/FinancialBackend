using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Serilog;
using WebService;

namespace Tests
{
    public class GetTicketsQueryHandlerShould
    {
        private string _userId = Guid.NewGuid().ToString();
        private IJwtHelper _jwt = new JwtHelper(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        private Mock<IUserRepository> _repo;
        private GetTicketsQueryHandler _handler;

        public GetTicketsQueryHandlerShould()
        {
            IEnumerable<SupportTicket> tickets = new List<SupportTicket>()
            {
                new SupportTicket() { SubmittedById = Guid.NewGuid().ToString(), Messages = new List<Message>() },
                new SupportTicket() { SubmittedById = _userId, Messages = new List<Message>() }
            };

            var logger = new Mock<ILogger>();
            _repo = new Mock<IUserRepository>();
            _repo.Setup(x => x.GetSupportTicketsAsync())
                .Returns(Task.FromResult(tickets));
            _repo.Setup(x => x.GetSupportTicketsSubmittedByUser(_userId))
                .Returns(Task.FromResult(from ticket in tickets where ticket.SubmittedById == _userId select ticket));

            _handler = new GetTicketsQueryHandler(logger.Object, _repo.Object, _jwt);
        }

        [Fact]
        public async Task ThrowIfInvalidJwt()
        {
            var query = CreateValidQuery();
            query.Token.Jwt = Guid.NewGuid().ToString();
            await _handler.AssertThrowsArgumentExceptionWithMessage(query, $"Unable to retrieve role from jwt {query.Token.Jwt}.");
        }

        [Fact]
        public async Task ReturnAllTicketsIfAdmin()
        {
            var query = CreateValidQuery();
            query.Token = _jwt.CreateToken(Guid.NewGuid().ToString(), "Admin");
            var tickets = await _handler.Handle(query, new CancellationToken());
            Assert.True(tickets.Count() == 2);
        }

        [Fact]
        public async Task ReturnUserTicketsIfUser()
        {
            var query = CreateValidQuery();
            var tickets = await _handler.Handle(query, new CancellationToken());
            Assert.Single(tickets);

        }

        private GetTicketsQuery CreateValidQuery() =>
            new GetTicketsQuery()
            {
                Token = _jwt.CreateToken(_userId, "User")
            };
    }
}