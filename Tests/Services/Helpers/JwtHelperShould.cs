using System;
using Xunit;
using WebService;

namespace Tests
{
    public class JwtHelperShould
    {
        private JwtHelper _jwt;

        public JwtHelperShould()
        {
            var secret = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            _jwt = new JwtHelper(secret, Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        }

        [Theory]
        [InlineData("", "something")]
        [InlineData(null, "something")]
        [InlineData("something", "")]
        [InlineData("something", null)]
        public void JwtNullWithBadUserIdOrRole(string userId, string role)
        {
            var token = _jwt.CreateToken(userId, role);
            Assert.NotNull(token);
            Assert.Null(token.Jwt);
            Assert.NotNull(token.Refresh);
        }

        [Fact]
        public void ReturnToken()
        {
            var token = _jwt.CreateToken(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            Assert.NotNull(token);
            Assert.NotNull(token.Jwt);
            Assert.NotNull(token.Refresh);
        }

        [Fact]
        public void ReturnNullUserIdIfInvalidJwt() =>
            Assert.True(string.IsNullOrEmpty(_jwt.GetUserIdFromToken(Guid.NewGuid().ToString())));


        [Fact]
        public void ReturnNullRoleIfInvalidJwt() =>
            Assert.True(string.IsNullOrEmpty(_jwt.GetRoleFromToken(Guid.NewGuid().ToString())));


        [Fact]
        public void GetUserIdFromJwt()
        {
            var userId = Guid.NewGuid().ToString();

            var token = _jwt.CreateToken(userId, Guid.NewGuid().ToString());
            Assert.NotNull(token);
            Assert.NotNull(token.Jwt);

            var result = _jwt.GetUserIdFromToken(token.Jwt);
            Assert.NotNull(result);
            Assert.Equal(userId, result);
        }

        [Fact]
        public void GetRoleFromJwt()
        {
            var role = Guid.NewGuid().ToString();

            var token = _jwt.CreateToken(Guid.NewGuid().ToString(), role);
            Assert.NotNull(token);
            Assert.NotNull(token.Jwt);

            var result = _jwt.GetRoleFromToken(token.Jwt);
            Assert.NotNull(result);
            Assert.Equal(role, result);
        }
    }
}