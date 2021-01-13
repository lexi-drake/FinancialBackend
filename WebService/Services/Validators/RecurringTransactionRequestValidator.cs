using System;
using System.Linq;
using FluentValidation;

namespace WebService
{
    public class RecurringTransactionRequestValidator : AbstractValidator<RecurringTransactionRequest>
    {
        private const int MAX_CATEGORY_LENGTH = 24;
        private const int MAX_DESCRIPTION_LENGTH = 24;

        private ILedgerRepository _repo;

        public RecurringTransactionRequestValidator(ILedgerRepository repo)
        {
            _repo = repo;

            RuleFor(x => x.Category)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(category =>
                 {
                     return category.Length <= MAX_CATEGORY_LENGTH;
                 }).WithMessage($"Category must not exceed {MAX_CATEGORY_LENGTH} characters.");
            RuleFor(x => x.Description).Must(description =>
            {
                return string.IsNullOrEmpty(description) ? true : description.Length <= MAX_DESCRIPTION_LENGTH;
            }).WithMessage($"Description must not exceed {MAX_DESCRIPTION_LENGTH} characters.");
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.FrequencyId).NotEmpty();
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
            RuleFor(x => x).MustAsync(async (request, cancellation) =>
            {
                var frequencies = from frequency in await _repo.GetAllAsync<Frequency>()
                                  where frequency.Id == request.FrequencyId
                                  select frequency;
                if (!frequencies.Any())
                {
                    return false;
                }
                var weeks = 52 / frequencies.First().ApproxTimesPerYear;
                var minDate = DateTime.Now.AddDays(-((weeks + 1) * 7));
                return request.LastTriggered >= minDate && request.LastTriggered <= DateTime.Now;
            }).WithMessage("Last Triggered is outside of the reasonable window.");
        }
    }
}