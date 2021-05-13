using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebService
{
    public class LedgerEntry
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UserId { get; set; }
        // This is not stored as an id because of a TTL index on the LedgerEntryCategory collection
        public string Category { get; set; }
        public string Description { get; set; }
        public Decimal Amount { get; set; }
        public string TransactionTypeId { get; set; }
        public string RecurringTransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}