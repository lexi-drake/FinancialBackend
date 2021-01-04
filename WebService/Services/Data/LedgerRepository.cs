using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace WebService
{
    public class LedgerRepository : ILedgerRepository
    {
        private IMongoDatabase _db;

        public LedgerRepository(string connectionString, string database)
        {
            _db = new MongoClient(connectionString).GetDatabase(database);
        }

        public async Task<IEnumerable<LedgerEntry>> GetLedgerEntriesByUserIdAsync(string userId)
        {
            var filter = Builders<LedgerEntry>.Filter.Eq(x => x.UserId, userId);
            return await _db.FindWithFilterAsync(filter);
        }

        public async Task<LedgerEntry> InsertLedgerEntryAsync(LedgerEntry entry)
        {
            await _db.GetCollection<LedgerEntry>().InsertOneAsync(entry);
            return entry;
        }

        public async Task<IEnumerable<LedgerEntryCategory>> GetLedgerEntryCategoriesByNameAsync(string name)
        {
            var filter = Builders<LedgerEntryCategory>.Filter.Eq(x => x.Name, name);
            return await _db.FindWithFilterAsync(filter);
        }

        public async Task<IEnumerable<LedgerEntryCategory>> GetLedgerEntryCategoriesLikeAsync(string regex)
        {
            var filter = Builders<LedgerEntryCategory>.Filter.Regex(x => x.Name, regex);
            return await _db.FindWithFilterAsync(filter);
        }

        public async Task<LedgerEntryCategory> InsertLedgerEntryCategoryAsync(LedgerEntryCategory category)
        {
            await _db.GetCollection<LedgerEntryCategory>().InsertOneAsync(category);
            return category;
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

        public async Task<IEnumerable<TransactionType>> GetTransactionTypesAsync()
        {
            return await _db.FindWithFilterAsync(FilterDefinition<TransactionType>.Empty);
        }

        public async Task<TransactionType> InsertTransactionTypeAsync(TransactionType type)
        {
            await _db.GetCollection<TransactionType>().InsertOneAsync(type);
            return type;
        }

        public async Task<IEnumerable<SalaryType>> GetSalaryTypesAsync()
        {
            return await _db.FindWithFilterAsync(FilterDefinition<SalaryType>.Empty);
        }

        public async Task<SalaryType> InsertSalaryTypeAsync(SalaryType type)
        {
            await _db.GetCollection<SalaryType>().InsertOneAsync(type);
            return type;
        }

        public async Task<IEnumerable<Frequency>> GetFrequenciesAsync()
        {
            return await _db.FindWithFilterAsync(FilterDefinition<Frequency>.Empty);
        }

        public async Task<Frequency> InsertFrequencyAsync(Frequency frequency)
        {
            await _db.GetCollection<Frequency>().InsertOneAsync(frequency);
            return frequency;
        }
    }
}