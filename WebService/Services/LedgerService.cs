using System;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;
using Serilog;

namespace WebService
{
    public class LedgerService : ILedgerService
    {
        private ILogger _logger;
        private ILedgerRepository _repo;

        public LedgerService(ILogger logger, ILedgerRepository repo)
        {
            _logger = logger;
            _repo = repo;
        }

        public async Task<IEnumerable<LedgerEntryResponse>> GetLedgerEntriesByUserIdAsync(string userId)
        {
            var transactionTypes = await _repo.GetAllAsync<TransactionType>();
            return from entry in await _repo.GetLedgerEntriesByUserIdAsync(userId)
                   select LedgerEntryResponse.FromDBObject(entry, transactionTypes);
        }

        public async Task<LedgerEntryResponse> AddLedgerEntryAsync(LedgerEntryRequest request, string userId)
        {
            await UpdateOrInsertLedgerEntryCategoryAsync(request.Category);
            var entry = await _repo.InsertLedgerEntryAsync(new LedgerEntry()
            {
                UserId = userId,
                Category = request.Category,
                Description = request.Description,
                Amount = new Decimal(request.Amount),
                TransactionTypeId = request.TransactionTypeId,
                TransactionDate = request.TransactionDate,
                RecurringTransactionId = request.RecurringTransactionId,
                CreatedDate = DateTime.Now
            });

            var transactionTypes = await _repo.GetAllAsync<TransactionType>();
            return LedgerEntryResponse.FromDBObject(entry, transactionTypes);
        }

        public async Task DeleteLedgerEntryAsync(string id, string userId) =>
            await _repo.DeleteLedgerEntryAsync(id, userId);


        public async Task<IEnumerable<LedgerEntryCategory>> GetLedgerEntryCategoriesLikeAsync(CategoryCompleteRequest request)
        {
            var regex = $"^.*{request.Partial}.*";  // The ^ at the beginning should let this request use the index
            return await _repo.GetLedgerEntryCategoriesLikeAsync(regex);
        }

        public async Task<IEnumerable<IncomeGeneratorResponse>> GetIncomeGeneratorsByUserIdAsync(string userId)
        {
            var recurringTransactions = await _repo.GetRecurringTransactionsByUserIdAsync(userId);
            var generators = await _repo.GetIncomeGeneratorsByUserIdAsync(userId);
            var transactionTypes = await _repo.GetAllAsync<TransactionType>();

            var responses = from generator in generators
                            select new IncomeGeneratorResponse()
                            {
                                Id = generator.Id,
                                Description = generator.Description,
                                SalaryTypeId = generator.SalaryTypeId,
                                FrequencyId = generator.FrequencyId,
                                RecurringTransactions = CompileRecurringTransactions(generator.RecurringTransactions, recurringTransactions, transactionTypes)
                            };
            return responses;
        }

        public async Task<IncomeGeneratorResponse> AddIncomeGeneratorAsync(IncomeGeneratorRequest request, string userId)
        {
            // The transactions need to be inserted first, so that the is can be included
            // in the income generator. Also, LINQ doesn't allow 'await' in the select 
            // part of the query.
            var transactionIds = new List<string>();
            foreach (var transaction in request.RecurringTransactions)
            {
                transactionIds.Add((await AddRecurringTransactionAsync(transaction, userId)).Id);
            }

            var generator = await _repo.InsertIncomeGeneratorAsync(new IncomeGenerator()
            {
                UserId = userId,
                Description = request.Description,
                SalaryTypeId = request.SalaryTypeId,
                FrequencyId = request.FrequencyId,
                RecurringTransactions = transactionIds,
                CreatedDate = DateTime.Now
            });

            var recurringTransactions = await _repo.GetRecurringTransactionsByUserIdAsync(userId);
            var transactionTypes = await _repo.GetAllAsync<TransactionType>();
            return new IncomeGeneratorResponse()
            {
                Id = generator.Id,
                Description = generator.Description,
                SalaryTypeId = generator.SalaryTypeId,
                FrequencyId = generator.FrequencyId,
                RecurringTransactions = CompileRecurringTransactions(generator.RecurringTransactions, recurringTransactions, transactionTypes)
            };
        }

        public async Task DeleteIncomeGeneratorAsync(string id, string userId)
        {
            var generator = await _repo.GetIncomeGeneratorsByIdAsync(id);
            if (!generator.Any())
            {
                return;
            }

            // Delete all recurring transactions associated with the IncomeGenerator
            foreach (var transactionId in generator.First().RecurringTransactions)
            {
                await DeleteRecurringTransactionAsync(transactionId, userId);
            }
            await _repo.DeleteIncomeGeneratorAsync(id, userId);
        }

        public async Task<IEnumerable<RecurringTransactionResponse>> GetRecurringTransactionsByUserIdAsync(string userId)
        {
            var transactionTypes = await _repo.GetAllAsync<TransactionType>();
            return from transaction in await _repo.GetRecurringTransactionsByUserIdAsync(userId)
                   select RecurringTransactionResponse.FromDBObject(transaction, transactionTypes);
        }

        public async Task<RecurringTransaction> AddRecurringTransactionAsync(RecurringTransactionRequest request, string userId)
        {
            await UpdateOrInsertLedgerEntryCategoryAsync(request.Category);
            var recurringTransaction = await _repo.InsertRecurringTransactionAsync(new RecurringTransaction()
            {
                UserId = userId,
                Category = request.Category,
                Description = request.Description,
                Amount = new Decimal(request.Amount),
                FrequencyId = request.FrequencyId,
                TransactionTypeId = request.TransactionTypeId,
                LastTriggered = request.LastTriggered,
                LastExecuted = request.LastTriggered,
                CreatedDate = DateTime.Now
            });
            await AddLedgerEntryAsync(new LedgerEntryRequest()
            {
                Category = request.Category,
                Description = request.Description,
                Amount = request.Amount,
                TransactionTypeId = request.TransactionTypeId,
                RecurringTransactionId = recurringTransaction.Id,
                TransactionDate = request.LastTriggered
            }, userId);
            return recurringTransaction;
        }

        public async Task DeleteRecurringTransactionAsync(string id, string userId) =>
            await _repo.DeleteRecurringTransactionAsync(id, userId);




        public async Task<IEnumerable<T>> GetAllAsync<T>() where T : AbstractLedgerItem => await _repo.GetAllAsync<T>();
    }
}