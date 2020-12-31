using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace financial_backend
{
    public interface ILedgerService
    {
        Task<IEnumerable<LedgerEntry>> GetAllLedgerEntriesAsync(string userId);
        Task<IEnumerable<LedgerEntry>> GetLedgerEntriesSinceDateAsync(DateTime startDate, string userId);
        Task<LedgerEntry> InsertLedgerEntry(LedgerEntryRequest entry, string userId);
        Task<IEnumerable<PurchaseCategory>> GetPurchaseCategoriesAsync();
    }
}