using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using Moq;
using WebService;

namespace Tests
{
    public class RecurringTransactionRequestValidatorShould
    {
        private string _validFrequencyId = Guid.NewGuid().ToString();
        private string _validTransactionTypeId = Guid.NewGuid().ToString();

        private RecurringTransactionRequestValidator _validator;

        public RecurringTransactionRequestValidatorShould()
        {
            IEnumerable<TransactionType> transactionTypes = new List<TransactionType>() { new TransactionType() { Id = _validTransactionTypeId } };
            IEnumerable<Frequency> frequencies = new List<Frequency>() { new Frequency() { Id = _validFrequencyId, ApproxTimesPerYear = new Random().Next(1, 52) } };

            var repo = new Mock<ILedgerRepository>();
            repo.Setup(x => x.GetAllAsync<TransactionType>())
                .Returns(Task.FromResult(transactionTypes));
            repo.Setup(x => x.GetAllAsync<Frequency>())
                .Returns(Task.FromResult(frequencies));

            _validator = new RecurringTransactionRequestValidator(repo.Object);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task FailsForEmptyCategory(string category)
        {
            var request = CreateRecurringTransactionRequest();
            request.Category = category;

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, "'Category' must not be empty.");
        }

        [Fact]
        public async Task FailsForLongCategory()
        {
            var request = CreateRecurringTransactionRequest();
            request.Category = Guid.NewGuid().ToString();

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, "Category must not exceed 24 characters.");
        }

        [Fact]
        public async Task FailsForLongDescription()
        {
            var request = CreateRecurringTransactionRequest();
            request.Description = Guid.NewGuid().ToString();

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, "Description must not exceed 24 characters.");
        }

        [Fact]
        public async Task FailsForZeroAmount()
        {
            var request = CreateRecurringTransactionRequest();
            request.Amount = 0F;

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, "'Amount' must be greater than '0'.");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task FailsForEmptyFrequencyId(string id)
        {
            var request = CreateRecurringTransactionRequest();
            request.FrequencyId = id;

            var result = await _validator.ValidateAsync(request);
            Assert.False(result.IsValid);
            Assert.Equal(2, result.Errors.Count());

            var emptyError = from error in result.Errors
                             where error.ErrorMessage == "'Frequency Id' must not be empty."
                             select error;
            Assert.NotEmpty(emptyError);

            var dateError = from error in result.Errors
                            where error.ErrorMessage == "Last Triggered is outside of the reasonable window."
                            select error;
            Assert.NotEmpty(dateError);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task FailsForEmptyTransactionTypeId(string id)
        {
            var request = CreateRecurringTransactionRequest();
            request.TransactionTypeId = id;

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, "'Transaction Type Id' must not be empty.");
        }

        [Fact]
        public async Task FailsForBadTransactionTypeId()
        {
            var request = CreateRecurringTransactionRequest();
            request.TransactionTypeId = Guid.NewGuid().ToString();

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, "Transaction Type Id must be valid.");
        }

        [Fact]
        public async Task FailsForBadFrequencyId()
        {
            var request = CreateRecurringTransactionRequest();
            request.FrequencyId = Guid.NewGuid().ToString();

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, "Last Triggered is outside of the reasonable window.");
        }

        [Fact]
        public async Task FailsForOldLastTriggered()
        {
            var request = CreateRecurringTransactionRequest();
            request.LastTriggered = DateTime.Now.AddMonths(-13);

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, "Last Triggered is outside of the reasonable window.");
        }

        [Fact]
        public async Task FailsForFutureLastTriggered()
        {
            var request = CreateRecurringTransactionRequest();
            request.LastTriggered = DateTime.Now.AddMinutes(1);

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, "Last Triggered is outside of the reasonable window.");
        }

        private RecurringTransactionRequest CreateRecurringTransactionRequest() =>
            new RecurringTransactionRequest()
            {
                Category = Guid.NewGuid().ToString().Substring(0, 24),
                Amount = new Random().Next(1, 1000),
                FrequencyId = _validFrequencyId,
                TransactionTypeId = _validTransactionTypeId,
                LastTriggered = DateTime.Now.AddDays(-(new Random().Next(1, 7)))
            };
    }
}