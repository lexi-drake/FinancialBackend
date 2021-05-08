using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver;

namespace WebService
{
    public class LedgerRepository : ILedgerRepository
    {
        private IMemoryCache _cache;
        private IMongoDatabase _db;

        public LedgerRepository(IMemoryCache cache, string connectionString, string database)
        {
            _cache = cache;
            _db = new MongoClient(connectionString).GetDatabase(database);
        }

        public async Task<IEnumerable<LedgerEntry>> GetLedgerEntriesBetweenDatesAsync(DateTime start, DateTime end, string userId)
        {
            var builder = Builders<LedgerEntry>.Filter;
            var userIdFilter = builder.Eq(x => x.UserId, userId);
            var startFilter = builder.Gte(x => x.TransactionDate, start);
            var endFilter = builder.Lte(x => x.TransactionDate, end);

            return await _db.FindWithFilterAsync(userIdFilter & startFilter & endFilter);
        }

        public async Task<LedgerEntry> InsertLedgerEntryAsync(LedgerEntry entry)
        {
            await _db.GetCollection<LedgerEntry>().InsertOneAsync(entry);
            return entry;
        }

        public async Task DeleteLedgerEntryAsync(string id, string userId)
        {
            var builder = Builders<LedgerEntry>.Filter;
            var idFilter = builder.Eq(x => x.Id, id);
            var userIdFilter = builder.Eq(x => x.UserId, userId);

            await _db.GetCollection<LedgerEntry>().DeleteOneAsync(idFilter & userIdFilter);
        }

        public async Task<IEnumerable<LedgerEntryCategory>> GetLedgerEntryCategoriesLikeAsync(string regex)
        {
            var filter = Builders<LedgerEntryCategory>.Filter.Regex(x => x.Category, regex);
            return await _db.FindWithFilterAsync(filter);
        }

        public async Task InsertOrUpdateCategoryAsync(string category)
        {
            var categories = from c in await GetLedgerEntryCategoriesByCategoryAsync(category)
                             where c.Category.Equals(category, StringComparison.InvariantCultureIgnoreCase)
                             select c;

            if (categories.Any())
            {
                // If we already have this category in the database, use the existing id.
                await UpdateLedgerEntryCategoryLastUsedAsync(categories.First().Id, DateTime.Now);
                return;
            }
            // If we don't have this category in the database, insert a new category
            // and use its id
            await InsertLedgerEntryCategoryAsync(new LedgerEntryCategory()
            {
                Category = category,
                CreatedDate = DateTime.Now,
                LastUsed = DateTime.Now
            });
        }

        private async Task<IEnumerable<LedgerEntryCategory>> GetLedgerEntryCategoriesByCategoryAsync(string category) =>
            await _db.FindWithFilterAsync(Builders<LedgerEntryCategory>.Filter.Eq(x => x.Category, category));

        private async Task InsertLedgerEntryCategoryAsync(LedgerEntryCategory category) =>
            await _db.GetCollection<LedgerEntryCategory>().InsertOneAsync(category);


        private async Task UpdateLedgerEntryCategoryLastUsedAsync(string id, DateTime lastUsed) =>
            await _db.GetCollection<LedgerEntryCategory>().UpdateOneAsync(
                Builders<LedgerEntryCategory>.Filter.Eq(x => x.Id, id),
                Builders<LedgerEntryCategory>.Update.Set(x => x.LastUsed, lastUsed));

        public async Task<IEnumerable<IncomeGenerator>> GetIncomeGeneratorsByIdAsync(string id)
        {
            var filter = Builders<IncomeGenerator>.Filter.Eq(x => x.Id, id);
            return await _db.FindWithFilterAsync(filter);
        }

        public async Task<IEnumerable<IncomeGenerator>> GetIncomeGeneratorsByUserIdAsync(string userId)
        {
            var filter = Builders<IncomeGenerator>.Filter.Eq(x => x.UserId, userId);
            return await _db.FindWithFilterAsync(filter);
        }

        public async Task<IncomeGenerator> InsertIncomeGeneratorAsync(IncomeGenerator generator)
        {
            await _db.GetCollection<IncomeGenerator>().InsertOneAsync(generator);
            return generator;
        }

        public async Task SetIncomeGeneratorRecurringTransactionsAsync(string id, IEnumerable<string> transactions)
        {
            var filter = Builders<IncomeGenerator>.Filter.Eq(x => x.Id, id);
            var update = Builders<IncomeGenerator>.Update.Set(x => x.RecurringTransactions, transactions);

            await _db.GetCollection<IncomeGenerator>().UpdateOneAsync(filter, update);
        }

        public async Task DeleteIncomeGeneratorAsync(string id, string userId)
        {
            var builder = Builders<IncomeGenerator>.Filter;
            var idFilter = builder.Eq(x => x.Id, id);
            var userIdFilter = builder.Eq(x => x.UserId, userId);

            await _db.GetCollection<IncomeGenerator>().DeleteOneAsync(idFilter & userIdFilter);
        }

        public async Task<IEnumerable<RecurringTransaction>> GetRecurringTransactionsByUserIdAsync(string userId)
        {
            var filter = Builders<RecurringTransaction>.Filter.Eq(x => x.UserId, userId);
            return await _db.FindWithFilterAsync(filter);
        }

        public async Task<RecurringTransaction> InsertRecurringTransactionAsync(RecurringTransaction transaction)
        {
            await _db.GetCollection<RecurringTransaction>().InsertOneAsync(transaction);
            return transaction;
        }

        public async Task DeleteRecurringTransactionAsync(string id, string userId)
        {
            var builder = Builders<RecurringTransaction>.Filter;
            var idFilter = builder.Eq(x => x.Id, id);
            var userIdFilter = builder.Eq(x => x.UserId, userId);

            await _db.GetCollection<RecurringTransaction>().DeleteOneAsync(idFilter & userIdFilter);
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>() where T : AbstractLedgerItem
        {
            var items = await _cache.GetOrCreateAsync(typeof(T), async (entry) =>
            {
                return await _db.FindWithFilterAsync(FilterDefinition<T>.Empty);
            });
            return items;
        }

        public async Task<T> InsertOneAsync<T>(T item) where T : AbstractLedgerItem
        {
            _cache.Remove(typeof(T));
            await _db.GetCollection<T>().InsertOneAsync(item);
            return item;
        }
    }
}