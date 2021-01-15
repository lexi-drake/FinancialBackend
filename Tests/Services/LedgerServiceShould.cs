using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;
using WebService;

namespace Tests
{
    public class LedgerServiceShould
    {
        private string _userId = Guid.NewGuid().ToString();
        private string _categoryInDb = Guid.NewGuid().ToString();
        private string _categoryNotInDb = Guid.NewGuid().ToString();
        private TransactionType _transactionType;
        private LedgerEntry _ledgerEntry;
        private RecurringTransaction _recurringTransaction;
        private IncomeGenerator _generator;
        private Mock<ILedgerRepository> _repo;

        private ILedgerService _service;

        public LedgerServiceShould()
        {
            _transactionType = new TransactionType()
            {
                Id = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                CreatedBy = Guid.NewGuid().ToString(),
                CreatedDate = DateTime.Now
            };
            _ledgerEntry = new LedgerEntry()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = _userId,
                Category = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                Amount = new Decimal(new Random().NextDouble()),
                TransactionTypeId = _transactionType.Id,
                RecurringTransactionId = Guid.NewGuid().ToString(),
                TransactionDate = DateTime.Now,
                CreatedDate = DateTime.Now,
            };
            _recurringTransaction = new RecurringTransaction()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = _userId,
                Category = _categoryInDb,
                Description = Guid.NewGuid().ToString(),
                Amount = new decimal(new Random().NextDouble()),
                FrequencyId = Guid.NewGuid().ToString(),
                TransactionTypeId = _transactionType.Id,
                LastTriggered = DateTime.Now,
                LastExecuted = DateTime.Now,
                CreatedDate = DateTime.Now
            };
            _generator = new IncomeGenerator()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = _userId,
                Description = Guid.NewGuid().ToString(),
                SalaryTypeId = Guid.NewGuid().ToString(),
                FrequencyId = Guid.NewGuid().ToString(),
                RecurringTransactions = new List<string> { _recurringTransaction.Id },
                CreatedDate = DateTime.Now
            };
            var category = new LedgerEntryCategory()
            {
                Id = Guid.NewGuid().ToString(),
                Category = _categoryInDb,
                LastUsed = DateTime.Now,
                CreatedDate = DateTime.Now
            };

            IEnumerable<TransactionType> transactionTypes = new List<TransactionType>() { _transactionType };
            IEnumerable<LedgerEntry> ledgerEntries = new List<LedgerEntry>() { _ledgerEntry };
            IEnumerable<LedgerEntryCategory> categories = new List<LedgerEntryCategory>() { category };
            IEnumerable<LedgerEntryCategory> noCategories = new List<LedgerEntryCategory>();
            IEnumerable<RecurringTransaction> recurringTransactions = new List<RecurringTransaction>() { _recurringTransaction };
            IEnumerable<IncomeGenerator> incomeGenerators = new List<IncomeGenerator>() { _generator };

            _repo = new Mock<ILedgerRepository>();
            _repo.Setup(x => x.GetAllAsync<TransactionType>())
                .Returns(Task.FromResult(transactionTypes));
            _repo.Setup(x => x.GetLedgerEntriesByUserIdAsync(_userId))
                .Returns(Task.FromResult(ledgerEntries));
            _repo.Setup(x => x.GetLedgerEntriesBetweenDatesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), _userId))
                .Returns(Task.FromResult(ledgerEntries));
            _repo.Setup(x => x.GetLedgerEntryCategoriesByCategoryAsync(_categoryInDb))
                .Returns(Task.FromResult(categories));
            _repo.Setup(x => x.GetLedgerEntryCategoriesByCategoryAsync(_categoryNotInDb))
                .Returns(Task.FromResult(noCategories));
            _repo.Setup(x => x.InsertLedgerEntryAsync(It.IsAny<LedgerEntry>()))
                .Returns<LedgerEntry>(entry =>
                {
                    entry.Id = Guid.NewGuid().ToString();
                    return Task.FromResult(entry);
                });
            _repo.Setup(x => x.GetLedgerEntryCategoriesLikeAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(categories));
            _repo.Setup(x => x.GetRecurringTransactionsByUserIdAsync(_userId))
                .Returns(Task.FromResult(recurringTransactions));
            _repo.Setup(x => x.GetIncomeGeneratorsByUserIdAsync(_userId))
                .Returns(Task.FromResult(incomeGenerators));
            _repo.Setup(x => x.InsertIncomeGeneratorAsync(It.IsAny<IncomeGenerator>()))
                .Returns<IncomeGenerator>(generator =>
                {
                    generator.Id = Guid.NewGuid().ToString();
                    return Task.FromResult(generator);
                });
            _repo.Setup(x => x.InsertRecurringTransactionAsync(It.IsAny<RecurringTransaction>()))
                .Returns<RecurringTransaction>(transaction =>
                {
                    // This is a shortcut for returning a recurring transaction with an id
                    // that will match in CompileRecurringTransactionsc
                    return Task.FromResult(_recurringTransaction);
                });
            _repo.Setup(x => x.GetIncomeGeneratorsByIdAsync(_generator.Id))
                .Returns(Task.FromResult(incomeGenerators));

            _service = new LedgerService(new Mock<ILogger<LedgerService>>().Object, _repo.Object);
        }

        [Fact]
        public async Task GetsOnlyLedgerEntriesByUserId()
        {
            var list = await _service.GetLedgerEntriesByUserIdAsync(_userId);
            Assert.NotNull(list);
            Assert.Single(list);

            var response = list.First();
            Assert.Equal(_transactionType.Description, response.TransactionType);
        }

        [Fact]
        public async Task GetsLedgerEntriesBetweenDates()
        {
            var list = await _service.GetLedgerEntriesBetweenDatesAsync("01012021", "02012021", _userId);
            Assert.NotNull(list);
            Assert.Single(list);

            var response = list.First();
            Assert.Equal(_transactionType.Description, response.TransactionType);
        }

        [Fact]
        public async Task AddsLedgerEntryAndInsertsNewCategory()
        {
            var request = CreateLedgerEntryRequest();
            request.Category = _categoryNotInDb;

            var ledgerEntry = await _service.AddLedgerEntryAsync(request, _userId);
            Assert.NotNull(ledgerEntry);
            Assert.Equal(_transactionType.Description, ledgerEntry.TransactionType);

            _repo.Verify(x => x.InsertLedgerEntryCategoryAsync(It.IsAny<LedgerEntryCategory>()), Times.Once());
            _repo.Verify(x => x.UpdateLedgerEntryCategoryLastUsedAsync(It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never());
        }

        [Fact]
        public async Task AddsLedgerEntryAndUpdatesCategory()
        {
            var request = CreateLedgerEntryRequest();
            request.Category = _categoryInDb;

            var ledgerEntry = await _service.AddLedgerEntryAsync(request, _userId);
            Assert.NotNull(ledgerEntry);
            Assert.Equal(_transactionType.Description, ledgerEntry.TransactionType);

            _repo.Verify(x => x.InsertLedgerEntryCategoryAsync(It.IsAny<LedgerEntryCategory>()), Times.Never());
            _repo.Verify(x => x.UpdateLedgerEntryCategoryLastUsedAsync(It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once());
        }

        [Fact]
        public async Task DeletesLedgerEntry()
        {
            await _service.DeleteLedgerEntryAsync(Guid.NewGuid().ToString(), _userId);

            _repo.Verify(x => x.DeleteLedgerEntryAsync(It.IsAny<string>(), _userId), Times.Once());
        }

        [Fact]
        public async Task GetsLedgerEntryCategoriesLike()
        {
            var request = new CategoryCompleteRequest()
            {
                Partial = Guid.NewGuid().ToString()
            };
            var regex = $"^.*{request.Partial}.*";

            var list = await _service.GetLedgerEntryCategoriesLikeAsync(request);
            Assert.NotNull(list);
            Assert.Single(list);

            _repo.Verify(x => x.GetLedgerEntryCategoriesLikeAsync(regex), Times.Once());
        }

        [Fact]
        public async Task ReturnsIncomeGeneratorsByUserId()
        {
            var list = await _service.GetIncomeGeneratorsByUserIdAsync(_userId);
            Assert.NotNull(list);
            Assert.Single(list);

            var generator = list.First();
            Assert.NotNull(generator.RecurringTransactions);
            Assert.Single(generator.RecurringTransactions);

            var transaction = generator.RecurringTransactions.First();
            Assert.Equal(_transactionType.Description, transaction.TransactionType);
        }

        [Fact]
        public async Task AddIncomeGenerator()
        {
            var request = new IncomeGeneratorRequest()
            {
                Description = Guid.NewGuid().ToString(),
                SalaryTypeId = Guid.NewGuid().ToString(),
                FrequencyId = Guid.NewGuid().ToString(),
                RecurringTransactions = new List<RecurringTransactionRequest>()
                {
                    new RecurringTransactionRequest()
                    {
                        Category = _categoryInDb,
                        Description = Guid.NewGuid().ToString(),
                        Amount = (float)new Random().NextDouble(),
                        FrequencyId = Guid.NewGuid().ToString(),
                        TransactionTypeId = Guid.NewGuid().ToString(),
                        LastTriggered = DateTime.Now
                    }
                }
            };

            var generator = await _service.AddIncomeGeneratorAsync(request, _userId);
            Assert.NotNull(generator);
            Assert.NotNull(generator.RecurringTransactions);
            Assert.Single(generator.RecurringTransactions);

            var transaction = generator.RecurringTransactions.First();
            Assert.Equal(_transactionType.Description, transaction.TransactionType);

            _repo.Verify(x => x.InsertLedgerEntryAsync(It.IsAny<LedgerEntry>()), Times.Once());
        }

        [Fact]
        public async Task DeletesIncomeGeneratorAndRecurringTransactions()
        {
            await _service.DeleteIncomeGeneratorAsync(_generator.Id, _userId);

            _repo.Verify(x => x.DeleteRecurringTransactionAsync(It.IsAny<string>(), _userId), Times.Once());
            _repo.Verify(x => x.DeleteIncomeGeneratorAsync(_generator.Id, _userId), Times.Once());
        }

        [Fact]
        public async Task GetsRecurringTransactionsByUserId()
        {
            var list = await _service.GetRecurringTransactionsByUserIdAsync(_userId);
            Assert.NotNull(list);
            Assert.Single(list);

            var transaction = list.First();
            Assert.Equal(_transactionType.Description, transaction.TransactionType);
        }

        [Fact]
        public async Task DeletesRecurringTransaction()
        {
            await _service.DeleteRecurringTransactionAsync(_recurringTransaction.Id, _userId);

            _repo.Verify(x => x.DeleteRecurringTransactionAsync(_recurringTransaction.Id, _userId), Times.Once());
        }

        private LedgerEntryRequest CreateLedgerEntryRequest()
        {
            return new LedgerEntryRequest()
            {
                Category = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString(),
                Amount = (float)new Random().NextDouble(),
                TransactionTypeId = _transactionType.Id,
                RecurringTransactionId = Guid.NewGuid().ToString(),
                TransactionDate = DateTime.Now
            };
        }
    }
}