using System;
using System.Linq;
using FluentValidation;

namespace WebService
{
    public class TransactionTypeRequestValidator : AbstractValidator<TransactionTypeRequest>
    {
        private ILedgerRepository _repo;

        public TransactionTypeRequestValidator(ILedgerRepository repo)
        {
            _repo = repo;

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (description, cancellation) =>
                {
                    var types = from type in await _repo.GetAllAsync<TransactionType>()
                                where type.Description.Equals(description, StringComparison.InvariantCultureIgnoreCase)
                                select type;
                    return !types.Any();
                }).WithMessage("Description must be unique.");
        }
    }
}