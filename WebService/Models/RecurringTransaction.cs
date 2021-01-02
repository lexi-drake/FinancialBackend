using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebService
{
    public class RecurringTransaction
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
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