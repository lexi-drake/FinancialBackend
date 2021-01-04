using System.Linq;
using FluentValidation;

namespace WebService
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        private const int MIN_PASSWORD_LENGTH = 8;
        private IUserRepository _repo;

        public CreateUserRequestValidator(IUserRepository repo)
        {
            _repo = repo;

            RuleFor(x => x.Username)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(username => !username.Contains(' ')).WithMessage("Username cannot contain spaces.")
                .MustAsync(async (username, cancellation) =>
                {
                    var usernames = await _repo.GetUsersByUsernameAsync(username);
                    return !usernames.Any();
                }).WithMessage("Username already exists.");
            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(password => password.Length >= MIN_PASSWORD_LENGTH)
                .WithMessage($"Password must be at least {MIN_PASSWORD_LENGTH} characters.");
        }
    }
}