using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WebService
{
    public interface ILedgerService
    {
        Task<IEnumerable<LedgerEntry>> GetLedgerEntriesByUserIdAsync(string userId);
        Task<IEnumerable<LedgerEntry>> GetLedgerEntriesBetweenDatesAsync(DateTime startDate, DateTime endDate, string userId);
        Task<LedgerEntry> InsertLedgerEntryAsync(LedgerEntryRequest request, string userId);
        Task<IEnumerable<LedgerEntryCategory>> GetLedgerEntryCategoriesAsync();
        Task<IEnumerable<IncomeGenerator>> GetIncomeGeneratorsByUserIdAsync(string userId);
        Task<IncomeGenerator> InsertIncomeGeneratorAsync(IncomeGeneratorRequest request, string userId);
        Task<IEnumerable<RecurringTransaction>> GetRecurringTransactionsByUserIdAsync(string userId);
        Task<RecurringTransaction> InsertRecurringTransactionAsync(RecurringTransactionRequest request, string userId);
    }
}