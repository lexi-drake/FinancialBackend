using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace financial_backend
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

        public async Task<IEnumerable<LedgerEntry>> GetAllLedgerEntriesAsync(string userId)
        {
            return await _repo.GetLedgerEntriesForUserAsync(userId);
        }

        public async Task<IEnumerable<LedgerEntry>> GetLedgerEntriesSinceDateAsync(DateTime startDate, string userId)
        {
            return from ledger in await _repo.GetLedgerEntriesForUserAsync(userId)
                   where ledger.PurchaseDate >= startDate
                   select ledger;
        }

        public async Task<LedgerEntry> InsertLedgerEntry(LedgerEntryRequest request, string userId)
        {
            var categoryIds = from category in await _repo.GetPurchaseCategoriesAsync()
                              where category.Name.Equals(request.PurchaseCategory, StringComparison.InvariantCultureIgnoreCase)
                              select category.Id;

            string categoryId = null;
            if (categoryIds.Any())
            {
                // If we already have this category in the database, use the existing id
                categoryId = categoryIds.First();
            }
            else
            {
                // If we don't have this category in the database, insert a new category
                // and use its id
                var category = await _repo.InsertPurchaseCategoryAsync(new PurchaseCategory()
                {
                    Name = request.PurchaseCategory,
                    CreatedDate = DateTime.Now
                });
                categoryId = category.Id;
            }

            return await _repo.InsertLedgerEntryAsync(new LedgerEntry()
            {
                UserId = userId,
                PurchaseCategoryId = categoryId,
                Vendor = request.Vendor,
                Amount = request.Amount,
                PurchaseDate = request.PurchaseDate,
                CreatedDate = DateTime.Now
            });
        }

        public async Task<IEnumerable<PurchaseCategory>> GetPurchaseCategoriesAsync()
        {
            return await _repo.GetPurchaseCategoriesAsync();
        }
    }
}