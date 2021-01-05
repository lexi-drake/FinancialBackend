using System;
using System.Linq;
using FluentValidation;

namespace WebService
{
    public class LedgerEntryRequestValidator : AbstractValidator<LedgerEntryRequest>
    {
        private ILedgerRepository _repo;
        public LedgerEntryRequestValidator(ILedgerRepository repo)
        {
            _repo = repo;

            RuleFor(x => x.Category).NotEmpty();
            // Description is optional.
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.TransactionTypeId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (id, cancellation) =>
                {
                    var transactionTypes = from type in await _repo.GetAllAsync<TransactionType>()
                                           where type.Id == id
                                           select type;
                    return transactionTypes.Any();
                }).WithMessage("Transaction Type Id must be valid.");
            // RecurringTransactionId is optional.
            RuleFor(x => x.TransactionDate).LessThanOrEqualTo(DateTime.Now);
        }
    }
}