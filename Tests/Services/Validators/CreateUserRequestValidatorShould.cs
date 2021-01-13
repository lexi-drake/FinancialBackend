using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using Moq;
using FluentValidation;
using WebService;

namespace Tests
{
    public class CreateUserRequestValidatorShould : IClassFixture<CreateUserRequestValidatorTestFixture>
    {
        private CreateUserRequestValidatorTestFixture _fixture;
        private IValidator<CreateUserRequest> _validator;

        public CreateUserRequestValidatorShould(CreateUserRequestValidatorTestFixture fixture)
        {
            _fixture = fixture;
            _validator = new CreateUserRequestValidator(_fixture.Repository);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task FailForEmptyUsername(string username)
        {
            var request = CreateCreateUserRequest();
            request.Username = username;

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, "'Username' must not be empty.");
        }


        [Fact]
        public async Task FailForSpaceInUsername()
        {
            var request = CreateCreateUserRequest();
            var midCharacter = request.Username[request.Username.Length / 2];
            request.Username = request.Username.Replace(midCharacter, ' ');

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, "Username must not contain spaces.");
        }

        [Fact]
        public async Task FailForUsernameAlreadyExists()
        {
            var request = CreateCreateUserRequest();
            request.Username = _fixture.UsernameThatAlreadyExists;

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, "Username already exists.");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("        ")]    // 8 Spaces
        public async Task FailForEmptyPassword(string password)
        {
            var request = CreateCreateUserRequest();
            request.Password = password;

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, "'Password' must not be empty.");
        }

        [Fact]
        public async Task FailForShortPassword()
        {
            var request = CreateCreateUserRequest();
            request.Password = request.Password.Substring(0, 7);

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, "Password must be at least 8 characters.");
        }

        [Fact]
        public async Task PassesValidRequest()
        {
            var request = CreateCreateUserRequest();

            var result = await _validator.ValidateAsync(request);
            Assert.True(result.IsValid);
        }


        private CreateUserRequest CreateCreateUserRequest()
        {
            return new CreateUserRequest()
            {
                Username = _fixture.UsernameThatDoesNotExist,
                Password = Guid.NewGuid().ToString()
            };
        }
    }

    public class CreateUserRequestValidatorTestFixture
    {
        public string UsernameThatAlreadyExists { get; set; } = Guid.NewGuid().ToString();
        public string UsernameThatDoesNotExist { get; set; } = Guid.NewGuid().ToString();

        public IUserRepository Repository { get; set; }

        public CreateUserRequestValidatorTestFixture()
        {
            IEnumerable<User> users = new List<User>() { new User() { Username = UsernameThatAlreadyExists } };
            IEnumerable<User> emptyUsers = new List<User>();

            var repository = new Mock<IUserRepository>();
            repository.Setup(x => x.GetUsersByUsernameAsync(UsernameThatAlreadyExists))
                .Returns(Task.FromResult(users));
            repository.Setup(x => x.GetUsersByUsernameAsync(UsernameThatDoesNotExist))
                .Returns(Task.FromResult(emptyUsers));

            Repository = repository.Object;
        }
    }
}
