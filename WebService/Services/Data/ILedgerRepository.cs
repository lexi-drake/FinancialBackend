using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebService
{
    public interface ILedgerRepository
    {
        Task<IEnumerable<LedgerEntry>> GetLedgerEntriesByUserIdAsync(string userId);
        Task<IEnumerable<LedgerEntry>> GetLedgerEntriesBetweenDatesAsync(DateTime start, DateTime end, string userId);
        Task<LedgerEntry> InsertLedgerEntryAsync(LedgerEntry entry);
        Task DeleteLedgerEntryAsync(string id, string userId);
        Task<IEnumerable<LedgerEntryCategory>> GetLedgerEntryCategoriesByCategoryAsync(string category);
        Task<IEnumerable<LedgerEntryCategory>> GetLedgerEntryCategoriesLikeAsync(string partial);
        Task<LedgerEntryCategory> InsertLedgerEntryCategoryAsync(LedgerEntryCategory category);
        Task UpdateLedgerEntryCategoryLastUsedAsync(string id, DateTime lastUsed);
        Task<IEnumerable<IncomeGenerator>> GetIncomeGeneratorsByIdAsync(string id);
        Task<IEnumerable<IncomeGenerator>> GetIncomeGeneratorsByUserIdAsync(string userId);
        Task<IncomeGenerator> InsertIncomeGeneratorAsync(IncomeGenerator generator);
        Task SetIncomeGeneratorRecurringTransactionsAsync(string id, IEnumerable<string> transactions);
        Task DeleteIncomeGeneratorAsync(string id, string userId);
        Task<IEnumerable<RecurringTransaction>> GetRecurringTransactionsByUserIdAsync(string userId);
        Task<RecurringTransaction> InsertRecurringTransactionAsync(RecurringTransaction transaction);
        Task DeleteRecurringTransactionAsync(string id, string userId);
        Task<IEnumerable<T>> GetAllAsync<T>() where T : AbstractLedgerItem;
        Task<T> InsertOneAsync<T>(T item) where T : AbstractLedgerItem;
    }
}