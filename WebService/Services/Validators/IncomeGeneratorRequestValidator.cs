using System.Linq;
using FluentValidation;

namespace WebService
{
    public class IncomeGeneratorRequestValidator : AbstractValidator<IncomeGeneratorRequest>
    {
        private const int MAX_DESCRIPTION_LENGTH = 24;

        private ILedgerRepository _repo;

        public IncomeGeneratorRequestValidator(ILedgerRepository repo, IValidator<RecurringTransactionRequest> recurringTransactionValidator)
        {
            _repo = repo;

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(description => description.Length <= MAX_DESCRIPTION_LENGTH).WithMessage($"Description must not exceed {MAX_DESCRIPTION_LENGTH} characters.");
            RuleFor(x => x.SalaryTypeId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (id, cancellation) =>
                {
                    var salaryTypes = from type in await _repo.GetAllAsync<SalaryType>()
                                      where type.Id == id
                                      select type;
                    return salaryTypes.Any();
                }).WithMessage("Salary Type Id must be valid.");
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
            RuleFor(x => x.RecurringTransactions)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MustAsync(async (transactions, cancellation) =>
                {
                    foreach (var transaction in transactions)
                    {
                        var result = await recurringTransactionValidator.ValidateAsync(transaction);
                        if (!result.IsValid)
                        {
                            return false;
                        }
                    }
                    return true;
                }).WithMessage("Recurring Transactions must be valid.");
        }
    }
}