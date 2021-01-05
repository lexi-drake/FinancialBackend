using System;
using System.Linq;
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

        public async Task<IEnumerable<LedgerEntry>> GetLedgerEntriesByUserIdAsync(string userId)
        {
            return await _repo.GetLedgerEntriesByUserIdAsync(userId);
        }

        public async Task<IEnumerable<LedgerEntry>> GetLedgerEntriesBetweenDatesAsync(DateTime startDate, DateTime endDate, string userId)
        {
            return from ledger in await _repo.GetLedgerEntriesByUserIdAsync(userId)
                   where ledger.TransactionDate >= startDate && ledger.TransactionDate <= endDate
                   select ledger;
        }

        public async Task<LedgerEntry> AddLedgerEntryAsync(LedgerEntryRequest request, string userId)
        {
            var categoryId = await GetOrInsertLedgerEntryCategoryAsync(request.Category);
            return await _repo.InsertLedgerEntryAsync(new LedgerEntry()
            {
                UserId = userId,
                CategoryId = categoryId,
                Description = request.Description,
                Amount = request.Amount,
                TransactionTypeId = request.TransactionTypeId,
                TransactionDate = request.TransactionDate,
                CreatedDate = DateTime.Now
            });
        }

        public async Task<IEnumerable<LedgerEntryCategory>> GetLedgerEntryCategoriesLikeAsync(CategoryCompleteRequest request)
        {
            var regex = $".*{request.Partial}.*";
            return await _repo.GetLedgerEntryCategoriesLikeAsync(regex);
        }

        public async Task<IEnumerable<IncomeGenerator>> GetIncomeGeneratorsByUserIdAsync(string userId)
        {
            return await _repo.GetIncomeGeneratorsByUserIdAsync(userId);
        }

        public async Task<IncomeGenerator> AddIncomeGeneratorAsync(IncomeGeneratorRequest request, string userId)
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

            return await _repo.InsertIncomeGeneratorAsync(new IncomeGenerator()
            {
                UserId = userId,
                Description = request.Description,
                SalaryTypeId = request.SalaryTypeId,
                FrequencyId = request.FrequencyId,
                RecurringTransactions = transactionIds,
                CreatedDate = DateTime.Now
            });
        }

        public async Task<IEnumerable<RecurringTransaction>> GetRecurringTransactionsByUserIdAsync(string userId)
        {
            return await _repo.GetRecurringTransactionsByUserIdAsync(userId);
        }

        public async Task<RecurringTransaction> AddRecurringTransactionAsync(RecurringTransactionRequest request, string userId)
        {
            var categoryId = await GetOrInsertLedgerEntryCategoryAsync(request.Category);
            return await _repo.InsertRecurringTransactionAsync(new RecurringTransaction()
            {
                UserId = userId,
                CategoryId = categoryId,
                Description = request.Description,
                Amount = request.Amount,
                FrequencyId = request.FrequencyId,
                TransactionTypeId = request.TransactionTypeId,
                CreatedDate = DateTime.Now
            });
        }

        private async Task<string> GetOrInsertLedgerEntryCategoryAsync(string category)
        {
            var categoryIds = from c in await _repo.GetLedgerEntryCategoriesByCategoryAsync(category)
                              where c.Category.Equals(category, StringComparison.InvariantCultureIgnoreCase)
                              select c.Id;

            if (categoryIds.Any())
            {
                // If we already have this category in the database, use the existing id
                return categoryIds.First();
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
                return ledgerEntryCategory.Id;
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>() where T : AbstractLedgerItem
        {
            return await _repo.GetAllAsync<T>();
        }
    }
}