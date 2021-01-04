using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebService
{
    public interface ILedgerRepository
    {
        Task<IEnumerable<LedgerEntry>> GetLedgerEntriesByUserIdAsync(string userId);
        Task<LedgerEntry> InsertLedgerEntryAsync(LedgerEntry entry);
        Task<IEnumerable<LedgerEntryCategory>> GetLedgerEntryCategoriesByNameAsync(string name);
        Task<IEnumerable<LedgerEntryCategory>> GetLedgerEntryCategoriesLikeAsync(string partial);
        Task<LedgerEntryCategory> InsertLedgerEntryCategoryAsync(LedgerEntryCategory category);
        Task<IEnumerable<IncomeGenerator>> GetIncomeGeneratorsByUserIdAsync(string userId);
        Task<IncomeGenerator> InsertIncomeGeneratorAsync(IncomeGenerator generator);
        Task SetIncomeGeneratorRecurringTransactionsAsync(string id, IEnumerable<string> transactions);
        Task<IEnumerable<RecurringTransaction>> GetRecurringTransactionsByUserIdAsync(string userId);
        Task<RecurringTransaction> InsertRecurringTransactionAsync(RecurringTransaction transaction);
        Task<IEnumerable<TransactionType>> GetTransactionTypesAsync();
        Task<TransactionType> InsertTransactionTypeAsync(TransactionType type);
        Task<IEnumerable<SalaryType>> GetSalaryTypesAsync();
        Task<SalaryType> InsertSalaryTypeAsync(SalaryType type);
        Task<IEnumerable<Frequency>> GetFrequenciesAsync();
        Task<Frequency> InsertFrequencyAsync(Frequency frequency);
    }
}