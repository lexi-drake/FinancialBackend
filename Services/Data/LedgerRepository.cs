using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace financial_backend
{
    public class LedgerRepository : ILedgerRepository
    {
        public async Task<IEnumerable<LedgerEntry>> GetLedgerEntriesForUserAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<LedgerEntry> InsertLedgerEntryAsync(LedgerEntry entry)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<PurchaseCategory>> GetPurchaseCategoriesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<PurchaseCategory> InsertPurchaseCategoryAsync(PurchaseCategory category)
        {
            throw new NotImplementedException();
        }
    }
}