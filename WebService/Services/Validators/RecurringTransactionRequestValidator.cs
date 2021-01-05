using System.Linq;
using FluentValidation;

namespace WebService
{
    public class RecurringTransactionRequestValidator : AbstractValidator<RecurringTransactionRequest>
    {
        private ILedgerRepository _repo;

        public RecurringTransactionRequestValidator(ILedgerRepository repo)
        {
            _repo = repo;

            RuleFor(x => x.Category).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Amount).GreaterThan(0);
            RuleFor(x => x.FrequencyId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (id, cancellation) =>
                {
                    var frequencies = from frequency in await _repo.GetAllAsync<Frequency>()
                                      where frequency.Id == id
                                      select frequency;
                    return frequencies.Any();
                }).WithMessage("Frequency Id must be valid.");
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
        }
    }
}