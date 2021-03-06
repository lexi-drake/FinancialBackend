using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebService
{
    public abstract class AbstractLedgerItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}