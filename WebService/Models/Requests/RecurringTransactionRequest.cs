using System;

namespace WebService
{
    public class RecurringTransactionRequest
    {
        public string Category { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
        public string FrequencyId { get; set; }
        public string TransactionTypeId { get; set; }
    }
}