using System;
using System.Linq;
using FluentValidation;

namespace WebService
{
    public class FrequencyRequestValidator : AbstractValidator<FrequencyRequest>
    {
        private ILedgerRepository _repo;

        public FrequencyRequestValidator(ILedgerRepository repo)
        {
            _repo = repo;

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (description, cancellation) =>
                {
                    var types = from type in await _repo.GetAllAsync<Frequency>()
                                where type.Description.Equals(description, StringComparison.InvariantCultureIgnoreCase)
                                select type;
                    return !types.Any();
                }).WithMessage("Description must be unique.");
            RuleFor(x => x.ApproxTimesPerYear)
                .GreaterThan(0)
                .LessThanOrEqualTo(365);
        }
    }
}