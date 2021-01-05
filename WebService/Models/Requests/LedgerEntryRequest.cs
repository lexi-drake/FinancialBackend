using System;

namespace WebService
{
    public class LedgerEntryRequest
    {
        public string Category { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
        public string TransactionTypeId { get; set; }
        public string RecurringTransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}