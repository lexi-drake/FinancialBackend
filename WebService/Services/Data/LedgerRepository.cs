using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebService
{
    public class LedgerRepository : ILedgerRepository
    {
        public LedgerRepository(string connectionString, string database)
        {

        }

        public async Task<IEnumerable<LedgerEntry>> GetLedgerEntriesForUserAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<LedgerEntry> InsertLedgerEntryAsync(LedgerEntry entry)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<LedgerEntryCategory>> GetLedgerEntryCategoriesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<LedgerEntryCategory> InsertLedgerEntryCategoryAsync(LedgerEntryCategory category)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<IncomeGenerator>> GetIncomeGeneratorsByUserIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<IncomeGenerator> InsertIncomeGeneratorAsync(IncomeGenerator generator)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateIncomeGeneratorRecurringTransactionsAsync(string userId, IEnumerable<string> transactions)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<RecurringTransaction>> GetRecurringTransactionsByUserIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<RecurringTransaction> InsertRecurringTransactionAsync(RecurringTransaction transaction)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TransactionType>> GetTransactionTypesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<TransactionType> InsertTransactionTypeAsync(TransactionType type)
        {
            throw new NotImplementedException();
        }
        public async Task<IEnumerable<SalaryType>> GetSalaryTypesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<SalaryType> InsertSalaryTypeAsync(SalaryType type)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Frequency>> GetFrequenciesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Frequency> InsertFrequencyAsync(Frequency frequency)
        {
            throw new NotImplementedException();
        }
    }
}