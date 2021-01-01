using System;

namespace WebService
{
    public class RecurringTransaction
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string CategoryId { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
        public string FrequencyId { get; set; }
        public string TransactionTypeId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}