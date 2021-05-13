using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Serilog;
using BC = BCrypt.Net.BCrypt;
using WebService;

namespace Tests
{
    public class LoginQueryHandlerShould
    {
        private string _username = Guid.NewGuid().ToString();
        private string _password = Guid.NewGuid().ToString();
        private IJwtHelper _jwt = new JwtHelper(Convert.ToBase64String(Guid.NewGuid().ToByteArray()), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        private Mock<IUserRepository> _repo;
        private LoginQueryHandler _handler;

        public LoginQueryHandlerShould()
        {
            IEnumerable<User> users = new List<User>()
            {
                new User()
                {
                    Username = _username,
                    PasswordHash = BC.HashPassword(_password)
                }
            };
            var logger = new Mock<ILogger>();
            _repo = new Mock<IUserRepository>();
            _repo.Setup(x => x.GetUsersByUsernameAsync(It.IsAny<string>()))
                .Returns<string>(username => Task.FromResult(username == _username ? users : new List<User>()));

            _handler = new LoginQueryHandler(logger.Object, _repo.Object, _jwt);
        }

        [Fact]
        public async Task ThrowIfInvalidUsername()
        {
            var query = CreateValidQuery();
            query.Request.Username = Guid.NewGuid().ToString();
            await _handler.AssertThrowsArgumentExceptionWithMessage(query, $"Attempted login for user {query.Request.Username} failed; user does not exist.");
        }

        [Fact]
        public async Task ThrowIfIncorrectPassword()
        {
            var query = CreateValidQuery();
            query.Request.Password = Guid.NewGuid().ToString();
            await _handler.AssertThrowsArgumentExceptionWithMessage(query, $"Attempted login for user {query.Request.Username} failed; incorrect password.");
        }

        [Fact]
        public async Task ReturnLoginResponse()
        {
            var query = CreateValidQuery();
            var response = await _handler.Handle(query, new CancellationToken());
            Assert.NotNull(response);
            Assert.Equal(_username, response.Username);
        }

        private LoginQuery CreateValidQuery() =>
            new LoginQuery()
            {
                Request = new LoginRequest()
                {
                    Username = _username,
                    Password = _password
                }
            };
    }
}