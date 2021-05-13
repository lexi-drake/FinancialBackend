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
    public class RefreshTokenQueryHandlerShould
    {
        private Token _token;
        private string _validUserId = Guid.NewGuid().ToString();
        private IJwtHelper _jwt = new JwtHelper(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        private Mock<IUserRepository> _repo;
        private RefreshTokenQueryHandler _handler;

        public RefreshTokenQueryHandlerShould()
        {
            _token = _jwt.CreateToken(_validUserId, Guid.NewGuid().ToString());
            IEnumerable<RefreshData> data = new List<RefreshData>()
            {
                new RefreshData()
                {
                    Refresh = _token.Refresh
                }
            };

            var logger = new Mock<ILogger>();
            _repo = new Mock<IUserRepository>();
            _repo.Setup(x => x.GetRefreshDataByUserIdAsync(It.IsAny<string>()))
                .Returns<string>(id => Task.FromResult(id == _validUserId ? data : new List<RefreshData>()));

            _handler = new RefreshTokenQueryHandler(logger.Object, _repo.Object, _jwt);
        }

        [Fact]
        public async Task ThrowIfEmptyJwt()
        {
            var query = CreateValidQuery();
            query.Token.Jwt = "";
            await _handler.AssertThrowsArgumentExceptionWithMessage(query, $"Invalid token with jwt {query.Token.Jwt} and refresh {query.Token.Refresh}.");

        }

        [Fact]
        public async Task ThrowIfEmptyRefresh()
        {
            var query = CreateValidQuery();
            query.Token.Refresh = "";
            await _handler.AssertThrowsArgumentExceptionWithMessage(query, $"Invalid token with jwt {query.Token.Jwt} and refresh {query.Token.Refresh}.");

        }

        [Fact]
        public async Task ThrowIfBadJwt()
        {
            var query = CreateValidQuery();
            query.Token.Jwt = Guid.NewGuid().ToString();
            await _handler.AssertThrowsArgumentExceptionWithMessage(query, $"Unable to retrieve user id or role from jwt {query.Token.Jwt}.");
        }

        [Fact]
        public async Task ThrowIfNoRefreshData()
        {
            var query = CreateValidQuery();
            query.Token = _jwt.CreateToken(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            await _handler.AssertThrowsArgumentExceptionWithMessage(query, $"Unable to find refresh data with refresh {query.Token.Refresh}.");
        }

        [Fact]
        public async Task ReturnRefreshData()
        {
            var query = CreateValidQuery();
            var token = await _handler.Handle(query, new CancellationToken());
            Assert.NotNull(token);
        }

        private RefreshTokenQuery CreateValidQuery() =>
            new RefreshTokenQuery()
            {
                Token = _token
            };
    }
}