using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using Moq;
using WebService;

namespace Tests
{
    public class IncomeGeneratorRequestValidatorShould
    {
        private string _validSalaryTypeId = Guid.NewGuid().ToString();
        private string _invalidSalaryTypeId = Guid.NewGuid().ToString();
        private string _validFrequencyId = Guid.NewGuid().ToString();
        private string _invalidFrequencyId = Guid.NewGuid().ToString();
        private int _approxTimesPerYear = new Random().Next(1, 52);
        private string _validTransactionTypeId = Guid.NewGuid().ToString();
        private string _nvalidTransactionTypeId = Guid.NewGuid().ToString();
        private RecurringTransactionRequest _validRecurringTransactionRequest;

        private IncomeGeneratorRequestValidator _validator;

        public IncomeGeneratorRequestValidatorShould()
        {
            _validRecurringTransactionRequest = new RecurringTransactionRequest()
            {
                Category = Guid.NewGuid().ToString().Substring(0, 24),
                Description = Guid.NewGuid().ToString().Substring(0, 24),
                Amount = new Random().Next(1, 1000),
                FrequencyId = _validFrequencyId,
                TransactionTypeId = _validTransactionTypeId,
                LastTriggered = DateTime.Now.AddDays(-(new Random().Next(1, 7)))
            };
            IEnumerable<SalaryType> salaryTypes = new List<SalaryType>() { new SalaryType() { Id = _validSalaryTypeId } };
            IEnumerable<Frequency> frequencies = new List<Frequency>() { new Frequency() { Id = _validFrequencyId, ApproxTimesPerYear = _approxTimesPerYear } };
            IEnumerable<TransactionType> transactionTypes = new List<TransactionType>() { new TransactionType() { Id = _validTransactionTypeId } };

            var repository = new Mock<ILedgerRepository>();
            // Setup for IncomeGeneratorRequestValidator
            repository.Setup(x => x.GetAllAsync<SalaryType>())
                .Returns(Task.FromResult(salaryTypes));
            repository.Setup(x => x.GetAllAsync<Frequency>())
                .Returns(Task.FromResult(frequencies));

            // Setup for RecurringTransactionRequestValidator
            repository.Setup(x => x.GetAllAsync<TransactionType>())
                .Returns(Task.FromResult(transactionTypes));
            _validator = new IncomeGeneratorRequestValidator(repository.Object, new RecurringTransactionRequestValidator(repository.Object));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task FailsForEmptyDescription(string description)
        {
            var request = CreateIncomeGeneratorRequest();
            request.Description = description;

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, "'Description' must not be empty.");
        }

        [Fact]
        public async Task FailsForLongDescription()
        {
            var description = new StringBuilder();
            for (var i = 0; i < 25; i++)
            {
                description.Append('a');
            }
            var request = CreateIncomeGeneratorRequest();
            request.Description = description.ToString();

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, "Description must not exceed 24 characters.");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task FailsForEmptySalaryTypeId(string id)
        {
            var request = CreateIncomeGeneratorRequest();
            request.SalaryTypeId = id;

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, "'Salary Type Id' must not be empty.");
        }

        [Fact]
        public async Task FailsForInvalidSalaryTypeId()
        {
            var request = CreateIncomeGeneratorRequest();
            request.SalaryTypeId = _invalidSalaryTypeId;

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, "Salary Type Id must be valid.");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task FailsForEmptyFrequencyId(string id)
        {
            var request = CreateIncomeGeneratorRequest();
            request.FrequencyId = id;

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, "'Frequency Id' must not be empty.");
        }

        [Fact]
        public async Task FailsForInvalidFrequencyId()
        {
            var request = CreateIncomeGeneratorRequest();
            request.FrequencyId = _invalidFrequencyId;

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, "Frequency Id must be valid.");
        }

        [Fact]
        public async Task FailsForNullRecurringTransactions()
        {
            var request = CreateIncomeGeneratorRequest();
            request.RecurringTransactions = null;

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, "'Recurring Transactions' must not be empty.");
        }

        // InlineData doesn't allow for typed arguments.
        [Fact]
        public async Task FailsForEmptyRecurringTransactions()
        {
            var request = CreateIncomeGeneratorRequest();
            request.RecurringTransactions = new List<RecurringTransactionRequest>();

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, "'Recurring Transactions' must not be empty.");
        }

        [Fact]
        public async Task FailsForInvalidRecurringTransaction()
        {
            var request = CreateIncomeGeneratorRequest();
            request.RecurringTransactions = new List<RecurringTransactionRequest>()
            {
                new RecurringTransactionRequest()
            };

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, "Recurring Transactions must be valid.");
        }

        [Fact]
        public async Task PassesValidRequest()
        {
            var request = CreateIncomeGeneratorRequest();

            var result = await _validator.ValidateAsync(request);
            Assert.True(result.IsValid);
        }

        private IncomeGeneratorRequest CreateIncomeGeneratorRequest()
        {
            return new IncomeGeneratorRequest()
            {
                Description = Guid.NewGuid().ToString().Substring(0, 24),
                SalaryTypeId = _validSalaryTypeId,
                FrequencyId = _validFrequencyId,
                RecurringTransactions = new List<RecurringTransactionRequest>() { _validRecurringTransactionRequest }
            };
        }
    }
}