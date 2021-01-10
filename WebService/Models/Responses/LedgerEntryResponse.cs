using System;
using System.Linq;
using System.Collections.Generic;

namespace WebService
{
    public class LedgerEntryResponse
    {
        public string Id { get; set; }
        // This is not stored as an id because of a TTL index on the LedgerEntryCategory collection
        public string Category { get; set; }
        public string Description { get; set; }
        public float Amount { get; set; }
        // For easier handling on the front end
        public string TransactionType { get; set; }
        public string RecurringTransactionId { get; set; }
        public DateTime TransactionDate { get; set; }

        public static LedgerEntryResponse FromDBObject(LedgerEntry ledgerEntry, IEnumerable<TransactionType> transactionTypes)
        {
            var transactions = from type in transactionTypes
                               where type.Id == ledgerEntry.TransactionTypeId
                               select type;
            // Handle the event that there isn't a matching transaction type very lightly.
            // TODO (alexa): revisit this if this becomes more important.
            string description = transactions.Any() ? transactions.First().Description : "UNKNOWN";
            return new LedgerEntryResponse()
            {
                Id = ledgerEntry.Id,
                Category = ledgerEntry.Category,
                Description = ledgerEntry.Description,
                Amount = Decimal.ToSingle(ledgerEntry.Amount),
                TransactionType = description,
                RecurringTransactionId = ledgerEntry.RecurringTransactionId,
                TransactionDate = ledgerEntry.TransactionDate
            };
        }

    }
}