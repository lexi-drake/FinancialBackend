using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Serilog;
using WebService;

namespace Tests
{
    public class CheckLoginStatusQueryHandlerShould
    {
        private string _validUserId = Guid.NewGuid().ToString();
        private string _invalidUserId = Guid.NewGuid().ToString();
        private IJwtHelper _jwt = new JwtHelper(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        private Mock<IUserRepository> _repo;
        private CheckLoginStatusQueryHandler _handler;

        public CheckLoginStatusQueryHandlerShould()
        {
            var logger = new Mock<ILogger>();
            _repo = new Mock<IUserRepository>();
            _repo.SetupUserRepo(_invalidUserId);

            _handler = new CheckLoginStatusQueryHandler(logger.Object, _repo.Object, _jwt);
        }

        [Fact]
        public async Task ThrowIfBadJwt()
        {
            var query = CreateValidQuery();
            query.Token.Jwt = Guid.NewGuid().ToString();
            await _handler.AssertThrowsArgumentExceptionWithMessage(query, $"Unable to retrieve id from jwt {query.Token.Jwt}.");
        }

        [Fact]
        public async Task ThrowIfNoUsers()
        {
            var query = new CheckLoginStatusQuery()
            {
                Token = _jwt.CreateToken(_invalidUserId, Guid.NewGuid().ToString())
            };

            await _handler.AssertThrowsArgumentExceptionWithMessage(query, $"Unable to find user with id {_invalidUserId}.");
        }

        [Fact]
        public async Task ReturnLoginResponse()
        {
            var query = CreateValidQuery();
            var response = await _handler.Handle(query, new CancellationToken());
            Assert.NotNull(response);
        }

        private CheckLoginStatusQuery CreateValidQuery() =>
            new CheckLoginStatusQuery()
            {
                Token = _jwt.CreateToken(_validUserId, Guid.NewGuid().ToString())
            };
    }
}