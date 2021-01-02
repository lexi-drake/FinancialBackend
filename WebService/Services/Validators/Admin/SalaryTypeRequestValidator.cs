using System;
using System.Linq;
using FluentValidation;

namespace WebService
{
    public class SalaryTypeRequestValidator : AbstractValidator<SalaryTypeRequest>
    {
        private ILedgerRepository _repo;

        public SalaryTypeRequestValidator(ILedgerRepository repo)
        {
            _repo = repo;

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (description, cancellation) =>
                {
                    var types = from type in await _repo.GetSalaryTypesAsync()
                                where type.Description.Equals(description, StringComparison.InvariantCultureIgnoreCase)
                                select type;
                    return !types.Any();
                }).WithMessage("Description must be unique.");
        }
    }
}