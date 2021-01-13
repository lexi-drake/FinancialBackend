using System;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using Moq;
using FluentValidation;
using WebService;

namespace Tests
{
    public class IncomeGeneratorRequestValidatorShould : IClassFixture<IncomeGeneratorReqeustValidatorTestFixture>
    {
        private IncomeGeneratorReqeustValidatorTestFixture _fixture;
        private IncomeGeneratorRequestValidator _validator;

        public IncomeGeneratorRequestValidatorShould(IncomeGeneratorReqeustValidatorTestFixture fixture)
        {
            _fixture = fixture;
            _validator = new IncomeGeneratorRequestValidator(_fixture.Repository, new RecurringTransactionRequestValidator(_fixture.Repository));
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
            request.SalaryTypeId = _fixture.InvalidSalaryTypeId;

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
            request.FrequencyId = _fixture.InvalidFrequencyId;

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
                SalaryTypeId = _fixture.ValidSalaryTypeId,
                FrequencyId = _fixture.ValidFrequencyId,
                RecurringTransactions = new List<RecurringTransactionRequest>() { _fixture.ValidRecurringTransactionRequest }
            };
        }
    }

    public class IncomeGeneratorReqeustValidatorTestFixture
    {
        public string ValidSalaryTypeId { get; set; } = Guid.NewGuid().ToString();
        public string InvalidSalaryTypeId { get; set; } = Guid.NewGuid().ToString();
        public string ValidFrequencyId { get; set; } = Guid.NewGuid().ToString();
        public string InvalidFrequencyId { get; set; } = Guid.NewGuid().ToString();
        public int ApproxTimesPerYear { get; set; } = new Random().Next(1, 52);
        public string ValidTransactionTypeId { get; set; } = Guid.NewGuid().ToString();
        public string InvalidTransactionTypeId { get; set; } = Guid.NewGuid().ToString();
        public RecurringTransactionRequest ValidRecurringTransactionRequest { get; set; }

        public ILedgerRepository Repository { get; set; }

        public IncomeGeneratorReqeustValidatorTestFixture()
        {
            ValidRecurringTransactionRequest = new RecurringTransactionRequest()
            {
                Category = Guid.NewGuid().ToString().Substring(0, 24),
                Description = Guid.NewGuid().ToString().Substring(0, 24),
                Amount = new Random().Next(1, 1000),
                FrequencyId = ValidFrequencyId,
                TransactionTypeId = ValidTransactionTypeId,
                LastTriggered = DateTime.Now.AddDays(-(new Random().Next(1, 7)))
            };
            IEnumerable<SalaryType> salaryTypes = new List<SalaryType>() { new SalaryType() { Id = ValidSalaryTypeId } };
            IEnumerable<Frequency> frequencies = new List<Frequency>() { new Frequency() { Id = ValidFrequencyId, ApproxTimesPerYear = ApproxTimesPerYear } };
            IEnumerable<TransactionType> transactionTypes = new List<TransactionType>() { new TransactionType() { Id = ValidTransactionTypeId } };

            var repository = new Mock<ILedgerRepository>();
            // Setup for IncomeGeneratorRequestValidator
            repository.Setup(x => x.GetAllAsync<SalaryType>())
                .Returns(Task.FromResult(salaryTypes));
            repository.Setup(x => x.GetAllAsync<Frequency>())
                .Returns(Task.FromResult(frequencies));

            // Setup for RecurringTransactionRequestValidator
            repository.Setup(x => x.GetAllAsync<TransactionType>())
                .Returns(Task.FromResult(transactionTypes));


            Repository = repository.Object;
        }
    }
}