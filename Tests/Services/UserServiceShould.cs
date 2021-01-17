using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using BC = BCrypt.Net.BCrypt;
using Serilog;
using Xunit;
using Moq;
using WebService;

namespace Tests
{
    public class UserServiceShould
    {
        private long _count = new Random().Next();
        private Token _token;
        private string _rawTextPassword = Guid.NewGuid().ToString();
        private User _user;
        private string _invalidJwt = Guid.NewGuid().ToString();
        private string _invalidUserId = Guid.NewGuid().ToString();
        private string _invalidUsername = Guid.NewGuid().ToString();
        private Mock<IUserRepository> _repo;
        private Mock<IJwtHelper> _jwt;

        private IUserService _service;

        public UserServiceShould()
        {
            _token = new Token()
            {
                Jwt = Guid.NewGuid().ToString(),
                Refresh = Guid.NewGuid().ToString()
            };
            _user = new User()
            {
                Id = Guid.NewGuid().ToString(),
                Role = Guid.NewGuid().ToString(),
                Username = Guid.NewGuid().ToString(),
                PasswordHash = BC.HashPassword(_rawTextPassword),
                LastLoggedIn = DateTime.Now,
                CreatedDate = DateTime.Now
            };
            var refresh = new RefreshData()
            {
                Id = Guid.NewGuid().ToString(),
                Refresh = _token.Refresh,
                UserId = _user.Id,
                CreatedDate = DateTime.Now
            };

            IEnumerable<User> users = new List<User>() { _user };
            IEnumerable<User> noUsers = new List<User>();
            IEnumerable<RefreshData> refreshData = new List<RefreshData>() { refresh };

            _repo = new Mock<IUserRepository>();
            _repo.Setup(x => x.GetUserCountAsync())
                .Returns(Task.FromResult(_count));
            _repo.Setup(x => x.GetUsersByIdAsync(_user.Id))
                .Returns(Task.FromResult(users));
            _repo.Setup(x => x.GetUsersByIdAsync(_invalidUserId))
                .Returns(Task.FromResult(noUsers));
            _repo.Setup(x => x.InsertUserAsync(It.IsAny<User>()))
                .Returns(Task.FromResult(_user));
            _repo.Setup(x => x.GetUsersByUsernameAsync(_user.Username))
                .Returns(Task.FromResult(users));
            _repo.Setup(x => x.GetRefreshDataByUserIdAsync(_user.Id))
                .Returns(Task.FromResult(refreshData));
            _repo.Setup(x => x.UpdateUsernameAsync(_user.Id, _invalidUsername))
                .Returns(Task.FromResult(0L));
            _repo.Setup(x => x.UpdateUsernameAsync(_user.Id, _user.Username))
                .Returns(Task.FromResult(1L));

            _jwt = new Mock<IJwtHelper>();
            _jwt.Setup(x => x.GetUserIdFromToken(_token.Jwt))
                .Returns(_user.Id);
            _jwt.Setup(x => x.GetUserIdFromToken(_invalidJwt))
                .Returns(_invalidUserId);
            _jwt.Setup(x => x.CreateToken(_user.Id, _user.Role))
                .Returns(_token);
            _jwt.Setup(x => x.GetRoleFromToken(_token.Jwt))
                .Returns(_user.Role);

            _service = new UserService(new Mock<ILogger>().Object, _repo.Object, _jwt.Object);
        }

        [Fact]
        public async Task GetsUserCount()
        {
            var response = await _service.GetUserCountAsync();
            Assert.NotNull(response);
            Assert.Equal(_count, response);
        }

        [Fact]
        public async Task ReturnsNullWithNoId()
        {
            var token = new Token()
            {
                Jwt = Guid.NewGuid().ToString(),
                Refresh = Guid.NewGuid().ToString(),
            };

            var response = await _service.GetUserAsync(token);
            Assert.Null(response);
        }

        [Fact]
        public async Task ReturnsNullWithInvalidToken()
        {
            var token = new Token()
            {
                Jwt = _invalidJwt,
                Refresh = Guid.NewGuid().ToString(),
            };

            var response = await _service.GetUserAsync(token);
            Assert.Null(response);
        }

        [Fact]
        public async Task ReturnsUser()
        {
            var response = await _service.GetUserAsync(_token);
            Assert.NotNull(response);
            Assert.Equal(_user.Username, response.Username);
            Assert.Equal(_user.Role, response.Role);
        }

        [Fact]
        public async Task AddsAndLogsInUser()
        {
            var request = new CreateUserRequest()
            {
                Username = _user.Username,
                Password = _rawTextPassword
            };

            var response = await _service.CreateUserAsync(request);
            Assert.NotNull(response);
            Assert.Equal(_user.Username, response.Username);
            Assert.Equal(_user.Role, response.Role);
            Assert.Equal(_token, response.Token);

            _repo.Verify(x => x.UpdateUserLastLoggedInAsync(_user.Id, It.IsAny<DateTime>()), Times.Once());
            _repo.Verify(x => x.InsertRefreshDataAsync(It.IsAny<RefreshData>()), Times.Once());
        }

        [Theory]
        [InlineData("", "something")]
        [InlineData(null, "something")]
        [InlineData("something", "")]
        [InlineData("something", null)]
        public async Task ReturnsNullWithBadToken(string jwt, string refresh)
        {
            var token = new Token()
            {
                Jwt = jwt,
                Refresh = refresh
            };
            var response = await _service.RefreshLoginAsync(token);
            Assert.Null(response);
        }


        [Fact]
        public async Task RefreshesLogin()
        {
            var response = await _service.RefreshLoginAsync(_token);
            Assert.NotNull(response);

            _repo.Verify(x => x.DeleteRefreshDataByIdAsync(It.IsAny<string>()), Times.Once());
            _repo.Verify(x => x.InsertRefreshDataAsync(It.IsAny<RefreshData>()), Times.Once());
        }

        [Fact]
        public async Task ThrowsIfBadToken()
        {
            var token = new Token()
            {
                Jwt = "",
                Refresh = Guid.NewGuid().ToString()
            };

            await Assert.ThrowsAsync<ArgumentException>(async () => await _service.LogoutUserAsync(token));
        }

        [Fact]
        public async Task LogsOutUser()
        {
            await _service.LogoutUserAsync(_token);

            _repo.Verify(x => x.DeleteRefreshDataByIdAsync(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public async Task ReturnsNullIfUserToUpdateDoesNotExist()
        {
            var request = new UpdateUsernameRequest()
            {
                Username = _invalidUsername
            };
            var response = await _service.UpdateUsernameAsync(request, _token);
            Assert.Null(response);
        }

        [Fact]
        public async Task ReturnsUpdatedUsername()
        {
            var request = new UpdateUsernameRequest()
            {
                Username = _user.Username
            };
            var response = await _service.UpdateUsernameAsync(request, _token);
            Assert.NotNull(response);
            Assert.Equal(request.Username, response.Username);
        }
    }
}