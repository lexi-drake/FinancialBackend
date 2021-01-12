using System.Threading.Tasks;
using System.Collections.Generic;

namespace WebService
{
    public interface ILedgerService
    {
        Task<IEnumerable<LedgerEntryResponse>> GetLedgerEntriesByUserIdAsync(string userId);
        Task<IEnumerable<LedgerEntryResponse>> GetLedgerEntriesBetweenDatesAsync(string start, string end, string userId);
        Task<LedgerEntryResponse> AddLedgerEntryAsync(LedgerEntryRequest request, string userId);
        Task DeleteLedgerEntryAsync(string id, string userId);
        Task<IEnumerable<LedgerEntryCategory>> GetLedgerEntryCategoriesLikeAsync(CategoryCompleteRequest request);
        Task<IEnumerable<IncomeGeneratorResponse>> GetIncomeGeneratorsByUserIdAsync(string userId);
        Task<IncomeGeneratorResponse> AddIncomeGeneratorAsync(IncomeGeneratorRequest request, string userId);
        Task<IEnumerable<RecurringTransaction>> GetRecurringTransactionsByUserIdAsync(string userId);
        Task<RecurringTransaction> AddRecurringTransactionAsync(RecurringTransactionRequest request, string userId);
        Task<IEnumerable<T>> GetAllAsync<T>() where T : AbstractLedgerItem;
    }
}