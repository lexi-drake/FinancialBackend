using System;

namespace WebService
{
    public class LedgerEntry
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string CategoryId { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
        public string TransactionTypeId { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}