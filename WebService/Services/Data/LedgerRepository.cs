using System;
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

        public async Task<IEnumerable<LedgerEntry>> GetLedgerEntriesByUserIdAsync(string userId)
        {
            var filter = Builders<LedgerEntry>.Filter.Eq(x => x.UserId, userId);
            return await _db.FindWithFilterAsync(filter);
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

        public async Task<IEnumerable<LedgerEntryCategory>> GetLedgerEntryCategoriesByCategoryAsync(string category)
        {
            var filter = Builders<LedgerEntryCategory>.Filter.Eq(x => x.Category, category);
            return await _db.FindWithFilterAsync(filter);
        }

        public async Task<IEnumerable<LedgerEntryCategory>> GetLedgerEntryCategoriesLikeAsync(string regex)
        {
            var filter = Builders<LedgerEntryCategory>.Filter.Regex(x => x.Category, regex);
            return await _db.FindWithFilterAsync(filter);
        }

        public async Task<LedgerEntryCategory> InsertLedgerEntryCategoryAsync(LedgerEntryCategory category)
        {
            await _db.GetCollection<LedgerEntryCategory>().InsertOneAsync(category);
            return category;
        }

        public async Task UpdateLedgerEntryCategoryLastUsedAsync(string id, DateTime lastUsed)
        {
            var filter = Builders<LedgerEntryCategory>.Filter.Eq(x => x.Id, id);
            var update = Builders<LedgerEntryCategory>.Update.Set(x => x.LastUsed, lastUsed);

            await _db.GetCollection<LedgerEntryCategory>().UpdateOneAsync(filter, update);
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