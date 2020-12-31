using System.Collections.Generic;
using System.Threading.Tasks;

namespace financial_backend
{
    public interface ILedgerRepository
    {
        Task<IEnumerable<LedgerEntry>> GetLedgerEntriesForUserAsync(string userId);
        Task<LedgerEntry> InsertLedgerEntryAsync(LedgerEntry entry);
        Task<IEnumerable<PurchaseCategory>> GetPurchaseCategoriesAsync();
        Task<PurchaseCategory> InsertPurchaseCategoryAsync(PurchaseCategory category);
    }
}