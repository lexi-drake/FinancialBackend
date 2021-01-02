using System;
using System.Linq;
using FluentValidation;

namespace WebService
{
    public class UserRoleRequestValidator : AbstractValidator<UserRoleRequest>
    {
        private IUserRepository _repo;

        public UserRoleRequestValidator(IUserRepository repo)
        {
            _repo = repo;

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (description, cancellation) =>
                {
                    var types = from type in await _repo.GetUserRolesAsync()
                                where type.Description.Equals(description, StringComparison.InvariantCultureIgnoreCase)
                                select type;
                    return !types.Any();
                }).WithMessage("Description must be unique.");
        }
    }
}