using System;
using System.Linq;
using FluentValidation;

namespace WebService
{
    public class LedgerEntryRequestValidator : AbstractValidator<LedgerEntryRequest>
    {
        private const int MAX_DESCRIPTION_LENGTH = 24;
        private ILedgerRepository _repo;
        public LedgerEntryRequestValidator(ILedgerRepository repo)
        {
            _repo = repo;

            RuleFor(x => x.Category).NotEmpty();
            RuleFor(x => x.Description).Must(description =>
            {
                return string.IsNullOrEmpty(description) ? true : description.Length <= MAX_DESCRIPTION_LENGTH;

            }).WithMessage($"Description must not exceed {MAX_DESCRIPTION_LENGTH} characters.");
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