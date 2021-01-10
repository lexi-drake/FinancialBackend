using System;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace WebService
{
    public class LedgerService : ILedgerService
    {
        private ILogger<LedgerService> _logger;
        private ILedgerRepository _repo;

        public LedgerService(ILogger<LedgerService> logger, ILedgerRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public async Task<IEnumerable<LedgerEntryResponse>> GetLedgerEntriesByUserIdAsync(string userId)
        {
            var transactionTypes = await _repo.GetAllAsync<TransactionType>();
            return from entry in await _repo.GetLedgerEntriesByUserIdAsync(userId)
                   select LedgerEntryResponse.FromDBObject(entry, transactionTypes);
        }

        public async Task<IEnumerable<LedgerEntryResponse>> GetLedgerEntriesBetweenDatesAsync(string start, string end, string userId)
        {
            var startDate = FromMMDDYYYY(start);
            var endDate = FromMMDDYYYY(end);
            if (startDate == DateTime.MinValue || endDate == DateTime.MinValue)
            {
                return null;
            }

            var transactionTypes = await _repo.GetAllAsync<TransactionType>();
            return from entry in await _repo.GetLedgerEntriesBetweenDatesAsync(startDate, endDate, userId)
                   select LedgerEntryResponse.FromDBObject(entry, transactionTypes);
        }

        private DateTime FromMMDDYYYY(string date)
        {
            // This returns false in the case that date is null or empty.
            var success = DateTime.TryParseExact(date, "MMDDYYYY", null, DateTimeStyles.None, out var parsedDate);
            if (success)
            {
                return parsedDate;
            }
            return DateTime.MinValue;
        }

        public async Task<LedgerEntryResponse> AddLedgerEntryAsync(LedgerEntryRequest request, string userId)
        {
            var category = await GetOrInsertLedgerEntryCategoryAsync(request.Category);
            var entry = await _repo.InsertLedgerEntryAsync(new LedgerEntry()
            {
                UserId = userId,
                Category = category.Category,
                Description = request.Description,
                Amount = request.Amount,
                TransactionTypeId = request.TransactionTypeId,
                TransactionDate = request.TransactionDate,
                CreatedDate = DateTime.Now
            });

            var transactionTypes = await _repo.GetAllAsync<TransactionType>();
            return LedgerEntryResponse.FromDBObject(entry, transactionTypes);
        }

        public async Task<IEnumerable<LedgerEntryCategory>> GetLedgerEntryCategoriesLikeAsync(CategoryCompleteRequest request)
        {
            var regex = $"^.*{request.Partial}.*";  // The ^ at the beginning should let this request use the index
            return await _repo.GetLedgerEntryCategoriesLikeAsync(regex);
        }

        public async Task<IEnumerable<IncomeGeneratorResponse>> GetIncomeGeneratorsByUserIdAsync(string userId)
        {
            var recurringTransactions = await _repo.GetRecurringTransactionsByUserIdAsync(userId);
            var generators = await _repo.GetIncomeGeneratorsByUserIdAsync(userId);

            var responses = from generator in generators
                            select new IncomeGeneratorResponse()
                            {
                                Id = generator.Id,
                                Description = generator.Description,
                                SalaryTypeId = generator.SalaryTypeId,
                                FrequencyId = generator.FrequencyId,
                                RecurringTransactions = CompileRecurringTransactions(generator.RecurringTransactions, recurringTransactions)
                            };
            return responses;
        }

        public async Task<IncomeGeneratorResponse> AddIncomeGeneratorAsync(IncomeGeneratorRequest request, string userId)
        {
            // Validation ensures non-duplicate income generator

            // The transactions need to be inserted first, so that the is can be included
            // in the income generator. Also, LINQ doesn't allow 'await' in the select 
            // part of the query.
            var transactionIds = new List<string>();
            foreach (var transaction in request.RecurringTransactions)
            {
                transactionIds.Add((await AddRecurringTransactionAsync(transaction, userId)).Id);
            }

            var generator = await _repo.InsertIncomeGeneratorAsync(new IncomeGenerator()
            {
                UserId = userId,
                Description = request.Description,
                SalaryTypeId = request.SalaryTypeId,
                FrequencyId = request.FrequencyId,
                RecurringTransactions = transactionIds,
                CreatedDate = DateTime.Now
            });

            var recurringTransactions = await _repo.GetRecurringTransactionsByUserIdAsync(userId);
            return new IncomeGeneratorResponse()
            {
                Id = generator.Id,
                Description = generator.Description,
                SalaryTypeId = generator.SalaryTypeId,
                FrequencyId = generator.FrequencyId,
                RecurringTransactions = CompileRecurringTransactions(generator.RecurringTransactions, recurringTransactions)
            };
        }

        private IEnumerable<RecurringTransaction> CompileRecurringTransactions(IEnumerable<string> ids, IEnumerable<RecurringTransaction> recurringTransactions)
        {
            // This was making vscode all fucky when it was inline above.
            return from id in ids
                   from transaction in recurringTransactions
                   where transaction.Id == id
                   select transaction;
        }

        public async Task<IEnumerable<RecurringTransaction>> GetRecurringTransactionsByUserIdAsync(string userId)
        {
            return await _repo.GetRecurringTransactionsByUserIdAsync(userId);
        }

        public async Task<RecurringTransaction> AddRecurringTransactionAsync(RecurringTransactionRequest request, string userId)
        {
            var category = await GetOrInsertLedgerEntryCategoryAsync(request.Category);
            return await _repo.InsertRecurringTransactionAsync(new RecurringTransaction()
            {
                UserId = userId,
                Category = category.Category,
                Description = request.Description,
                Amount = request.Amount,
                FrequencyId = request.FrequencyId,
                TransactionTypeId = request.TransactionTypeId,
                CreatedDate = DateTime.Now
            });
        }

        private async Task<LedgerEntryCategory> GetOrInsertLedgerEntryCategoryAsync(string category)
        {
            var categories = from c in await _repo.GetLedgerEntryCategoriesByCategoryAsync(category)
                             where c.Category.Equals(category, StringComparison.InvariantCultureIgnoreCase)
                             select c;

            if (categories.Any())
            {
                // If we already have this category in the database, use the existing id
                return categories.First();
            }
            else
            {
                // If we don't have this category in the database, insert a new category
                // and use its id
                var ledgerEntryCategory = await _repo.InsertLedgerEntryCategoryAsync(new LedgerEntryCategory()
                {
                    Category = category,
                    CreatedDate = DateTime.Now
                });
                return ledgerEntryCategory;
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>() where T : AbstractLedgerItem
        {
            return await _repo.GetAllAsync<T>();
        }
    }
}