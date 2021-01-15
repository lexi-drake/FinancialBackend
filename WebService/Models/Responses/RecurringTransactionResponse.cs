using System;
using System.Linq;
using System.Collections.Generic;

namespace WebService
{
    public class RecurringTransactionResponse
    {
        public string Id { get; set; }
        // This is not stored as an id because of a TTL index on the LedgerEntryCategory collection
        public string Category { get; set; }
        public string Description { get; set; }
        public float Amount { get; set; }
        public string FrequencyId { get; set; }
        public string TransactionType { get; set; }
        public DateTime LastTriggered { get; set; }
        public DateTime LastExecuted { get; set; }

        public static RecurringTransactionResponse FromDBObject(RecurringTransaction recurringTransaction, IEnumerable<TransactionType> transactionTypes)
        {
            var transactions = from type in transactionTypes
                               where type.Id == recurringTransaction.TransactionTypeId
                               select type;
            // Handle the event that there isn't a matching transaction type very lightly.
            string description = transactions.Any() ? transactions.First().Description : "UNKNOWN";
            return new RecurringTransactionResponse()
            {
                Id = recurringTransaction.Id,
                Category = recurringTransaction.Category,
                Description = recurringTransaction.Description,
                Amount = Decimal.ToSingle(recurringTransaction.Amount),
                FrequencyId = recurringTransaction.FrequencyId,
                TransactionType = description,
                LastTriggered = recurringTransaction.LastTriggered,
                LastExecuted = recurringTransaction.LastExecuted
            };
        }
    }
}