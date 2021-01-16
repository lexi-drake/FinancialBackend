using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using Moq;
using WebService;

namespace Tests
{
    public class LedgerEntryRequestValidatorShould
    {
        private TransactionType _transactionType;

        private LedgerEntryRequestValidator _validator;

        public LedgerEntryRequestValidatorShould()
        {
            _transactionType = new TransactionType()
            {
                Id = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString()
            };

            IEnumerable<TransactionType> transactionTypes = new List<TransactionType>() { _transactionType };

            var repo = new Mock<ILedgerRepository>();
            repo.Setup(x => x.GetAllAsync<TransactionType>())
                .Returns(Task.FromResult(transactionTypes));

            _validator = new LedgerEntryRequestValidator(repo.Object);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task FailForEmptyCategory(string category)
        {
            var request = CreateLedgerEntryRequest();
            request.Category = category;

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, "'Category' must not be empty.");
        }

        [Fact]
        public async Task FailForLongDescription()
        {
            var description = new StringBuilder();
            for (var i = 0; i < 25; i++)
            {
                description.Append('a');
            }

            var request = CreateLedgerEntryRequest();
            request.Description = description.ToString();

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, "Description must not exceed 24 characters.");
        }

        [Fact]
        public async Task FailForZeroAmount()
        {
            var request = CreateLedgerEntryRequest();
            request.Amount = 0F;

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, "'Amount' must be greater than '0'.");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task FailsForEmptyTransactionTypeId(string id)
        {
            var request = CreateLedgerEntryRequest();
            request.TransactionTypeId = id;

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, "'Transaction Type Id' must not be empty.");
        }

        [Fact]
        public async Task FailsForBadTransactionTypeId()
        {
            var request = CreateLedgerEntryRequest();
            request.TransactionTypeId = Guid.NewGuid().ToString();

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, "Transaction Type Id must be valid.");
        }

        [Fact]
        public async Task FailsForFutureTransactionDate()
        {
            var request = CreateLedgerEntryRequest();
            request.TransactionDate = DateTime.Now.AddMinutes(1);

            var result = await _validator.ValidateAsync(request);
            AssertHelper.FailsWithMessage(result, $"'Transaction Date' must be less than or equal to '{DateTime.Now.ToString()}'.");
        }

        [Fact]
        public async Task PassesValidRequest()
        {
            var result = await _validator.ValidateAsync(CreateLedgerEntryRequest());
            Assert.True(result.IsValid);
        }

        private LedgerEntryRequest CreateLedgerEntryRequest() =>
            new LedgerEntryRequest()
            {
                Category = Guid.NewGuid().ToString(),
                Amount = (float)new Random().NextDouble() + 1,
                TransactionTypeId = _transactionType.Id,
                TransactionDate = DateTime.Now.AddDays(-1)
            };
    }
}