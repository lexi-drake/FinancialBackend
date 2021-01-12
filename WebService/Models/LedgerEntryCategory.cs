using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebService
{
    public class LedgerEntryCategory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Category { get; set; }
        // TTL index
        public DateTime LastUsed { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}