using System.Linq;
using FluentValidation;

namespace WebService
{
    public class UpdateUserRoleRequestValidator : AbstractValidator<UpdateUserRoleRequest>
    {
        private IUserRepository _repo;

        public UpdateUserRoleRequestValidator(IUserRepository repo)
        {
            _repo = repo;

            RuleFor(x => x.Username)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(username => !username.Contains(' ')).WithMessage("Username cannot contain spaces.")
                .MustAsync(async (username, cancellation) =>
                {
                    var users = await _repo.GetUsersByUsernameAsync(username);
                    return users.Any();
                }).WithMessage("User must already exist.");
            RuleFor(x => x.Role)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (role, cancellation) =>
                {
                    var matches = from userRole in await _repo.GetUserRolesAsync()
                                  where userRole.Description == role
                                  select userRole;
                    return matches.Any();
                }).WithMessage("Role must already exist.");

        }
    }
}