using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WebService
{
    public interface ILedgerService
    {
        Task<IEnumerable<LedgerEntry>> GetLedgerEntriesByUserIdAsync(string userId);
        Task<IEnumerable<LedgerEntry>> GetLedgerEntriesBetweenDatesAsync(DateTime startDate, DateTime endDate, string userId);
        Task<LedgerEntry> AddLedgerEntryAsync(LedgerEntryRequest request, string userId);
        Task<IEnumerable<LedgerEntryCategory>> GetLedgerEntryCategoriesLikeAsync(CategoryCompleteRequest request);
        Task<IEnumerable<IncomeGenerator>> GetIncomeGeneratorsByUserIdAsync(string userId);
        Task<IncomeGenerator> AddIncomeGeneratorAsync(IncomeGeneratorRequest request, string userId);
        Task<IEnumerable<RecurringTransaction>> GetRecurringTransactionsByUserIdAsync(string userId);
        Task<RecurringTransaction> AddRecurringTransactionAsync(RecurringTransactionRequest request, string userId);
        Task<IEnumerable<T>> GetAllAsync<T>() where T : AbstractLedgerItem;
    }
}