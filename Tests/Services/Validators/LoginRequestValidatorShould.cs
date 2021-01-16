using System;
using Xunit;
using WebService;

namespace Tests
{
    public class LoginRequestValidatorShould
    {
        private LoginRequestValidator _validator = new LoginRequestValidator();

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void FailsForEmptyUsername(string username)
        {
            var request = CreateLoginRequest();
            request.Username = username;

            var result = _validator.Validate(request);
            AssertHelper.FailsWithMessage(result, "'Username' must not be empty.");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void FailsForEmptyPassword(string password)
        {
            var request = CreateLoginRequest();
            request.Password = password;

            var result = _validator.Validate(request);
            AssertHelper.FailsWithMessage(result, "'Password' must not be empty.");
        }

        [Fact]
        public void PassesValidRequest()
        {
            var result = _validator.Validate(CreateLoginRequest());
            Assert.True(result.IsValid);
        }

        private LoginRequest CreateLoginRequest() =>
            new LoginRequest()
            {
                Username = Guid.NewGuid().ToString(),
                Password = Guid.NewGuid().ToString()
            };
    }
}